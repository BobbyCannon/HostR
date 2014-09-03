$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath

$projectName = "HostR"
$configuration = "Release"
$destination = "C:\Binaries\$projectName";
$nugetDestination = "C:\Workspaces\Nuget\Developer"

if ($args.Count -gt 0) {
	$configuration = $args
}

if (!(Test-Path $destination -PathType Container)){
    New-Item $destination -ItemType Directory | Out-Null
}

if (!(Test-Path $nugetDestination -PathType Container)){
    New-Item $nugetDestination -ItemType Directory | Out-Null
}

Write-Host "Deploying HostR..." -ForegroundColor Green
Copy-Item "$scriptPath\bin\$configuration\HostR*.dll" "$destination"

$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$destination\HostR.dll")
$version = $versionInfo.FileVersion.ToString()

cmd /c "..\.nuget\NuGet.exe" pack ..\HostR.nuspec -Prop Configuration="$Configuration" -Version $version
Move-Item "HostR.$version.nupkg" "$destination" -force
Copy-Item "$destination\HostR.$version.nupkg" "$nugetDestination" -force