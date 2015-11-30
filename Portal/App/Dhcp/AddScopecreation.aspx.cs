using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.dhcp;
using Portal.App.Common;
using Sevices.Dhcp;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal.App.Dhcp
{
    public partial class AddScopecreation : BasePage
    {
        [SetterProperty]
        public IScopeCreationService ScopeCreationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            bool returnResult = false;
            var Hostname = txtHostname.Text.Trim();
            var Ipaddress = txtIPAddress.Text.Trim();
            var Csvfile = txtCsvfilename.Text.Trim();
            
            // Switch Name validation
            if (string.IsNullOrWhiteSpace(Hostname))
            {
                this.ShowErrorMessage("Please enter Host name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Ipaddress))
            {
                this.ShowErrorMessage("Please enter IP Address.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Csvfile))
            {
                this.ShowErrorMessage("Please enter CSV file path.");
                return;
            }



            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Hostname, Ipaddress, Csvfile);
               
                
                if (0 == EditScopeCreationId)
                {
                    var clientUser = new Scopecreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Hostname = Hostname,
                        Ipaddress = Ipaddress,
                        Csvfilename = Csvfile
                    };

                    ScopeCreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");

                    txtHostname.Text = string.Empty;
                    txtIPAddress.Text = string.Empty;
                    txtCsvfilename.Text = string.Empty;
                }
                else
                {
                    var Scopecreation = ScopeCreationService.Retrieve(EditScopeCreationId);
                    Scopecreation.Hostname = Hostname;
                    Scopecreation.Ipaddress = Ipaddress;
                    Scopecreation.Csvfilename = Csvfile;
                    ScopeCreationService.Update(Scopecreation);
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
                EditScopeCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditScopeCreationId = 0;
            }

            // check if edit mode
            if (EditScopeCreationId != 0)
            {
                var Scopecreation = this.ScopeCreationService.Retrieve(EditScopeCreationId);
                if (Scopecreation != null)
                {
                    lblTitle.Text = "Edit Scope Parameters"; // change caption
                    txtHostname.Text = Scopecreation.Hostname;
                    txtIPAddress.Text = Scopecreation.Ipaddress;
                    txtCsvfilename.Text = Scopecreation.Csvfilename;
                  }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditScopeCreationId { get; set; }

       
        private bool CreatePSIFile(string Hostname, string Ipaddress, string Csvfile)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "Scopecreation" + ".ps1";

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
                    writer.WriteLine("$Hostname=" + Hostname);
                    writer.WriteLine("<# Enter the remote session of the server#>");
                    writer.WriteLine("New-PSSession –Name DNSinstall –ComputerName $Hostname");
                    writer.WriteLine("Enter-PSSession –Name DNSinstall");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("$filename=" + Csvfile);
                    @writer.WriteLine(@"Import-CSV $filename | %{Add-DhcpServerv4Scope -Name $_.""Name"" -StartRange $_.""StartRange"" -EndRange $_.""EndRange"" -SubnetMask $_.""SubnetMask""}");
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