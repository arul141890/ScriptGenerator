// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePage.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The base page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Portal.App.WebControls;
using StructureMap;

namespace Portal.App.Common
{
    public class BasePage : Page
    {
        #region Fields

        protected string DropdownDefaultText = "-- SELECT --";

        protected Label ErrorMessageLabel;

        protected Label SuccessMessageLabel;

        #endregion

        #region Constructors and Destructors

        public BasePage()
        {
            ObjectFactory.BuildUp(this);
        }

        #endregion

        #region Public Methods and Operators

        public void CreateGridViewExFilterPanel(GridViewEx gv, Panel panel, List<string> ignoreFields = null)
        {
            if (null == ignoreFields)
            {
                ignoreFields = new List<string>();
            }

            var ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("class", "ullist");

            int count = 0;
            foreach (object column in gv.Columns)
            {
                var bc = column as BoundField;
                if (null == bc || ignoreFields.Contains(bc.DataField))
                {
                    continue;
                }

                var liPrompt = new HtmlGenericControl("li");
                liPrompt.InnerText = bc.DataField;
                ul.Controls.Add(liPrompt);

                var liInput = new HtmlGenericControl("li");

                var txtFiled = new TextBox();
                txtFiled.AutoPostBack = false;
                txtFiled.ID = "txt" + bc.DataField;

                liInput.Controls.Add(txtFiled);

                ul.Controls.Add(liInput);

                count++;
            }

            if (count > 0)
            {
                var li = new HtmlGenericControl("li");

                var btnFilter = new Button { Text = "Submit", ID = "btn" + this.ID + "Filter" };

                li.Controls.Add(btnFilter);
                ul.Controls.Add(li);

                panel.CssClass = "divSearch";
                panel.Controls.Add(ul);
            }
        }

        public bool IsInteger(string integer)
        {
            try
            {
                Convert.ToInt16(integer);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool IsNumeric(string data)
        {
            var regex = new Regex("^[0-9]*$");
            return regex.IsMatch(data);
        }

        #endregion

        #region Methods

        protected string GetCsvFromDataTable(DataTable dt)
        {
            var sb = new StringBuilder();
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                sb.Append(dt.Columns[k].ColumnName + ',');
            }
            sb.Append("\r\n");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    // CSV rules: http://en.wikipedia.org/wiki/Comma-separated_values#Basic_rules
                    // From the rules:
                    // 1. if the data has quote, escape the quote in the data
                    // 2. if the data contains the delimiter (in our case ','), double-quote it
                    // 3. if the data contains the new-line, double-quote it.
                    var data = dt.Rows[i][k].ToString();
                    if (data.Contains("\""))
                    {
                        data = data.Replace("\"", "\"\"");
                    }

                    if (data.Contains(","))
                    {
                        data = String.Format("\"{0}\"", data);
                    }

                    if (data.Contains(System.Environment.NewLine))
                    {
                        data = String.Format("\"{0}\"", data);
                    }
                    //do not pass min date
                    if (DateTime.MinValue.Date.ToString() == data)
                    {
                        sb.Append(" " + ',');
                    }
                    else
                    {
                        sb.Append(data + ',');

                    }
                }
                sb.Append("\r\n");
            }

            return sb.ToString();
        }
        public DataTable CopyGenericToDataTable<T>(IEnumerable<T> items)
        {
            var properties = typeof(T).GetProperties();
            var result = new DataTable();

            //Build the columns
            foreach (var prop in properties)
            {
                result.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            //Fill the DataTable
            foreach (var item in items)
            {
                var row = result.NewRow();

                foreach (var prop in properties)
                {
                    var itemValue = prop.GetValue(item, new object[] { }) ?? DBNull.Value;
                    row[prop.Name] = itemValue;
                }

                result.Rows.Add(row);
            }

            return result;
        }

        protected void ClearMultipleSelectionCreateControls(HtmlGenericControl div, List<string> values)
        {
        }

        protected void HideLabels()
        {
            if (this.ErrorMessageLabel != null)
            {
                this.ErrorMessageLabel.Visible = false;
            }

            if (this.SuccessMessageLabel != null)
            {
                this.SuccessMessageLabel.Visible = false;
            }
        }

        protected bool IsAlphaNumericWithSpace(string data)
        {
            var regex = new Regex("^[a-zA-Z0-9 ]*$");
            return regex.IsMatch(data);
        }

        protected bool IsAlphabetic(string data)
        {
            var regex = new Regex("^[a-zA-Z ]*$");
            return regex.IsMatch(data);
        }

        protected bool IsValidClientId(string data)
        {
            var regex = new Regex("^[0-9 ]*[a-zA-Z]+[0-9 ]*$");
            return regex.IsMatch(data);
        }

        protected bool IsValidEmailId(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void MatrixSelectionCreateControl(
            HtmlGenericControl div,
            List<string> xvalues,
            IEnumerable<string> yvalues,
            Dictionary<string, List<string>> currentSelection)
        {
            var table = new Table { CssClass = "s-table-matrix-select" };
            div.Controls.Add(table);

            var xHeaderRow = new TableRow();

            var selectAllCell = new TableCell();
            selectAllCell.Controls.Add(new CheckBox { CssClass = "s-matrix-selectall" });
            xHeaderRow.Cells.Add(new TableCell());
            xHeaderRow.Cells.Add(new TableCell());

            var selectAllColumnItemsRow = new TableRow();
            selectAllColumnItemsRow.Cells.Add(selectAllCell);
            selectAllColumnItemsRow.Cells.Add(new TableCell());

            table.Rows.Add(selectAllColumnItemsRow);
            table.Rows.Add(xHeaderRow);

            foreach (string xvalue in xvalues)
            {
                xHeaderRow.Cells.Add(new TableCell { Text = xvalue });
                var selectAllColumsnCell = new TableCell();
                selectAllColumnItemsRow.Cells.Add(selectAllColumsnCell);
                selectAllColumsnCell.Controls.Add(new CheckBox { CssClass = "s-matrix-column-selectall" });
            }

            foreach (string yvalue in yvalues)
            {
                var row = new TableRow();
                table.Rows.Add(row);

                var yHeaderCell = new TableCell { Text = yvalue };

                var selectAllRowsCell = new TableCell();
                selectAllRowsCell.Controls.Add(new CheckBox { CssClass = "s-matrix-row-selectall" });

                row.Cells.Add(selectAllRowsCell);
                row.Cells.Add(yHeaderCell);

                foreach (string xvalue in xvalues)
                {
                    var cell = new TableCell();
                    row.Cells.Add(cell);

                    var chkBox = new CheckBox();
                    chkBox.CssClass = "s-matrix-item-checkbox";
                    chkBox.Checked = currentSelection.ContainsKey(yvalue) && currentSelection[yvalue].Contains(xvalue);
                    cell.Controls.Add(chkBox);
                }
            }

            string script = @"<script type=""text/javascript"">$(function () {
    var matrixTable = $('table.s-table-matrix-select');
    var selectAllCheckBox = matrixTable.find('.s-matrix-selectall input:checkbox');
    var selectAllRowsCheckBox = matrixTable.find('.s-matrix-row-selectall input:checkbox');
    var selectAllColumnsCheckBox = matrixTable.find('.s-matrix-column-selectall input:checkbox');
    var matrixItemCheckBox = matrixTable.find('.s-matrix-item-checkbox input:checkbox');

    selectAllCheckBox.on('change', function (e) {
        var isChecked = $(this).is(':checked');
        $(this).closest('table').find('input:checkbox').prop('checked', isChecked);
    });

    selectAllRowsCheckBox.on('click', function (e) {
        var isChecked = $(this).is(':checked');
        var currentTd = $(this).closest('td');

        while (currentTd.next().length != 0) {
            currentTd = currentTd.next();
            var items = currentTd.find('input:checkbox');
            items.prop('checked', isChecked);
            items.trigger('change');
        }

    });

    selectAllColumnsCheckBox.on('click', function (e) {
        var isChecked = $(this).is(':checked');
        var index = $(this).closest('td').index() + 1;
        var currentTr = $(this).closest('tr');

        while (currentTr.next().length != 0) {
            currentTr = currentTr.next();
            var items = currentTr.find('td:nth-child(' + index + ')')
                .find('input:checkbox');
            items.prop('checked', isChecked);
            items.trigger('change');
        }

    });

    matrixItemCheckBox.on('change', function (e) {
        var parentTr = $(this).closest('tr');
        var chkRow = (parentTr.find('.s-matrix-item-checkbox input:checkbox').length ==
            parentTr.find('.s-matrix-item-checkbox input:checkbox:checked').length);

        var index = $(this).closest('td').index() + 1;
        var columnTds = parentTr.parent().find('tr td:nth-child(' + index + ')');
        var chkColumn = (columnTds.find('.s-matrix-item-checkbox input:checkbox').length ==
            columnTds.find('.s-matrix-item-checkbox input:checkbox:checked').length);

        parentTr.find('.s-matrix-row-selectall input:checkbox').prop('checked', chkRow);
        columnTds.find('.s-matrix-column-selectall input:checkbox').prop('checked', chkColumn);

        if (!$(this).is(':checked'))
            selectAllCheckBox.prop('checked', false);
        else {
            var chkSelectAll = true;
            $.each(selectAllRowsCheckBox, function () {
                if (!$(this).is(':checked'))
                    chkSelectAll = false;
            });
            $.each(selectAllColumnsCheckBox, function () {
                if (!$(this).is(':checked'))
                    chkSelectAll = false;
            });
            selectAllCheckBox.prop('checked', chkSelectAll);
        }
    });
})</script>";

            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "matrix-select", script);
        }

        protected Dictionary<string, List<string>> MatrixSelectionGetSelectedValues(HtmlGenericControl div)
        {
            var dic = new Dictionary<string, List<string>>();
            var tbl = (Table)div.Controls[0];

            for (int i = 2; i < tbl.Rows.Count; i++)
            {
                for (int j = 2; j < tbl.Rows[i].Cells.Count; j++)
                {
                    var chkBox = (CheckBox)tbl.Rows[i].Cells[j].Controls[0];
                    if (chkBox.Checked)
                    {
                        string key = tbl.Rows[i].Cells[1].Text;
                        if (!dic.ContainsKey(key))
                        {
                            dic.Add(key, new List<string>());
                        }

                        dic[key].Add(tbl.Rows[1].Cells[j].Text);
                    }
                }
            }

            return dic;
        }

        protected List<string> MulitpleSelectionGetSelectedValues(HtmlGenericControl div)
        {
            var chkBoxList = FindChildControlsRecursive<CheckBoxList>(div);

            if (chkBoxList == null)
            {
                return new List<string>();
            }

            var lst = new List<string>();

            for (int i = 0; i < chkBoxList.Items.Count; i++)
            {
                if (chkBoxList.Items[i].Selected)
                {
                    lst.Add(chkBoxList.Items[i].Value);
                }
            }

            return lst;
        }

        protected void MultipleSelectionCreateControls(
            HtmlGenericControl div, List<string> values, List<string> selected = null)
        {
            var selectAllDiv = new HtmlGenericControl("div");
            var selectAllCheckBox = new CheckBox { Text = "Select All" };
            selectAllDiv.Style.Add("style", "background-color:#dedede; border-bottom: 1px solid gray");
            selectAllDiv.Controls.Add(selectAllCheckBox);
            selectAllDiv.Attributes.Add(
                "onclick",
                "$(this).parent().find(\"div.items\").find(\"input:checkbox\").prop(\"checked\", $(this).find(\"input:checkbox\").attr(\"checked\") == \"checked\");");

            var listDiv = new HtmlGenericControl("div");
            listDiv.Attributes.Add("class", "items");

            var chkBoxList = new CheckBoxList();

            if (selected == null)
            {
                selected = new List<string>();
            }

            foreach (string value in values)
            {
                var li = new ListItem { Value = value, Text = value, Selected = selected.Contains(value) };

                chkBoxList.Items.Add(li);
            }

            listDiv.Controls.Add(chkBoxList);

            div.Controls.Add(selectAllDiv);
            div.Controls.Add(listDiv);
        }

        protected void MultipleSelectionCreateControls(
            HtmlGenericControl div, List<ListItem> items, List<string> selected = null)
        {
            var selectAllDiv = new HtmlGenericControl("div");
            var selectAllCheckBox = new CheckBox { Text = "Select All" };
            selectAllDiv.Style.Add("style", "background-color:#dedede; border-bottom: 1px solid gray");
            selectAllDiv.Controls.Add(selectAllCheckBox);
            selectAllDiv.Attributes.Add(
                "onclick",
                "$(this).parent().find(\"div.items\").find(\"input:checkbox\").prop(\"checked\",  $(this).find(\"input:checkbox\").is(\":checked\"));");

            var listDiv = new HtmlGenericControl("div");
            listDiv.Attributes.Add("class", "items");

            var chkBoxList = new CheckBoxList();

            if (selected == null)
            {
                selected = new List<string>();
            }

            foreach (ListItem item in items)
            {
                var li = new ListItem { Value = item.Value, Text = item.Text, Selected = selected.Contains(item.Value) };

                chkBoxList.Items.Add(li);
            }

            listDiv.Controls.Add(chkBoxList);

            div.Controls.Add(selectAllDiv);
            div.Controls.Add(listDiv);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.ErrorMessageLabel = FindControlIterative(this, "lblErrorMessage") as Label;
            this.SuccessMessageLabel = FindControlIterative(this, "lblSuccessMessage") as Label;

            base.OnLoad(e);
        }

        protected void ShowErrorMessage(string msg)
        {
            if (this.ErrorMessageLabel != null)
            {
                this.ErrorMessageLabel.Text = msg;
                this.ErrorMessageLabel.Visible = true;
            }
        }

        protected void ShowSuccessMessage(string msg)
        {
            if (this.SuccessMessageLabel != null)
            {
                this.SuccessMessageLabel.Text = msg;
                this.SuccessMessageLabel.Visible = true;
            }
        }

        private static T FindChildControlsRecursive<T>(Control control) where T : class
        {
            return
                (from Control childControl in control.Controls
                 select childControl as T ?? FindChildControlsRecursive<T>(childControl)).FirstOrDefault(
                        obj => null != obj);
        }

        private static Control FindControlIterative(Control control, string id)
        {
            Control ctl = control;

            var controls = new LinkedList<Control>();

            while (ctl != null)
            {
                if (ctl.ID == id)
                {
                    return ctl;
                }

                if (ctl.HasControls())
                {
                    foreach (Control child in ctl.Controls)
                    {
                        if (child.ID == id)
                        {
                            return child;
                        }

                        if (child.HasControls())
                        {
                            controls.AddLast(child);
                        }
                    }
                }

                if (controls.Count == 0)
                {
                    break;
                }

                ctl = controls.First.Value;
                controls.Remove(ctl);
            }

            return null;
        }

        #endregion
    }
}