<# 
PowerShell script to create a website
This script needs to be executed on a server where website is to be hosted
The Physical path of the code specified should exist before executing this script
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
Import-Module ServerManager
Import-Module WebAdministration
$Apppool=testpool
$Website=defaultwebsite
$Portnumber=8080
$Physicalpath="c:\test\test page"
New-WebAppPool $Apppool -force
new-website -name $Website -port $Portnumber -Physicalpath $Physicalpath -ApplicationPool $Apppool
