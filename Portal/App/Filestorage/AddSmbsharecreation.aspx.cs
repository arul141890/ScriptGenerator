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
    public partial class AddSmbsharecreation : BasePage
    {
        [SetterProperty]
        public ISmbsharecreationService SmbsharecreationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Directoryname = txtDirectoryname.Text.Trim();
            var Smbname = txtSmbname.Text.Trim();
            var Accessgroup = txtAccessgroups.Text.Trim();
            
            // SMB parameters validation
            if (string.IsNullOrWhiteSpace(Directoryname))
            {
                this.ShowErrorMessage("Please enter directory name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Smbname))
            {
                this.ShowErrorMessage("Please enter SMB name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Accessgroup))
            {
                this.ShowErrorMessage("Please enter Accesss Group.");
                return;
            }

            
            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Directoryname, Smbname, Accessgroup);
               
                
                if (0 == EditsmbsharecreationId)
                {
                    var clientUser = new Smbsharecreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Directoryname = Directoryname,
                        Smbname = Smbname,
                        Accessgroups = Accessgroup
                    };

                    SmbsharecreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtDirectoryname.Text = string.Empty;
                    txtSmbname.Text = string.Empty;
                    txtAccessgroups.Text = string.Empty;
                }
                else
                {
                    var SmbshareCreation = SmbsharecreationService.Retrieve(EditsmbsharecreationId);
                    SmbshareCreation.Directoryname = Directoryname;
                    SmbshareCreation.Smbname = Smbname;
                    SmbshareCreation.Accessgroups = Accessgroup;

                    SmbsharecreationService.Update(SmbshareCreation);
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
                EditsmbsharecreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditsmbsharecreationId = 0;
            }

            // check if edit mode
            if (EditsmbsharecreationId != 0)
            {
                var SmbshareCreation = this.SmbsharecreationService.Retrieve(EditsmbsharecreationId);
                if (SmbshareCreation != null)
                {
                    lblTitle.Text = "Edit Smbshare paramaters"; // change caption
                    txtDirectoryname.Text = SmbshareCreation.Directoryname;
                    txtSmbname.Text = SmbshareCreation.Smbname;
                    txtAccessgroups.Text = SmbshareCreation.Accessgroups;
                   
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditsmbsharecreationId { get; set; }

       
        private bool CreatePSIFile(string Directoryname,string Smbname,string Accessgroup)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "Smbsharecreation" + ".ps1";

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
                    writer.WriteLine("PowerShell script to create SMB share");
                    writer.WriteLine("SMB share should be created first before proceeding with DFS namespace creation");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("$Dirname="+Directoryname);
                    writer.WriteLine("$smbname="+Smbname);
                    writer.WriteLine("$accessgroup="+Accessgroup);
                    @writer.WriteLine(@"icacls ""$Dirname"" /grant ""Authenticated Users"": (OI)(CI)(M)");
                    @writer.WriteLine(@"New-SmbShare -Name $smbname -Path ""$Dirname"" -FolderEnumerationMode AccessBased -CachingMode Documents -EncryptData $True -FullAccess ""$accessgroup"" ");
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