// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPaymentGatewayUnitOfWork.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The ScriptGeneratorUnitOfWork interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Core.Data;
using Core.Domain;

namespace Data.UnitOfWorks
{
    public interface IScriptGeneratorUnitOfWork : IUnitOfWork
    {
        #region Public Properties

        IRepository<VirtualSwitchCreation> VirtualSwitchCreations { get; }
        IRepository<User> Users { get; }

        #endregion
    }
}