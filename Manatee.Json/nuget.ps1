param([String]$nuspecfile)

if (!(Test-Path -path $nuspecfile)) {
  Write-Output "Cannot find project/nuspec file '$nuspecfile'"
  return 1
}

$nuget_exe = ".\nuget.exe"

if (!(Test-Path -path $nuget_exe)) {
  $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
  Invoke-WebRequest $sourceNugetExe -OutFile $nuget_exe
}

Write-Output "Building project file '$nuspecfile'"

& $nuget_exe pack "$nuspecfile" -NonInteractive -properties Configuration=Release

exit $LASTEXITCODE