@echo off
set codec="oggdec.exe"
for /r "%1" %%i in ("*.ogg") do (
	%codec% "%%~dpni.ogg
	if errorlevel 1 exit
	del /f "%%~dpni.ogg
	if errorlevel 1 goto errormark
)
exit /b
:errormark
	echo.
	echo ERROR!
	echo. 
