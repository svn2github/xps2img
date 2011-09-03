@rem Repacks all zip files in current folder with level=store. 
@rem Usage: rzip.bat extension
@echo off
set StartTime=%TIME%
set RootFolder=!Unpacked
set PackedFolder=!Packed
if "%1"=="" (
	echo Usage: rzip.bat extension
	exit -1
)
if exist %RootFolder% rmdir /s /q %RootFolder% || exit /b
if not exist %RootFolder% (
	mkdir %RootFolder%\%PackedFolder% || exit /b
)
for %%i in (*.%1) do (
	unzip %%i -d "%RootFolder%\%%i" || exit /b
	cd %RootFolder%/%%i
	zip -0 -r ../%PackedFolder%/%%i . || exit /b
	cd ../..
)
echo Start time: %StartTime%
echo   End time: %TIME%
 