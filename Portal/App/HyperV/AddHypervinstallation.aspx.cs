using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.Hyperv;
using Portal.App.Common;
using Sevices.HyperV;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal.App.HyperV
{
    public partial class AddHypervinstallation : BasePage
    {
        [SetterProperty]
        public IHypervinstallationService HypervinstallationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Isphysicalmachine = DDphysicalmachine.SelectedValue;
            var Isvtenabled = DDIsvtenabled.SelectedValue;
            var IPAddress = txtIPAddress.Text.Trim();
            var Hostname = txtHostname.Text.Trim();
            
            // HyperV paramaters validation validation
            if (string.IsNullOrWhiteSpace(IPAddress))
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
                CreatePSIFile(Isphysicalmachine, Isvtenabled, IPAddress, Hostname);
               
                
                if (0 == EditHypervinstallationId)
                {
                    var clientUser = new Hypervinstallation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Isphysicalmachine=Isphysicalmachine,
                        Isvtenabled=Isvtenabled,
                        IPAddress=IPAddress,
                        Hostname=Hostname
                    };

                    HypervinstallationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    DDIsvtenabled.Text = DropdownDefaultText;
                    DDphysicalmachine.Text = DropdownDefaultText;
                    txtHostname.Text = string.Empty;
                    txtIPAddress.Text = string.Empty;
                }
                else
                {
                    var Hypervinstallation = HypervinstallationService.Retrieve(EditHypervinstallationId);
                    Hypervinstallation.Isphysicalmachine = Isphysicalmachine;
                    Hypervinstallation.Isvtenabled = Isvtenabled;
                    Hypervinstallation.IPAddress = IPAddress;
                    Hypervinstallation.Hostname = Hostname;

                    HypervinstallationService.Update(Hypervinstallation);
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
                EditHypervinstallationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditHypervinstallationId = 0;
            }

            // check if edit mode
            if (EditHypervinstallationId != 0)
            {
                var Hypervinstallation = this.HypervinstallationService.Retrieve(EditHypervinstallationId);
                if (Hypervinstallation != null)
                {
                    lblTitle.Text = "Edit Hyper-v paramaters"; // change caption
                    txtHostname.Text = Hypervinstallation.Hostname;
                    txtIPAddress.Text = Hypervinstallation.IPAddress;
                    DDIsvtenabled.SelectedValue = Hypervinstallation.Isvtenabled;
                    DDphysicalmachine.SelectedValue = Hypervinstallation.Isphysicalmachine;
                    
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditHypervinstallationId { get; set; }

       
        private bool CreatePSIFile(string Isphysicalmachine,string Isvtenabled,string IPAddress,string Hostname)
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
                    writer.WriteLine("PowerShell script to create virtual switch");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("Import-Module Hyper-V");
                    writer.WriteLine("$switchname=");
                    writer.WriteLine("$physicaladapter=");
                    writer.WriteLine("$allowmos=");
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