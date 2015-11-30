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
    public partial class AddDnsinstallation : BasePage
    {
        [SetterProperty]
        public IDnsinstallationService DnsinstallationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var isstaticip = DDStaticIP.SelectedItem.Text;
            var hostname = txtHostname.Text.Trim();
            var Ipaddress = txtIpaddress.Text.Trim();

            // DNS installation validation
            if (isstaticip == "--SELECT--")
            {
                this.ShowErrorMessage("Please confirm if server has static IP Address");
                return;
            }

            if (isstaticip == "False")
            {
                this.ShowErrorMessage("DNS Server should have static IP Address");
                return;
            }

            if (string.IsNullOrWhiteSpace(hostname))
            {
                this.ShowErrorMessage("Please enter Hostname.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Ipaddress))
            {
                this.ShowErrorMessage("Please enter IP Address.");
                return;
            }


             

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(isstaticip, hostname, Ipaddress);
               
                
                if (0 == EditDnsinstallationId)
                {
                    var clientUser = new Dnsinstallation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Staticip = isstaticip,
                        Hostname = hostname,
                        Ipaddress = Ipaddress
                    };

                    DnsinstallationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");

                    DDStaticIP.SelectedItem.Text= DropdownDefaultText;
                    txtHostname.Text = string.Empty;
                    txtIpaddress.Text = string.Empty;
                }
                else
                {
                    var Dnsinstallation = DnsinstallationService.Retrieve(EditDnsinstallationId);
                    Dnsinstallation.Staticip = isstaticip;
                    Dnsinstallation.Hostname = hostname;
                    Dnsinstallation.Ipaddress = Ipaddress;
                    
                    DnsinstallationService.Update(Dnsinstallation);
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
                EditDnsinstallationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditDnsinstallationId = 0;
            }

            // check if edit mode
            if (EditDnsinstallationId != 0)
            {
                var Dnsinstallation = this.DnsinstallationService.Retrieve(EditDnsinstallationId);
                if (Dnsinstallation != null)
                {
                    lblTitle.Text = "Edit DNS parameters"; // change caption
                    DDStaticIP.SelectedValue = Dnsinstallation.Staticip;
                    txtHostname.Text = Dnsinstallation.Hostname;
                    txtIpaddress.Text = Dnsinstallation.Ipaddress;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditDnsinstallationId { get; set; }

       
        private bool CreatePSIFile(string isstaticip, string hostname, string Ipaddress)
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
                    writer.WriteLine("PowerShell script to Install DNS Server");
                    writer.WriteLine("DNS Server should have a static IP address");
                    writer.WriteLine("Install DNS Server before installing Active Directory server in windows 2012 R2 Environment");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("$Hostname=" + hostname);
                    writer.WriteLine("<# Enter the remote session of the server#>");
                    writer.WriteLine("New-PSSession –Name DNSinstall –ComputerName $Hostname");
                    writer.WriteLine("Enter-PSSession –Name DNSinstall");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("Add-WindowsFeature -IncludeManagementTools -name dns -IncludeAllSubFeature");
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