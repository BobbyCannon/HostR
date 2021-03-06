param (
    [Parameter()]
    [switch] $IncludeDocumentation,
    [Parameter()]
    [string] $Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$watch = [System.Diagnostics.Stopwatch]::StartNew()
$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath
$destination = "C:\Binaries\HostR"

if (Test-Path $destination -PathType Container){
    Remove-Item $destination -Recurse -Force
}

$build = [Math]::Floor([DateTime]::UtcNow.Subtract([DateTime]::Parse("01/01/2000").Date).TotalDays)
$revision = [Math]::Floor([DateTime]::UtcNow.TimeOfDay.TotalSeconds / 2)

$folders = Get-ChildItem | ?{ $_.PSIsContainer }
foreach ($folder in $folders) {
    .\IncrementVersion.ps1 $folder.Name $build $revision
}

Write-Host "Building HostR (build: $build, revision: $revision)..." -ForegroundColor Green

try
{
    $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
    cmd /c $msbuild "$scriptPath\HostR.sln" /p:Configuration="$Configuration" /p:Platform="Any CPU" /t:Rebuild /p:VisualStudioVersion=12.0 /v:m /m /clp:ErrorsOnly

    foreach ($folder in $folders) {
        $folderName = $folder.Name
        if (Test-Path -Type Leaf "$folderName\Build.ps1") {
            & .\$folderName\Build.ps1 "$Configuration"
            Set-Location $scriptPath
        }
    }

    ResetAssemblyInfos
}
finally
{
    Set-Location $scriptPath
}

Write-Host
Write-Host "HostR Build: " $watch.Elapsed -ForegroundColor Yellow