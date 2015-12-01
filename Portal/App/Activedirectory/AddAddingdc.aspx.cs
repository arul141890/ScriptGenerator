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
    public partial class AddAddingdc : BasePage
    {
        [SetterProperty]
        public IAddingdcService AddingdcCreationService { get; set; }
        
                protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var Hostname = TxtHostname.Text.Trim();
            var IPAddress = TxtIPAddress.Text.Trim();
            var DomainName = txtdomainname.Text.Trim();
            var DatabasePath = txtdbpath.Text.Trim();
            var LogPath = txtlogpath.Text.Trim();
            var Sysvolpath = txtsysvol.Text.Trim();
            var safemodeadminpassword = txtsafemodepwd.Text.Trim();
                        
            // DC Parameters validation
            if (string.IsNullOrWhiteSpace(Hostname))
            {
                this.ShowErrorMessage("Please enter Hostname.");
                return;
            }

            if (string.IsNullOrWhiteSpace(IPAddress))
            {
                this.ShowErrorMessage("Please enter IP Address.");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(DomainName))
            {
                this.ShowErrorMessage("Please enter Domain Name.");
                return;
            }
            if (string.IsNullOrWhiteSpace(DatabasePath))
            {
                this.ShowErrorMessage("Please enter Database Path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(LogPath))
            {
                this.ShowErrorMessage("Please enter Log Path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Sysvolpath))
            {
                this.ShowErrorMessage("Please enter Sysvol Path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(safemodeadminpassword))
            {
                this.ShowErrorMessage("Please enter safe mode Admin Password.");
                return;
            }

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Hostname, IPAddress, DomainName, DatabasePath, LogPath, Sysvolpath, safemodeadminpassword);
               
                
                if (0 == EditAdddcCreationId)
                {
                    var clientUser = new Addingdc()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Hostname=Hostname,
                        Ipaddress=IPAddress,
                        Userdomain = DomainName,
                        Databasepath = DatabasePath,
                        Logpath=LogPath,
                        Sysvolpath=Sysvolpath,
                     };

                    AddingdcCreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    TxtHostname.Text = string.Empty;
                    TxtIPAddress.Text = string.Empty;
                    txtdomainname.Text = string.Empty;
                    txtdbpath.Text = string.Empty;
                    txtlogpath.Text = string.Empty;
                    txtsysvol.Text = string.Empty;
                    txtsafemodepwd.Text = string.Empty;
                }
                else
                {
                    var Adddc = AddingdcCreationService.Retrieve(EditAdddcCreationId);
                    Adddc.Hostname = Hostname;
                    Adddc.Ipaddress = IPAddress;
                    Adddc.Userdomain = DomainName;
                    Adddc.Databasepath = DatabasePath;
                    Adddc.Logpath = LogPath;
                    Adddc.Sysvolpath = Sysvolpath;
                    
                    AddingdcCreationService.Update(Adddc);
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
                EditAdddcCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditAdddcCreationId = 0;
            }

            // check if edit mode
            if (EditAdddcCreationId != 0)
            {
                var Adddc = this.AddingdcCreationService.Retrieve(EditAdddcCreationId);
                if (Adddc != null)
                {
                    lblTitle.Text = "Edit AD Paramaters"; // change caption
                    TxtHostname.Text = Adddc.Hostname;
                    TxtIPAddress.Text = Adddc.Ipaddress;
                    txtdomainname.Text = Adddc.Userdomain;
                    txtdbpath.Text = Adddc.Databasepath;
                    txtlogpath.Text = Adddc.Logpath;
                    txtsysvol.Text = Adddc.Sysvolpath;
                    txtsafemodepwd.Text = "";
                    
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditAdddcCreationId { get; set; }

       
        private bool CreatePSIFile(string hostname, string IPAddress, string DomainName, string DatabasePath, string LogPath, string Sysvolpath, string safemodeadminpassword)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "NewDC" + ".ps1";

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
                    writer.WriteLine("PowerShell script to add a Domain controller to a domain");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    @writer.WriteLine(@"$Hostname=" + "" + hostname + "");
                    writer.WriteLine("<# Enter the remote session of the server#>");
                    writer.WriteLine("New-PSSession –Name ADDC –ComputerName $Hostname");
                    writer.WriteLine("Enter-PSSession –Name ADDC");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("Install-windowsfeature AD-Domain-Services");
                    writer.WriteLine("Import-Module ADDSDeployment");
                    @writer.WriteLine(@"$Databasepath=" + "" + DatabasePath + "");
                    @writer.WriteLine(@"$Logpath=" + "" + LogPath + "");
                    @writer.WriteLine(@"$Sysvolpath=" + "" + Sysvolpath + "");
                    @writer.WriteLine(@"$Domainname=" + "" + DomainName + "");
                    @writer.WriteLine(@"$adminpassword =" + "" + safemodeadminpassword + "");
                    @writer.WriteLine(@"Install-ADDSDomainController -DomainName ""$Domainname"" -ApplicationPartitionsToReplicate * -DatabasePath ""$Databasepath"" -CreateDnsDelegation:$false -Force -InstallDns -LogPath ""$Logpath""  -SysvolPath ""$Sysvolpath"" -safemodeadministratorpassword:$adminpassword");
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