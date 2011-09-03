#ifndef __SETUP_COMMON_ISS__
#define __SETUP_COMMON_ISS__

#include "../../_Common/Config.isi"

#define EvaInstallRegPath   "HKCU\Software\GAINAX\EVANGELION"
#define EvaInstallRegValue  "InstallPath"

#define DefaultDirName      "{reg:" + EvaInstallRegPath + "," + EvaInstallRegValue + "|{sd}\eva95}"

#define AppExe              "EXEC\ADV2.EXE"
#define AppVersion          "1.0.0.1"

#endif
