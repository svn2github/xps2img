// Script cleaner options class.

// argsNamed - WshNamed collection.
ScriptCleaner.Options = function(agrsNamed)
{
	// Sorted sections list.
	this.sectionsToSort = ScriptCleaner.Options.GetOpt(agrsNamed, ScriptCleaner.Options.optSort, "").split(",");

	// Make lower case.
	for(var i in this.sectionsToSort)
	{
		this.sectionsToSort[i] = this.sectionsToSort[i].trim().toLowerCase();
	}
	
	// Compile scripts flag.
	this.compileScripts = ScriptCleaner.Options.GetOpt(agrsNamed, ScriptCleaner.Options.optCompileScripts);

	// View scripts flag.
	this.viewScripts = ScriptCleaner.Options.GetOpt(agrsNamed, ScriptCleaner.Options.optViewScripts);
	
	// View scripts flag.
	this.waitEnter = ScriptCleaner.Options.GetOpt(agrsNamed, ScriptCleaner.Options.optWaitEnter, false);
	
	// No logo flag.
	this.noLogo = ScriptCleaner.Options.GetOpt(agrsNamed, ScriptCleaner.Options.optNoLogo, false);
	
	// Compiler executable.
	this.compiler = ScriptCleaner.Options.GetOpt(agrsNamed, ScriptCleaner.Options.optCompiler, ScriptCleaner.Options.defaultCompiler);
};

// Returns named option or default option.
ScriptCleaner.Options.GetOpt = function(agrsNamed, opt, defOpt)
{
	// If no option, returns default option.
	return !agrsNamed.Exists(opt) ?
				defOpt :
				// Returns true if typeof(defOpt) is boolean, otherwise returns empty string
				// if default option is null and agrsNamed.Item(opt) is null.
				(typeof(defOpt) == "boolean") || agrsNamed.Item(opt) || (defOpt || "");
};

ScriptCleaner.Options.prototype.FormatScript = function()
{
	return this.sectionsToSort.length != 0;
};

ScriptCleaner.Options.InScriptFlag = function(opt)
{
	return (opt != null) && (opt.toLowerCase().indexOf(ScriptCleaner.Options.flagInScript) != -1);
};

ScriptCleaner.Options.OutScriptFlag = function(opt)
{
	return (opt != null) && (opt.trim().length == 0 || opt.toLowerCase().indexOf(ScriptCleaner.Options.flagOutScript) != -1);
};

ScriptCleaner.Options.prototype.CompileInScript = function()
{
	return ScriptCleaner.Options.InScriptFlag(this.compileScripts);
};

ScriptCleaner.Options.prototype.CompileOutScript = function()
{
	return ScriptCleaner.Options.OutScriptFlag(this.compileScripts);
};

ScriptCleaner.Options.prototype.ViewInScript = function()
{
	return ScriptCleaner.Options.InScriptFlag(this.viewScripts);
};

ScriptCleaner.Options.prototype.ViewOutScript = function()
{
	return ScriptCleaner.Options.OutScriptFlag(this.viewScripts);
};

// Default compiler excutable.
ScriptCleaner.Options.defaultCompiler	=	"ISCC.exe";

// Sort option.
ScriptCleaner.Options.optSort			=	"s";
// Compile scripts.
ScriptCleaner.Options.optCompileScripts	=	"c";
// View scripts.
ScriptCleaner.Options.optViewScripts	=	"v";
// Wait until user presses Enter after script is completed.
ScriptCleaner.Options.optWaitEnter		=	"w";
// Compiler executable.
ScriptCleaner.Options.optCompiler		=	"cc";
// Do not show logo.
ScriptCleaner.Options.optNoLogo			=	"nologo";

// Input script flag.
ScriptCleaner.Options.flagInScript		=	"i";
// Output script flag.
ScriptCleaner.Options.flagOutScript		=	"o";
