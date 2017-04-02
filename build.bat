REM build script for MyGet builds

set config=%1
if "%config%" == "" (
   set config=Release
)

REM Build
dotnet restore

"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" Manatee.Json.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false /maxcpucount 1

REM Package

powershell .\Manatee.Json\nuget.ps1 -nuspecfile .\Manatee.Json\project.nuspec