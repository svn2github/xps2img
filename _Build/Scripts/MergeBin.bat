@echo off

set scriptDir=%~dp0
set targetDir=%~1

echo.

if "%targetDir%"=="" (
	echo Specify binaries path.
	exit 1
)

set ilMerge=%scriptDir%..\Bin\ILMerge.exe

set commonFiles="%targetDir%\CommandLine.dll" "%targetDir%\Gnu.Getopt.dll" "%targetDir%\xps2imgShared.dll"
set uiFiles="%targetDir%\Microsoft.WindowsAPICodePack.dll"

call :merge xps2img.exe "exe"
call :merge xps2imgUI.exe "winexe" 

exit /b 0

:ERROR
@echo off
echo ILMerge failed with errors.
exit 1

:merge
set startFile=%targetDir%\%~1
set mergeType=%~2
set tempName=%~n1_temp_%~x1
set tempFile=%targetDir%\%tempName%

echo Merging "%startFile%"...

if "%mergeType%"=="winexe" set additionalFiles=%uiFiles%

ren "%startFile%" "%tempName%" || goto ERROR
@echo on
"%ilMerge%" "/out:%startFile%" /t:%mergeType% "%tempFile%" %commonFiles% %additionalFiles% || goto ERROR
@echo off
del /q "%tempFile%"

exit /b 0
