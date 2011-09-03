#include "../../../_Common/Config.isi"

#define DIR_LEVEL	"../"

#define OutputBaseFilename		"Thief2Setup"

#define AppName					"Thief 2 - The Metal Age"
#define AppExe					"thief2.exe"

#define AppDirName				"Thief 2 - The Metal Age"

#define DirectXVersion			"7.0"

#define RegAppName				"thief2.exe"
#define RegAppSettingsRootKey	"Thief 2 - The Metal Age"

#include "../../_Common/AppCommon.iss"
#include "../../../_Common/Setup.isi"
#include "../../_Common/Common.iss"

#include "Files.iss"

#define InstallCfg	'{app}\darkinst.cfg'

#define Active_AfterInstall "SaveInstallCfg('" + InstallCfg + "')"
	<FileStub(InstallCfg)>
<Reset_ActiveAfterInstall()>

<IconEggs()>
<IconFaq()>
<IconHints()>
<IconCheats()>
<IconSecrets()>

<Debug_ViewTranslation>
