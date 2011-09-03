[Code]

function CheckOS(): Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);
  
  Result :=
    ((Version.Major >= 5) and ((Version.Major <> 5) or (Version.Minor <> 2))) or
    (MsgBox(ExpandConstant('{cm:Message_NotSupportedOSes}'), mbCriticalError, MB_YESNO or MB_DEFBUTTON2) = IDYES);
end;

[/Code]
