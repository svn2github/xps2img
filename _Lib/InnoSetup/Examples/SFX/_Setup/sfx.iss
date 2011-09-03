; InnoSetup SFX © 2006, Ivan Ivon
; i1inst@gmail.com
; InnoSetup 5.1.6 script

; Debug mode
#define DEBUG

#define FilesCount            14
#define MagicSkipCheckNumber  1

;#define DISK_SPANNING
#define DISK_SLICE_SIZE       713031680

#define SPACE_REQUIRED        92999999

#define EMBEDDED_WIZ_IMAGE
#define NO_APP_ICON

#define OutputBaseFilename		"SFX"
#define AppName             	"SFX"
#define AppDirName            "SFX"

#define VersionInfoCopyright		  "Copyright © 2006, i1"
#define VersionInfoCompany		    "i1inst@gmail.com"
#define VersionInfoDescription    AppName + " InnoSetup SFX"
#define VersionInfoVersion        "1.0.0.1"
#define VersionInfoTextVersion    VersionInfoVersion

#ifdef EMBEDDED_WIZ_IMAGE
  #define WizardImageFile       CompilerPath + "\WizModernImage.bmp"
  #define WizardSmallImageFile  CompilerPath + "\WizModernSmallImage.bmp"
#elif defined(EMBEDDED_WIZ_IMAGE_IS)
  #define WizardImageFile       CompilerPath + "\WizModernImage-IS.bmp"
  #define WizardSmallImageFile  CompilerPath + "\WizModernSmallImage-IS.bmp"
#endif

[Setup]
#ifdef DISK_SPANNING
DiskSpanning=yes
SlicesPerDisk=1
DiskSliceSize={#DISK_SLICE_SIZE}
#endif
ExtraDiskSpaceRequired={#SPACE_REQUIRED}
AllowRootDirectory=yes
DisableReadyPage=yes
DisableReadyMemo=yes
DisableProgramGroupPage=yes
Uninstallable=no
DirExistsWarning=no
#if defined(DEBUG)
AppName={#AppName} [DEBUG]
#else
AppName={#AppName}
#endif
AppVerName={#AppName}
AllowCancelDuringInstall=yes
DefaultDirName={src}\{#AppName}
; Wizard appearence
WizardImageFile={#WizardImageFile}
WizardSmallImageFile={#WizardSmallImageFile}
WizardImageStretch=no
WizardImageBackColor=clWhite
; Output
OutputDir=..\!Output
OutputBaseFilename={#OutputBaseFilename}
; Setup version
VersionInfoCopyright={#VersionInfoCopyright}
VersionInfoCompany={#VersionInfoCompany}
VersionInfoDescription={#VersionInfoDescription}
VersionInfoVersion={#VersionInfoVersion}
VersionInfoTextVersion={#VersionInfoTextVersion}
#if defined(DEBUG)
Compression=none
#else
Compression=lzma/ultra
SolidCompression=yes
InternalCompressLevel=ultra
#endif

[Code]

#include "FileForm.iss"

var _LeftToCheckOn: integer;

function InitializeSetup(): Boolean;
begin
  _YesToAll := false;
  _NoToAll := false;
  _LeftToCheckOn := {#FilesCount} * {#MagicSkipCheckNumber};
  Result := true;
end;

procedure CancelButtonClick(CurPageID: Integer; var Cancel, Confirm: Boolean);
begin
  Confirm := false;
end;

function OverwriteFile(fileName: string): boolean;
begin
  _LeftToCheckOn := _LeftToCheckOn - 1;
  if _LeftToCheckOn >= 0 then
  begin
    Result := true;
  end
  else
    begin
      Log('*** CHECK FILE ***');
      Log(fileName);
      if _YesToAll or _NoToAll then
      begin
        Result := _YesToAll or (not _NoToAll);
      end
      else
      begin
        Result := (not FileExists(fileName)) or OverwriteFileUser(fileName);
      end;
  end;
end;

[Run]
Filename: {app}\; WorkingDir: {app}; Description: {cm:BrowseExplorer}; Flags: postinstall nowait shellexec skipifsilent unchecked
Filename: {reg:HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Far.exe,|'far.exe'}; Parameters: {app}\; WorkingDir: {app}; Description: {cm:BrowseFAR}; Flags: postinstall nowait skipifsilent unchecked skipifdoesntexist

[CustomMessages]
BrowseExplorer=Browse with Windows &Explorer
BrowseFAR=Browse with &FAR

[Files]
#include "files.iss"

[Dirs]
#include "dirs.iss"


