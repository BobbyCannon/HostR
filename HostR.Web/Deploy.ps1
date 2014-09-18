Write-Host "Building and deploying HostR Web (inetpub/wwwroot)..." -ForegroundColor Green

$scriptPath = Split-Path (Get-Variable MyInvocation).Value.MyCommand.Path 
Set-Location $scriptPath

$configuration = "Release"
if ($args.Count -gt 0) {
	$configuration = $args
}

$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
cmd /c $msbuild "$scriptPath\HostR.Web.csproj" /p:Configuration="$configuration" /p:Platform=AnyCPU /p:PublishProfile=Localhost /p:VisualStudioVersion=12.0 /p:DeployOnBuild=True /v:m /m /clp:ErrorsOnly