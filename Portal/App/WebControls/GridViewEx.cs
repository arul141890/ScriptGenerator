// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridViewEx.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The grid view ex.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal.App.WebControls
{
    public sealed class GridViewEx : GridView
    {
        #region Static Fields

        public static string ArrowDownImage = "~\\App\\Resource\\img\\Desc.png";

        public static string ArrowUpImage = "~\\App\\Resource\\img\\Asc.png";

        #endregion

        #region Constructors and Destructors

        public GridViewEx()
        {
            this.AllowPaging = true;
            this.PageSize = 15;

            this.AllowSorting = true;
            this.AutoGenerateColumns = false;

            this.EnableEdit = true;
            this.EnableDelete = true;

            this.CssClass = "gridview";
            this.AlternatingRowStyle.CssClass = "gridview_alter";
            this.PagerStyle.CssClass = "gridview_pager";

            this.EmptyDataText = "0 records found";
            this.ShowSortIndicators = true;

            this.ReadOnlyProperties = string.Empty;

            this.RowCreated += GridViewExRowCreated;
            this.RowDataBound += GridViewExRowDataBound;

            this.DataFieldMapDictionaries = new Dictionary<string, Dictionary<string, string>>();
            this.EditModeDropDownFields = new Dictionary<string, Dictionary<string, string>>();
        }

        #endregion

        #region Public Properties

        public Dictionary<string, Dictionary<string, string>> DataFieldMapDictionaries { get; set; }

        public string EditButtonUrlFields { get; set; }

        public string EditButtonUrlFormatString { get; set; }

        public Dictionary<string, Dictionary<string, string>> EditModeDropDownFields { get; set; }

        public bool EnableDelete
        {
            get
            {
                return (this.ViewState["EnableDelete"] != null) && Convert.ToBoolean(this.ViewState["EnableDelete"]);
            }

            set { this.ViewState["EnableDelete"] = value; }
        }

        public bool EnableEdit
        {
            get { return (this.ViewState["EnableEdit"] != null) && Convert.ToBoolean(this.ViewState["EnableEdit"]); }

            set { this.ViewState["EnableEdit"] = value; }
        }

        public string HiddenProperties
        {
            get { return (this.ViewState["HiddenProperties"] ?? string.Empty).ToString(); }

            set { this.ViewState["HiddenProperties"] = value; }
        }

        public string ReadOnlyProperties
        {
            get { return (this.ViewState["ReadOnlyProperties"] ?? string.Empty).ToString(); }

            set { this.ViewState["ReadOnlyProperties"] = value; }
        }

        public bool ShowSortIndicators
        {
            get
            {
                object o = this.ViewState["ShowSortIndicators"];
                if (o != null)
                {
                    return (bool) o;
                }

                return true;
            }

            set { this.ViewState["ShowSortIndicators"] = value; }
        }

        public string SortAscendingImage
        {
            get
            {
                object o = this.ViewState["SortAscendingImage"];
                if (o != null)
                {
                    return (string) o;
                }

                return ArrowDownImage;
            }

            set { this.ViewState["SortAscendingImage"] = value; }
        }

        public string SortDescendingImage
        {
            get
            {
                object o = this.ViewState["SortDescendingImage"];
                if (o != null)
                {
                    return (string) o;
                }

                return ArrowUpImage;
            }

            set { this.ViewState["SortDescendingImage"] = value; }
        }

        #endregion

        #region Properties

        private List<string> HiddenPropertiesList
        {
            get
            {
                return this.HiddenProperties.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        private List<string> ReadOnlyPropertiesList
        {
            get
            {
                return this.ReadOnlyProperties.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void GenerateColumns()
        {
            if (this.AutoGenerateColumns)
            {
                throw new Exception("GridViewEx: AutoGenerateColumns must be set to false.");
            }

            var dataSource = this.GetDataSource() as ObjectDataSource;
            if (null == dataSource)
            {
                return;
            }

            Type dataObjectType = BuildManager.GetType(dataSource.DataObjectTypeName, false);

            if (null != dataObjectType)
            {
                IOrderedEnumerable<PropertyInfo> allProperties =
                    dataObjectType.GetProperties().Where(
                        x =>
                            x.PropertyType.IsPrimitive || x.PropertyType == typeof (string)
                            || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?)).OrderBy(x => x.DeclaringType == dataObjectType);

                foreach (PropertyInfo property in allProperties)
                {
                    bool readOnly = this.ReadOnlyPropertiesList.Contains(property.Name);
                    bool visibility = !this.HiddenPropertiesList.Contains(property.Name);

                    if (property.PropertyType == typeof (bool))
                    {
                        var checkBoxField = new CheckBoxField
                        {
                            DataField = property.Name,
                            HeaderText = property.Name,
                            SortExpression = property.Name,
                            ReadOnly = readOnly,
                            Visible = visibility
                        };

                        this.Columns.Add(checkBoxField);
                    }
                    else
                    {
                        var field = new BoundField
                        {
                            DataField = property.Name,
                            HeaderText = property.Name,
                            SortExpression = property.Name,
                            ReadOnly = readOnly,
                            Visible = visibility
                        };

                        if (this.EditModeDropDownFields.ContainsKey(property.Name))
                        {
                            // TODO: set drop down as edit mode control
                        }

                        this.Columns.Add(field);
                    }

                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        var parameter = new Parameter
                        {
                            Type = TypeCode.DateTime,
                            DbType = DbType.Object,
                            Name = property.Name,
                            Direction = ParameterDirection.Input,
                            DefaultValue = null
                        };

                        dataSource.UpdateParameters.Add(parameter);
                    }
                }
            }

            if (this.EnableEdit || this.EnableDelete)
            {
                var cf = new CommandField
                {
                    ShowEditButton = this.EnableEdit,
                    ShowDeleteButton = this.EnableDelete,
                    HeaderText = "Commands"
                };

                this.Columns.Add(cf);
            }

            this.RowEditing += GridViewExRowEditing;
        }

        public int GetColumnIndexFromDataFieldName(string fieldName)
        {
            int i = -1;

            foreach (DataControlField column in this.Columns)
            {
                i++;
                var bc = column as BoundField;
                if (bc != null && bc.DataField == fieldName)
                {
                    return i;
                }
            }

            return -1;
        }

        public Type GetDataObjectType()
        {
            var dataSource = this.GetDataSource() as ObjectDataSource;
            if (null == dataSource)
            {
                return null;
            }

            Type dataObjectType = BuildManager.GetType(dataSource.DataObjectTypeName, false);

            return dataObjectType;
        }

        #endregion

        #region Methods

        private static void GridViewExRowCreated(object sender, GridViewRowEventArgs e)
        {
            var gv = sender as GridViewEx;
            if (gv == null)
            {
                return;
            }

            // Show sort indicators
            if (gv.ShowSortIndicators)
            {
                if (e.Row != null && e.Row.RowType == DataControlRowType.Header)
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        if (!cell.HasControls())
                        {
                            continue;
                        }

                        var button = (LinkButton) (cell.Controls[0]);

                        var image = new Image {ImageUrl = "images/default.gif", ImageAlign = ImageAlign.Baseline};

                        if (gv.SortExpression != button.CommandArgument)
                        {
                            continue;
                        }

                        image.ImageUrl = gv.SortDirection == SortDirection.Ascending
                            ? gv.SortAscendingImage
                            : gv.SortDescendingImage;

                        var space = new Literal {Text = "&nbsp;"};
                        cell.Controls.Add(space);
                        cell.Controls.Add(image);
                    }
                }
            }
        }

        private static void GridViewExRowDataBound(object sender, GridViewRowEventArgs e)
        {
            var gv = sender as GridViewEx;
            if (gv == null)
            {
                return;
            }

            if (e.Row != null && e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (DataControlFieldCell cell in e.Row.Cells)
                {
                    foreach (Control control in cell.Controls)
                    {
                        // Add delete confirmation
                        var button = control as LinkButton;
                        if (button != null && button.CommandName == "Delete")
                        {
                            button.OnClientClick = " return confirm('Confirm delete?');";
                        }
                    }
                }

                foreach (string field in gv.DataFieldMapDictionaries.Keys)
                {
                    Dictionary<string, string> dic = gv.DataFieldMapDictionaries[field];
                    int index = gv.GetColumnIndexFromDataFieldName(field);
                    string oldText = e.Row.Cells[index].Text;

                    if (dic.ContainsKey(oldText))
                    {
                        e.Row.Cells[index].Text = dic[oldText];
                    }
                }
            }
        }

        private static void GridViewExRowEditing(object sender, GridViewEditEventArgs e)
        {
            var gv = sender as GridViewEx;
            if (gv == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(gv.EditButtonUrlFormatString))
            {
                // redirect to edit page
                string[] fieldNames = gv.EditButtonUrlFields.Split(
                    ",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int count = fieldNames.Length;

                var values = new object[count];

                for (int i = 0; i < count; i++)
                {
                    GridViewRow row = gv.Rows[e.NewEditIndex];
                    values[i] = row.Cells[gv.GetColumnIndexFromDataFieldName(fieldNames[i])].Text;
                }

                string url = string.Format(gv.EditButtonUrlFormatString, values);

                HttpContext.Current.Response.Redirect(url);
            }
        }

        #endregion
    }
}