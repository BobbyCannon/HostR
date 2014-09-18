function ZipFiles( $zipfilename, $sourcedir )
{
    if (Test-Path $zipfilename -Type Leaf) {
        Remove-Item $zipfilename -Force
    }

    $zipfilename
    Add-Type -Assembly System.IO.Compression.FileSystem
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir, $zipfilename, $compressionLevel, $false)
}

$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath

if ($args.Count -gt 0) {
	$configuration = $args
}

$projectName = "HostR.BasicService"
$configuration = "Release"
$source = "$scriptPath\bin\$configuration"
$destination = "C:\inetpub\wwwroot\appdata";

if (!(Test-Path $destination -PathType Container)){
    New-Item $destination -ItemType Directory | Out-Null
}

$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$source\$projectName.exe")
$version = $versionInfo.FileVersion.ToString()
$destinationFilePath = "$destination\$projectName-$version.zip"

Write-Host
Write-Host "Deploying $projectName (inetpub/wwwroot/appdata)..." -ForegroundColor Green
ZipFiles $destinationFilePath $source