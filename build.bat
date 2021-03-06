@echo off

:: First command line parameter. Valid values: Debug, Release
set buildConfig=Release

:: If empty removes corresponding checks and build steps.
set checkHelpAndSetup=1
set buildHelp=1
set buildSetup=1

set logFile=build.log

if not "%~1"=="" set buildConfig=%~1

if exist "%logFile%" del /f /q "%logFile%"

set slnFolder=%~dp0
set helpFolder=%slnFolder%Help
set setupFolder=%slnFolder%Setup
set outFolder=%slnFolder%_bin\%buildConfig%
set ismFolder=%slnFolder%_Lib\InnoSetup\ISM
set buildScripts=%slnFolder%_Build\Scripts
set mergeResources=%buildScripts%\MergeResources.bat
set editBin=%buildScripts%\EditBin.bat
set mergeBin=%buildScripts%\MergeBin.bat

set PFx86=%PROGRAMFILES(x86)%
if "%PFx86%"=="" set PFx86=%PROGRAMFILES%

set msbuild="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set hhc="%PFx86%\HTML Help Workshop\hhc.exe"

set isFolder=%PFx86%\Inno Setup 5
set isCompiler="%isFolder%\iscc.exe"
set isVersion=Inno Setup 5.5.7(u) or higher
set isDownload=http://www.jrsoftware.org/isdl.php

set buildOptions=/p:Configuration="%buildConfig%" /t:Rebuild "/l:FileLogger,Microsoft.Build.Engine;logfile=%logFile%;append=true;encoding=utf-8"

call :isInstalled %msbuild% "Microsoft .NET Framework 4" "http://www.microsoft.com/download/en/details.aspx?id=17851" || goto ERROR

if not "%checkHelpAndSetup%"=="" (
	call :isInstalled %hhc% "HTML Help Workshop" "http://www.microsoft.com/download/en/details.aspx?id=21138" || goto ERROR
	call :isInstalled %isCompiler% "%isVersion%" "%isDownload%" || goto ERROR
)

if not exist "%isFolder%\Include\ISM" (
	echo Copying "%ismFolder%" to "%isFolder%\Include\ISM"...
	xcopy "%ismFolder%" "%isFolder%\Include\ISM" /s /i /h || goto ERROR
)

@echo on

%msbuild% "%slnFolder%Xps2Img.sln" %buildOptions% || goto ERROR
%msbuild% "%slnFolder%Xps2ImgUI.sln" %buildOptions% || goto ERROR

@if "%buildHelp%"=="" goto NO_HELP
%hhc% "%helpFolder%\xps2img.hhp" && goto ERROR
copy "%helpFolder%\xps2img.chm" "%outFolder%" /Y || goto ERROR

:NO_HELP
@echo off

if /i "%buildConfig%"=="Release" (
	call "%mergeResources%" "%outFolder%\CommandLine.dll" || goto ERROR
	call "%mergeResources%" "%outFolder%\xps2imgShared.dll" || goto ERROR
	call "%mergeResources%" "%outFolder%\xps2img.exe" || goto ERROR
	call "%mergeResources%" "%outFolder%\xps2imgUI.exe" || goto ERROR

	call "%mergeBin%" "%outFolder%" || goto ERROR
	call "%editBin%"  "%outFolder%\xps2img.exe" || goto ERROR
	call "%editBin%"  "%outFolder%\xps2imgUI.exe" || goto ERROR
)

@if "%buildSetup%"=="" goto NO_SETUP

@echo on
%isCompiler% /dBinariesPath=..\_bin\%buildConfig%\ "%setupFolder%\Xps2ImgSetup.iss" || goto IS_ERROR
copy "%setupFolder%\_Output\Xps2ImgSetup.exe" "%outFolder%" /Y || goto ERROR
@echo off

:NO_SETUP
echo.
echo All OK. Output folder is '%outFolder%'
exit /b 0

:IS_ERROR
@echo off
echo.
echo.
echo IMPORTANT: Execute setup-ism.bat first!
echo %isVersion% is required. Download at %isDownload%

:ERROR
@echo off
echo.
echo.
echo There were errors!
echo.
pause
exit /b 1

:isInstalled
if exist "%~1" exit /b 0
echo.
echo %~2 is not found. Download at %~3
exit /b 1
