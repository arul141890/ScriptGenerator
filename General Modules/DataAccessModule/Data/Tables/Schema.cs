// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Schema.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The schema.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Tables
{
    public class Schema
    {

        public bool HasAutoIncrement { get; set; }

        public string PrimaryKey { get; set; }

        public string TableName { get; set; }

    }
}