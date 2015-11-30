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
    public partial class AddCollectioncreation : BasePage
    {
        [SetterProperty]
        public ICollectioncreationService Collectioncreationservice { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Collectionname = txtCollectionname.Text.Trim();
            var Sessionhost = txtSessionhost.Text.Trim();
            var Collectiondescription = txtCollectiondescription.Text.Trim();
            var Connectionbroker = txtConnectionbroker.Text.Trim();
            
            // Switch Name validation
            if (string.IsNullOrWhiteSpace(Collectionname))
            {
                this.ShowErrorMessage("Please enter collection name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Sessionhost))
            {
                this.ShowErrorMessage("Please enter Session host server name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Collectiondescription))
            {
                this.ShowErrorMessage("Please enter collection Description.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Connectionbroker))
            {
                this.ShowErrorMessage("Please enter Connection Broker server name.");
                return;
            }



            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Collectionname, Sessionhost, Collectiondescription, Connectionbroker);
               
                
                if (0 == EditCollectionCreationId)
                {
                    var clientUser = new Collectioncreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Collectionname =Collectionname,
                        Sessionhost = Sessionhost,
                        Collectiondescription = Collectiondescription,
                        Connectionbroker = Connectionbroker
                    };

                    Collectioncreationservice.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtCollectiondescription.Text = string.Empty;
                    txtCollectionname.Text = string.Empty;
                    txtConnectionbroker.Text = string.Empty;
                    txtSessionhost.Text = string.Empty;
                }
                else
                {
                    var collectioncreation = Collectioncreationservice.Retrieve(EditCollectionCreationId);
                    collectioncreation.Collectionname = Collectionname;
                    collectioncreation.Sessionhost = Sessionhost;
                    collectioncreation.Collectiondescription = Collectiondescription;
                    collectioncreation.Connectionbroker = Connectionbroker;

                    Collectioncreationservice.Update(collectioncreation);
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
                EditCollectionCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditCollectionCreationId = 0;
            }

            // check if edit mode
            if (EditCollectionCreationId != 0)
            {
                var collectioncreation = this.Collectioncreationservice.Retrieve(EditCollectionCreationId);
                if (collectioncreation != null)
                {
                    lblTitle.Text = "Edit Collection Parameter"; // change caption
                    txtCollectiondescription.Text = collectioncreation.Collectiondescription;
                    txtCollectionname.Text = collectioncreation.Collectionname;
                    txtConnectionbroker.Text = collectioncreation.Connectionbroker;
                    txtSessionhost.Text = collectioncreation.Sessionhost;
                    
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditCollectionCreationId { get; set; }

       
        private bool CreatePSIFile(string Collectionname, string Sessionhost, string Collectiondescription, string Connectionbroker)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "CollectionCreation" + ".ps1";

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
                    writer.WriteLine("PowerShell script to create Remote Desktop Services Application Colection");
                    writer.WriteLine("Remote Desktop Services infrastructure servers should have static IP Address");
                    writer.WriteLine("Publish Applications once the collection is created");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("import-module RemoteDesktop");
                    writer.WriteLine("$Collectioname="+Collectionname);
                    writer.WriteLine("$Collectiondesc="+Collectiondescription);
                    writer.WriteLine("$connectionbroker="+Connectionbroker);
                    writer.WriteLine("$Sessionhost=" + Sessionhost);
                    writer.WriteLine("New-RDSessionCollection -CollectionName $Collectioname -SessionHost $Sessionhost -CollectionDescription $Collectiondesc -ConnectionBroker $connectionbroker");
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