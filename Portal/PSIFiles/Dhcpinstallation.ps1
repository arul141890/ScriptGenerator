<# 
PowerShell script to create virtual switch
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
Import-Module ServerManager
Import-Module Hyper-V
$switchname=--SELECT--
$physicaladapter=--SELECT--
$allowmos=True
New-VMSwitch -Name $switchname -NetAdapterNAme $physicaladapter -AllowMAnagementOS $allowmos
