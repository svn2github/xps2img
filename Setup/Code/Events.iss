[Code]

var
  SetupModePage: TInputOptionWizardPage;
  NoIconsCheckValues: array [0..1] of Boolean;
  GroupEditValues: array [0..1] of String;
  TaskDesktopValues: array [0..1] of Boolean;
  TaskExtensionValues: array [0..1] of Boolean;
  TaskFirewallValues: array [0..1] of Boolean;
  DirValues: array [0..1] of String;
  TaskValuesInit: Boolean;
  NoIconsCheckValuesInit: Boolean;
  InstallModeCM: String;

function ApplicationData: String;
begin
    Result := ExpandConstant(AddBackslash('{userappdata}') + '{#AppName}');
end;

function TaskDesktopIndex: Integer;
begin
  Result := WizardForm.TasksList.Items.IndexOf(ExpandConstant('{cm:CreateDesktopIcon}'));
end;

function TaskExtensionIndex: Integer;
begin
  Result := WizardForm.TasksList.Items.IndexOf(ExpandConstant('{cm:Task_RegisterSettingsExtension}'));
end;

function TaskFirewallIndex: Integer;
begin
  Result := WizardForm.TasksList.Items.IndexOf(ExpandConstant('{cm:Task_AddWindowsFirewallException}'));
  WizardForm.TasksList.ItemEnabled[Result] := WindowsFirewall_IsConfigurable;
end;

function IsInstallableIndex: Integer;
begin
  Result := BooleanToInteger(IsInstallable);
end;

procedure UpdateTaskValues(IsInstallableBoolean: Boolean);
begin
  TaskDesktopValues[BooleanToInteger(IsInstallableBoolean)]   := WizardForm.TasksList.Checked[TaskDesktopIndex];
  TaskExtensionValues[BooleanToInteger(IsInstallableBoolean)] := WizardForm.TasksList.Checked[TaskExtensionIndex];
  TaskFirewallValues[BooleanToInteger(IsInstallableBoolean)]  := WizardForm.TasksList.Checked[TaskFirewallIndex] and WizardForm.TasksList.ItemEnabled[TaskFirewallIndex];
end;

function InitializeSetup: Boolean;
var
  errorCode: Integer;
  isInstalled: Cardinal;
begin
  
  Result := SingleSetupInstance_InitializeSetup;
  if not Result then Exit;
  
  // Check for the .Net 3.5 framework
  isInstalled := 0;
  if (not RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5', 'Install', isInstalled)) or (isInstalled <> 1) then
  begin
    if MsgBox(ExpandConstant('{cm:Msg_DotNetIsMissing}'), mbConfirmation, MB_YESNO) = idYes then
      ShellExec('open', 'http://www.microsoft.com/downloads/details.aspx?FamilyID=AB99342F-5D1A-413D-8319-81DA479AB0D7', '', '', SW_SHOWNORMAL, ewNoWait, errorCode);
    Result := False;
  end;
end;

const
  IndexOfPortable = 0;
  IndexOfInstall  = 1;
  
procedure InitializeWizard;
begin 
  SingleSetupInstance_InitializeWizard;

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

procedure DeinitializeSetup;
begin
    SingleSetupInstance_DeinitializeSetup;
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  case CurPageID of
    wpSelectDir:
      WizardForm.DirEdit.Text := DirValues[IsInstallableIndex];
    wpSelectTasks:
    begin
      if not TaskValuesInit then
      begin
        UpdateTaskValues(True);
        TaskValuesInit := True;
      end;
      WizardForm.TasksList.Checked[TaskDesktopIndex]   := TaskDesktopValues[IsInstallableIndex];
      WizardForm.TasksList.Checked[TaskExtensionIndex] := TaskExtensionValues[IsInstallableIndex];
      WizardForm.TasksList.Checked[TaskFirewallIndex]  := TaskFirewallValues[IsInstallableIndex];
    end;
    wpSelectProgramGroup:
    begin
      if not NoIconsCheckValuesInit then
      begin
        NoIconsCheckValues[IndexOfPortable] := true;
        NoIconsCheckValues[IndexOfInstall]  := WizardForm.NoIconsCheck.Checked;
        GroupEditValues[IndexOfPortable]    := WizardForm.GroupEdit.Text;
        GroupEditValues[IndexOfInstall]     := WizardForm.GroupEdit.Text;
        NoIconsCheckValuesInit:= true;
      end;
      WizardForm.NoIconsCheck.Checked := NoIconsCheckValues[IsInstallableIndex];
      WizardForm.GroupEdit.Text := GroupEditValues[IsInstallableIndex];
    end;
  end; 
end;

#define FirewallProcessImageFileName  "ExpandConstant('" + AppExe + "')"

procedure CurStepChanged(CurStep: TSetupStep);
begin
  case CurStep of
    ssPostInstall:
      if IsTaskSelected('{#Task_RegisterExtension}') then
        WindowsFirewall_AddException('{#AppName}', {#FirewallProcessImageFileName}, '{#FirewallGroup}');
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  case CurUninstallStep of
    usPostUninstall:
      WindowsFirewall_RemoveException('{#AppName}', {#FirewallProcessImageFileName});
    usUninstall:
      if DirExists(ApplicationData) and (UninstallSilent or (MsgBox(ExpandConstant('{cm:Msg_KeepSettings}'), mbConfirmation, MB_YESNO) = idNo)) then
        DelTree(ApplicationData, True, True, True);
   end;
end;

procedure UpdateSetupTypeData(CurPageID: Integer);
begin
  case CurPageID of
    wpSelectDir:
      DirValues[IsInstallableIndex] := WizardForm.DirEdit.Text;
    wpSelectTasks:
      UpdateTaskValues(IsInstallable);
    wpSelectProgramGroup:
    begin
      NoIconsCheckValues[IsInstallableIndex] := WizardForm.NoIconsCheck.Checked;
      GroupEditValues[IsInstallableIndex] := WizardForm.GroupEdit.Text;
    end;
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
  
  if MemoGroupInfo <> '' then S := S + CR + MemoGroupInfo;
  if MemoTasksInfo <> '' then S := S + CR + MemoTasksInfo;
 
  Result := S;
end;

[/Code]
