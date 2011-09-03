@echo off
rem ***************** Settings *****************
rem ***************** Files folder *************
set docFolder=%~dp0..
rem ***************** Applications *************
rem ***************** Saxon ********************
set saxon=%PROGRAMFILES%\saxon\bin
set transform="%saxon%\Transform"
rem ***************** HLP Compiler *************
set hcwFolder=%PROGRAMFILES%\Help Workshop
set hcw="%hcwFolder%\hcw"
rem ***************** HLP Compiler *************
set hhcFolder=%PROGRAMFILES%\HTML Help Workshop
set hhc="%hhcFolder%\hhc"
rem ***************** PDF Creator **************
set pdfCreatorFolder=%PROGRAMFILES%\PDFCreator
set pdfCreator="%pdfCreatorFolder%\PDFCreator"
rem ***************** VC.NET 2005 **************
set msvcFolder=%PROGRAMFILES%\Microsoft Visual Studio 8
set devenv="%msvcFolder%\Common7\IDE\devenv"
rem ***************** Settings *****************
rem ***************** PDF Creator files*********
set pdfCreatorINIFolder=%USERPROFILE%\Application Data\PDFCreator
set pdfCreatorINIFileName=PDFCreator.ini
set pdfCreatorOldINIFileName=%pdfCreatorINIFileName%.old
set pdfCreatorINI="%pdfCreatorINIFolder%\%pdfCreatorINIFileName%"
set pdfCreatorOldINI="%pdfCreatorINIFolder%\%pdfCreatorOldINIFileName%"
rem ***************** XSLT folder **************
set xsl=XSL
rem ********************************************

if not exist %transform%.* goto :SAXON_NOT_FOUND

call _clean.bat

cd "%docFolder%" || goto :ERROR

call :applyTransform "Version.h" "rc" "" ".\Bin\_Common\Version.h"

if exist %devenv%.* (
	call :buildSolution "%~dp0..\Bin\Dummy.dll\Dummy.sln" "Release" "dummy.dll"
	call :buildSolution "%~dp0..\Bin\Dummy.exe\Dummy.sln" "Console Release" "dummyc.exe"
	call :buildSolution "%~dp0..\Bin\Dummy.exe\Dummy.sln" "Empty Release" "empty.exe"
	call :buildSolution "%~dp0..\Bin\Dummy.exe\Dummy.sln" "GUI Release" "dummy.exe"
) else (
	echo [EXE] Warning: MSVS.NET 2005 is not found! No builds could be made!
)

call :applyTransform "HTML" "html"
call :applyTransform "TXT " "txt"
call :applyTransform "RTF " "rtf"

if exist %hcw%.* (
	call :applyTransform "HLP " "hpj" "hlp"
	call %hcw% /E /M /C dummy.hpj || goto :HLP_ERROR
	if not exist dummy.hlp goto :HLP_ERROR
	del /q /f dummy.hpj
	ren DUMMY.HLP dummy.hlp || goto :HLP_ERROR
) else (
	echo [HLP] Warning: Microsoft Help Workshop is not found! HLP could not be generated!
)

if exist %hhc%.* (
	call :applyTransform "CHM " "hhp" "chm"
	call %hhc% dummy.hhp > hhc.log && goto :CHM_ERROR
	if not exist dummy.chm goto :CHM_ERROR
	del /q /f dummy.hhp hhc.log
) else (
	echo [CHM] Warning: Microsoft HTML Help Workshop is not found! CHM could not be generated!
)

if exist %pdfCreator%.* (
	if exist %pdfCreatorINI% (
		call :applyTransform "PDF " "pdf" "" "%pdfCreatorINIFileName%"
		echo AutosaveDirectory=%docFolder%>> "%pdfCreatorINIFileName%"
		echo   Please close PDFCreator manually after dummy.pdf creation.
		ren %pdfCreatorINI% "%pdfCreatorOldINIFileName%" || goto :PDF_ERROR
		copy "%pdfCreatorINIFileName%" "%pdfCreatorINIFolder%" > nul || goto :PDF_ERROR
		start /wait "" %pdfCreator% /PF"%docFolder%\dummy.rtf" /NoAbortIfRunning || goto :PDF_ERROR
		del "%pdfCreatorINIFileName%"
		del %pdfCreatorINI% || goto :PDF_ERROR
		ren %pdfCreatorOldINI% "%pdfCreatorINIFileName%" || goto :PDF_ERROR
	) else (
		echo [PDF] Warning: %pdfCreatorINI% is not found! PDF could not be generated! Possibly PDFCreator was installed without /UseINI setup flag.
	)
) else (
	echo [PDF] Warning: PDFCreator is not installed! PDF could not be generated!
)

echo.
echo All OK.
exit 0

:SAXON_NOT_FOUND
echo [Saxon] Error: Saxon XSLT transformation tool is not installed!
goto :ERROR

:HLP_ERROR
echo [Microsoft Help Workshop] Error: dummy.hpj comlilation failed!
goto :ERROR

:CHM_ERROR
echo [Microsoft HTML Help Workshop] Error: dummy.hhp comlilation failed! See hhc.log for details.
goto :ERROR

:PDF_ERROR
echo [PDFCreator] Error: PDF creation failed! Possibly PDFCreator was installed without /UseINI setup flag specified. Check also %pdfCreatorINI% settings.
goto :ERROR

:ERROR
echo. 
echo There were errors!
exit 1

rem ***************** Functions ****************

rem ***************** applyTransform ***********
rem Invokes saxon to generate file.
rem		%1 - file type to display.
rem		%2 - file extension.
rem		%3 - [optional] XSLT name. %2 by default.
rem		%4 - [optional] Out file name. dummy.%ext% by default.
:applyTransform
echo Creating %~1 document...
set ext=%~2
set name=%~3
if "%name%"=="" set name=%ext%
set output=%~4
if "%output%"=="" set output=dummy.%ext%
call %transform% -s:dummy.xml -xsl:"%xsl%\%name%.xsl" -o:"%output%" || goto :ERROR
exit /b

rem ***************** buildSolution ************
rem Invokes saxon to generate file.
rem		%1 - path to solution.
rem		%2 - build configuration.
rem		%3 - copied file name.
:buildSolution
set solution=%~1
set solutionFolder=%~dp1
set solutionName=%~n1
set conf=%~2
set copyFrom="%solutionFolder%_bin\%conf%\bin\%~3"
echo Building "%solutionName%" ^| "%conf%" (%solution%)...
call %devenv% "%solution%" /rebuild "%conf%" || goto :ERROR
xcopy %copyFrom% "%docFolder%" /Y || goto :ERROR
exit /b
