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

function AppSettingsFilePath(Param: String): String;
begin
  case IsPortable of
    True:  Result := ExpandConstant('{app}');
    False: Result := ApplicationData;
  end;
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

procedure OnUseCurrentDirCheckBoxClick(Sender: TObject); forward;

procedure AddUseCurrentDirCheckBox;
begin
  UseCurrentDirCheckBox := TNewCheckBox.Create(WizardForm);
  
  UseCurrentDirCheckBox.Parent := WizardForm.SelectDirPage;
  UseCurrentDirCheckBox.Top := WizardForm.DirEdit.Top + WizardForm.DirEdit.Height + 8;
  UseCurrentDirCheckBox.Width := WizardForm.Width;
  UseCurrentDirCheckBox.Caption := ExpandConstant('{cm:Msg_InstallToCurrentDirectory}');
  
  UseCurrentDirCheckBox.OnClick := @OnUseCurrentDirCheckBoxClick;
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

const
  ApplicationLanguageXPath = '//Settings/Preferences/ApplicationLanguage';
  DefaultAppLanguageName   = 'English';
  AppSettingsFileTemplate  = '<?xml version="1.0"?>' + #13#10 +
'<Settings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">' + #13#10 +
'  <PropertySort>Categorized</PropertySort>' + #13#10 +
'  <Preferences>' + #13#10 +
'    <ApplicationLanguage>%s</ApplicationLanguage>' + #13#10 +
'  </Preferences>' + #13#10 +
'</Settings>';

function AppLanguageName : String; forward;

function ManageAppSettingsFile(const file: String) : Boolean;
var
  fileName, text: String;
  xmlDoc, node: Variant;
begin
  Result := False;
  
  fileName := ExpandConstant(file);
  
  if not FileExists(fileName) then
  begin
    SaveStringToFile(fileName, Format(AppSettingsFileTemplate, [ AppLanguageName ]), False);
    Exit;
  end;
  
  try
    xmlDoc := MSXML_Open(fileName); 
   
    node := MSXML_GetSingleNode(xmlDoc, ApplicationLanguageXPath);
    text := node.text;
    
    if text = '' then text := DefaultAppLanguageName;
    
    if text <> AppLanguageName then
    begin
      node.text := AppLanguageName;
      MSXML_SaveWithIndent(xmlDoc, fileName);
    end;
  except
  end
end;

[/Code]
