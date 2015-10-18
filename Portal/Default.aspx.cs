using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Interfaces;
using Services;


namespace Portal
{
    public partial class Default : System.Web.UI.Page
    {
        public IUserService userService { get; set; }
    
        private const string DefaultUrl = "App/Metadata.aspx";

        public Default()
        {
            userService = new UserService();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                    Response.Redirect(DefaultUrl);
            }
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
                if (userService.AuthenticateUser(txtUserId.Text, txtPassword.Text.Md5()))
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