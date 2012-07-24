[Code]
 
function InitializeSetup: Boolean;
var
  errorCode: Integer;
  isInstalled: Cardinal;
begin
  Result := True;
  // Check for the .Net 3.5 framework
  isInstalled := 0;
  if (not RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5', 'Install', isInstalled)) or (isInstalled <> 1) then
  begin
    if MsgBox(ExpandConstant('{cm:Msg_DotNetIsMissing}'), mbConfirmation, MB_YESNO) = idYes then
    begin
      ShellExec('open', 'http://www.microsoft.com/downloads/details.aspx?FamilyID=AB99342F-5D1A-413D-8319-81DA479AB0D7', '', '', SW_SHOWNORMAL, ewNoWait, errorCode);
    end;
    Result := False;
  end;
end;

var
  SetupModePage: TInputOptionWizardPage;
  TaskValues: array [0..1] of Boolean;
  DirValues: array [0..1] of String;
  TaskValuesInit: Boolean;

function ApplicationData: String;
begin
    Result := ExpandConstant(AddBackslash('{userappdata}') + '{#AppName}');
end;

function TaskValueIndex: Integer;
begin
  Result := WizardForm.TasksList.Items.IndexOf(ExpandConstant('{cm:CreateDesktopIcon}'));
end;

procedure UpdateTaskValues(IsInstallableBoolean: Boolean);
begin
  TaskValues[BooleanToInteger(IsInstallableBoolean)] := WizardForm.TasksList.Checked[TaskValueIndex];
end;

procedure InitializeWizard;
begin
  SetupModePage := CreateInputOptionPage(wpWelcome,
    ExpandConstant('{cm:Msg_SetupMode}'),
    ExpandConstant('{cm:Msg_SetupModeQuestion}'),
    ExpandConstant('{cm:Msg_SetupModeGroupTitle}'),
    True, False);
  SetupModePage.Add(ExpandConstant('{cm:Msg_SetupModeInstall}'));
  SetupModePage.Add(ExpandConstant('{cm:Msg_SetupModePortable}'));

  SetupModePage.SelectedValueIndex := BooleanToInteger(IsPortable);
  
  DirValues[0] := ApplicationData;
  DirValues[1] := WizardForm.DirEdit.Text;
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpSelectDir then
  begin
    WizardForm.DirEdit.Text := DirValues[BooleanToInteger(IsInstallable)];
    Exit;
  end;

  if CurPageID = wpSelectTasks then
  begin
    if not TaskValuesInit then
    begin
      UpdateTaskValues(True);
      TaskValuesInit := True;
    end;
    WizardForm.TasksList.Checked[TaskValueIndex] := TaskValues[BooleanToInteger(IsInstallable)];
    Exit;
  end; 
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
    Result := IsPortable and (PageID = wpSelectProgramGroup);
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if (CurUninstallStep <> usUninstall) then Exit;
    
  if DirExists(ApplicationData) and (UninstallSilent or (MsgBox(ExpandConstant('{cm:Msg_KeepSettings}'), mbConfirmation, MB_YESNO) = idNo)) then
  begin
    DelTree(ApplicationData, True, True, True);
  end;
end;

function BackButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  if CurPageID = wpSelectDir then
  begin
    DirValues[BooleanToInteger(IsInstallable)] := WizardForm.DirEdit.Text;
    Exit;
  end;
   
  if CurPageID = wpSelectTasks then
  begin
    UpdateTaskValues(IsInstallable);
    Exit;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  if CurPageID = SetupModePage.ID then
  begin
    IsUserPortable := SetupModePage.SelectedValueIndex <> 0;
    Exit;
  end;
  
  if CurPageID = wpSelectTasks then
  begin
    UpdateTaskValues(IsInstallable);
    Exit;
  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  S: String;
  CR: String;
begin
  CR := NewLine + NewLine;
  
  S := ExpandConstant('{cm:Msg_SetupMode}') + ':' + NewLine + Space;

  case IsPortable of
    True:   S := S + ExpandConstant('{cm:Msg_SetupModePortable}');
    False:  S := S + ExpandConstant('{cm:Msg_SetupModeInstall}');
  end;
  
  StringChangeEx(S, '&', '', True);
  
  S := S + CR + MemoDirInfo;
  
  if IsInstallable and (MemoGroupInfo <> '') then
  begin
    S := S + CR + MemoGroupInfo;
  end;
  
  if MemoTasksInfo <> '' then
  begin
    S := S + CR + MemoTasksInfo;
  end;
 
  Result := S;
end;

[/Code]
