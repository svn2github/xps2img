[CustomMessages]
en.Msg_DotNetIsMissing=This application requires Microsoft.NET 3.5 which is not installed.%n%nWould you like to download it now?

[Code]
// Check for the .Net 3.5 framework
function InitializeSetup(): Boolean;
var
    errorCode: Integer;
    isInstalled: Cardinal;
begin
    Result := True;
    isInstalled := 0;
    if (not RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5', 'Install', isInstalled)) or (isInstalled <> 1) then
    begin
        if MsgBox(ExpandConstant('{cm:Msg_DotNetIsMissing}'), mbConfirmation, MB_YESNO) = idYes then
        begin
            ShellExec(
            'open',
            'http://www.microsoft.com/downloads/details.aspx?FamilyID=AB99342F-5D1A-413D-8319-81DA479AB0D7',
            '', '', SW_SHOWNORMAL, ewNoWait, errorCode);
        end;
        Result := False;
    end;
end;
[/Code]