REM build script for MyGet builds

set config=%1
if "%config%" == "" (
   set config=Release
)
set PackageVersion=

REM Restore packages
call powershell "& .\nuget-restore.ps1"

REM Detect MSBuild 15.0 path
if exist "%programfiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" (
    set msbuild="%programfiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
    set mstest="%programfiles(x86)%\Microsoft Visual Studio\2017\Community\Common7\IDE\MSTest.exe"
)
if exist "%programfiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" (
    set msbuild="%programfiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
    set mstest="%programfiles(x86)%\Microsoft Visual Studio\2017\Professional\Common7\IDE\MSTest.exe"
)
if exist "%programfiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" (
    set msbuild="%programfiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
    set mstest="%programfiles(x86)%\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\MSTest.exe"
)

REM *** NOTE *** When running this locally, use the version without the quotes.
REM              MyGet requires the quotes, but the command line doesn't like them.  Don't ask me...

call "%msbuild%" Manatee.Json.sln /p:Configuration="%config%" /m:1 /v:m /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Run Tests

"%mstest%" /testcontainer:Manatee.Json.Tests\bin\%config%\Manatee.Json.Tests.dll

REM Package
call "%msbuild%" Manatee.Json\Manatee.Json.csproj /t:pack /p:Configuration=Release
