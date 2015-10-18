// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Field.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The fields.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Tables
{
    using System.Collections.Generic;
    using System.Text;

    public class Fields
    {

        private readonly IList<Field> fields = new List<Field>();



        public Fields Add(string name, object value)
        {
            this.fields.Add(new Field { Name = name, Value = value });
            return this;
        }

        public SqlAndValues ToSqlAndValues()
        {
            var sb = new StringBuilder();
            var values = new object[this.fields.Count];

            if (this.fields.Count > 0)
            {
                sb.Append(" SET ");
            }

            var i = 0;
            var first = true;
            foreach (var field in this.fields)
            {
                sb.AppendFormat("{0} {1}=@{2}", first ? string.Empty : ",", field.Name, i);
                values[i] = field.Value;
                i++;
                first = false;
            }

            return new SqlAndValues { Sql = sb.ToString(), Values = values };
        }


        private class Field
        {

            public string Name { get; set; }

            public object Value { get; set; }

        }
    }
}