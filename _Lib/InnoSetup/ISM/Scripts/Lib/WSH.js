// WSH extensions.

// WSH Objects.
var wshFSO		= new ActiveXObject("Scripting.FileSystemObject");
var wshShell	= new ActiveXObject("WScript.Shell");

// Addtional WScript functionalty.
function WScript2()
{
}

// Prints text if not in silent mode.
// [optional] text - text to display.
WScript2.Echo = function(text)
{
	if(!WScript2.IsSilent())
	{
		WScript.Echo(text);
	}
};

// Returns true if script is running in silent mode.
WScript2.IsSilent = function()
{
	return !WScript.Interactive || WScript.Arguments.Named.Exists("silent");
};

WScript2.pressEnterToContinue = "Press Enter to continue...";

// Waits until Enter key is pressed.
// Suppressed in silent mode.
WScript2.WaitEnterKey = function(message)
{
	if(!WScript2.IsSilent())
	{
		WScript2.Echo("\n" + (message || WScript2.pressEnterToContinue));
		try
		{
			WScript.StdIn.ReadLine();
		}
		catch(ex)
		{
			// Supress "Invalid handle" error when running by WScript.exe host.
		}
	}
};

WScript2.ShellOpenFile = function(fileName)
{
	wshShell.Run(fileName);
};

// Reads process output.
// scriptExec - WshScriptExec object - returned by WshShell.Exec function.
// Returns merged StdOut and StdErr output.
WScript2.ReadProcessOutput = function(scriptExec)
{
	var output = "";

	if (!scriptExec.StdOut.AtEndOfStream)
	{
		output += scriptExec.StdOut.ReadAll()
	}

	if (!scriptExec.StdErr.AtEndOfStream)
	{
		output += scriptExec.StdErr.ReadAll()
	}
	
	return output;
};
