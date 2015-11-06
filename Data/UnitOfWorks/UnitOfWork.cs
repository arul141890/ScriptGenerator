// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The unit of work.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data.Entity.Validation;
using Core.Data;
using Data.DbContexts;

namespace Data.UnitOfWorks
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        #region Fields

        protected readonly MyDbContext Context;

        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        protected UnitOfWork()
        {
            this.Context = new MyDbContext("ConnectionString");
        }

        #endregion

        #region Public Methods and Operators

        public void Commit()
        {
            try
            {
                this.Context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                Exception ex = dbEx.DbEntityValidationExceptionToException();
                throw ex;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }
            }

            this._disposed = true;
        }

        public abstract IRepository<T> Repository<T>();

        #endregion
    }
}