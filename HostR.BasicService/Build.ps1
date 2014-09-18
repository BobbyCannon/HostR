$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath

$projectName = "HostR.BasicService"
$configuration = "Release"
$destination = "C:\Binaries\HostR\$projectName";

if ($args.Count -gt 0) {
	$configuration = $args
}

if (!(Test-Path $destination -PathType Container)){
    New-Item $destination -ItemType Directory | Out-Null
}

Write-Host "Publishing $projectName..." -ForegroundColor Green
Copy-Item "$scriptPath\bin\$configuration\*" "$destination"