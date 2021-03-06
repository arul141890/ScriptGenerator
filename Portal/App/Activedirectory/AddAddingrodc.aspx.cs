﻿using System;
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
            var delegatedadminacc = txtDelegatedacc.Text.Trim();
            var DenypraName = txtdenypraccount.Text.Trim();
            var DomainName = txtdomainname.Text.Trim();
            var SiteName = txtSitename.Text.Trim();
            var dbpath = txtdbpath.Text.Trim();
            var logpath = txtlogpath.Text.Trim();
            var sysvolpath = txtsysvol.Text.Trim();
            
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

            if (string.IsNullOrWhiteSpace(SiteName))
            {
                this.ShowErrorMessage("Please enter Site name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(dbpath))
            {
                this.ShowErrorMessage("Please enter Database path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(sysvolpath))
            {
                this.ShowErrorMessage("Please enter System volume path.");
                return;
            }

            if (string.IsNullOrWhiteSpace(logpath))
            {
                this.ShowErrorMessage("Please enter Log path.");
                return;
            }

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(Hostname, Ipaddress, AllowpraName, delegatedadminacc, DenypraName, DomainName, SiteName, dbpath, sysvolpath, logpath);
               
                
                if (0 == EditRODCId)
                {
                    var clientUser = new Addingrodc()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Hostname = Hostname,
                        Ipaddress = Ipaddress,
                        Allowreplicationaccount = AllowpraName,
                        Delegatedadminiaccount = delegatedadminacc,
                        Denyreplicationaccount = DenypraName,
                        DomainName= DomainName,
                        SiteName = SiteName,
                        Databasepath=dbpath,
                        Sysvolpath=sysvolpath,
                        Logpath=logpath
                        
                    };

                    AddingrodcService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtHostname.Text = string.Empty;
                    txtIpaddress.Text = string.Empty;
                    txtAllowpraccount.Text = string.Empty;
                    txtDelegatedacc.Text = string.Empty;
                    txtdenypraccount.Text = string.Empty;
                    txtdomainname.Text = string.Empty;
                    txtSitename.Text = string.Empty;
                    txtdbpath.Text = string.Empty;
                    txtsysvol.Text = string.Empty;
                    txtlogpath.Text = string.Empty;

                }
                else
                {
                    var RODCcreation = AddingrodcService.Retrieve(EditRODCId);
                    RODCcreation.Hostname = Hostname;
                    RODCcreation.Ipaddress = Ipaddress;
                    RODCcreation.Allowreplicationaccount = AllowpraName;
                    RODCcreation.Denyreplicationaccount = DenypraName;
                    RODCcreation.Delegatedadminiaccount = delegatedadminacc;
                    RODCcreation.DomainName = DomainName;
                    RODCcreation.SiteName = SiteName;
                    RODCcreation.Databasepath = dbpath;
                    RODCcreation.Logpath = logpath;
                    RODCcreation.Sysvolpath = sysvolpath;

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
                    txtAllowpraccount.Text = RODCcreation.Allowreplicationaccount;
                    txtDelegatedacc.Text = RODCcreation.Delegatedadminiaccount;
                    txtdenypraccount.Text = RODCcreation.Denyreplicationaccount;
                    txtdomainname.Text = RODCcreation.DomainName;
                    txtSitename.Text = RODCcreation.SiteName;
                    txtdbpath.Text = RODCcreation.Databasepath;
                    txtlogpath.Text = RODCcreation.Logpath;
                    txtsysvol.Text = RODCcreation.Sysvolpath;

                 }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditRODCId { get; set; }

       
        private bool CreatePSIFile(string Hostname,string Ipaddress,string AllowpraName,string delegatedadminacc,string DenypraName,string DomainName,string SiteName,string dbpath,string sysvolpath,string logpath)
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
                    writer.WriteLine("PowerShell script to Add a new Read Only Domain Controller");
                    writer.WriteLine("Execute the below command if powershell script execution is disabled");
                    writer.WriteLine("set-executionpolicy unrestricted");
                    writer.WriteLine("#>");
                    @writer.WriteLine(@"$Hostname=" + "" + Hostname + "");
                    writer.WriteLine("<# Enter the remote session of the server#>");
                    writer.WriteLine("New-PSSession –Name ADRODC –ComputerName $Hostname");
                    writer.WriteLine("Enter-PSSession –Name ADRODC");
                    writer.WriteLine("Import-Module ServerManager");
                    writer.WriteLine("Install-windowsfeature AD-Domain-Services");
                    writer.WriteLine("Import-Module ADDSDeployment");
                    @writer.WriteLine(@"$AllowpraName=" + "" + AllowpraName + "");
                    @writer.WriteLine(@"$delegatedadminacc=" + "" + delegatedadminacc + "");
                    @writer.WriteLine(@"$DenypraName=" + "" + DenypraName + "");
                    @writer.WriteLine(@"$Domainname=" + "" + DomainName + "");
                    @writer.WriteLine(@"$SiteName=" + "" + SiteName + "");
                    @writer.WriteLine(@"$Dbpath=" + "" + dbpath + "");
                    @writer.WriteLine(@"$Logpath=" + "" + logpath + "");
                    @writer.WriteLine(@"$Sysvolpath=" + "" + sysvolpath + "");
                    @writer.WriteLine(@"Install-ADDSDomainController -AllowPasswordReplicationAccountName @$AllowpraName -DelegatedAdministratorAccountName @$delegatedadminacc -DenyPasswordReplicationAccountName @$DenypraName  -Credential (Get-Credential) -CriticalReplicationOnly:$false -DomainName ""$Domainname"" -ApplicationPartitionsToReplicate * -CreateDnsDelegation:$false -DatabasePath ""$Dbpath"" -LogPath ""$Logpath"" -SysvolPath ""$Sysvolpath"" -DomainName ""$Domainname"" -ReadOnlyReplica:$true -SiteName ""$SiteName"" -InstallDns:$true -Force:$true");
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