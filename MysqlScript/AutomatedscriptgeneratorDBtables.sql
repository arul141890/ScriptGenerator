CREATE DATABASE `automatedscriptgenerator` /*!40100 DEFAULT CHARACTER SET utf8 */;
CREATE TABLE `addingdc` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Hostname` varchar(30) NOT NULL,
  `Ipaddress` varchar(30) NOT NULL,
  `Userdomain` varchar(15) NOT NULL,
  `Databasepath` varchar(100) NOT NULL DEFAULT 'C:\\Windows\\NTDS',
  `Logpath` varchar(100) NOT NULL DEFAULT 'C:\\Windows\\NTDS',
  `Sysvolpath` varchar(100) NOT NULL DEFAULT 'C:\\Windows\\SYSVOL',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `addingrodc` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `AllowpasswordreplicationaccountName` varchar(100) NOT NULL,
  `CriticalReplicationOnly` varchar(15) NOT NULL,
  `Delegatedadministratoraccountname` varchar(100) DEFAULT NULL,
  `Denypasswordreplicationaccountname` varchar(100) NOT NULL,
  `DomainName` varchar(30) NOT NULL,
  `InstallDNS` varchar(15) NOT NULL,
  `SiteName` varchar(45) NOT NULL,
  `Hostname` varchar(45) NOT NULL,
  `Ipaddress` varchar(20) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
CREATE TABLE `apppublish` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Alias` varchar(20) NOT NULL,
  `Displayname` varchar(25) NOT NULL,
  `Filepath` varchar(100) NOT NULL,
  `collectionname` varchar(30) NOT NULL,
  `Connectionbroker` varchar(40) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `collectioncreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Collectionname` varchar(30) NOT NULL,
  `Sessionhost` varchar(40) NOT NULL,
  `Collectiondescription` varchar(40) NOT NULL,
  `Connectionbroker` varchar(40) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `dhcpinstallation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Staticip` varchar(20) NOT NULL,
  `Joindomain` varchar(20) NOT NULL,
  `Authorize` varchar(25) DEFAULT NULL,
  `Hostname` varchar(30) NOT NULL,
  `Ipaddress` varchar(20) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `dnsinstallation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Staticip` varchar(30) NOT NULL,
  `Hostname` varchar(30) NOT NULL,
  `Ipaddress` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
CREATE TABLE `dnsrecordcreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Hostname` varchar(30) NOT NULL,
  `Ipaddress` varchar(20) NOT NULL,
  `Zonename` varchar(20) DEFAULT NULL,
  `Csvfilename` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
CREATE TABLE `forestcreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Domainmode` varchar(20) NOT NULL,
  `Domainname` varchar(25) NOT NULL,
  `Domainnetbiosname` varchar(15) NOT NULL,
  `Forestmode` varchar(20) NOT NULL,
  `Hostname` varchar(45) NOT NULL,
  `Ipaddress` varchar(20) NOT NULL,
  `Databasepath` varchar(100) NOT NULL DEFAULT 'C:\\Windows\\NTDS',
  `Logpath` varchar(100) NOT NULL DEFAULT 'C:\\Windows\\NTDS',
  `Sysvolpath` varchar(100) NOT NULL DEFAULT 'C:\\Windows\\SYSVOL',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
CREATE TABLE `hypervinstallation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Isphysicalmachine` varchar(10) NOT NULL,
  `Isvtenabled` varchar(10) NOT NULL,
  `IPAddress` varchar(25) NOT NULL,
  `Hostname` varchar(40) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `namespacecreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Dfsservername` varchar(30) NOT NULL,
  `Dfspath` varchar(100) NOT NULL,
  `Fileservername` varchar(30) NOT NULL,
  `Targetpath` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `rdsinstallation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Connectionbroker` varchar(40) NOT NULL,
  `Webaccessserver` varchar(40) NOT NULL,
  `Sessionhost` varchar(40) NOT NULL,
  `Gatewayserver` varchar(40) NOT NULL,
  `Gatewayfqdn` varchar(40) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `roleinstallation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `IPAddress` varchar(30) NOT NULL,
  `Hostname` varchar(30) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `scopecreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Hostname` varchar(30) NOT NULL,
  `Ipaddress` varchar(30) NOT NULL,
  `Csvfilename` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `smbsharecreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Directoryname` varchar(100) NOT NULL,
  `Smbname` varchar(30) NOT NULL,
  `Encyptdata` varchar(10) NOT NULL,
  `Accessgroups` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `user` (
  `UserId` varchar(30) NOT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `PasswordHash` char(32) NOT NULL,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Id` int(5) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserId_UNIQUE` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
CREATE TABLE `virtualdiskcreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `VHDPath` varchar(150) NOT NULL,
  `VHDSize` varchar(20) NOT NULL,
  `VHDType` varchar(45) NOT NULL,
  `ParentPath` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `virtualswitchcreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `SwitchName` varchar(45) NOT NULL,
  `PhysicalAdapter` varchar(45) NOT NULL,
  `AllowManagementOs` varchar(45) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
CREATE TABLE `vmcreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Vmname` varchar(45) NOT NULL,
  `Vmpath` varchar(150) NOT NULL,
  `Physicaladapter` varchar(45) NOT NULL,
  `SwitchName` varchar(45) NOT NULL,
  `Maxmem` varchar(20) NOT NULL,
  `Minmem` varchar(20) NOT NULL,
  `Isopath` varchar(150) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `webserverinstallation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Ipaddress` varchar(20) NOT NULL,
  `Hostname` varchar(40) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `websitecreation` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreatedDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(30) DEFAULT NULL,
  `Apppoolname` varchar(20) NOT NULL,
  `Websitename` varchar(25) NOT NULL,
  `Portnumber` varchar(6) NOT NULL,
  `Physicalpath` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
