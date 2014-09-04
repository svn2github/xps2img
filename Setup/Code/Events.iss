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
  UseCurrentDirCheckBox: TNewCheckBox;
  PreviousDir: String;

function ApplicationData: String;
begin
    Result := ExpandConstant(AddBackslash('{userappdata}') + '{#AppName}');
end;

function TasksList: TNewCheckListBox;
begin
  Result := WizardForm.TasksList;
end;

function GetTasksListIndex(message: String) : Integer;
begin
  Result := TasksList.Items.IndexOf(ExpandConstant('{cm:' + message + '}'));
end;

function TaskDesktopIndex: Integer;
begin
  Result := GetTasksListIndex('CreateDesktopIcon');
end;

function TaskExtensionIndex: Integer;
begin
  Result := GetTasksListIndex('Task_RegisterFileAssociations');
end;

function TaskFirewallIndex: Integer;
begin
  Result := GetTasksListIndex('Task_AddWindowsFirewallException');
  TasksList.ItemEnabled[Result] := WindowsFirewall_IsConfigurable;
end;

function IsInstallableIndex: Integer;
begin
  Result := Convert_BooleanToInteger(IsInstallable);
end;

procedure UpdateTaskValues(IsInstallableBoolean: Boolean);
begin
  TaskDesktopValues[Convert_BooleanToInteger(IsInstallableBoolean)]   := TasksList.Checked[TaskDesktopIndex];
  TaskExtensionValues[Convert_BooleanToInteger(IsInstallableBoolean)] := TasksList.Checked[TaskExtensionIndex];
  TaskFirewallValues[Convert_BooleanToInteger(IsInstallableBoolean)]  := TasksList.Checked[TaskFirewallIndex] and TasksList.ItemEnabled[TaskFirewallIndex];
end;

function InitializeSetup: Boolean;
var
  errorCode: Integer;
  msgBoxResult: Integer;
begin
  Result := SingleSetupInstance_InitializeSetup;
  if not Result then Exit;
  
  // .NET 3.5+ framework check.
  if not NETFW_IsDetectedNoSP(NETFW_Version35) and not NETFW_IsDetectedNoSP(NETFW_Version4) and not NETFW_IsDetectedNoSP(NETFW_Version45) then
  begin
    msgBoxResult := MsgBox(ExpandConstant('{cm:Msg_DotNetIsMissing}'), mbConfirmation, MB_YESNOCANCEL);
    if msgBoxResult = idYes then
      ShellExec('open', NETFW_Version35Uri, '', '', SW_SHOWNORMAL, ewNoWait, errorCode);
    Result := msgBoxResult = idCancel;
  end;
end;

const
  IndexOfPortable = 0;
  IndexOfInstall  = 1;
  
function UseCurrentDir: Boolean;
begin
  Result := UseCurrentDirCheckBox.Visible and UseCurrentDirCheckBox.Checked;
end;
  
procedure UpdateUseCurrentDir;
begin
  WizardForm.DirEdit.Enabled := not UseCurrentDir;
  WizardForm.DirBrowseButton.Enabled := WizardForm.DirEdit.Enabled;
end;

procedure OnUseCurrentDirCheckBoxClick(Sender: TObject);
begin
  if UseCurrentDir then
  begin
    PreviousDir := WizardForm.DirEdit.Text;
    WizardForm.DirEdit.Text := ExpandConstant('{src}') + '\' + ExtractFileName(PreviousDir);
  end
  else
  begin
    WizardForm.DirEdit.Text := PreviousDir;
  end;
  UpdateUseCurrentDir;
end;

procedure AddUseCurrentDirCheckBox;
begin
  UseCurrentDirCheckBox := TNewCheckBox.Create(WizardForm);
  
  UseCurrentDirCheckBox.Parent := WizardForm.SelectDirPage;
  UseCurrentDirCheckBox.Top := WizardForm.DirEdit.Top + WizardForm.DirEdit.Height + 8;
  UseCurrentDirCheckBox.Width := WizardForm.Width;
  UseCurrentDirCheckBox.Caption := ExpandConstant('{cm:Msg_InstallToCurrentDirectory}');
  
  UseCurrentDirCheckBox.OnClick := @OnUseCurrentDirCheckBoxClick;
end;

procedure InitializeWizard;
begin
  SingleSetupInstance_InitializeWizard;
  
  AddUseCurrentDirCheckBox;

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

  SetupModePage.SelectedValueIndex := Convert_BooleanToInteger(IsPortable);
end;

procedure DeinitializeSetup;
begin
  SingleSetupInstance_DeinitializeSetup;
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  case CurPageID of
    wpSelectDir:
    begin
      WizardForm.DirEdit.Text := DirValues[IsInstallableIndex];
      UseCurrentDirCheckBox.Visible := IsUserPortable;
      UpdateUseCurrentDir;
    end;
    
    wpSelectTasks:
    begin
      if not TaskValuesInit then
      begin
        UpdateTaskValues(True);
        TaskValuesInit := True;
      end;
      TasksList.Checked[TaskDesktopIndex]   := TaskDesktopValues[IsInstallableIndex];
      TasksList.Checked[TaskExtensionIndex] := TaskExtensionValues[IsInstallableIndex];
      TasksList.Checked[TaskFirewallIndex]  := TaskFirewallValues[IsInstallableIndex];
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
    begin
      if IsTaskSelected('{#Task_AddWFRule}') then
        WindowsFirewall_AddException('{#AppName}', {#FirewallProcessImageFileName}, '{#FirewallGroup}');
      if IsTaskSelected('{#Task_RegisterExtension}') then
        MRU_Append('{#XPSFileExtension}', '{#AppExe}');
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  case CurUninstallStep of
    usPostUninstall:
    begin
      WindowsFirewall_RemoveException('{#AppName}', {#FirewallProcessImageFileName});
      MRU_Remove('{#XPSFileExtension}', '{#AppExe}');
    end;

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
