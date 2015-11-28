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
using Core.Domain.Activedirectory;
using Core.Domain.dhcp;
using Core.Domain.dns;
using Core.Domain.Filestorage;
using Core.Domain.Hyperv;
using Core.Domain.Remotedesktopservices;
using Core.Domain.Webserver;
using Data.Repositories;

namespace Data.UnitOfWorks
{
    public class ScriptGeneratorUnitOfWork : UnitOfWork, IScriptGeneratorUnitOfWork
    {
        #region Fields

        private Hashtable _repositories;

        #endregion

        #region Public Properties

        public IRepository<Addingdc> Addingdc
        {
            get
            {
                return this.Repository<Addingdc>();
            }
        }


        public IRepository<Addingrodc> Addingrodc
        {
            get
            {
                return this.Repository<Addingrodc>();
            }
        }
        
        
        
        public IRepository<Forestcreation> Forestcreation
        {
            get
            {
                return this.Repository<Forestcreation>();
            }
        }


        public IRepository<Dhcpinstallation> Dhcpinstallation
        {
            get
            {
                return this.Repository<Dhcpinstallation>();
            }
        }
        
        
           public IRepository<Scopecreation> Scopecreation
        {
            get
            {
                return this.Repository<Scopecreation>();
            }
        }


        public IRepository<Dnsinstallation> Dnsinstallation
        {
            get
            {
                return this.Repository<Dnsinstallation>();
            }
        }
        
        
        
        public IRepository<Dnsrecordcreation> Dnsrecordcreation
        {
            get
            {
                return this.Repository<Dnsrecordcreation>();
            }
        }


        public IRepository<Namespacecreation> Namespacecreation
        {
            get
            {
                return this.Repository<Namespacecreation>();
            }
        }        

        
        public IRepository<Roleinstallation> Roleinstallation
        {
            get
            {
                return this.Repository<Roleinstallation>();
            }
        }


        public IRepository<Smbsharecreation> Smbsharecreation
        {
            get
            {
                return this.Repository<Smbsharecreation>();
            }
        }
        
        
        
        public IRepository<Hypervinstallation> Hypervinstallation
        {
            get
            {
                return this.Repository<Hypervinstallation>();
            }
        }


        public IRepository<VirtualDiskCreation> Virtualdiskcreation
        {
            get
            {
                return this.Repository<VirtualDiskCreation>();
            }
        }

        public IRepository<VirtualSwitchCreation> VirtualSwitchCreations
        {
            get
            {
                return this.Repository<VirtualSwitchCreation>();
            }
        }


        public IRepository<VMCreation> VMCreation
        {
            get
            {
                return this.Repository<VMCreation>();
            }
        }


        public IRepository<Apppublish> Apppublish
        {
            get
            {
                return this.Repository<Apppublish>();
            }
        }
        
        
        
        public IRepository<Collectioncreation> Collectioncreation
        {
            get
            {
                return this.Repository<Collectioncreation>();
            }
        }


        public IRepository<Rdsinstallation> Rdsinstallation
        {
            get
            {
                return this.Repository<Rdsinstallation>();
            }
        }        

         public IRepository<Webserverinstallation> Webserverinstallation
        {
            get
            {
                return this.Repository<Webserverinstallation>();
            }
        }


        public IRepository<Websitecreation> Websitecreation
        {
            get
            {
                return this.Repository<Websitecreation>();
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