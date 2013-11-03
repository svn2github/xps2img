#ifndef __ISM_TEST_PATH_UTILS_ISS__
#define __ISM_TEST_PATH_UTILS_ISS__

#include "..\Common\UnitTest.isi"
#include "..\Common\PathUtils.isi"

#pragma option -c-

// PathUtils_IsPathBig

#if UnitTest_NotEqual(True, PathUtils_IsPathBig("\\?\"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsPathBig("c:\"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsPathBig(""))
    #pragma error UnitTest_MsgFailed
#endif

// PathUtils_ExtractPathFromBigPath

#if UnitTest_NotEqual("c:\folder", PathUtils_ExtractPathFromBigPath("c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("c:\folder", PathUtils_ExtractPathFromBigPath("\\?\c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("", PathUtils_ExtractPathFromBigPath(""))
    #pragma error UnitTest_MsgFailed
#endif

// PathUtils_IsFullPath

#if UnitTest_NotEqual(True, PathUtils_IsFullPath("c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, PathUtils_IsFullPath("c:\"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, PathUtils_IsFullPath("c:"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsFullPath("..\path"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsFullPath(""))
    #pragma error UnitTest_MsgFailed
#endif

// PathUtils_IsFullPathBig

#if UnitTest_NotEqual(False, PathUtils_IsFullPathBig("c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, PathUtils_IsFullPathBig("\\?\c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, PathUtils_IsFullPathBig("\\?\c:\"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, PathUtils_IsFullPathBig("\\?\c:"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsFullPathBig("\\?\path"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsFullPathBig("\\?\"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsFullPathBig(""))
    #pragma error UnitTest_MsgFailed
#endif

// PathUtils_IsUNCPath

#if UnitTest_NotEqual(False, PathUtils_IsUNCPath("c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsUNCPath("\\?\c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsUNCPath("\\?\UNC\server\share"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, PathUtils_IsUNCPath("\\server\share"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsUNCPath(""))
    #pragma error UnitTest_MsgFailed
#endif

// PathUtils_IsUNCPathBig

#if UnitTest_NotEqual(False, PathUtils_IsUNCPathBig("c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsUNCPathBig("\\?\c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, PathUtils_IsUNCPathBig("\\?\UNC\server\share"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsUNCPathBig("\\server\share"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsUNCPathBig(""))
    #pragma error UnitTest_MsgFailed
#endif

// PathUtils_IsInnoSetupPath

#if UnitTest_NotEqual(False, PathUtils_IsInnoSetupPath("c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsInnoSetupPath("\\?\c:\folder"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsInnoSetupPath("\\?\UNC\server\share"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsInnoSetupPath("\\server\share"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, PathUtils_IsInnoSetupPath("{app}\path"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsInnoSetupPath("{app\"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, PathUtils_IsInnoSetupPath(""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("", PathUtils_AddBackslash(""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("", PathUtils_AddBackslash("", True))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("\", PathUtils_AddBackslash("\"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("\", PathUtils_AddBackslash("\", True))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("dir\", PathUtils_AddBackslash("dir"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("dir\", PathUtils_AddBackslash("dir\\"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("\dir", PathUtils_AddBackslash("dir", True))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("c:\12\345\6", PathUtils_RemoveFileExt("c:\12\345\6.ext"))
    #pragma error UnitTest_MsgFailed
#endif
  
#if UnitTest_NotEqual("c:\12\345\6", PathUtils_RemoveFileExt("c:\12\345\6.ex"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("c:\12\345\6", PathUtils_RemoveFileExt("c:\12\345\6"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("c:\12\345\6", PathUtils_RemoveFileExt("c:\12\345\6."))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("6", PathUtils_ExtractFileNameWithoutExt("c:\12\345\6.ext"))
    #pragma error UnitTest_MsgFailed
#endif
  
#if UnitTest_NotEqual("6", PathUtils_ExtractFileNameWithoutExt("c:\12\345\6.ex"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("6", PathUtils_ExtractFileNameWithoutExt("c:\12\345\6"))
    #pragma error UnitTest_MsgFailed
#endif

// Tests are OK message.

#pragma message UnitTest_MsgPassed

#endif

