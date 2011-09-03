@echo off
set codec="oggenc2.exe"
set opt=--managed -q 10
for /r "%1" %%i in ("*.wav") do (
	%codec% %opt% "%%~fi"
	if errorlevel 1 goto errormark
)
exit /b
:errormark
	echo.
	echo ERROR!
	echo. 
