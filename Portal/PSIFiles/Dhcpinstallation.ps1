<# 
PowerShell script to Install DHCP Server
DHCP server should have a static IP Address
DHCP server should be joined to domain and authorized to issue IP Addresses to the clients.
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
$Hostname=test
<# Enter the remote session of the server#>
New-PSSession –Name DHCPinstall –ComputerName $Hostname
Enter-PSSession –Name DHCPinstall
Import-Module ServerManager
$IPAddress=10.18.240.109
Add-WindowsFeature -IncludeManagementTools dhcp
netsh dhcp add securitygroups
Restart-service dhcpserver
$authorize=True
if($authorize -eq "True"){Add-DhcpServerInDC  $Hostname  $IPAddress}
Set-ItemProperty –Path registry::HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\ServerManager\Roles\12 –Name ConfigurationState –Value 2
