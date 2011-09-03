#include "stdafx.h"

#include "resource.h"

#ifdef _CONSOLE
int _tmain(int /*argc*/, _TCHAR* /*argv*/[])
#else
int APIENTRY _tWinMain(
					HINSTANCE	hInstance,
					HINSTANCE	/*hPrevInstance*/,
					LPTSTR		/*lpCmdLine*/,
					int			/*nCmdShow*/)
#endif
{
#ifndef EMPTY
	TCHAR szPath[MAX_PATH * 3];

	*szPath = 0;

	GetCurrentDirectory(sizeof(szPath)/sizeof(szPath[0]), szPath);

#ifndef _CONSOLE
	CString strCaption(MAKEINTRESOURCE(IDS_CAPTION));
#endif

	CString strText;
	strText.Format(IDS_TEXT, szPath, GetCommandLine());

#ifdef _CONSOLE
	strText.AnsiToOem();
	_putts(strText);
#else // _WINDOWS

	// We have to create window in order to show message box.
	// Otherwise it won't be shown on XP SP2 + latest updates.

	INITCOMMONCONTROLSEX initCtrls = { sizeof(INITCOMMONCONTROLSEX), 0 };
	ATLVERIFY(InitCommonControlsEx(&initCtrls));

	HWND hwnd = CreateWindow(_T("button"), _T(""), WS_OVERLAPPED | BS_PUSHBUTTON, 0, 0, 0, 0, NULL, NULL, hInstance, NULL);
	ATLASSERT(hwnd != NULL);

	ATLVERIFY(MessageBox(hwnd, strText, strCaption, MB_OK | MB_ICONINFORMATION | MB_TOPMOST | MB_SETFOREGROUND));

	ATLVERIFY(DestroyWindow(hwnd));
#endif // _WINDOWS
#else // EMPTY
	hInstance;
#endif // EMPTY
	return 0;
}
