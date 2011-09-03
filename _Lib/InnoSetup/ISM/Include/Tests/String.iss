#ifndef __ISM_TEST_STRING_ISS__
#define __ISM_TEST_STRING_ISS__

#include "..\Common\UnitTest.isi"
#include "..\Common\String.isi"

#pragma option -c-

// Str_Reverse

#if UnitTest_NotEqual("", Str_Reverse(""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("1", Str_Reverse("1"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("21", Str_Reverse("12"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("321", Str_Reverse("123"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("4321", Str_Reverse("1234"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("54321", Str_Reverse("12345"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_Replace

#if UnitTest_NotEqual("", Str_Replace("", "A", "X"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abcdefhg", Str_Replace("abcdefhg", "A", "X"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("ABCdefhg", Str_Replace("abcdefhg", "abc", "ABC"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("Abcdefhg", Str_Replace("abcdefhg", "a", "A"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abcdefhg", Str_Replace("abcdefhg", "x", "X"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("", Str_Replace("", "A", "X"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abCdefhg", Str_Replace("abCdefhg", "abc", "X"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("zXDXabX", Str_Replace("zabcDabcababc", "abc", "X"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_GetLeft

#if UnitTest_NotEqual("", Str_GetLeft("", "def"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abc", Str_GetLeft("abcdefhg", "def"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("", Str_GetLeft("Abcdefhg", "A"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abcdefhg", Str_GetLeft("abcdefhg", "XXX"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_GetRight

#if UnitTest_NotEqual("", Str_GetRight("", "def"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("hg", Str_GetRight("abcdefhg", "def"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("bcdefhg", Str_GetRight("Abcdefhg", "A"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("", Str_GetRight("abcdefhgA", "A"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abcdefhg", Str_GetRight("abcdefhg", "XXX"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_GetLeftLast

#if UnitTest_NotEqual("", Str_GetLeftLast("", "def"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abcdefhg", Str_GetLeftLast("abcdefhgdefAx", "def"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("", Str_GetLeftLast("Abcdefhg", "A"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abcdefhg", Str_GetLeftLast("abcdefhg", "XXX"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("abc\def", Str_GetLeftLast("abc\def\hg", "\"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_GetRightLast

#if UnitTest_NotEqual("", Str_GetRightLast("", "def"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("hg", Str_GetRightLast("abcdefhg", "def"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("defhg", Str_GetRightLast("AbcAdefhg", "A"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual("hg", Str_GetRightLast("abc\def\hg", "\"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_IsEmpty/Str_IsNotEmpty

// Str_IsEmpty

#if UnitTest_NotEqual(True, Str_IsEmpty(""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_IsEmpty("   "))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_IsEmpty("  abcd  "))
    #pragma error UnitTest_MsgFailed
#endif

// Str_IsNotEmpty

#if UnitTest_NotEqual(False, Str_IsNotEmpty(""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsNotEmpty("   "))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsNotEmpty("  abcd  "))
    #pragma error UnitTest_MsgFailed
#endif

#pragma parseroption -p-

// Str_IsCharSpace

#if UnitTest_NotEqual(True, Str_IsCharSpace(""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsCharSpace(" "))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsCharSpace("\x20"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsCharSpace("\t"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsCharSpace("\v"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsCharSpace("\r"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsCharSpace("\n"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_IsCharSpace("X"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_IsBlank

#if UnitTest_NotEqual(True, Str_IsBlank(""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsBlank("\t\n   \r\n\t  "))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_IsBlank("\t\n   \r\n\t  X"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_IsBlank("X\t\n   \r\n\t  "))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_IsBlank("  X  \r\n"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_IsNotBlank

#if UnitTest_NotEqual(False, Str_IsNotBlank(""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsNotBlank("X"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_IsNotBlank("\t\n   \r\n\t  "))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsNotBlank("\t\n   \r\n\t  X"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsNotBlank("X\t\n   \r\n\t  "))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_IsNotBlank("  X  \r\n"))
    #pragma error UnitTest_MsgFailed
#endif

#pragma parseroption -p+

// Str_StartsWith

#if UnitTest_NotEqual(True, Str_StartsWith("abcdef", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_StartsWith("abcdef", "b"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_StartsWith("a", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_StartsWith("ba", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_StartsWith("abc", "abc"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_StartsWith("a", ""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_StartsWith("", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_StartsWith("", ""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_StartsWith("\\c:\a", "\\c:\"))
    #pragma error UnitTest_MsgFailed
#endif

// Str_EndsWith

#if UnitTest_NotEqual(False, Str_EndsWith("abcdef", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_EndsWith("abcdef", "b"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_EndsWith("a", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_EndsWith("ab", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_EndsWith("ba", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_EndsWith("abc", "abc"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_EndsWith("a", ""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(False, Str_EndsWith("", "a"))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_EndsWith("", ""))
    #pragma error UnitTest_MsgFailed
#endif

#if UnitTest_NotEqual(True, Str_EndsWith("c:\", "\\"))
    #pragma error UnitTest_MsgFailed
#endif

// Tests are OK message.

#pragma message UnitTest_MsgPassed

#endif

