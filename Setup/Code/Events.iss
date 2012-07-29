[Code]

var
  SetupModePage: TInputOptionWizardPage;
  TaskValues: array [0..1] of Boolean;
  DirValues: array [0..1] of String;
  TaskValuesInit: Boolean;
  InstallModeCM: String;

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

const
  IndexOfPortable = 0;
  IndexOfInstall  = 1;
  
procedure InitializeWizard;
begin 
  DirValues[IndexOfPortable] := ApplicationData;
  case IsAdminLoggedOn of
    True:
    begin
      InstallModeCM := '{cm:Msg_SetupModeInstall}';
      DirValues[IndexOfInstall] := WizardForm.DirEdit.Text;
    end;
    False:
    begin
      InstallModeCM := '{cm:Msg_SetupModeInstallUserOnly}';
      DirValues[IndexOfInstall] := DirValues[IndexOfPortable];
    end;
  end;
  
  if IsUpdate then
  begin
    DirValues[IndexOfPortable] := WizardForm.DirEdit.Text;
    DirValues[IndexOfInstall]  := DirValues[IndexOfPortable];
  end;
  
  SetupModePage := CreateInputOptionPage(wpWelcome,
    ExpandConstant('{cm:Msg_SetupMode}'),
    ExpandConstant('{cm:Msg_SetupModeQuestion}'),
    ExpandConstant('{cm:Msg_SetupModeGroupTitle}'),
    True, False);
  SetupModePage.Add(ExpandConstant(InstallModeCM));
  SetupModePage.Add(ExpandConstant('{cm:Msg_SetupModePortable}'));

  SetupModePage.SelectedValueIndex := BooleanToInteger(IsPortable);
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

procedure UpdateSetupTypeData(CurPageID: Integer);
begin
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

function BackButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  UpdateSetupTypeData(CurPageID);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  if CurPageID = SetupModePage.ID then
  begin
    IsUserPortable := SetupModePage.SelectedValueIndex <> 0;
    Exit;
  end;
  
  UpdateSetupTypeData(CurPageID);
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  S: String;
  CR: String;
  InstallMode: String;
begin
  CR := NewLine + NewLine;
  
  S := ExpandConstant('{cm:Msg_SetupModeReadyPage}') + NewLine + Space;

  case IsPortable of
    True:   InstallMode := '{cm:Msg_SetupModePortable}';
    False:  InstallMode := InstallModeCM;
  end;
  
  S := S + ExpandConstant(InstallMode);
  
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
