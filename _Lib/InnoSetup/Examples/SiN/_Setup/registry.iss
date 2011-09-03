<RegAppPaths("sin.exe")>

<Reg("HKLM\SOFTWARE\Ritual Entertainment")>
<Reg("HKLM\SOFTWARE\Ritual Entertainment\Sin")>
<Reg("HKLM\SOFTWARE\Ritual Entertainment\Sin\1.11")>
<Reg("HKLM\SOFTWARE\Ritual Entertainment\Sin", "FTP:string", "SIN")>
<Reg("HKLM\SOFTWARE\Ritual Entertainment\Sin", "EXE:string", "Sin.exe")>
<Reg("HKLM\SOFTWARE\Ritual Entertainment\Sin", "PATH:string", "{app}\")>

#define Active_Components "wos"
    <RegAppPaths("Wages of Sin", "{app}\2015")>

    <Reg("HKLM\SOFTWARE\Ritual Entertainment\Wages of Sin")>
    <Reg("HKLM\SOFTWARE\Ritual Entertainment\Wages of Sin\1.06.000")>
    <Reg("HKLM\SOFTWARE\Ritual Entertainment\Wages of Sin", "EXE:string", "Wages of Sin")>
    <Reg("HKLM\SOFTWARE\Ritual Entertainment\Wages of Sin", "START:string", "Sin Mission Pack-Wages of Sin")>
    <Reg("HKLM\SOFTWARE\Ritual Entertainment\Wages of Sin", "VERSION:string", "1.06")>
<Reset_ActiveComponents()>

