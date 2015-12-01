<# 
PowerShell script to Install DNS Server
DNS Server should have a static IP address
Install DNS Server before installing Active Directory server in windows 2012 R2 Environment
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
$Hostname=testvm
<# Enter the remote session of the server#>
New-PSSession –Name DNSinstall –ComputerName $Hostname
Enter-PSSession –Name DNSinstall
Import-Module ServerManager
$filename=C:\test.csv
Import-CSV $filename | %{Add-DhcpServerv4Scope -Name $_."Name" -StartRange $_."StartRange" -EndRange $_."EndRange" -SubnetMask $_."SubnetMask"}
