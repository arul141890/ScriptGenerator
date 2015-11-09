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

namespace Portal.App.HyperV
{
    public partial class AddVirtualSwitchCreation : BasePage
    {
        [SetterProperty]
        public IVirtualSwitchCreationService VirtualSwitchCreationService { get; set; }

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();

            var switchName = txtSwitchName.Text;
            var adapter = this.txtAdapter.Text;
            var allowManagementOs = this.txtAllowManagementOs.Text;

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
                    ShowSuccessMessage("Virtual Switch created successfully.");

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
                    ShowSuccessMessage("Virtual Switch updated successfully.");
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
    }
}