// Version.h

#pragma once

#define VER_VALUE_VERSION	"5.2.2.0"
#define VER_HEADER_VERSION	 5,2,2,0

#define LEGAL_COPYRIGHT		"Copyright © 2007-2008, Ivan Ivon."
#define PRODUCT_NAME		"Inno Setup Meta"
#define COMMENTS			"This file is a part of Inno Setup Meta 5.2.2.0 distribution."

#ifdef EMPTY
	#define FILE_DESCRIPTION	"Inno Setup Meta GUI Empty Dummy Application."
#elif defined(_WINDOWS)
	#define FILE_DESCRIPTION	"Inno Setup Meta GUI Dummy Application."
#elif defined(_CONSOLE)
	#define FILE_DESCRIPTION	"Inno Setup Meta Console Dummy Application."
#elif defined(_WINDLL)
	#define FILE_DESCRIPTION	"Inno Setup Meta Dummy DLL."
#endif

#define INTERNAL_NAME			FILE_NAME
#define ORIGINAL_FILENAME		FILE_NAME

#define DUMMY_TEXT				"This is dummy file. If you see this text you might created installation with ISM_OnFileStub defined.\n\nThis file is a part of Inno Setup Meta 5.2.2.0 distribution. Copyright © 2007-2008, Ivan Ivon."

// Version.h
