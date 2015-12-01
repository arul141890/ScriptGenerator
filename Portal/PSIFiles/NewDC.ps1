<# 
PowerShell script to add a Domain controller to a domain
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
$Hostname=testname
<# Enter the remote session of the server#>
New-PSSession –Name ADDC –ComputerName $Hostname
Enter-PSSession –Name ADDC
Import-Module ServerManager
Install-windowsfeature AD-Domain-Services
Import-Module ADDSDeployment
$Databasepath=C:\Windows\NTDS
$Logpath=C:\Windows\NTDS
$Sysvolpath=C:\Windows\SYSVOL
$Domainname=text.com
$adminpassword =test123
Install-ADDSDomainController -DomainName "$Domainname" -ApplicationPartitionsToReplicate * -DatabasePath "$Databasepath" -CreateDnsDelegation:$false -Force -InstallDns -LogPath "$Logpath"  -SysvolPath "$Sysvolpath" -safemodeadministratorpassword:$adminpassword
