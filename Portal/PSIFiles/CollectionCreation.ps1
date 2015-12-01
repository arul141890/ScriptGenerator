<# 
PowerShell script to create Remote Desktop Services Application Colection
Remote Desktop Services infrastructure servers should have static IP Address
Publish Applications once the collection is created
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
Import-Module ServerManager
import-module RemoteDesktop
$Collectioname=testcollection
$Collectiondesc=this is a test session
$connectionbroker=testbroker
$Sessionhost=testsession
New-RDSessionCollection -CollectionName $Collectioname -SessionHost $Sessionhost -CollectionDescription $Collectiondesc -ConnectionBroker $connectionbroker
