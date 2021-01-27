#
# Usage deploy-nugets -Solution .. -Target C:\Nuget\local
#
[CmdletBinding()]
Param (
    [Parameter(Mandatory=$true)] [string] $Solution,
    [Parameter(Mandatory=$true)] [string] $Target
)
 
Process {
 $Files = Get-ChildItem -Path $Solution -Filter *.nupkg -Recurse -Name 
            try {
            foreach ($File in $Files) {
                Join-Path -Path $Solution -ChildPath $File | Copy-Item -Destination $Target
            }
        } catch {
            Write-Error $_.Exception.Message
        }
}