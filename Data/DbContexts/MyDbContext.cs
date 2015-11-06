// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyDbContext.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The my db context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Data.Initializers;

namespace Data.DbContexts
{
    public class MyDbContext : DbContext
    {
        #region Constructors and Destructors

        public MyDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.SetInitializer(new MyCreateDatabaseIfNotExists<MyDbContext>());
        }

        #endregion

        #region Methods

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // EF6 feature
            modelBuilder.Configurations.AddFromAssembly(this.GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}