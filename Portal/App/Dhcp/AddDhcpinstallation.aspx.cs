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
                        
            // Switch Name validation
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
                    DDStaticIP.SelectedValue = DropdownDefaultText;
                    DDDomainjoined.SelectedValue = DropdownDefaultText;
                    DDAuthorize.SelectedValue = DropdownDefaultText;
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
            // Add this to show default --SELECT-- option for drop down
            DDStaticIP.Items.Insert(0, DropdownDefaultText);
            DDAuthorize.Items.Insert(0, DropdownDefaultText);
            DDDomainjoined.Items.Insert(0, DropdownDefaultText);
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
                    writer.WriteLine("PowerShell script to create virtual switch");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("Import-Module Hyper-V");
                    writer.WriteLine("$switchname="+isstaticip);
                    writer.WriteLine("$physicaladapter="+joindomain);
                    writer.WriteLine("$allowmos="+authorize);
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