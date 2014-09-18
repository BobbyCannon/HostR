$ErrorActionPreference = "Stop"
$watch = [System.Diagnostics.Stopwatch]::StartNew()
$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath

$configuration = "Release"
if ($args.Count -gt 0) {
	$configuration = $args
}

try
{
    & $scriptPath\HostR.Web\Deploy.ps1
    & $scriptPath\HostR.Agent\Deploy.ps1
    & $scriptPath\HostR.BasicService\Deploy.ps1
}
finally
{
    Set-Location $scriptPath
}

Write-Host
Write-Host "HostR Deploy: " $watch.Elapsed -ForegroundColor Yellow