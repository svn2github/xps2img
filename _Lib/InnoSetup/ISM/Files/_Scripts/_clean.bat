@echo off
set docFolder=%~dp0..
echo Cleaning up...
cd "%docFolder%" || exit /b 1
if exist DUMMY.GID attrib -R -H -S -A DUMMY.GID
del /q /f ^
	"dummy.pdf" ^
	"PDFCreator.ini" ^
	"dummy.chm" ^
	"dummy.hhp" ^
	"DUMMY.GID" ^
	"dummy.hlp" ^
	"dummy.hpj" ^
	"dummy.html" ^
	"dummy.rtf" ^
	"dummy.txt" ^
	"dummy.exe" ^
	"dummyc.exe" ^
	"empty.exe" ^
	"dummy.dll" ^
	"*.log" ^
	> nul 2<&1
echo.
cd "%~dp0" || exit /b 1
exit /b
