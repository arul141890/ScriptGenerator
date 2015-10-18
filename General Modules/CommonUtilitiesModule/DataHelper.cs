using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CommonUtilitiesModule
{
    public static class DataHelper
    {
        public static string GetCsvFromDataTable(DataTable dt, bool hideMinDate=true)
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
                    if (hideMinDate && DateTime.MinValue.Date.ToString() == data)
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
        public static DataTable CopyGenericToDataTable<T>(IEnumerable<T> items)
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
    }
}
