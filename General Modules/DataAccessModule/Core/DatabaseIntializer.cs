// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseIntializer.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The database intializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataAccessModule.Core
{
    using DataAccessModule.Data.Tables;

    using PetaPoco;

    public static class DatabaseIntializer
    {

        static DatabaseIntializer()
        {
            // set default schema provider
            SetSchemaProvider(new AppSchemaProvider());
        }



        public static void SetSchemaProvider(ISchemaProvider schemaProvider)
        {
            Database.Mapper = new CustomMapper(schemaProvider);
        }

    }
}