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
using Core.Domain.Hyperv;
using Core.Domain.Activedirectory;
using Core.Domain.dns;
using Core.Domain.dhcp;
using Core.Domain.Filestorage;
using Core.Domain.Remotedesktopservices;
using Core.Domain.Webserver;

namespace Data.UnitOfWorks
{
    public interface IScriptGeneratorUnitOfWork : IUnitOfWork
    {
        #region Public Properties

        
        IRepository<User> Users { get; }
        IRepository<Addingdc> Addingdc { get; }
        IRepository<Addingrodc> Addingrodc { get; }
        IRepository<Forestcreation> Forestcreation { get; }
        IRepository<Dhcpinstallation> Dhcpinstallation { get; }
        IRepository<Scopecreation> Scopecreation { get; }
        IRepository<Dnsinstallation> Dnsinstallation { get; }
        IRepository<Dnsrecordcreation> Dnsrecordcreation { get; }
        IRepository<Namespacecreation> Namespacecreation { get; }
        IRepository<Roleinstallation> Roleinstallation { get; }
        IRepository<Smbsharecreation> Smbsharecreation { get; }
        IRepository<Hypervinstallation> Hypervinstallation { get; }
        IRepository<VirtualDiskCreation> Virtualdiskcreation { get; }
        IRepository<VirtualSwitchCreation> VirtualSwitchCreations { get; }
        IRepository<VMCreation> VMCreation { get; }
        IRepository<Apppublish> Apppublish { get; }
        IRepository<Collectioncreation> Collectioncreation { get; }
        IRepository<Rdsinstallation> Rdsinstallation { get; }
        IRepository<Webserverinstallation> Webserverinstallation { get; }
        IRepository<Websitecreation> Websitecreation { get; }

        #endregion
    }
}