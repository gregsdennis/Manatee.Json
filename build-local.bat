REM build script for MyGet builds

REM Start build
set config=%1
if "%config%" == "" (
   set config=Release
)
set PackageVersion=

REM Restore packages
call powershell "& .\nuget-restore.ps1"

REM Run build
call msbuild Manatee.Json.sln /p:Configuration="%config%" /m:1 /v:m /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Get latest for test suites
cd json-patch-tests
git pull https://github.com/json-patch/json-patch-tests.git master
cd ../Json-Path-Test-Suite
git pull https://github.com/gregsdennis/JSON-Path-Test-Suite.git master
cd ../Json-Schema-Test-Suite
git pull https://github.com/json-schema-org/JSON-Schema-Test-Suite.git master
cd ..

REM Run tests
set nunit="packages\NUnit.ConsoleRunner.3.7.0\tools\nunit3-console.exe"
%nunit% Manatee.Json.Tests\bin\%config%\Manatee.Json.Tests.dll
if not "%errorlevel%"=="0" goto failure

REM package
call %msbuild% Manatee.Json\Manatee.Json.csproj /t:pack /p:Configuration=Release
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1