<# 
PowerShell script to create Active Directory Forest
This script will create a new AD forest and install the first domain controller
Active Directory Domain controller should have a static IP Address
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
$Hostname=testname
<# Enter the remote session of the server#>
New-PSSession –Name ADForest –ComputerName $Hostname
Enter-PSSession –Name ADForest
Import-Module ServerManager
Install-windowsfeature AD-Domain-Services
Import-Module ADDSDeployment
$Databasepath=C:\Windows\NTDS
$Logpath=C:\Windows\NTDS
$Sysvolpath=C:\Windows\SYSVOL
$Domainname=test.com
$Domainnetbios=TEST
$Domainmode=Win2012R2
$Forestmode=Win2012R2
$adminpassword =test
Install-ADDSForest -CreateDnsDelegation:$false -DatabasePath "$Databasepath" -LogPath "$Logpath"  -SysvolPath "$Sysvolpath" -safemodeadministratorpassword:$adminpassword -DomainMode "$Domainmode" -DomainName "$Domainname" -DomainNetbiosName "$Domainnetbios" -ForestMode "$Forestmode" -InstallDns:$true -NoRebootOnCompletion:$false -Force:$true
