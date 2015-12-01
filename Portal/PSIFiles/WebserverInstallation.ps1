<# 
PowerShell script to create install webserver IIS
IIS installation include all sub features and management tools
Execute the below command if powershell script execution is disabled
set-executionpolicy unrestricted #>
$hostname=testname
<# Enter the server session #>
New - PSSession –Name IISinstallation –ComputerName $hostname
Enter - PSSession –Name IISinstallation
<# Import server manager module #>
Import-Module ServerManager
<# Install web server role with all sub features and also includes management tools #>
add-windowsfeature web-server -includeallsubfeature -includeManagementTools
