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
    public partial class AddNamespacecreation : BasePage
    {
        [SetterProperty]
        public INamespacecreationService NamespacecreationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Dfsservername = txtDfsservername.Text.Trim();
            var DFSpath = txtDfspath.Text.Trim();
            var Fileservername = txtFileservername.Text.Trim();
            var namespacetargetpath = txtTargetpath.Text.Trim();
                        
            // Switch Name validation
            if (string.IsNullOrWhiteSpace(Dfsservername))
            {
                this.ShowErrorMessage("Please enter DFS server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(DFSpath))
            {
                this.ShowErrorMessage("Please enter DFS Path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Fileservername))
            {
                this.ShowErrorMessage("Please enter File Server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(namespacetargetpath))
            {
                this.ShowErrorMessage("Please enter Target path.");
                return;
            }

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Dfsservername, DFSpath, Fileservername, namespacetargetpath);
               
                
                if (0 == EditNamespaceCreationId)
                {
                    var clientUser = new Namespacecreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Dfsservername = Dfsservername,
                        Dfspath = DFSpath,
                        Fileservername = Fileservername,
                        Targetpath = namespacetargetpath
                    };

                    NamespacecreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");

                    txtDfsservername.Text = string.Empty;
                    txtFileservername.Text = string.Empty;
                    txtDfspath.Text = string.Empty;
                    txtTargetpath.Text = string.Empty;
                }
                else
                {
                    var namespacecreation = NamespacecreationService.Retrieve(EditNamespaceCreationId);
                    namespacecreation.Dfsservername = Dfsservername;
                    namespacecreation.Dfspath = DFSpath;
                    namespacecreation.Fileservername = Fileservername;
                    namespacecreation.Targetpath = namespacetargetpath;

                    NamespacecreationService.Update(namespacecreation);
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
                EditNamespaceCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditNamespaceCreationId = 0;
            }

            // check if edit mode
            if (EditNamespaceCreationId != 0)
            {
                var namespaceCreation = this.NamespacecreationService.Retrieve(EditNamespaceCreationId);
                if (namespaceCreation != null)
                {
                    lblTitle.Text = "Edit Namespace parameters"; // change caption
                    txtDfsservername.Text = namespaceCreation.Dfsservername;
                    txtFileservername.Text = namespaceCreation.Fileservername;
                    txtDfspath.Text = namespaceCreation.Dfspath;
                    txtTargetpath.Text = namespaceCreation.Targetpath;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditNamespaceCreationId { get; set; }

       
        private bool CreatePSIFile(string Dfsservername, string DFSpath, string Fileservername, string namespacetargetpath)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "Namespacecreation" + ".ps1";

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
                    writer.WriteLine("$switchname="+Dfsservername);
                    writer.WriteLine("$physicaladapter="+DFSpath);
                    writer.WriteLine("$allowmos="+Fileservername);
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