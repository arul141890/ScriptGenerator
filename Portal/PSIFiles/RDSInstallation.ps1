<# 
PowerShell script to deploy Remote Desktop Services
Remote Desktop Services Infrastructure servers should have a static IP Address
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
Import-Module ServerManager
import-module RemoteDesktop
$Connectionbroker=testbroker
$Webaccess=testwebaccess
$Sessionhost=testsessionhost
$Gateway=testgateway
$GatewayFQDN=test.website.com
New-SessionDeployment -ConnectionBroker $Connectionbroker -WebAccessServer $Webaccess -SessionHost $Sessionhost
<# Specify Gateway and Website FQDN#>
Set-RDDeploymentGatewayConfiguration -GatewayMode Custom -GatewayExternalFqdn $GatewayFQDN -LogonMethod Password -UseCachedCredentials $True -BypassLocal $True -ConnectionBroker $Connectionbroker
