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
            var Smbsharename = txtsmbname.Text.Trim();
            var Fileservername = txtFileservername.Text.Trim();
            var Domainname = txtDomain.Text.Trim();
                        
            // Switch Name validation
            if (string.IsNullOrWhiteSpace(Dfsservername))
            {
                this.ShowErrorMessage("Please enter DFS server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Smbsharename))
            {
                this.ShowErrorMessage("Please enter SMB Share name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Fileservername))
            {
                this.ShowErrorMessage("Please enter File Server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Domainname))
            {
                this.ShowErrorMessage("Please enter Domain Name.");
                return;
            }

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Dfsservername, Smbsharename, Fileservername, Domainname);
               
                
                if (0 == EditNamespaceCreationId)
                {
                    var clientUser = new Namespacecreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Dfsservername = Dfsservername,
                        Smbsharename = Smbsharename,
                        Fileservername = Fileservername,
                        Domainname = Domainname
                    };

                    NamespacecreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");

                    txtDfsservername.Text = string.Empty;
                    txtFileservername.Text = string.Empty;
                    txtDomain.Text = string.Empty;
                    txtsmbname.Text = string.Empty;
                }
                else
                {
                    var namespacecreation = NamespacecreationService.Retrieve(EditNamespaceCreationId);
                    namespacecreation.Dfsservername = Dfsservername;
                    namespacecreation.Domainname = Domainname;
                    namespacecreation.Fileservername = Fileservername;
                    namespacecreation.Smbsharename = Smbsharename;

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
                    txtDomain.Text = namespaceCreation.Domainname;
                    txtsmbname.Text = namespaceCreation.Smbsharename;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditNamespaceCreationId { get; set; }

       
        private bool CreatePSIFile(string Dfsservername, string Smbsharename, string Fileservername, string Domainname)
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
                    writer.WriteLine("PowerShell script to create DFS Namespace");
                    writer.WriteLine("Folder should be created and SMB share has to be created before adding DFS Namespace");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("$Hostname=" + Dfsservername);
                    writer.WriteLine("<# Enter the remote session of the server#>");
                    writer.WriteLine("New-PSSession –Name Namespace –ComputerName $Hostname");
                    writer.WriteLine("Enter-PSSession –Name Namespace");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("$smbname=" + Smbsharename);
                    writer.WriteLine("$Fileserver=" + Fileservername);
                    writer.WriteLine("$Domainname=" + Domainname);
                    @writer.WriteLine(@"New-DfsnRoot –Path ""\\$Domainname\$smbname"" -TargetPath ""\\$Fileserver\$smbname"" -Type Domainv2 | Format-List");
                    @writer.WriteLine(@"Path: ""\\$Domainname\$smbname""");
                    @writer.WriteLine(@"Description : Domain-based $smbname namespace");
                    @writer.WriteLine(@"Type : Domain V2");
                    @writer.WriteLine(@"State : Online");
                    @writer.WriteLine(@"TimeToLiveSec : 300");
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