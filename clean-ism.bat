@set PFx86=%PROGRAMFILES(x86)%
@if "%PFx86%"=="" set PFx86=%PROGRAMFILES%

@set isFolder=%PFx86%\Inno Setup 5\Include\ISM

if exist "%isFolder%" rmdir /s /q "%isFolder%"
