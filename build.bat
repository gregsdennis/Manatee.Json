REM build script for MyGet builds

set config=%1
if "%config%" == "" (
   set config=Release
)

REM Build
dotnet restore

"%programfiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" Manatee.Json.sln /p:Configuration="%config%" /m:1 /v:m /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Package

"%programfiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" Manatee.Json.sln /t:pack /p:Configuration=Release