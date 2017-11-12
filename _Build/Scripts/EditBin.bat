@echo off

set scriptDir=%~dp0

set vsvars=

set PFx86=%PROGRAMFILES(x86)%
if "%PFx86%"=="" set PFx86=%PROGRAMFILES%

call :setEditBin "16" || goto EDIT
call :setEditBin "15" || goto EDIT
call :setEditBin "14" || goto EDIT
call :setEditBin "12" || goto EDIT
call :setEditBin "11" || goto EDIT
call :setEditBin "10" || goto EDIT

call :setEditBin "2017" "Community"    || goto EDIT
call :setEditBin "2017" "Professional" || goto EDIT
call :setEditBin "2017" "Enterprise"   || goto EDIT

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

set execPatch="%scriptDir%..\Bin\gsar.exe" "-sSystem.Drawing, Version=4.0.0.0" "-rSystem.Drawing, Version=2.0.0.0" -o %target%
echo %execPatch%...
%execPatch% || exit 1

exit /b 0

:setEditBin
set vsvarsPath=%PFx86%\Microsoft Visual Studio %~1.0\VC\bin\vcvars32.bat
if not "%~2"=="" set vsvarsPath=%PFx86%\Microsoft Visual Studio\%~1\%~2\VC\Auxiliary\Build\vcvars32.bat
if not exist "%vsvarsPath%" exit /b 0
set vsvars=%vsvarsPath%
exit /b 1
