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
    public partial class AddVMCreation : BasePage
    {
        [SetterProperty]
        public IVMCreationService VMcreationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Vmname = txtVmname.Text.Trim();
            var Vmpath = txtVmpath.Text.Trim();
            var Physicaladapter = txtPhysicaladapter.Text.Trim();
            var SwitchName = txtSwitchName.Text.Trim();
            var Maxmem = txtMaxmem.Text.Trim();
            var Minmem = txtMinmem.Text.Trim();
            var Isopath = txtIsopath.Text.Trim();

            // VM Parameters validation
            if (string.IsNullOrWhiteSpace(Vmname))
            {
                this.ShowErrorMessage("Please enter Virtual Machine name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Vmpath))
            {
                this.ShowErrorMessage("Please enter VM Path.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Physicaladapter))
            {
                this.ShowErrorMessage("Please enter Physical switch adapter name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(SwitchName))
            {
                this.ShowErrorMessage("Please enter Switch name.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Maxmem))
            {
                this.ShowErrorMessage("Please enter Maximum memory for VM.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Minmem))
            {
                this.ShowErrorMessage("Please enter Minimum memory for VM.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Isopath))
            {
                this.ShowErrorMessage("Please enter the location of OS ISO.");
                return;
            }
            

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Vmname, Vmpath, Physicaladapter, SwitchName, Maxmem, Minmem, Isopath);
               
                
                if (0 == EditVMCreationId)
                {
                    var clientUser = new VMCreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Vmname=Vmname,
                        Vmpath=Vmpath,
                        Physicaladapter=Physicaladapter,
                        SwitchName=SwitchName,
                        Maxmem=Maxmem,
                        Minmem=Minmem,
                        Isopath=Isopath,
                    };

                    VMcreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtVmname.Text = string.Empty;
                    txtVmpath.Text = string.Empty;
                    txtPhysicaladapter.Text = string.Empty;
                    txtSwitchName.Text = string.Empty;
                    txtMaxmem.Text = string.Empty;
                    txtMinmem.Text = string.Empty;
                    txtIsopath.Text = string.Empty;
                }
                else
                {
                    var virtualMachineCreation = VMcreationService.Retrieve(EditVMCreationId);
                    virtualMachineCreation.Vmname = Vmname;
                    virtualMachineCreation.Vmpath = Vmpath;
                    virtualMachineCreation.Physicaladapter = Physicaladapter;
                    virtualMachineCreation.SwitchName = SwitchName;
                    virtualMachineCreation.Maxmem = Maxmem;
                    virtualMachineCreation.Minmem = Minmem;
                    virtualMachineCreation.Isopath = Isopath;

                    VMcreationService.Update(virtualMachineCreation);
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
                EditVMCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditVMCreationId = 0;
            }

            // check if edit mode
            if (EditVMCreationId != 0)
            {
                var virtualMachineCreation = this.VMcreationService.Retrieve(EditVMCreationId);
                if (virtualMachineCreation != null)
                {
                    lblTitle.Text = "Edit VM Parameters"; // change caption
                    txtVmname.Text = virtualMachineCreation.Vmname;
                    txtVmpath.Text = virtualMachineCreation.Vmpath;
                    txtPhysicaladapter.Text = virtualMachineCreation.Physicaladapter;
                    txtSwitchName.Text = virtualMachineCreation.SwitchName;
                    txtMaxmem.Text = virtualMachineCreation.Maxmem;
                    txtMinmem.Text = virtualMachineCreation.Minmem;
                    txtIsopath.Text = virtualMachineCreation.Isopath;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditVMCreationId { get; set; }

       
        private bool CreatePSIFile(string Vmname,string Vmpath,string Physicaladapter,string SwitchName,string Maxmem,string Minmem,string Isopath)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "VirtualMachine" + ".ps1";

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