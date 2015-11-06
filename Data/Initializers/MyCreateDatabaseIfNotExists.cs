// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyCreateDatabaseIfNotExists.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The my create database if not exists.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using Data.Initializers.Seeders;

namespace Data.Initializers
{
    public class MyCreateDatabaseIfNotExists<T> : IDatabaseInitializer<T>
        where T : DbContext
    {
        #region Public Methods and Operators

        public void InitializeDatabase(T context)
        {
            if (!context.Database.Exists())
            {
                try
                {
                    context.Database.Create();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Specified key was too long; max key length is 767 bytes"))
                    {
                        // ignore mysql exception of max key length
                    }
                    else
                    {
                        throw;
                    }
                }

                this.Seed(context);
                context.SaveChanges();
            }
        }

        #endregion

        #region Methods

        protected void Seed(T context)
        {
            DatabaseSeeder.Seed(context);
        }

        #endregion
    }
}