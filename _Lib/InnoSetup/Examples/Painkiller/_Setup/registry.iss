<RegAppPaths("Painkiller.exe")>

<Reg("HKLM\SOFTWARE\PeopleCanFly")>
<Reg("HKLM\SOFTWARE\PeopleCanFly\Painkiller", "EXEpath:expandsz", "{app}\" + AppExe)>
<Reg("HKLM\SOFTWARE\PeopleCanFly\Painkiller", "AddonPathTemp:expandsz", "{app}\")>
