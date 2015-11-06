using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Portal.App.WebControls
{
    public class GridViewDropDownTemplate : ITemplate
    {
        //A variable to hold the type of ListItemType.
        ListItemType _templateType;

        //A variable to hold the column name.
        string _columnName;

        //Constructor where we define the template type and column name.
        public GridViewDropDownTemplate(ListItemType type, string colname)
        {
            //Stores the template type.
            _templateType = type;

            //Stores the column name.
            _columnName = colname;
        }

        void ITemplate.InstantiateIn(System.Web.UI.Control container)
        {

            switch (_templateType)
            {

                case ListItemType.Header:
                    //Creates a new label control and add it to the container.
                    var lbl = new Label();            //Allocates the new label object.
                    lbl.Text = _columnName;             //Assigns the name of the column in the lable.
                    container.Controls.Add(lbl);        //Adds the newly created label control to the container.

                    break;

                case ListItemType.Item:
                    //Creates a new text box control and add it to the container.
                    var tb1 = new TextBox();                            //Allocates the new text box object.
                    tb1.DataBinding += new EventHandler(tb1_DataBinding);   //Attaches the data binding event.
                    tb1.Columns = 4;                                        //Creates a column with size 4.
                    container.Controls.Add(tb1);                            //Adds the newly created textbox to the container.

                    break;

                case ListItemType.EditItem:

                    //As, I am not using any EditItem, I didnot added any code here.
                    break;

                case ListItemType.Footer:

                    CheckBox chkColumn = new CheckBox();
                    chkColumn.ID = "Chk" + _columnName;
                    container.Controls.Add(chkColumn);
                    break;

            }

        }





        void tb1_DataBinding(object sender, EventArgs e)
        {
            var txtdata = (TextBox)sender;
            var container = (GridViewRow)txtdata.NamingContainer;

            object dataValue = DataBinder.Eval(container.DataItem, _columnName);

            if (dataValue != DBNull.Value)
            {
                txtdata.Text = dataValue.ToString();
            }
        }
    }
}