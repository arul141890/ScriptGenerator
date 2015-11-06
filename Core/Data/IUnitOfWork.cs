// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The UnitOfWork interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Core.Data
{
    public interface IUnitOfWork : IDisposable
    {
        #region Public Methods and Operators

        void Commit();

        IRepository<T> Repository<T>();

        #endregion
    }
}