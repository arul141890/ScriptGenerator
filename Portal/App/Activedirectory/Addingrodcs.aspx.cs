using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Core.Domain;
using Core.Domain.Activedirectory;
using Portal.App.Common;
using Portal.App.DataSources;
using Sevices.Users;

namespace Portal.App.Activedirectory
{
    public partial class Addingrodcs : BasePage
    {
        public IUserService ClientUserService { get; set; }
        protected void Page_Init(object sender, EventArgs e)
        {
            this.dsData.TypeName = typeof(AddingrodcDataSource).FullName;
            this.dsData.DataObjectTypeName = typeof(Addingrodc).FullName;
            this.dsData.SelectParameters.Add("startDate", "");
            this.dsData.SelectParameters.Add("endDate", "");


            this.gvData.DataKeyNames = new[] { "Id" };
            this.gvData.Sort("Id", SortDirection.Descending);

            // set edit button
            this.gvData.EditButtonUrlFields = "Id";
            this.gvData.EditButtonUrlFormatString = "/App/Activedirectory/AddAddingrodc.aspx?vscId={0}";

            this.gvData.GenerateColumns();

            this.pnlFilter.IgnoreFields = "Id,Allowreplicationaccount,Delegatedadminiaccount,Denyreplicationaccount,Logpath,Sysvolpath,Databasepath";
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
                User clientUser = this.ClientUserService.GetUserByUserName(HttpContext.Current.User.Identity.Name);
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