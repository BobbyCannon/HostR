$ErrorActionPreference = "Stop"
$watch = [System.Diagnostics.Stopwatch]::StartNew()
$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath
$destination = "C:\Binaries\HostR"

if (Test-Path $destination -PathType Container){
    Remove-Item $destination -Recurse -Force
}

$configuration = "Release"
if ($args.Count -gt 0) {
	$configuration = $args
}

$build = [Math]::Floor([DateTime]::UtcNow.Subtract([DateTime]::Parse("01/01/2000").Date).TotalDays)
$revision = [Math]::Floor([DateTime]::UtcNow.TimeOfDay.TotalSeconds / 2)

.\IncrementVersion.ps1 HostR $build $revision

Write-Host "Building HostR (build: $build, revision: $revision)..." -ForegroundColor Green

try
{
    $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
    cmd /c $msbuild "$scriptPath\HostR.sln" /p:Configuration="$configuration" /p:Platform="Any CPU" /t:Rebuild /p:VisualStudioVersion=12.0 /v:m /m /clp:ErrorsOnly

    .\HostR\Deploy.ps1 "$configuration"
    Set-Location $scriptPath
        
    ResetAssemblyInfos
}
finally
{
    Set-Location $scriptPath
}

Write-Host "HostR Deploy: " $watch.Elapsed -ForegroundColor Yellow
Write-Host