REM build script for MyGet builds

set config=%1
if "%config%" == "" (
   set config=Release
)

REM Build
dotnet restore

"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" Manatee.Json.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM packaging takes place as part of post-build of Manatee.Json-4.5 project.