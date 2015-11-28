﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Core.Domain;
using Core.Domain.Filestorage;
using Portal.App.Common;
using Portal.App.DataSources;
using Sevices.Users;

namespace Portal.App.Filestorage
{
    public partial class Smbsharecreations : BasePage
    {
        public IUserService ClientUserService { get; set; }
        protected void Page_Init(object sender, EventArgs e)
        {
            this.dsData.TypeName = typeof(SmbsharecreationDataSource).FullName;
            this.dsData.DataObjectTypeName = typeof(Smbsharecreation).FullName;
            this.dsData.SelectParameters.Add("startDate", "");
            this.dsData.SelectParameters.Add("endDate", "");


            this.gvData.DataKeyNames = new[] { "Id" };
            this.gvData.Sort("Id", SortDirection.Descending);

            // set edit button
            this.gvData.EditButtonUrlFields = "Id";
            this.gvData.EditButtonUrlFormatString = "/App/Filestorage/AddSmbsharecreation.aspx?vscId={0}";

            this.gvData.GenerateColumns();

            this.pnlFilter.IgnoreFields = "Id";
            this.pnlFilter.DateRangeFields = "CreatedDate";
            this.pnlFilter.GenerateFilters();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetupFilterExpressions();

            if (!this.IsPostBack)
            {
                this.HideLabels();
            }
        }

        private void SetupFilterExpressions()
        {
            var filterExpressions = new List<string>();

            if (HttpContext.Current.User.Identity.Name != "admin")
            {
                User clientUser = this.ClientUserService.Retrieve(HttpContext.Current.User.Identity.Name);
                if (clientUser != null)
                {
                    filterExpressions.Add(string.Format("CreatedBy=\"{0}\"", clientUser.UserId));
                }
            }
            string startDate, endDate;
            string pnlFilterExpression = this.pnlFilter.GetFilterExpression(out startDate, out endDate);
            if (!string.IsNullOrEmpty(pnlFilterExpression))
            {
                filterExpressions.Add(pnlFilterExpression);
            }

            this.dsData.SelectParameters["startDate"].DefaultValue = startDate;
            this.dsData.SelectParameters["endDate"].DefaultValue = endDate;

            this.dsData.SelectParameters["filterExpression"].DefaultValue = string.Join(" AND ", filterExpressions);

        }
    }
}