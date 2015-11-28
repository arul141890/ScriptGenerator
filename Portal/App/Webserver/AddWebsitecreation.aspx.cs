﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonUtilitiesModule;
using Core.Domain.Webserver;
using Portal.App.Common;
using Sevices.Webserver;
using StructureMap.Attributes;
using System.IO;
using System.Configuration;
namespace Portal.App.Webserver
{
    public partial class AddWebsitecreation : BasePage
    {
        [SetterProperty]
        public IWebsitecreationService WebsitecreationService { get; set; }
        

        protected void ButtonClick(object sender, EventArgs e)
        {
            this.HideLabels();
            var apppoolname = txtapppool.Text.Trim();
            var website = this.txtwebsite.Text.Trim();
            var Portnumber = this.txtportnumber.Text.Trim();
            var Dnsname = this.Txtdnsname.Text.Trim();
            var Physicalpath = this.Txtphysicalpath.Text.Trim();

            // Website Parameters validation
            if (string.IsNullOrWhiteSpace(apppoolname))
            {
                this.ShowErrorMessage("Please enter Application pool name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(website))
            {
                this.ShowErrorMessage("Please enter Website name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Portnumber))
            {
                this.ShowErrorMessage("Please enter Port number.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Dnsname))
            {
                this.ShowErrorMessage("Please enter DNS Server name.");
                return;
            }
            if(string.IsNullOrWhiteSpace(Physicalpath))
            {
                this.ShowErrorMessage("Please enter Physical path of the website");
            }

            try
            {
                //Call PSI file creater Method:
                CreatePSIFile(apppoolname, website, Portnumber, Dnsname, Physicalpath);
               
                
                if (0 == EditwebsiteCreationId)
                {
                    var clientUser = new Websitecreation()
                    {
                        CreatedBy = Context.User.Identity.Name,
                        CreatedDate = DateTimeHelper.Now,
                        Apppoolname=apppoolname,
                        Websitename=website,
                        Portnumber=Portnumber,
                        Websitednsname=Dnsname,
                        Physicalpath=Physicalpath
                    };

                    WebsitecreationService.Create(clientUser);
                    ShowSuccessMessage("Script Generated. Click to download.");
                    txtapppool.Text = string.Empty;
                    txtwebsite.Text = string.Empty;
                    txtportnumber.Text = string.Empty;
                    Txtdnsname.Text = string.Empty;
                    Txtphysicalpath.Text = string.Empty;
                }
                else
                {
                    var WebsiteCreation = WebsitecreationService.Retrieve(EditwebsiteCreationId);
                    WebsiteCreation.Apppoolname = apppoolname;
                    WebsiteCreation.Websitednsname = website;
                    WebsiteCreation.Portnumber = Portnumber;
                    WebsiteCreation.Websitednsname = Dnsname;
                    WebsiteCreation.Physicalpath = Physicalpath;

                    WebsitecreationService.Update(WebsiteCreation);
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
                EditwebsiteCreationId = Convert.ToInt32(Request.QueryString["vscId"]);
            }
            catch (Exception)
            {
                EditwebsiteCreationId = 0;
            }

            // check if edit mode
            if (EditwebsiteCreationId != 0)
            {
                var WebsiteCreation = this.WebsitecreationService.Retrieve(EditwebsiteCreationId);
                if (WebsiteCreation != null)
                {
                    lblTitle.Text = "Edit Website Parameters"; // change caption
                    txtapppool.Text = WebsiteCreation.Apppoolname;
                    txtwebsite.Text = WebsiteCreation.Websitename;
                    Txtdnsname.Text = WebsiteCreation.Websitednsname;
                    txtportnumber.Text = WebsiteCreation.Portnumber;
                    Txtphysicalpath.Text = WebsiteCreation.Physicalpath;
                    
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HideLabels();
        }

        public int EditwebsiteCreationId { get; set; }

       
        private bool CreatePSIFile(string apppoolname,string website,string Portnumber,string Dnsname,string Physicalpath)
        {
            bool returnResult = false;
           // string folderName = ConfigurationManager.ConnectionStrings["PSIFilePath"].ToString();
            string psiFilePath =  "Websitecreation" + ".ps1";

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
                    writer.WriteLine("$switchname="+apppoolname);
                    writer.WriteLine("$physicaladapter="+website);
                    writer.WriteLine("$allowmos="+Portnumber);
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