[Code]

var _YesToAll: boolean;
var _NoToAll: boolean;
var _CurrentFile: string;

procedure BrowseFile(Sender: TObject);
var
  ErrorCode: Integer;
begin
  Exec(ExpandConstant('{win}\explorer.exe'), '/select,"' + _CurrentFile + '"', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
end;

function OverwriteFileUser(fileName: string): boolean;
var
    AbortInstall: boolean;
    Form: TSetupForm;
    Label1: TLabel;
    Label2: TLabel;
    FileNameLabel: TLabel;
    NoButton: TButton;
    YesButton: TButton;
    NoAllButton: TButton;
    YesAllButton: TButton;
    CancelButton: TButton;
begin

  AbortInstall := false;

  Form := CreateCustomForm();
  try

    Form.ClientWidth := ScaleX(172);
    Form.ClientHeight := ScaleY(190);
    Form.Caption := 'Overwrite File';
    Form.CenterInsideControl(WizardForm, False);

    { Label1 }
    Label1 := TLabel.Create(Form);
    with Label1 do
    begin
      Parent := Form;
      Left := ScaleX(8);
      Top := ScaleY(64);
      Width := ScaleX(153);
      Height := ScaleY(13);
      Caption := 'Would you like to overwrite it?';
    end;

    { Label2 }
    Label2 := TLabel.Create(Form);
    with Label2 do
    begin
      Parent := Form;
      Left := ScaleX(8);
      Top := ScaleY(8);
      Width := ScaleX(150);
      Height := ScaleY(13);
      Caption := 'The following file already exists';
    end;

    { FileNameLabel }
    FileNameLabel := TLabel.Create(Form);
    with FileNameLabel do
    begin
      Parent := Form;
      Left := ScaleX(16);
      Top := ScaleY(35);
      Width := ScaleX(144);
      Height := ScaleY(13);
      OnClick := @BrowseFile;

      _CurrentFile := ExpandConstant(fileName);
      Hint := _CurrentFile;
      ShowHint := true;

      Cursor := crHand;
      Font.Style := Font.Style + [fsUnderline];
      Font.Color := clBlue;

      Caption := ExtractFileName(fileName);
    end;

    { NoButton }
    NoButton := TButton.Create(Form);
    with NoButton do
    begin
      Parent := Form;
      Left := ScaleX(6);
      Top := ScaleY(114);
      Width := ScaleX(75);
      Height := ScaleY(23);
      Caption := '&No';
      ModalResult := 7;
      TabOrder := 2;
    end;

    { YesButton }
    YesButton := TButton.Create(Form);
    with YesButton do
    begin
      Parent := Form;
      Left := ScaleX(6);
      Top := ScaleY(90);
      Width := ScaleX(75);
      Height := ScaleY(23);
      Caption := '&Yes';
      ModalResult := 6;
      TabOrder := 0;
    end;

    { NoAllButton }
    NoAllButton := TButton.Create(Form);
    with NoAllButton do
    begin
      Parent := Form;
      Left := ScaleX(82);
      Top := ScaleY(114);
      Width := ScaleX(75);
      Height := ScaleY(23);
      Caption := 'N&o All';
      ModalResult := 9;
      TabOrder := 3;
    end;

    { YesAllButton }
    YesAllButton := TButton.Create(Form);
    with YesAllButton do
    begin
      Parent := Form;
      Left := ScaleX(82);
      Top := ScaleY(90);
      Width := ScaleX(75);
      Height := ScaleY(23);
      Caption := 'Yes &All';
      ModalResult := 10;
      TabOrder := 1;
    end;

    { CancelButton }
    CancelButton := TButton.Create(Form);
    with CancelButton do
    begin
      Parent := Form;
      Left := ScaleX(6);
      Top := ScaleY(138);
      Width := ScaleX(151);
      Height := ScaleY(23);
      Cancel := True;
      Caption := 'Cancel Installation';
      ModalResult := 2;
      TabOrder := 4;
    end;

    Form.ActiveControl := YesButton;

    Result := false;

    case Form.ShowModal() of
      mrYes:
        begin
          Result := true;
          Log('*** YES ***');
        end;
      mrNo:
        begin
          Log('*** NO ***');
        end;
      mrYesToAll:
        begin
          _YesToAll := true;
          Result := true;
          Log('*** YES ALL ***');
        end;
      mrNoToAll:
        begin
          _NoToAll := true;
          Log('*** NO ALL ***');
        end;
      mrCancel:
        AbortInstall := true;
    end;

  finally
    Form.Free();
  end;

//  if AbortInstall then Abort();

end;

