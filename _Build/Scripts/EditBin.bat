@echo off

set vsvars=

set PFx86=%PROGRAMFILES(x86)%
if "%PFx86%"=="" set PFx86=%PROGRAMFILES%

call :setEditBin "16" || goto EDIT
call :setEditBin "15" || goto EDIT
call :setEditBin "14" || goto EDIT
call :setEditBin "12" || goto EDIT
call :setEditBin "11" || goto EDIT
call :setEditBin "10" || goto EDIT

if "%vsvars%"=="" (
	echo VS C++ was not found. Please ensure VS C++ is installed and/or modify VS directory at "%~0"
	exit 1
)

:EDIT

set target="%~1"

echo Executing "%vsvarsPath%"...

call "%vsvars%" || exit 1

set execEditBin=editbin.exe /NOLOGO /LARGEADDRESSAWARE %target%
echo %execEditBin%
%execEditBin% || exit 1

exit /b 0

:setEditBin
set vsvarsPath=%PFx86%\Microsoft Visual Studio %~1.0\VC\bin\vcvars32.bat
if not exist "%vsvarsPath%" exit /b 0
set vsvars=%vsvarsPath%
exit /b 1