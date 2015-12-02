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
    public partial class ResetPassword : BasePage
    {

        [SetterProperty]
        public IUserService UserService { get; set; }


        protected void OnBtnChangePassword(object sender, EventArgs e)
        {
            lblErrorMessage.Text = "";
            this.HideLabels();
            var oldpassword = txtOldPassword.Text.Md5();
            var newpassword = txtNewPassword.Text.Md5();
            var confirmpassword = txtPasswordconfirm.Text.Md5();

            // Password validation
            if (string.IsNullOrWhiteSpace(oldpassword))
            {
                this.ShowErrorMessage("Please enter Old Password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newpassword))
            {
                this.ShowErrorMessage("Please enter New Password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmpassword))
            {
                this.ShowErrorMessage("Please confirm new Password.");
                return;
            }

            if (txtNewPassword.Text.Trim() != txtPasswordconfirm.Text.Trim())
            {
                this.ShowErrorMessage("Password does not match.");
            }

            try
            {
                var user = UserService.GetUserByUserName(Context.User.Identity.Name);

                if (user.PasswordHash != oldpassword)
                {
                    this.ShowErrorMessage("Please enter valid Old password.");
                    return;
                }

                user.PasswordHash = newpassword;

                UserService.Update(user);
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