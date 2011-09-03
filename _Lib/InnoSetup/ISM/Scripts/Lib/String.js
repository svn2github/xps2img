// String object expansion methods.

// Leading and trailing whitespaces trim regexp.
String._trimRE = new RegExp().compile(/^\s+|\s+$/g);

// Trims leading and trailing whitespaces.
// Returns trimmed string.
String.prototype.trim = function()
{
	return this.replace(String._trimRE, "");
}

// Case-insensitive string comparison.
String.CompareNoCase = function(a, b)
{
	var iA = a.toLowerCase();
	var iB = b.toLowerCase();
	return iA == iB ? 0 : (iA < iB ? -1 : 1);
};
