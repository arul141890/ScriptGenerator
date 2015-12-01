<# 
PowerShell script to create multiple A records creation
CSV file name should contain the complete path to the CSV file
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
$Hostname=test
<# Enter the remote session of the server#>
New-PSSession –Name DNSrecord –ComputerName $Hostname
Enter-PSSession –Name DNSrecord
Import-Module ServerManager
Import-Module DNSShell
$Zonename=testzone
$Csvfilename=C:\test.csv
Import-CSV $Csvfilename | %{New -DNSRecord -Name $_."HostName" - RecordType A - ZoneName $Zonename - IPAddress $_."IPAddress"}
IPAddr"}
