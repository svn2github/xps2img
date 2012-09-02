var Scripts = {};

Scripts.clipboardCopy = function(id){
	window.clipboardData.setData('Text', document.getElementById(id).innerText + '\r\n');
};
