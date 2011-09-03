@echo off
set lame="lame.exe"
set opt=--vbr-new -V 0 --silent
for /r "%1" %%i in ("*.wav") do (
	%lame% %%~fi %%~dpni.mp3 %opt%
	if errorlevel 1 goto errormark
)
exit /b
:errormark
	echo.
	echo ERROR!
	echo. 
