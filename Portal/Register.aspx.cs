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
    public partial class Register : BasePage
    {
        [SetterProperty]
        public IRegisterService Registerservice { get; set; }


        protected void BtnLogin(object sender, EventArgs e)
        {

            Response.Redirect("~/Default.aspx");
        }


        protected void BtnRegisterClick(object sender, EventArgs e)
        {
            lblRegisterError.Text = "";
            this.HideLabels();
            var Email = txtemail.Text.Trim();
            var Userid = txtUserId.Text.Trim();
            var Password = txtPassword.Text.Md5();
            var ConfPassword = txtPasswordconfirm.Text.Md5();
            var pwd1 = txtPassword.Text.ToString();
            var pwd2 = txtPasswordconfirm.Text.ToString();

            // Switch Name validation
            if (string.IsNullOrWhiteSpace(Email))
            {
                this.ShowErrorMessage("Please enter email id.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Userid))
            {
                this.ShowErrorMessage("Please enter user name.");
                return;
            }

            //check if user id already exists

            if (string.IsNullOrWhiteSpace(Password))
            {
                this.ShowErrorMessage("please enter Password");
            }

            if (string.IsNullOrWhiteSpace(ConfPassword))
            {
                this.ShowErrorMessage("please Confirm Password");
            }

            if (pwd1 != pwd2)
            {
                this.ShowErrorMessage("Password does not match");
            }



            try
            {
                var clientUser = new User()
                {
                    CreatedBy = "Admin",
                    CreatedDate = DateTimeHelper.Now,
                    Email = Email,
                    PasswordHash = Password,
                    UserId=Userid
                };

                Registerservice.Create(clientUser);
                ShowSuccessMessage("User Account Created");

                txtemail.Text = string.Empty;
                txtUserId.Text = string.Empty;
                txtPassword.Text = string.Empty;
                txtPasswordconfirm.Text = string.Empty;

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