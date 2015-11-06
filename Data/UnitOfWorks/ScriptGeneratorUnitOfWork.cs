// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptGeneratorUnitOfWork.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The payment gateway unit of work.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using Core.Data;
using Core.Domain;
using Data.Repositories;

namespace Data.UnitOfWorks
{
    public class ScriptGeneratorUnitOfWork : UnitOfWork, IScriptGeneratorUnitOfWork
    {
        #region Fields

        private Hashtable _repositories;

        #endregion

        #region Public Properties

        public IRepository<VirtualSwitchCreation> VirtualSwitchCreations
        {
            get
            {
                return this.Repository<VirtualSwitchCreation>();
            }
        }


        public IRepository<User> Users
        {
            get
            {
                return this.Repository<User>();
            }
        }

        #endregion

        #region Public Methods and Operators

        public override IRepository<T> Repository<T>()
        {
            if (this._repositories == null)
            {
                this._repositories = new Hashtable();
            }

            string type = typeof(T).Name;

            if (!this._repositories.ContainsKey(type))
            {
                Type repositoryType = typeof(Repository<>);

                object repositoryInstance = Activator.CreateInstance(
                    repositoryType.MakeGenericType(typeof(T)), new object[] { this.Context });

                this._repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)this._repositories[type];
        }

        #endregion
    }
}