$framework35 = "..\Manatee.Json\bin\Release-3.5\";
$framework40 = "..\Manatee.Json\bin\Release-4.0\";
$framework45 = "..\Manatee.Json\bin\Release-4.5\";
$frameworkCore = "..\Manatee.Json\bin\Release\netstandard1.6\";
$frameworkMono = "..\Manatee.Json\bin\Release-Portable\";

if (!(Test-Path -path $projFile)) {
  Write-Host "Cannot find project/nuspec file '$projFile'"
  return 1;
}

$nuget_exe = ".\nuget.exe"

$sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
Invoke-WebRequest $sourceNugetExe -OutFile $nuget_exe

& $nuget_exe pack "$projFile" -NonInteractive -symbols