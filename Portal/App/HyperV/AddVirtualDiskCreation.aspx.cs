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
    public partial class AddVirtualDiskCreation : BasePage
    {
        [SetterProperty]
        public IVirtualDiskCreationService VirtualdiskCreationService { get; set; }

        protected void disktypeDD_SelectedIndexChanged(object sender, EventArgs e)
        {
            string Check = DDdisktype.SelectedItem.Text;
            if (Check == "Differencing Disk")
            {
                txtParentPath.Enabled = true;
            }
            else
            {
                txtParentPath.Enabled = false;
            }
        }

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var VHDPath = txtvhdpath.Text.Trim();
            var VHDSize = txtVHDSize.Text.Trim();
           var VHDType = DDdisktype.SelectedItem.Text;
            var ParentPath = txtParentPath.Text.Trim();

            // Virtual Disk PArameters validation
            if (string.IsNullOrWhiteSpace(VHDPath))
            {
                this.ShowErrorMessage("Please enter Virtual Disk path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(VHDSize))
            {
                this.ShowErrorMessage("Please enter Virtual disk size.");
                return;
            }

            //correct definition for DD
            if (VHDType=="--select--")
            {
                this.ShowErrorMessage("Please select disk type.");
            }
            
            if(VHDType=="--SELECT--")
            {
                this.ShowErrorMessage("please select disk type");
            }

            if (string.IsNullOrWhiteSpace(ParentPath))
            {
                this.ShowErrorMessage("please enter Parent path");
            }

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(VHDPath, VHDSize, VHDType, ParentPath);


                if (0 == EditVirtualdiskCreationId)
                {
                    var clientUser = new VirtualDiskCreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        VHDPath=VHDPath,
                        VHDSize=VHDSize,
                        VHDType=VHDType,
                        ParentPath=ParentPath
                    };

                    VirtualdiskCreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtParentPath.Text = string.Empty;
                    txtvhdpath.Text = string.Empty;
                    txtVHDSize.Text = string.Empty;
                    DDdisktype.SelectedItem.Value = DropdownDefaultText;
                }
                else
                {
                    var virtualDiskCreation = VirtualdiskCreationService.Retrieve(EditVirtualdiskCreationId);
                    virtualDiskCreation.VHDPath = VHDPath;
                    virtualDiskCreation.VHDSize = VHDSize;
                    virtualDiskCreation.VHDType = VHDType;
                    virtualDiskCreation.ParentPath = ParentPath;
                    
                    VirtualdiskCreationService.Update(virtualDiskCreation);
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
                EditVirtualdiskCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditVirtualdiskCreationId = 0;
            }

            // check if edit mode
            if (EditVirtualdiskCreationId != 0)
            {
                var virtualDiskCreation = this.VirtualdiskCreationService.Retrieve(EditVirtualdiskCreationId);
                if (virtualDiskCreation != null)
                {
                    lblTitle.Text = "Edit Vitual Disk"; // change caption
                    txtParentPath.Text = virtualDiskCreation.ParentPath;
                    txtvhdpath.Text = virtualDiskCreation.VHDPath;
                    txtVHDSize.Text = virtualDiskCreation.VHDSize;
                    DDdisktype.SelectedItem.Text = virtualDiskCreation.VHDType;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditVirtualdiskCreationId { get; set; }


        private bool CreatePSIFile(string VHDPath,string VHDSize,string VHDType,string ParentPath)
        {
            bool returnResult = false;
            // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath = "VirtualDisk" + ".ps1";

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
                    writer.WriteLine("$switchname=" );
                    writer.WriteLine("$physicaladapter=" );
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