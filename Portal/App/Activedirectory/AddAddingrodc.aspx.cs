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
    public partial class AddVirtualSwitchCreation : BasePage
    {
        [SetterProperty]
        public IVirtualSwitchCreationService VirtualSwitchCreationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            bool returnResult = false;
            var switchName = txtSwitchName.Text.Trim();
            var adapter = this.txtAdapter.Text.Trim();
            var allowManagementOs = this.txtAllowManagementOs.Text.Trim();
            
            // Switch Name validation
            if (string.IsNullOrWhiteSpace(switchName))
            {
                this.ShowErrorMessage("Please enter switch name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(adapter))
            {
                this.ShowErrorMessage("Please enter adapter.");
                return;
            }



            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(switchName, adapter, allowManagementOs);
               
                
                if (0 == EditVirtualSwitchCreationId)
                {
                    var clientUser = new VirtualSwitchCreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        AllowManagementOs = allowManagementOs,
                        PhysicalAdapter = adapter,
                        SwitchName = switchName
                    };

                    VirtualSwitchCreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");

                    txtAdapter.Text = string.Empty;
                    txtAllowManagementOs.Text = string.Empty;
                    txtSwitchName.Text = string.Empty;
                }
                else
                {
                    var virtualSwitchCreation = VirtualSwitchCreationService.Retrieve(EditVirtualSwitchCreationId);
                    virtualSwitchCreation.AllowManagementOs = allowManagementOs;
                    virtualSwitchCreation.SwitchName = switchName;
                    virtualSwitchCreation.PhysicalAdapter = adapter;

                    VirtualSwitchCreationService.Update(virtualSwitchCreation);
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
                EditVirtualSwitchCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditVirtualSwitchCreationId = 0;
            }

            // check if edit mode
            if (EditVirtualSwitchCreationId != 0)
            {
                var virtualSwitchCreation = this.VirtualSwitchCreationService.Retrieve(EditVirtualSwitchCreationId);
                if (virtualSwitchCreation != null)
                {
                    lblTitle.Text = "Edit Vitual Switch"; // change caption
                    txtAdapter.Text = virtualSwitchCreation.PhysicalAdapter;
                    txtAllowManagementOs.Text = virtualSwitchCreation.AllowManagementOs;
                    txtSwitchName.Text = virtualSwitchCreation.SwitchName;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditVirtualSwitchCreationId { get; set; }

       
        private bool CreatePSIFile(string switchName, string adapter, string allowManagementOs)
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
                    writer.WriteLine("$switchname="+switchName);
                    writer.WriteLine("$physicaladapter="+adapter);
                    writer.WriteLine("$allowmos="+allowManagementOs);
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