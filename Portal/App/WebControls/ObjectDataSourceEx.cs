// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectDataSourceEx.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The object data source ex.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI.WebControls;

namespace Portal.App.WebControls
{
    public class ObjectDataSourceEx : ObjectDataSource
    {
        #region Fields

        private bool SelectAllCountMethodExecuted;

        #endregion

        #region Constructors and Destructors

        public ObjectDataSourceEx()
        {
            this.EnablePaging = true;
            this.SelectMethod = "SelectAll";
            this.SelectCountMethod = "SelectAllCount";
            this.SortParameterName = "sortExpression";

            this.UpdateMethod = "Update";
            this.DeleteMethod = "Delete";

            var filterParameter = new Parameter {Name = "filterExpression", DefaultValue = string.Empty};
            this.SelectParameters.Add(filterParameter);

            this.Selecting += this.ObjectDataSourceEx_Selecting;
            this.Selected += this.ObjectDataSourceEx_Selected;
        }

        #endregion

        #region Public Properties

        public int Count { get; set; }

        #endregion

        #region Methods

        private void ObjectDataSourceEx_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (this.SelectAllCountMethodExecuted)
            {
                this.Count = (int) e.ReturnValue;
            }
        }

        private void ObjectDataSourceEx_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            this.SelectAllCountMethodExecuted = e.ExecutingSelectCount;
        }

        #endregion
    }
}