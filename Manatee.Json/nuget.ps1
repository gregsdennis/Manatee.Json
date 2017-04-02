param([String]$nuspecfile)

Write-Output "Post-build running..."

if (!(Test-Path -path $nuspecfile)) {
  Write-Output "Cannot find project/nuspec file '$nuspecfile'"
  return 1
}

$nuget_exe = ".\.tools\nuget.exe"
$release_dir = ".\.release"

if (!(Test-Path -path ".\.tools")){
  mkdir ".\.tools"
}

if (!(Test-Path -path $nuget_exe)) {
  Write-Output "Attempting to download nuget.exe"
  $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
  Invoke-WebRequest $sourceNugetExe -OutFile $nuget_exe
}

Write-Output "Building project file '$nuspecfile'"

& $nuget_exe pack "$nuspecfile" -NonInteractive -properties Configuration=Release -outputdirectory $release_dir

exit $LASTEXITCODE