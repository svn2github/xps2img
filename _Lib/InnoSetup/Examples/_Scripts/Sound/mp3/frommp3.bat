@echo off
set lame="lame.exe"
set opt=--decode --silent
for /r "%1" %%i in ("*.wav") do (
	echo %lame% "%%~dpni.mp3" "%%~dpni.wav" %opt%
	echo del /f "%%~dpni.mp3"
	if errorlevel 1 goto errormark
)
exit /b
:errormark
	echo.
	echo ERROR!
	echo. 
