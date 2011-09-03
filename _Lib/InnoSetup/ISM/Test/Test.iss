#define DEBUG

#define ISM_OnFileStub
;#define ISM_OnViewTranslation

#define ISM_UseInnoSetupImageFilesXP

#define AppName             "MyAppName"
#define AppVersion          "1.0.1.0"

#define VersionInfoCopyright	'Copyright © 2006-2007, i1'
#define VersionInfoCompany		'i1inst@gmail.com'
#define VersionInfoDescription	AppName + ' Setup'

#define AppExe              "MyApp.exe"

#define OutputDir           "_Output"
#define OutputBaseFilename	"MySetup"

#define AppPublisher        "MyAppPublisher"
#define AppPublisherURL     "MyAppPublisherURL"
#define AppSupportURL       "MyAppSupportURL"

#define ISM_RootDir		        AddBackslash(CompilerPath) + "Include/ISM"

#include "../Include/Default.isi"

#include "../Include/Setup.isi"
#include "../Include/Sections.isi"

#include "../Include/Common/Debug.isi"

#include "../Include/Extra/Registry.isi"
#include "../Include/Extra/Url.isi"

#include "../Include/PrepMessages.isi"

#include "../Include/Pragmas.isi"

#define MyIni "{app}\Test.ini"

<File("n\test.txt", '{app}\n', FileFlag_Touch + FileFlag_IsReadme, '__new__')>
<File("*", '{app}\n', FileFlag_Touch + FileFlag_IsReadme)>

<File("n\test.txt", "{app}\n")>

<File("compiler:Examples\MyDll\C\MyDll.c", "{app}")>

<Reg("HKLM\Software\_MyCompany", "IntVal:dword", 10)>

<RegAppPaths("_MyCompany", "{app}\bin\MyManualApp.exe")>
<RegAppPaths("_MyCompany")>

<Dir("{app}\MyAppFolder")>
<Dir("{app}\MyAppFolder\MyAppSubFolder")>

<Icon(name="{group}\Folder Shortcut", filename="{app}\n", comment="My folder shortcut")>

<Ini(MyIni, "MySection", "MyKey", "MyValue")>

<CustomMessage("MyMsg1", "MyMsg1 text")>
<CustomMessage("MyMsg2", "MyMsg2 text")>

<Url("InnoSetup", "{app}\InnoSetup.url", "http://www.jrsoftware.org/isinfo.php")>
<IconUrl("{group}\WebSites\InnoSetup Home", "{app}\InnoSetupHome.url", "http://www.jrsoftware.org/isinfo.php", "InnoSetup Home")>

<UninstallDelete(MyIni)>

<LangOptions("English", "$0409")>

;<CompilerPath>

<Debug_ViewTranslation>

