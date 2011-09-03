[Setup]
AppName=TestForm
AppVerName=TestForm
DefaultDirName={src}
OutputDir=..\!Output
OutputBaseFilename=TestForm
Compression=none

[Code]

#include "FileForm.iss"

procedure CancelButtonClick(CurPageID: Integer; var Cancel, Confirm: Boolean);
begin
  Confirm := false;
end;

procedure InitializeWizard();
begin
  OverwriteFileUser('C:\WINDOWS\notepad.exe');
end;


