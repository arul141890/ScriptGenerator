// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormsAuthenticationUtil.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The forms authentication util.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System.Security.Principal;
    using System.Web;
    using System.Web.Security;
    using System.Xml;

    public class FormsAuthenticationUtil
    {

        private FormsAuthenticationUtil()
        {
        }



        public static void AttachRolesToUser()
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        var id = (FormsIdentity) HttpContext.Current.User.Identity;

                        var ticket = id.Ticket;

                        if (!FormsAuthentication.CookiesSupported)
                        {
                            // If cookie is not supported for forms authentication, then the 
                            // authentication ticket is stored in the Url, which is encrypted.
                            // So, decrypt it
                            ticket = FormsAuthentication.Decrypt(id.Ticket.Name);
                        }

                        // Get the stored user-data, in this case, user roles
                        if (!string.IsNullOrEmpty(ticket.UserData))
                        {
                            var userData = ticket.UserData;

                            var roles = userData.Split(',');

                            // Roles were put in the UserData property in the authentication ticket
                            // while creating it
                            HttpContext.Current.User = new GenericPrincipal(id, roles);
                        }
                    }
                }
            }
        }

        public static void RedirectFromLoginPage(
            string userName,
            string commaSeperatedRoles,
            bool createPersistentCookie,
            string strCookiePath,
            string redirectionUrl)
        {
            RedirectFromLoginPageMain(
                userName, commaSeperatedRoles, createPersistentCookie, strCookiePath, redirectionUrl);
        }

        public static void RedirectFromLoginPage(
            string userName, string commaSeperatedRoles, bool createPersistentCookie, string redirectionUrl)
        {
            RedirectFromLoginPageMain(userName, commaSeperatedRoles, createPersistentCookie, null, redirectionUrl);
        }

        public static void SetAuthCookie(string userName, string commaSeperatedRoles, bool createPersistentCookie)
        {
            SetAuthCookieMain(userName, commaSeperatedRoles, createPersistentCookie, null);
        }

        public static void SetAuthCookie(
            string userName, string commaSeperatedRoles, bool createPersistentCookie, string strCookiePath)
        {
            SetAuthCookieMain(userName, commaSeperatedRoles, createPersistentCookie, strCookiePath);
        }



        private static FormsAuthenticationTicket CreateAuthenticationTicket(
            string userName, string commaSeperatedRoles, bool createPersistentCookie, string strCookiePath)
        {
            var cookiePath = strCookiePath ?? FormsAuthentication.FormsCookiePath;

            // Determine the cookie timeout value from web.config if specified
            var expirationMinutes = GetCookieTimeoutValue();

            // Create the authentication ticket
            var ticket = new FormsAuthenticationTicket(
                1,
                // A dummy ticket version
                userName,
                // User name for whome the ticket is issued
                DateTimeHelper.Now,
                // Current date and time
                DateTimeHelper.Now.AddMinutes(expirationMinutes),
                // Expiration date and time
                createPersistentCookie,
                // Whether to persist coolkie on client side. If true, 
                // The authentication ticket will be issued for new sessions from
                // the same client PC    
                commaSeperatedRoles,
                // Comma seperated user roles
                cookiePath); // Path cookie valid for

            return ticket;
        }

        private static int GetCookieTimeoutValue()
        {
            var timeout = 30; // Default timeout is 30 minutes
            var webConfig = new XmlDocument();
            webConfig.Load(HttpContext.Current.Server.MapPath(@"~\web.config"));
            var node = webConfig.SelectSingleNode("/configuration/system.web/authentication/forms");
            if (node != null && node.Attributes["timeout"] != null)
            {
                timeout = int.Parse(node.Attributes["timeout"].Value);
            }

            return timeout;
        }

        private static void RedirectFromLoginPageMain(
            string userName,
            string commaSeperatedRoles,
            bool createPersistentCookie,
            string strCookiePath,
            string redirectionUrl)
        {
            SetAuthCookieMain(userName, commaSeperatedRoles, createPersistentCookie, strCookiePath);
            HttpContext.Current.Response.Redirect(redirectionUrl);
        }

        private static void SetAuthCookieMain(
            string userName, string commaSeperatedRoles, bool createPersistentCookie, string strCookiePath)
        {
            var ticket = CreateAuthenticationTicket(
                userName, commaSeperatedRoles, createPersistentCookie, strCookiePath);

            // Encrypt the authentication ticket
            var encrypetedTicket = FormsAuthentication.Encrypt(ticket);

            if (!FormsAuthentication.CookiesSupported)
            {
                // If the authentication ticket is specified not to use cookie, set it in the Uri
                FormsAuthentication.SetAuthCookie(encrypetedTicket, createPersistentCookie);
            }
            else
            {
                // If the authentication ticket is specified to use a cookie, wrap it within a cookie.
                // The default cookie name is .ASPXAUTH if not specified 
                // in the <forms> element in web.config
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypetedTicket);

                // Set the cookie's expiration time to the tickets expiration time
                if (ticket.IsPersistent)
                {
                    authCookie.Expires = ticket.Expiration;
                }

                HttpContext.Current.Response.Cookies.Add(authCookie);
            }
        }

    }
}