$nuget_exe = ".\nuget.exe"

$sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
Invoke-WebRequest $sourceNugetExe -OutFile $nuget_exe

. $nuget_exe restore "Manatee.Json.sln" -NonInteractive

exit $lastExitCode