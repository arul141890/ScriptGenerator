<# 
PowerShell script to create SMB share
SMB share should be created first before proceeding with DFS namespace creation
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
Import-Module ServerManager
$Dirname=C:\test
$smbname=testshare
$accessgroup="test\yuvi"
icacls "$Dirname" /grant "Authenticated Users": (OI)(CI)(M)
New-SmbShare -Name $smbname -Path "$Dirname" -FolderEnumerationMode AccessBased -CachingMode Documents -EncryptData $True -FullAccess "$accessgroup" 
