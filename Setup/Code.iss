[CustomMessages]
en.Msg_DotNetIsMissing=This application requires Microsoft.NET 3.5 which is not installed.%n%nWould you like to download it now?
en.Msg_KeepSettings=Would you like to keep saved settings?

[Code]

// Check for the .Net 3.5 framework
function InitializeSetup: Boolean;
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

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
    appDataDir: String;
begin
    if (CurUninstallStep <> usUninstall) then Exit;
    
    appDataDir := ExpandConstant(AddBackslash('{userappdata}') + '{#AppName}');
    
    if DirExists(appDataDir) and (UninstallSilent or (MsgBox(ExpandConstant('{cm:Msg_KeepSettings}'), mbConfirmation, MB_YESNO) = idNo)) then
    begin
        DelTree(appDataDir, True, True, True);
    end;
end;

[/Code]
