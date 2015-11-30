using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.dns;
using Portal.App.Common;
using Sevices.dns;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal.App.Dns
{
    public partial class AddDnsrecordcreation : BasePage
    {
        [SetterProperty]
        public IDnsrecordCreationService DnsrecordCreationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            bool returnResult = false;
            var Hostname = txtHostname.Text.Trim();
            var Ipaddress = txtIpaddress.Text.Trim();
            var Zonename = txtZonename.Text.Trim();
            var Csvfilename = txtCsvfilename.Text.Trim();
            // Switch Name validation
            if (string.IsNullOrWhiteSpace(Hostname))
            {
                this.ShowErrorMessage("Please enter Hostname.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Ipaddress))
            {
                this.ShowErrorMessage("Please enter IP Address.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Zonename))
            {
                this.ShowErrorMessage("Please enter Zone Name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Csvfilename))
            {
                this.ShowErrorMessage("Please enter Csv File Location.");
                return;
            }
            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Hostname, Ipaddress, Zonename, Csvfilename);
               
                
                if (0 == EditDnsrecordcreationId)
                {
                    var clientUser = new Dnsrecordcreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Hostname = Hostname,
                        Ipaddress = Ipaddress,
                        Zonename = Zonename,
                        Csvfilename = Csvfilename
                    };

                    DnsrecordCreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtHostname.Text = string.Empty;
                    txtIpaddress.Text = string.Empty;
                    txtZonename.Text = string.Empty;
                    txtCsvfilename.Text = string.Empty;
                }
                else
                {
                    var DnsrecordCreation = DnsrecordCreationService.Retrieve(EditDnsrecordcreationId);
                    DnsrecordCreation.Hostname = Hostname;
                    DnsrecordCreation.Ipaddress = Ipaddress;
                    DnsrecordCreation.Zonename = Zonename;
                    DnsrecordCreation.Csvfilename = Csvfilename;

                    DnsrecordCreationService.Update(DnsrecordCreation);
                    ShowSuccessMessage("Script Generated. Click to download.");
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorMessage(
                    ex.Message.Contains(
                        "An error occurred while updating the entries. See the inner exception for details.")
                        ? "Duplicate Entry"
                        : ex.Message);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            HideLabels();

            try
            {
                EditDnsrecordcreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditDnsrecordcreationId = 0;
            }

            // check if edit mode
            if (EditDnsrecordcreationId != 0)
            {
                var DnsrecordCreation = this.DnsrecordCreationService.Retrieve(EditDnsrecordcreationId);
                if (DnsrecordCreation != null)
                {
                    lblTitle.Text = "Edit DNS record parameters"; // change caption
                    txtHostname.Text = DnsrecordCreation.Hostname;
                    txtIpaddress.Text = DnsrecordCreation.Ipaddress;
                    txtZonename.Text = DnsrecordCreation.Zonename;
                    txtCsvfilename.Text = DnsrecordCreation.Csvfilename;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditDnsrecordcreationId { get; set; }

       
        private bool CreatePSIFile(string Hostname, string Ipaddress, string Zonename, string Csvfilename)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "VirtualSwitch" + ".ps1";

            try
            {

                //if (!Directory.Exists(folderName))
                //{
                //    Directory.CreateDirectory(folderName);
                //}

                FileStream fs1 = new FileStream(Server.MapPath("/PSIFiles/" + psiFilePath), FileMode.OpenOrCreate, FileAccess.Write);
                using (StreamWriter writer = new StreamWriter(fs1))
                {
                    writer.WriteLine("<# ");
                    writer.WriteLine("PowerShell script to create multiple A records creation");
                    writer.WriteLine("CSV file name should contain the complete path to the CSV file");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("$Hostname=" + Hostname);
                    writer.WriteLine("<# Enter the remote session of the server#>");
                    writer.WriteLine("New-PSSession –Name DNSrecord –ComputerName $Hostname");
                    writer.WriteLine("Enter-PSSession –Name DNSrecord");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("Import-Module DNSShell");
                    writer.WriteLine("$Zonename="+Zonename);
                    writer.WriteLine("$Csvfilename=" + Csvfilename);
                    @writer.WriteLine(@"Import-CSV $Csvfilename | %{New -DNSRecord -Name $_.""HostName"" - RecordType A - ZoneName $Zonename - IPAddress $_.""IPAddr""}");
                    writer.Close();
                    lbdownload.Visible = true;
                    returnResult = true;
                    hdnfileName.Value = psiFilePath;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return returnResult;
        }
        protected void lbdownload_Click(object sender, EventArgs e)
        {
            if (hdnfileName.Value != null)
            {
                Response.ContentType = "APPLICATION/OCTET-STREAM";
                String Header = "Attachment; Filename="+ hdnfileName.Value.Trim();
                Response.AppendHeader("Content-Disposition", Header);
                System.IO.FileInfo Dfile = new System.IO.FileInfo(Server.MapPath("/PSIFiles/" + hdnfileName.Value.Trim()));
                Response.WriteFile(Dfile.FullName);
                //Don't forget to add the following line
                Response.End();
            }
        }
    }
}