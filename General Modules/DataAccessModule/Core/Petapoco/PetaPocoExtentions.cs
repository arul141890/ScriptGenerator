// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PetaPocoExtentions.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PetaPoco
{
    using System.Collections.Generic;
    using System.Linq;

    public partial class Database
    {

        public void BatchInsert<T>(IEnumerable<T> collection)
        {
            try
            {
                this.OpenSharedConnection();
                using (var cmd = this.CreateCommand(this._sharedConnection, string.Empty))
                {
                    var pd = PocoData.ForType(typeof(T));
                    var tableName = this.EscapeTableName(pd.TableInfo.TableName);
                    var cols = string.Join(
                        ", ", (from c in pd.QueryColumns select tableName + "." + this.EscapeSqlIdentifier(c)).ToArray());
                    var pocoValues = new List<string>();
                    var index = 0;
                    foreach (var poco in collection)
                    {
                        var values = new List<string>();
                        foreach (var i in pd.Columns)
                        {
                            values.Add(string.Format("{0}{1}", this._paramPrefix, index++));
                            this.AddParam(cmd, i.Value.GetValue(poco), this._paramPrefix);
                        }

                        pocoValues.Add("(" + string.Join(",", values.ToArray()) + ")");
                    }

                    var sql = string.Format(
                        "INSERT INTO {0} ({1}) VALUES {2}", tableName, cols, string.Join(", ", pocoValues));
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                this.CloseSharedConnection();
            }
        }

    }
}