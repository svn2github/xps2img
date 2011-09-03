#include "stdafx.h"

#define STDDLLEXPORT extern "C" __declspec(dllexport) HRESULT

BOOL APIENTRY DllMain(
						HANDLE	/*hModule*/, 
						DWORD	/*ul_reason_for_call*/, 
						LPVOID	/*lpReserved*/
					  )
{
	return TRUE;
}

STDDLLEXPORT DllRegisterServer()
{
	return S_OK;
}

STDDLLEXPORT DllUnregisterServer()
{
	return S_OK;
}

STDDLLEXPORT DllCanUnloadNow()
{
	return S_OK;
}
