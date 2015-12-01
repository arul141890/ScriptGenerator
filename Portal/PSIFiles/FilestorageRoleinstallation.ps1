<# 
PowerShell script to install File Services
This will install File Services Role service along with management tools
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
$Hostname=testname
<# Enter the remote session of the server#>
New-PSSession –Name FSinstall –ComputerName $Hostname
Enter-PSSession –Name FSinstall
Import-Module ServerManager
Install-WindowsFeature File-Services -IncludeManagementTools
Install-WindowsFeature FS-Resource-Manager, FS-BranchCache, FS-Data-Deduplication, FS-DFS-Namespace, FS-DFS-Replication, FS-VSS-Agent, FS-iSCSITarget-Server, iSCSITarget-VSS-VDS, FS-NFS-Service
