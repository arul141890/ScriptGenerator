// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureMapBootstrap.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The platform db type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Core.Data;
using Data.Repositories;
using Data.UnitOfWorks;
using Sevices;
using Sevices.Users;
using StructureMap;

namespace Portal
{
    public enum PlatformDbType
    {
        MySql
    }

    public static class StructureMapBootstrap
    {
        #region Public Methods and Operators

        public static void Configure()
        {
            ObjectFactory.Initialize(
                c =>
                {
                    // database configuration
                    c.For(typeof(IRepository<>)).Use(typeof(Repository<>));

                    c.For<IScriptGeneratorUnitOfWork>().Use<ScriptGeneratorUnitOfWork>();
                    c.SetAllProperties(x => x.WithAnyTypeFromNamespace("Data"));

                 // Service Types Configuration
                    c.Scan(
                        x =>
                        {
                            x.AssemblyContainingType<IUserService>();
                            x.WithDefaultConventions();
                        });
                    c.SetAllProperties(x => x.WithAnyTypeFromNamespace("Services"));
                    c.For(typeof(IScriptGeneratorService<>)).Use(typeof(ScriptGeneratorService<>));

                });
        }

        #endregion
    }
}