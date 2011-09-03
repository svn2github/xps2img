// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#define WIN32_LEAN_AND_MEAN
#define STRICT

#define WINVER			0x0400
#define _WIN32_WINDOWS	0x0400
#define _WIN32_IE		0x0300

#ifdef EMPTY
#include <windows.h>
#include <tchar.h>
#else

#include <atlbase.h>
#include <atlstr.h>

#include <CommCtrl.h>

#ifdef _CONSOLE
#include <stdio.h>
#endif
#endif
