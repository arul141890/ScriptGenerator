// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridViewExFilterPanel.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The grid view ex filter panel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Portal.App.WebControls
{
    public class GridViewExFilterPanel : Panel
    {

        public string GridViewExID
        {
            get { return (this.ViewState["GridViewExID"] ?? string.Empty).ToString(); }

            set { this.ViewState["GridViewExID"] = value; }
        }

        public string IgnoreFields
        {
            get { return (this.ViewState["IgnoreFields"] ?? string.Empty).ToString(); }

            set { this.ViewState["IgnoreFields"] = value; }
        }

        public string FilterExactValues
        {
            get { return (this.ViewState["FilterExactValues"] ?? string.Empty).ToString(); }

            set { this.ViewState["FilterExactValues"] = value; }
        }

        public string DateRangeFields
        {
            get { return (this.ViewState["DateRangeFields"] ?? string.Empty).ToString(); }

            set { this.ViewState["DateRangeFields"] = value; }
        }

        public void GenerateFilters()
        {
            IEnumerable<BoundField> boundFileds = this.GetBoundFields(this.GetGridViewExFromId(this.GridViewExID));

            var ul = new HtmlGenericControl("ul");

            var lstControlIds = new List<string>();
            int count = 0;
            var dataObjectType = this.GetGridViewExFromId(this.GridViewExID).GetDataObjectType();

            foreach (BoundField boundField in boundFileds)
            {
                var prop = dataObjectType.GetProperty(boundField.DataField);
                if (prop.PropertyType != typeof(DateTime) && prop.PropertyType != typeof(DateTime?))
                {
                    var wrapperLi = new HtmlGenericControl("li");
                    var wrapperUl = new HtmlGenericControl("ul");

                    var liPrompt = new HtmlGenericControl("li") { InnerText = boundField.DataField };
                    wrapperUl.Controls.Add(liPrompt);

                    var liInput = new HtmlGenericControl("li");

                    var inputField = new TextBox
                    {
                        AutoPostBack = false,
                        ClientIDMode = ClientIDMode.Static,
                        ID = this.GetBoundFieldFilterInputControlName(boundField.DataField)
                    };

                    lstControlIds.Add(inputField.ID);

                    liInput.Controls.Add(inputField);

                    wrapperUl.Controls.Add(liInput);
                    wrapperLi.Controls.Add(wrapperUl);

                    ul.Controls.Add(wrapperLi);
                    count++;
                }
                else if (boundField.HeaderText == DateRangeFields)
                {
                    var wrapperLi = new HtmlGenericControl("li");
                    var wrapperUl = new HtmlGenericControl("ul");
                    var filterInputControlName = "From" + boundField.DataField;
                    var liPrompt = new HtmlGenericControl("li") { InnerText = filterInputControlName };
                    wrapperUl.Controls.Add(liPrompt);

                    var liInput = new HtmlGenericControl("li");

                    var inputField = new TextBox
                    {
                        AutoPostBack = false,
                        ClientIDMode = ClientIDMode.Static,
                        CssClass = "textbox-datepicker",
                        ID = this.GetBoundFieldFilterInputControlName(filterInputControlName),
                        Text = DateTime.Today.AddDays(-7).ToString("MM/dd/yyyy")
                    };
                    inputField.Text = inputField.Text.Replace('-', '/');
                    lstControlIds.Add(inputField.ID);

                    liInput.Controls.Add(inputField);

                    wrapperUl.Controls.Add(liInput);
                    wrapperLi.Controls.Add(wrapperUl);

                    ul.Controls.Add(wrapperLi);
                    count++;

                    filterInputControlName = "To" + boundField.DataField;
                    wrapperLi = new HtmlGenericControl("li");
                    wrapperUl = new HtmlGenericControl("ul");

                    liPrompt = new HtmlGenericControl("li") { InnerText = filterInputControlName };
                    wrapperUl.Controls.Add(liPrompt);

                    liInput = new HtmlGenericControl("li");

                    inputField = new TextBox
                    {
                        AutoPostBack = false,
                        ClientIDMode = ClientIDMode.Static,
                        CssClass = "textbox-datepicker",
                        ID = this.GetBoundFieldFilterInputControlName(filterInputControlName),
                        Text = DateTime.Today.AddDays(1).ToString("MM/dd/yyyy")
                    };
                    inputField.Text = inputField.Text.Replace('-', '/');
                    lstControlIds.Add(inputField.ID);

                    liInput.Controls.Add(inputField);

                    wrapperUl.Controls.Add(liInput);
                    wrapperLi.Controls.Add(wrapperUl);

                    ul.Controls.Add(wrapperLi);
                    count++;
                }
            }

            if (count <= 0)
            {
                return;
            }

            var liButtons = new HtmlGenericControl("li");

            var btnFilter = new Button { Text = "Submit", ID = "btn" + this.GridViewExID + "Filter" };

            var sbResetJs = new StringBuilder();
            foreach (string lstControlId in lstControlIds)
            {
                sbResetJs.AppendFormat("document.getElementById(\"{0}\").value= \"\"; ", lstControlId);
            }

            sbResetJs.Append("javascript:window.location.href=window.location.href;");

            var btnReset = new Button
            {
                Text = "Reset",
                ID = "btn" + this.GridViewExID + "Reset",
                Visible = false,
                ClientIDMode = ClientIDMode.Static,
                OnClientClick = sbResetJs.ToString()
            };

            liButtons.Controls.Add(btnFilter);
            liButtons.Controls.Add(new Literal { Text = "&nbsp;" });
            liButtons.Controls.Add(btnReset);
            ul.Controls.Add(liButtons);

            this.CssClass = "s-gridview-filter";
            this.Controls.Add(ul);
        }

        internal string GetFilterExpression()
        {
            var lstFilters = new List<string>();

            GridViewEx gv = this.GetGridViewExFromId(this.GridViewExID);

            if (gv == null)
            {
                return null;
            }

            Type dataObjectType = gv.GetDataObjectType();

            IEnumerable<BoundField> boundFields = this.GetBoundFields(gv);

            var filterExactValues = new string[] { };
            if (!string.IsNullOrWhiteSpace(FilterExactValues))
                filterExactValues = FilterExactValues.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (BoundField boundField in boundFields)
            {
                Control ctrl = this.FindControl(this.GetBoundFieldFilterInputControlName(boundField.DataField));
                var txtBox = ctrl as TextBox;
                if (txtBox != null && !string.IsNullOrWhiteSpace(txtBox.Text))
                {
                    var prop = dataObjectType.GetProperty(boundField.DataField);
                    string value = txtBox.Text.Trim();

                    if (prop.PropertyType == typeof(string))
                    {
                        lstFilters.Add(filterExactValues.Contains(prop.Name)
                            ? string.Format("{0}=\"{1}\"", boundField.DataField, value)
                            : string.Format("{0}.Contains(\"{1}\")", boundField.DataField, value));
                    }
                    else if (prop.PropertyType == typeof(DateTime))
                    {

                    }
                    else
                    {
                        lstFilters.Add(string.Format("{0}={1}", boundField.DataField, value));
                    }
                }
            }

            string resetBtnId = "btn" + this.GridViewExID + "Reset";
            Control resetBtn = this.FindControl(resetBtnId);

            if (resetBtn != null)
            {
                resetBtn.Visible = lstFilters.Count > 0;
            }

            return string.Join(" AND ", lstFilters);
        }

        internal string GetFilterExpression(out string startDate, out string endDate)
        {
            startDate = "";
            endDate = "";
            var showReset = false;
            var lstFilters = new List<string>();

            GridViewEx gv = this.GetGridViewExFromId(this.GridViewExID);

            if (gv == null)
            {
                return null;
            }

            var filterExactValues = new string[] { };
            if (!string.IsNullOrWhiteSpace(FilterExactValues))
                filterExactValues = FilterExactValues.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Type dataObjectType = gv.GetDataObjectType();

            IEnumerable<BoundField> boundFields = this.GetBoundFields(gv);

            foreach (BoundField boundField in boundFields)
            {
                if (boundField.DataField != DateRangeFields)
                {
                    Control ctrl = this.FindControl(this.GetBoundFieldFilterInputControlName(boundField.DataField));
                    var txtBox = ctrl as TextBox;
                    if (txtBox != null && !string.IsNullOrWhiteSpace(txtBox.Text))
                    {
                        PropertyInfo prop = dataObjectType.GetProperty(boundField.DataField);
                        string value = txtBox.Text.Trim();

                        if (prop.PropertyType == typeof(string))
                        {
                            lstFilters.Add(filterExactValues.Contains(prop.Name)
                                ? string.Format("{0}=\"{1}\"", boundField.DataField, value)
                                : string.Format("{0}.Contains(\"{1}\")", boundField.DataField, value));
                        }
                        else if (prop.PropertyType == typeof(DateTime))
                        {
                            if (prop.Name == DateRangeFields)
                            {
                                var ctrl1 =
                                    this.FindControl(
                                        this.GetBoundFieldFilterInputControlName("From" + boundField.DataField));
                                var txtBox1 = ctrl1 as TextBox;
                                if (txtBox1 != null && !string.IsNullOrWhiteSpace(txtBox1.Text))
                                {
                                    startDate = txtBox1.Text;
                                }

                                var ctrl2 =
                                    this.FindControl(
                                        this.GetBoundFieldFilterInputControlName("To" + boundField.DataField));
                                var txtBox2 = ctrl2 as TextBox;
                                if (txtBox2 != null && !string.IsNullOrWhiteSpace(txtBox2.Text))
                                {
                                    endDate = txtBox2.Text;
                                }
                            }
                        }
                        else
                        {
                            lstFilters.Add(string.Format("{0}={1}", boundField.DataField, value));
                        }
                    }
                }
                else
                {
                    var ctrl = this.FindControl(this.GetBoundFieldFilterInputControlName("From" + boundField.DataField));
                    var txtBox = ctrl as TextBox;
                    if (txtBox != null && !string.IsNullOrWhiteSpace(txtBox.Text))
                    {
                        startDate = txtBox.Text;
                    }
                    var ctrl1 = this.FindControl(this.GetBoundFieldFilterInputControlName("To" + boundField.DataField));
                    var txtBox1 = ctrl1 as TextBox;
                    if (txtBox1 != null && !string.IsNullOrWhiteSpace(txtBox1.Text))
                    {
                        endDate = txtBox1.Text;
                    }
                    showReset = true;
                }
            }

            if (!showReset && lstFilters.Count > 0)
                showReset = true;

            string resetBtnId = "btn" + this.GridViewExID + "Reset";
            Control resetBtn = this.FindControl(resetBtnId);

            if (resetBtn != null)
            {
                resetBtn.Visible = showReset;
            }

            return string.Join(" AND ", lstFilters);
        }

        private string GetBoundFieldFilterInputControlName(string boundFieldName)
        {
            return "fiterInput" + this.GridViewExID + boundFieldName;
        }

        private IEnumerable<BoundField> GetBoundFields(GridViewEx gv)
        {
            var lst = new List<BoundField>();

            if (null == gv)
            {
                return lst;
            }

            List<string> ignoreFields =
                this.IgnoreFields.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (object column in gv.Columns)
            {
                var bf = column as BoundField;
                if (null == bf || ignoreFields.Contains(bf.DataField))
                {
                    continue;
                }

                lst.Add(bf);
            }

            return lst;
        }

        private GridViewEx GetGridViewExFromId(string GridViewExID)
        {
            if (string.IsNullOrWhiteSpace(GridViewExID))
            {
                return null;
            }

            return this.Parent.FindControl(GridViewExID) as GridViewEx
                   ?? this.Page.FindControl(GridViewExID) as GridViewEx;
        }
    }
}