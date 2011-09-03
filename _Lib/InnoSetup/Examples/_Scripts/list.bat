@set rep=..\
@set out=files.iss
@set opt=/s:e+s-n+ /t:"<File(\x22%%f\x22, \x22{app}\%%l\x22, Common_FileFlags)>" /o:"%out%" /r:"%rep%"
flist.exe %1 %opt%
@echo . 
@if %ERRORLEVEL% EQU 0 echo All OK
@if %ERRORLEVEL% GTR 0 echo There were some wagnings!
@if %ERRORLEVEL% LSS 0 echo !!! ERROR !!!
