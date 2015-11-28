using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.Filestorage;
using Portal.App.Common;
using Sevices.Filestorage;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal.App.Filestorage
{
    public partial class AddRoleinstallation : BasePage
    {
        [SetterProperty]
        public IRoleinstallationService RoleinstallationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            bool returnResult = false;
            var Hostname = txtHostname.Text.Trim();
            var Ipaddress = txtIPAddress.Text.Trim();
            
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



            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Hostname, Ipaddress);
               
                
                if (0 == EditroleinstallationId)
                {
                    var clientUser = new Roleinstallation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        IPAddress = Ipaddress,
                        Hostname = Hostname
                    };

                    RoleinstallationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtHostname.Text = string.Empty;
                    txtIPAddress.Text = string.Empty;
                }
                else
                {
                    var Roleinstallation = RoleinstallationService.Retrieve(EditroleinstallationId);
                    Roleinstallation.Hostname = Hostname;
                    Roleinstallation.IPAddress = Ipaddress;

                    RoleinstallationService.Update(Roleinstallation);
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
                EditroleinstallationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditroleinstallationId = 0;
            }

            // check if edit mode
            if (EditroleinstallationId != 0)
            {
                var Roleinstallation = this.RoleinstallationService.Retrieve(EditroleinstallationId);
                if (Roleinstallation != null)
                {
                    lblTitle.Text = "Edit Role installation Parameters"; // change caption
                    txtHostname.Text = Roleinstallation.Hostname;
                    txtIPAddress.Text = Roleinstallation.IPAddress;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditroleinstallationId { get; set; }

       
        private bool CreatePSIFile(string Hostname, string IPAddress)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "FilestorageRoleinstallation" + ".ps1";

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
                    writer.WriteLine("$switchname="+Hostname);
                    writer.WriteLine("$physicaladapter="+IPAddress);
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