<# 
PowerShell script to create virtual switch
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted
#>
Import-Module ServerManager
Import-Module Hyper-V
$switchname=ewr
$physicaladapter=rwet
$allowmos=False
New-VMSwitch -Name $switchname -NetAdapterNAme $physicaladapter -AllowMAnagementOS $allowmos
PSSession –Name DNSrecord
Import-Module ServerManager
Import-Module DNSShell
$Zonename=testzone
$Csvfilename=C:\test.csv
Import-CSV $Csvfilename | %{New -DNSRecord -Name $_."HostName" - RecordType A - ZoneName $Zonename - IPAddress $_."IPAddress"}
IPAddr"}
