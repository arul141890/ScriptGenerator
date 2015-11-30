using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain;
using Portal.App.Common;
using Sevices.Users;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal
{
    public partial class Changepassword : BasePage
    {
        [SetterProperty]
        public IChangepassword Passwordchange { get; set; }


        protected void Btnchange(object sender, EventArgs e)
        {
            lblErrorMessage.Text = "";
            this.HideLabels();
            var Oldpassword = txtOldPassword.Text.Trim();
            var Newpassword = txtNewPassword.Text.Trim();

            // Password validation
            if (string.IsNullOrWhiteSpace(Oldpassword))
            {
                this.ShowErrorMessage("Please enter Old Password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Newpassword))
            {
                this.ShowErrorMessage("Please enter New Password.");
                return;
            }
            
            try
            {
                var clientUser = new User()
                {
                    PasswordHash = Newpassword
                 };

                Passwordchange.Update(clientUser);
                ShowSuccessMessage("Password updated successfully");

                txtNewPassword.Text = string.Empty;
                txtOldPassword.Text = string.Empty;

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

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }



    }     
        
}