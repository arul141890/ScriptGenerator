using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.Webserver;
using Portal.App.Common;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
using Sevices.Webserver;
namespace Portal.App.Webserver
{
    public partial class AddWebserverinstallation : BasePage
    {
        [SetterProperty]
        public IWebserverinstallationService  WebserverinstallationService { get; set; }


        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            bool returnResult = false;
            var Ipaddress = this.txtIPAddress.Text.Trim();
            var Hostname = this.txthostname.Text.Trim();
          
            // Input validation
            if (string.IsNullOrWhiteSpace(Ipaddress))
            {
                this.ShowErrorMessage("Please enter IP Address.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Hostname))
            {
                this.ShowErrorMessage("Please enter Hostname.");
                return;
            }



            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Ipaddress, Hostname);


                if (0 == EditWebserinstallationId)
                {
                    var clientUser = new Webserverinstallation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Ipaddress = Ipaddress,
                        Hostname = Hostname,
                     };

                    WebserverinstallationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");

                    txtIPAddress.Text = string.Empty;
                    txthostname.Text = string.Empty;
                    
                }
                else
                {
                    var WebserverInstallation = WebserverinstallationService.Retrieve(EditWebserinstallationId);
                    WebserverInstallation.Ipaddress = Ipaddress;
                    WebserverInstallation.Hostname = Hostname;

                    WebserverinstallationService.Update(WebserverInstallation);
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
                EditWebserinstallationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditWebserinstallationId = 0;
            }

            // check if edit mode
            if (EditWebserinstallationId != 0)
            {
                var WebserverInstallation = this.WebserverinstallationService.Retrieve(EditWebserinstallationId);
                if (WebserverInstallation != null)
                {
                    lblTitle.Text = "Edit Webserver Parameters"; // change caption
                    txthostname.Text = WebserverInstallation.Hostname;
                    txtIPAddress.Text = WebserverInstallation.Ipaddress;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditWebserinstallationId { get; set; }


        private bool CreatePSIFile(string Ipaddress, string Hostname)
        {
            bool returnResult = false;
            // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath = "WebserverInstallation" + ".ps1";

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
                    writer.WriteLine("PowerShell script to create virtual switch");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("Import-Module Hyper-V");
                    writer.WriteLine("$switchname=" + Ipaddress);
                    writer.WriteLine("$physicaladapter=" + Hostname);
                    writer.WriteLine("$allowmos=" );
                    writer.WriteLine("New-VMSwitch -Name $switchname -NetAdapterNAme $physicaladapter -AllowMAnagementOS $allowmos");
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
                String Header = "Attachment; Filename=" + hdnfileName.Value.Trim();
                Response.AppendHeader("Content-Disposition", Header);
                System.IO.FileInfo Dfile = new System.IO.FileInfo(Server.MapPath("/PSIFiles/" + hdnfileName.Value.Trim()));
                Response.WriteFile(Dfile.FullName);
                //Don't forget to add the following line
                Response.End();
            }
        }
    }
}