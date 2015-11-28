using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Portal.App.Common;
using Sevices;
using Sevices.Users;
using StructureMap.Attributes;


namespace Portal
{
    public partial class Default : BasePage
    {
        [SetterProperty]
        public IUserService UserService { get; set; }

        private const string DefaultUrl = "App/HyperV/VirtualSwitchCreations.aspx";

        public Default()
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                    Response.Redirect(DefaultUrl);
            }
        }

        protected void BtnRegisterClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Register.aspx");
        }

        protected void Btnreset(object sender, EventArgs e)
        {

            //reset password code

        }

        protected void BtnLoginClick(object sender, EventArgs e)
        {
            lblErrorMessage.Text = "";

            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                lblErrorMessage.Text = "Please enter User ID.";
            }
            else if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblErrorMessage.Text = "Please enter password.";
            }
            else
            {
                if (UserService.AuthenticateUser(txtUserId.Text, txtPassword.Text.Md5()))
                {
                    FormsAuthentication.SetAuthCookie(txtUserId.Text, true);
                    var returnUrl = Request.QueryString["ReturnUrl"];
                    Response.Redirect(returnUrl ?? DefaultUrl);
                }
                else
                {
                    lblErrorMessage.Text = "Invalid User ID or Password.";
                }
            }
        }
    }
}