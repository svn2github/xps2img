//#define DEBUG

#define ISM_CleanTranslationOnly

#define ISM_RootDir             AddBackslash(CompilerPath) + "Include/ISM"

#define PrivilegesRequired      "none"
#define Uninstallable           "no"

#define AppExe                  "ISMSample.exe"
#define AppName                 "ISM Sample"
#define AppVersion              "1.0.0.0"

#define OutputBaseFilename      StringChange(AppName, " ", "") + "Setup"
#define OutputDir               "_Output"

#include ISM_RootDir + "/Include/IncludeAll.isi"

<Debug_ViewTranslation>
