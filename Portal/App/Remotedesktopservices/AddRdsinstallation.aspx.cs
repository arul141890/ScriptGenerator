using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.Remotedesktopservices;
using Portal.App.Common;
using Sevices.Remotedesktopservices;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal.App.Remotedesktopservices
{
    public partial class AddRdsinstallation : BasePage
    {
        [SetterProperty]
        public IRdsinstallationService RdsinstallationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Connectionbroker = txtConnectionbroker.Text.Trim();
            var Webaccessserver = txtWebaccessserver.Text.Trim();
            var Sessionhost = txtSessionhost.Text.Trim();
            var Gatewayserver = txtGatewayserver.Text.Trim();
            var Gatewayfqdn = txtGatewayfqdn.Text.Trim();
            
            // RDS Parameters validation
            if (string.IsNullOrWhiteSpace(Connectionbroker))
            {
                this.ShowErrorMessage("Please enter Connection Borker Server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Webaccessserver))
            {
                this.ShowErrorMessage("Please enter Web access server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Sessionhost))
            {
                this.ShowErrorMessage("Please enter Session Host Server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Gatewayserver))
            {
                this.ShowErrorMessage("Please enter Gateway server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Gatewayfqdn))
            {
                this.ShowErrorMessage("Please enter Gateway server FQDN.");
                return;
            }

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Connectionbroker, Webaccessserver, Sessionhost, Gatewayserver, Gatewayfqdn);
               
                
                if (0 == EditrdsinstallationId)
                {
                    var clientUser = new Rdsinstallation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Connectionbroker=Connectionbroker,
                        Webaccessserver=Webaccessserver,
                        Sessionhost=Sessionhost,
                        Gatewayserver=Gatewayserver,
                        Gatewayfqdn=Gatewayfqdn
                    };

                    RdsinstallationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtConnectionbroker.Text = string.Empty;
                    txtWebaccessserver.Text = string.Empty;
                    txtSessionhost.Text = string.Empty;
                    txtGatewayserver.Text = string.Empty;
                    txtGatewayfqdn.Text = string.Empty;
                }
                else
                {
                    var Rdsinstallation = RdsinstallationService.Retrieve(EditrdsinstallationId);
                    Rdsinstallation.Connectionbroker = Connectionbroker;
                    Rdsinstallation.Webaccessserver = Webaccessserver;
                    Rdsinstallation.Sessionhost = Sessionhost;
                    Rdsinstallation.Gatewayserver = Gatewayserver;
                    Rdsinstallation.Gatewayfqdn = Gatewayfqdn;

                    RdsinstallationService.Update(Rdsinstallation);
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
                EditrdsinstallationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditrdsinstallationId = 0;
            }

            // check if edit mode
            if (EditrdsinstallationId != 0)
            {
                var Rdsinstallation = this.RdsinstallationService.Retrieve(EditrdsinstallationId);
                if (Rdsinstallation != null)
                {
                    lblTitle.Text = "Edit RDS parameters"; // change caption
                    txtConnectionbroker.Text = Rdsinstallation.Connectionbroker;
                    txtWebaccessserver.Text = Rdsinstallation.Webaccessserver;
                    txtSessionhost.Text = Rdsinstallation.Sessionhost;
                    txtGatewayfqdn.Text = Rdsinstallation.Gatewayfqdn;
                    txtGatewayserver.Text = Rdsinstallation.Gatewayserver;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditrdsinstallationId { get; set; }

       
        private bool CreatePSIFile(string Connectionbroker, string Webaccessserver, string Sessionhost, string Gatewayserver, string Gatewayfqdn)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "RDSInstallation" + ".ps1";

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
                    writer.WriteLine("$switchname="+Connectionbroker);
                    writer.WriteLine("$physicaladapter="+Webaccessserver);
                    writer.WriteLine("$allowmos="+Sessionhost);
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