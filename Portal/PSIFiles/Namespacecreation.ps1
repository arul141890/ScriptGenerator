<# 
PowerShell script to create DFS Namespace
Folder should be created and SMB share has to be created before adding DFS Namespace
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
$Hostname=testserver
<# Enter the remote session of the server#>
New-PSSession –Name Namespace –ComputerName $Hostname
Enter-PSSession –Name Namespace
Import-Module ServerManager
$smbname=testshare
$Fileserver=testfileserver
$Domainname=test.com
New-DfsnRoot –Path "\\$Domainname\$smbname" -TargetPath "\\$Fileserver\$smbname" -Type Domainv2 | Format-List
Path: "\\$Domainname\$smbname"
Description : Domain-based $smbname namespace
Type : Domain V2
State : Online
TimeToLiveSec : 300
