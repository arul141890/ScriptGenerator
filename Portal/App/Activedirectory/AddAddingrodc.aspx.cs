using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.Activedirectory;
using Portal.App.Common;
using Sevices.Activedirectory;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal.App.Activedirectory
{
    public partial class AddAddingrodc : BasePage
    {
        [SetterProperty]
        public IAddingrodcService AddingrodcService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Hostname = txtHostname.Text.Trim();
            var Ipaddress = txtIpaddress.Text.Trim();
            var AllowpraName = txtAllowpraccount.Text.Trim();
            // correct declaration for DD
            var Critreplica = DDCriticalreplication.SelectedItem.Text;
            var delegatedadminacc = txtDelegatedacc.Text.Trim();
            var DenypraName = txtdenypraccount.Text.Trim();
            var DomainName = txtdomainname.Text.Trim();
            var InstallDNS = DDInstallDNS.SelectedItem.Text;
            var SiteName = txtSitename.Text.Trim();
            
            // RODC parameter validation
            if (string.IsNullOrWhiteSpace(Hostname))
            {
                this.ShowErrorMessage("Please enter Hostname.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Ipaddress))
            {
                this.ShowErrorMessage("Please enter IP Address.");
                return;
            }

            if (string.IsNullOrWhiteSpace(AllowpraName))
            {
                this.ShowErrorMessage("Please enter account that is allowed to replicate Password.");
                return;
            }

            if (Critreplica == "--SELECT--")
            {
                this.ShowErrorMessage("Please select an option.");
            }

            if (string.IsNullOrWhiteSpace(delegatedadminacc))
            {
                this.ShowErrorMessage("Please enter admin account that can be delegated control.");
                return;
            }

            if (string.IsNullOrWhiteSpace(DenypraName))
            {
                this.ShowErrorMessage("Please enter account that is denied to replicate Password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(DomainName))
            {
                this.ShowErrorMessage("Please enter Domain name");
                return;
            }

            if (InstallDNS == "--SELECT--")
            {
                this.ShowErrorMessage("Please select an option.");
            }

            if (string.IsNullOrWhiteSpace(SiteName))
            {
                this.ShowErrorMessage("Please enter Site name.");
                return;
            }

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Hostname, Ipaddress, AllowpraName, Critreplica, delegatedadminacc, DenypraName, DomainName, InstallDNS, SiteName);
               
                
                if (0 == EditRODCId)
                {
                    var clientUser = new Addingrodc()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Hostname = Hostname,
                        Ipaddress = Ipaddress,
                        AllowpasswordreplicationaccountName = AllowpraName,
                        CriticalReplicationOnly=Critreplica,
                        Delegatedadministratoraccountname = delegatedadminacc,
                        Denypasswordreplicationaccountname = DenypraName,
                        DomainName= DomainName,
                        InstallDNS = InstallDNS,
                        SiteName = SiteName
                        
                    };

                    AddingrodcService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtHostname.Text = string.Empty;
                    txtIpaddress.Text = string.Empty;
                    txtAllowpraccount.Text = string.Empty;
                    DDCriticalreplication.SelectedItem.Value = DropdownDefaultText;
                    txtDelegatedacc.Text = string.Empty;
                    txtdenypraccount.Text = string.Empty;
                    txtdomainname.Text = string.Empty;
                    DDInstallDNS.SelectedItem.Value = DropdownDefaultText;
                    txtSitename.Text = string.Empty;
                }
                else
                {
                    var RODCcreation = AddingrodcService.Retrieve(EditRODCId);
                    RODCcreation.Hostname = Hostname;
                    RODCcreation.Ipaddress = Ipaddress;
                    RODCcreation.AllowpasswordreplicationaccountName = AllowpraName;
                    RODCcreation.CriticalReplicationOnly = Critreplica;
                    RODCcreation.Delegatedadministratoraccountname = delegatedadminacc;
                    RODCcreation.Denypasswordreplicationaccountname = DenypraName;
                    RODCcreation.DomainName = DomainName;
                    RODCcreation.InstallDNS = InstallDNS;
                    RODCcreation.SiteName = SiteName;

                    AddingrodcService.Update(RODCcreation);
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
                EditRODCId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditRODCId = 0;
            }

            // check if edit mode
            if (EditRODCId != 0)
            {
                var RODCcreation = this.AddingrodcService.Retrieve(EditRODCId);
                if (RODCcreation != null)
                {
                    lblTitle.Text = "Edit RODC Parameters"; // change caption
                    txtHostname.Text = RODCcreation.Hostname;
                    txtIpaddress.Text = RODCcreation.Ipaddress;
                    txtAllowpraccount.Text = RODCcreation.AllowpasswordreplicationaccountName;
                    DDCriticalreplication.SelectedItem.Value = RODCcreation.CriticalReplicationOnly;
                    txtDelegatedacc.Text = RODCcreation.Delegatedadministratoraccountname;
                    txtdenypraccount.Text = RODCcreation.Denypasswordreplicationaccountname;
                    txtdomainname.Text = RODCcreation.DomainName;
                    DDInstallDNS.SelectedItem.Value = RODCcreation.InstallDNS;
                    txtSitename.Text = RODCcreation.SiteName;

                 }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditRODCId { get; set; }

       
        private bool CreatePSIFile(string Hostname,string Ipaddress,string AllowpraName,string Critreplica,string delegatedadminacc,string DenypraName,string DomainName,string InstallDNS,string SiteName)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "NewRODC" + ".ps1";

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
                    writer.WriteLine("$switchname=");
                    writer.WriteLine("$physicaladapter=");
                    writer.WriteLine("$allowmos=");
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