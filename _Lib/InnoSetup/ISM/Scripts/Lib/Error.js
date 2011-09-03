// Error object methods.

// Returns error string representation.
Error.prototype.toString = function()
{
	return	"[ERROR] %DESCRIPTION% (%NAME% [%NUMBER%])"
				.replace(/%DESCRIPTION%/g, this.description)
				.replace(/%NAME%/g, this.name)
				.replace(/%NUMBER%/g, this.number);
};
