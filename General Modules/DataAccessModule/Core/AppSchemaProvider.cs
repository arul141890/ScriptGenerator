// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSchemaProvider.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The app schema provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Core
{
    using System;
    using System.Collections.Generic;

    using DataAccessModule.Data.Tables;

    public class AppSchemaProvider : ISchemaProvider
    {

        private readonly Dictionary<Type, Schema> _schemata = new Dictionary<Type, Schema>();



        public void AddSchema(Type type, string tableName, string primaryKey, bool hasAutoIncrement)
        {
            this._schemata.Add(
                type, new Schema { HasAutoIncrement = hasAutoIncrement, PrimaryKey = primaryKey, TableName = tableName });
        }

        public Schema GetSchema(Type type)
        {
            Schema schema = null;

            if (this._schemata.ContainsKey(type))
            {
                schema = this._schemata[type];
            }
            else if (type.IsSubclassOf(typeof(AppModel)))
            {
                schema = new Schema { HasAutoIncrement = true, PrimaryKey = "Id", TableName = type.Name };

                return schema;
            }

            return schema;
        }

    }
}