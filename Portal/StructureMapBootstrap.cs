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
(System.Action<IInitializationExpression>)                (                c =>
                {
                    // database configuration
                    c.For(typeof(IRepository<>)).Use(typeof(Repository<>));

                    c.For<IScriptGeneratorUnitOfWork>().Use<ScriptGeneratorUnitOfWork>();
                    c.SetAllProperties(x => x.WithAnyTypeFromNamespace("Data"));

                 // Service Types Configuration
                    c.Scan(
(System.Action<StructureMap.Graph.IAssemblyScanner>)                        (                        x =>
                        {
                            x.AssemblyContainingType<Sevices.Users.IUserService>();
                            x.WithDefaultConventions();
                        }));
                    c.SetAllProperties(x => x.WithAnyTypeFromNamespace("Services"));
                    c.For(typeof(Sevices.IScriptGeneratorService<>)).Use(typeof(Sevices.ScriptGeneratorService<>));

                }));
        }

        #endregion
    }
}