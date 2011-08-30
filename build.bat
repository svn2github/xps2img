@echo off

set buildConfig=Release

if not "%~1"=="" set buildConfig=Debug

set slnFolder=%~dp0
set helpFolder=%slnFolder%Help
set outFolder=%slnFolder%_bin\%buildConfig%

set PFx86=%PROGRAMFILES(x86)%
if "%PFx86%"=="" set PFx86=%PROGRAMFILES%

set msbuild="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set hhc="%PFx86%\HTML Help Workshop\hhc.exe"

set buildOptions=/p:Configuration=%buildConfig% /t:Rebuild

@echo on

%msbuild% "%slnFolder%Xps2Img.sln" %buildOptions% || goto ERROR
%msbuild% "%slnFolder%Xps2ImgUI.sln" %buildOptions% || goto ERROR

%hhc% "%helpFolder%\xps2img.hhp" && goto ERROR
copy "%helpFolder%\xps2img.chm" "%outFolder%" /Y

@echo off

echo.
echo All OK. Output folder is '%outFolder%'
exit /b 0

:ERROR
@echo off
echo.
echo.
echo There were errors!
echo.
pause
exit /b 1

