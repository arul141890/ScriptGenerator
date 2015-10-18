// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchemaProvider.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The SchemaProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Data.Tables
{
    using System;

    public interface ISchemaProvider
    {

        Schema GetSchema(Type type);

    }
}