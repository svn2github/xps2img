@echo off

set msbuild="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

set buildConfig=Release
::set buildConfig=Debug

set buildOptions=/p:Configuration=%buildConfig% /t:Rebuild

set PF=%PROGRAMFILES%

if defined PROCESSOR_ARCHITEW6432 set PF=%PROGRAMFILES(x86)%

%msbuild% Xps2Img.sln %buildOptions% || exit /b 1
%msbuild% Xps2ImgUI.sln %buildOptions% || exit /b 1
