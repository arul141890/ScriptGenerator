<# 
PowerShell script to Add a new Read Only Domain Controller
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
$Hostname=test
<# Enter the remote session of the server#>
New-PSSession –Name ADRODC –ComputerName $Hostname
Enter-PSSession –Name ADRODC
Import-Module ServerManager
Install-windowsfeature AD-Domain-Services
Import-Module ADDSDeployment
$AllowpraName=("test\arul.rajendran","test\yuvi")
$delegatedadminacc=("test\sam")
$DenypraName=("test\arul.rajendran")
$Domainname=tesdt.com
$SiteName=default site
$Dbpath=C:\Windows\NTDS
$Logpath=C:\Windows\NTDS
$Sysvolpath=C:\Windows\SYSVOL
Install-ADDSDomainController -AllowPasswordReplicationAccountName @$AllowpraName -DelegatedAdministratorAccountName @$delegatedadminacc -DenyPasswordReplicationAccountName @$DenypraName  -Credential (Get-Credential) -CriticalReplicationOnly:$false -DomainName "$Domainname" -ApplicationPartitionsToReplicate * -CreateDnsDelegation:$false -DatabasePath "$Dbpath" -LogPath "$Logpath" -SysvolPath "$Sysvolpath" -DomainName "$Domainname" -ReadOnlyReplica:$true -SiteName "$SiteName" -InstallDns:$true -Force:$true
