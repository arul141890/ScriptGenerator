<# 
PowerShell script to Publish application through Remote Desktop Services
This script should be executed on the Session Host server where the application will be hosted
File path should contain the location of .exe file of the application to be published
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
Import-Module ServerManager
import-module RemoteDesktop
$Alias=testakisa
$Displayname=testname
$Connectionbroker=testbroker
$Collectionname=testcoll
$Filepath=c:\test app\test.exe
new-rdremoteapp -Alias $Alias -DisplayName $Displayname -FilePath $Filepath -ShowInWebAccess 1 -collectionname $Collectionname -ConnectionBroker $Connectionbroker
