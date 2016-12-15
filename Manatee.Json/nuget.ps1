param([String]$projfile)

if (!(Test-Path -path $projFile)) {
  Write-Host "Cannot find project/nuspec file '$projFile'"
  return 1;
}

$nuget_exe = ".\nuget.exe"

$sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
Invoke-WebRequest $sourceNugetExe -OutFile $nuget_exe

& $nuget_exe pack "$projFile" -NonInteractive -symbols