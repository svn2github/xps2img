@echo off

set PFx86=%PROGRAMFILES(x86)%
if "%PFx86%"=="" set PFx86=%PROGRAMFILES%

set isFolder=%PFx86%\Inno Setup 5
set ismFolder=%isFolder%\Include\ISM

set slnFolder=%~dp0
set libFolder=%slnFolder%_Lib\InnoSetup\ISM

echo on

if exist "%ismFolder%" rmdir /s /q "%ismFolder%"

echo Copying "%ismFolder%" to "%ismFolder%"...
xcopy "%libFolder%" "%ismFolder%" /s /i /h || goto ERROR

@echo off

echo.
echo All OK. ISM folder is '%ismFolder%'
exit /b 0

:ERROR
@echo off
echo.
echo.
echo There were errors!
echo.
pause
exit /b 1
