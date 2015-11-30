using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.dhcp;
using Portal.App.Common;
using Sevices.Dhcp;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal.App.Dhcp
{
    public partial class AddDhcpinstallation : BasePage
    {
        [SetterProperty]
        public IDhcpinstallationService DhcpinstallationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            bool returnResult = false;
            var isstaticip = DDStaticIP.SelectedItem.Text;
            var joindomain = DDDomainjoined.SelectedItem.Text;
            var authorize = DDAuthorize.SelectedItem.Text;
            var hostname = txtHostname.Text.Trim();
            var ipaddress = txtIpaddress.Text.Trim();

            // DHCP parameters validation
            if (isstaticip == "--SELECT--")
            {
                this.ShowErrorMessage("Please confirm if server has static IP Address");
                return;
            }

            if (isstaticip == "False")
            {
                this.ShowErrorMessage("DHCP Server should have static IP Address");
                return;
            }

            if (joindomain == "--SELECT--")
            {
                this.ShowErrorMessage("Please confirm if server is joined to domain");
                return;
            }

            if (joindomain == "False")
            {
                this.ShowErrorMessage("DHCP Server should should be joined to domain");
                return;
            }

            if (authorize == "--SELECT--")
            {
                this.ShowErrorMessage("Please confirm if DHCP server needs to be authorized");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(hostname))
            {
                this.ShowErrorMessage("Please enter Hostname.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ipaddress))
            {
                this.ShowErrorMessage("Please enter IP Address.");
                return;
            }



            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(isstaticip, joindomain, authorize, hostname, ipaddress);
               
                
                if (0 == EditDhcpinstallationId)
                {
                    var clientUser = new Dhcpinstallation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Staticip = isstaticip,
                        Joindomain = joindomain,
                        Authorize = authorize,
                        Hostname = hostname,
                        Ipaddress = ipaddress
                    };

                    DhcpinstallationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    DDStaticIP.SelectedItem.Text = DropdownDefaultText;
                    DDDomainjoined.SelectedItem.Text = DropdownDefaultText;
                    DDAuthorize.SelectedItem.Text = DropdownDefaultText;
                    txtHostname.Text = string.Empty;
                    txtIpaddress.Text = string.Empty;
                    
                }
                else
                {
                    var Dhcpinstallation = DhcpinstallationService.Retrieve(EditDhcpinstallationId);
                    Dhcpinstallation.Staticip = isstaticip;
                    Dhcpinstallation.Joindomain = joindomain;
                    Dhcpinstallation.Authorize = authorize;
                    Dhcpinstallation.Hostname = hostname;
                    Dhcpinstallation.Ipaddress = ipaddress;

                    DhcpinstallationService.Update(Dhcpinstallation);
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
                EditDhcpinstallationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditDhcpinstallationId = 0;
            }

            // check if edit mode
            if (EditDhcpinstallationId != 0)
            {
                var Dhcpinstallation = this.DhcpinstallationService.Retrieve(EditDhcpinstallationId);
                if (Dhcpinstallation != null)
                {
                    lblTitle.Text = "Edit DHCP parameters"; // change caption

                    DDStaticIP.SelectedValue = Dhcpinstallation.Staticip;
                    DDAuthorize.SelectedValue = Dhcpinstallation.Authorize;
                    DDDomainjoined.SelectedValue = Dhcpinstallation.Joindomain;
                    txtHostname.Text = Dhcpinstallation.Hostname;
                    txtIpaddress.Text = Dhcpinstallation.Ipaddress;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditDhcpinstallationId { get; set; }

       
        private bool CreatePSIFile(string isstaticip, string joindomain, string authorize, string hostname, string ipaddress)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "Dhcpinstallation" + ".ps1";

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
                    writer.WriteLine("PowerShell script to Install DHCP Server");
                    writer.WriteLine("DHCP server should have a static IP Address");
                    writer.WriteLine("DHCP server should be joined to domain and authorized to issue IP Addresses to the clients.");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("$Hostname=" + hostname);
                    writer.WriteLine("<# Enter the remote session of the server#>");
                    writer.WriteLine("New-PSSession –Name DHCPinstall –ComputerName $Hostname");
                    writer.WriteLine("Enter-PSSession –Name DHCPinstall");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("$IPAddress=" + ipaddress);
                    writer.WriteLine("Add-WindowsFeature -IncludeManagementTools dhcp");
                    writer.WriteLine("netsh dhcp add securitygroups");
                    writer.WriteLine("Restart-service dhcpserver");
                    writer.WriteLine("$authorize=" + authorize);
                    @writer.WriteLine(@"if($authorize -eq ""True""){Add-DhcpServerInDC  $Hostname  $IPAddress}");
                    @writer.WriteLine(@"Set-ItemProperty –Path registry::HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\ServerManager\Roles\12 –Name ConfigurationState –Value 2");
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