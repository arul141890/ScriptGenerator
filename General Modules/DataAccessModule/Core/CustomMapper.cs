// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomMapper.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The custom mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Core
{
    using System;
    using System.Reflection;

    using DataAccessModule.Data.Tables;

    using PetaPoco;

    public class CustomMapper : IMapper
    {

        private readonly ISchemaProvider _schemaProvider;



        public CustomMapper(ISchemaProvider schemaProvider)
        {
            this._schemaProvider = schemaProvider;
        }



        public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
        {
            return null;
        }

        public void GetTableInfo(Type t, TableInfo ti)
        {
            var tableSchema = this._schemaProvider.GetSchema(t);

            if (tableSchema != null)
            {
                ti.AutoIncrement = tableSchema.HasAutoIncrement;
                ti.PrimaryKey = tableSchema.PrimaryKey;
                ti.TableName = tableSchema.TableName;
            }
        }

        public Func<object, object> GetToDbConverter(Type sourceType)
        {
            return null;
        }

        public bool MapPropertyToColumn(PropertyInfo pi, ref string columnName, ref bool resultColumn)
        {
            return true;
        }

    }
}