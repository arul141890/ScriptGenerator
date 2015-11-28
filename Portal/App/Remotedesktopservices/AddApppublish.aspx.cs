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
    public partial class AddApppublish : BasePage
    {
        [SetterProperty]
        public IApppublishService ApppublishService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            bool returnResult = false;
            var Alias = txtAlias.Text.Trim();
            var Displayname = txtDisplayname.Text.Trim();
            var Filepath = txtFilepath.Text.Trim();
            var collectionname = txtcollectionname.Text.Trim();
            var collectionbroker = txtConnectionbroker.Text.Trim();

            // App publish parameters validation
            if (string.IsNullOrWhiteSpace(Alias))
            {
                this.ShowErrorMessage("Please enter Alias name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Displayname))
            {
                this.ShowErrorMessage("Please enter Display Name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Filepath))
            {
                this.ShowErrorMessage("Please enter File Path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(collectionname))
            {
                this.ShowErrorMessage("Please enter Collection Name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(collectionbroker))
            {
                this.ShowErrorMessage("Please enter Collection Broker name.");
                return;
            }
            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Alias, Displayname, Filepath, collectionname, collectionbroker);
               
                
                if (0 == EditApppublishId)
                {
                    var clientUser = new Apppublish()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Alias = Alias,
                        Displayname=Displayname,
                        Filepath=Filepath,
                        collectionname=collectionname,
                        Connectionbroker=collectionbroker
                    };

                    ApppublishService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");

                    txtAlias.Text = string.Empty;
                    txtDisplayname.Text = string.Empty;
                    txtFilepath.Text = string.Empty;
                    txtcollectionname.Text = string.Empty;
                    txtConnectionbroker.Text = string.Empty;
                }
                else
                {
                    var Apppublish = ApppublishService.Retrieve(EditApppublishId);
                    Apppublish.Alias = Alias;
                    Apppublish.Displayname = Displayname;
                    Apppublish.Filepath = Filepath;
                    Apppublish.collectionname = collectionname;
                    Apppublish.Connectionbroker = collectionbroker;
                    
                    ApppublishService.Update(Apppublish);
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
                EditApppublishId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditApppublishId = 0;
            }

            // check if edit mode
            if (EditApppublishId != 0)
            {
                var Apppublish = this.ApppublishService.Retrieve(EditApppublishId);
                if (Apppublish != null)
                {
                    lblTitle.Text = "Edit App Paramaters"; // change caption
                    txtAlias.Text = Apppublish.Alias;
                    txtDisplayname.Text = Apppublish.Displayname;
                    txtcollectionname.Text = Apppublish.collectionname;
                    txtConnectionbroker.Text = Apppublish.Connectionbroker;
                    txtFilepath.Text = Apppublish.Connectionbroker;
                    
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditApppublishId { get; set; }

       
        private bool CreatePSIFile(string Alias, string Displayname, string Filepath, string collectionname, string collectionbroker)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "RDSAppPublish" + ".ps1";

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
                    writer.WriteLine("$switchname="+Alias);
                    writer.WriteLine("$physicaladapter="+collectionbroker);
                    writer.WriteLine("$allowmos="+collectionname);
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