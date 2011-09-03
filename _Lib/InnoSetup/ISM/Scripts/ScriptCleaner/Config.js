// Script cleaner configuration settings.

// Output sections order.
// Unknown section goes after known in order of addition.
ScriptCleaner.sectionsOrder = [
	"Messages",
	"Languages",
	"LangOptions",
	"Setup",
	"Code",
	"Types",
	"Components",
	"Tasks",
	"Files",
	"Dirs",
	"INI",
	"Registry",
	"Icons",
	"InstallDelete",
	"Run",
	"UninstallDelete",
	"UninstallRun",
	"CustomMessages"
];

// Section start and end tags regexps.
ScriptCleaner.reSectionStart=	new RegExp().compile(/^\[\s*([ _a-zA-Z1-9]+)\s*\]$/);
ScriptCleaner.reSectionEnd	=	new RegExp().compile(/^\[\s*\/\s*[ _a-zA-Z1-9]+\s*\]$/);
