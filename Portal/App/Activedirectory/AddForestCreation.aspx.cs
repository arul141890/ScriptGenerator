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
    public partial class AddForestCreation : BasePage
    {
        [SetterProperty]
        public IForestCreationService ForestCreationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Hostname = Txthostname.Text.Trim();
            var Ipaddress = txtipaddress.Text.Trim();
            var Databasepath = txtdbpath.Text.Trim();
            var Domainmode = DDdomainmode.SelectedItem.Text;
            var Domainname = txtdomainname.Text.Trim();
            var Domainnetbiosname = txtnetbios.Text.Trim();
            var Forestmode = DDforestmode.SelectedItem.Text;
            var Logpath = txtlogpath.Text.Trim();
            var Sysvolpath = txtsysvol.Text.Trim();
            var safemodeadminpwd = txtsafemodepwd.Text.Md5();
            
            // Switch Name validation
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

            if (string.IsNullOrWhiteSpace(Databasepath))
            {
                this.ShowErrorMessage("Please enter Database Path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Domainname))
            {
                this.ShowErrorMessage("Please enter Domain name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Domainnetbiosname))
            {
                this.ShowErrorMessage("Please enter Domain Netbios name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Logpath))
            {
                this.ShowErrorMessage("Please enter Log Path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Sysvolpath))
            {
                this.ShowErrorMessage("Please enter system volume path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(safemodeadminpwd))
            {
                this.ShowErrorMessage("Please enter Safemode administrator password.");
                return;
            }

            if (string.Equals(Domainmode, "--SELECT--"))
            {
                this.ShowErrorMessage("Please select domain mode.");
            }

            if (string.Equals(Forestmode, "--SELECT--"))
            {
                this.ShowErrorMessage("Please select AD Forest mode.");
            }

           
            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Hostname, Ipaddress, Databasepath, Domainname, Domainnetbiosname, Logpath, Sysvolpath, safemodeadminpwd, Domainmode, Forestmode);
               
                
                if (0 == EditADForestCreationId)
                {
                    var clientUser = new Forestcreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Hostname=Hostname,
                        Ipaddress=Ipaddress,
                        Databasepath=Databasepath,
                        Domainname=Domainname,
                        Domainmode=Domainmode,
                        Domainnetbiosname=Domainnetbiosname,
                        Forestmode=Forestmode,
                        Logpath=Logpath,
                        Sysvolpath=Sysvolpath,
                        safemodeadministratorpassword=safemodeadminpwd
                        
                    };

                    ForestCreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    Txthostname.Text = string.Empty;
                    txtipaddress.Text = string.Empty;
                    txtdbpath.Text = string.Empty;
                    txtdomainname.Text = string.Empty;
                    DDdomainmode.SelectedItem.Text = DropdownDefaultText;
                    txtnetbios.Text = string.Empty;
                    DDforestmode.SelectedItem.Text = DropdownDefaultText;
                    txtlogpath.Text = string.Empty;
                    txtsysvol.Text = string.Empty;
                    txtsafemodepwd.Text = string.Empty;

                }
                else
                {
                    var ForestCreation = ForestCreationService.Retrieve(EditADForestCreationId);
                    ForestCreation.Hostname = Hostname;
                    ForestCreation.Ipaddress = Ipaddress;
                    ForestCreation.Databasepath = Databasepath;
                    ForestCreation.Domainname = Domainname;
                    ForestCreation.Domainmode = Domainmode;
                    ForestCreation.Domainnetbiosname = Domainnetbiosname;
                    ForestCreation.Forestmode = Forestmode;
                    ForestCreation.Logpath = Logpath;
                    ForestCreation.Sysvolpath = Sysvolpath;
                    ForestCreation.safemodeadministratorpassword = safemodeadminpwd;
                                        
                    ForestCreationService.Update(ForestCreation);
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
                EditADForestCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditADForestCreationId = 0;
            }

            // check if edit mode
            if (EditADForestCreationId != 0)
            {
                var ForestCreation = this.ForestCreationService.Retrieve(EditADForestCreationId);
                if (ForestCreation != null)
                {
                    lblTitle.Text = "Edit AD Forest Parameters"; // change caption
                    Txthostname.Text = ForestCreation.Hostname;
                    txtipaddress.Text = ForestCreation.Ipaddress;
                    txtdbpath.Text = ForestCreation.Databasepath;
                    txtdomainname.Text = ForestCreation.Domainname;
                    txtnetbios.Text = ForestCreation.Domainnetbiosname;
                    txtlogpath.Text = ForestCreation.Logpath;
                    txtsysvol.Text = ForestCreation.Sysvolpath;
                    txtsafemodepwd.Text = ForestCreation.safemodeadministratorpassword;
                    DDdomainmode.SelectedItem.Value = ForestCreation.Domainmode;
                    DDforestmode.SelectedItem.Value = ForestCreation.Forestmode;
                 }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditADForestCreationId { get; set; }

       
        private bool CreatePSIFile(string Hostname,string Ipaddress,string Databasepath,string Domainname,string Domainnetbiosname,string Logpath,string Sysvolpath,string safemodeadminpwd,string Domainmode,string Forestmode)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "NewForest" + ".ps1";

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