@echo off

set scriptDir=%~dp0

set targetFile=%~1
set targetDir=%~dp1
set targetName=%~n1
set targetExt=%~x1

echo.

if "%targetDir%"=="" (
	echo Specify file path.
	exit 1
)

set ilMerge=%scriptDir%..\Bin\ILMerge.exe

set startFile=%~1

set outType=winexe

if /i "%targetExt%"==".dll" set outType=library

call :mergeLang "uk" || goto ERROR

exit /b 0

:ERROR
@echo off
echo ILMerge failed with errors.
exit 1

:mergeLang
set lang=%~1
set tempName=%targetName%_temp_%lang%_%targetExt%
set tempFile=%targetDir%\%tempName%
set resFile=%targetDir%\%lang%\%targetName%.resources.dll
echo Merging "%lang%" language...
if not exist %resFile% (
	echo Warning: "%lang%" language does not exist.
	exit /b 1
)
@echo on
ren "%targetFile%" "%tempName%" || goto ERROR
"%ilMerge%" "/out:%startFile%" /t:%outType% "%tempFile%" "%resFile%" || goto ERROR
@echo off
del /q "%resFile%"
rd "%targetDir%\%lang%" > nul 2>&1
del /q "%tempFile%"
exit /b 0
