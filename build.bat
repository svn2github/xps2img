@echo off

set msbuild="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set buildOptions=/p:Configuration=Release /t:Rebuild

%msbuild% Xps2Img.sln %buildOptions% || exit /b 1
%msbuild% Xps2ImgUI.sln %buildOptions% || exit /b 1
