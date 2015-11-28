<# 
PowerShell script to create virtual switch
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
Import-Module ServerManager
Import-Module Hyper-V
$switchname=test1
$physicaladapter=test1
$allowmos=False
New-VMSwitch -Name $switchname -NetAdapterNAme $physicaladapter -AllowMAnagementOS $allowmos
