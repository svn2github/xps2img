@echo off
call clear.bat "_setup_translated.iss" "_setup_translated_clean.iss" /s:Languages,Setup,Types,Components,INI,Icons,CustomMessages,Messages /cc:"C:\bin\Inno Setup 5\iscc.exe" %*