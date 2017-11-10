// InnoSetup script cleaner object.

// inFileName - file to read from.
// outFileName - file to write to. WILL BE OVERWRITTEN WITHOUT WARNING.
// [optional] options - cleanup options.
// [optional] progress - object { read, format, write, compile, view: function({file: processedFileName}) }
//		[optional] read    - called before reading.
//		[optional] format  - called after reading and before writing.
//		[optional] write   - called after formatting and before writing.
//		[optional] compile - called after writing and before compiling.
//		[optional] view    - called after compiling.
function ScriptCleaner(inFileName, outFileName, options, progress)
{
	this.inFileName		=	inFileName;
	this.outFileName	=	outFileName;
	this.options		=	options;
	this.BOM		=	null;
}
