/*
* FCKEditor internal url Plugin
*/

var dialog = window.parent;
var oEditor = dialog.InnerDialogLoaded();
var FCK = oEditor.FCK;
var FCKLang = oEditor.FCKLang;
var FCKConfig = oEditor.FCKConfig;
var FCKDebug = oEditor.FCKDebug;
var FCKTools = oEditor.FCKTools;

//#### Dialog Tabs

// Set the dialog tabs.
dialog.AddTab('Info', oEditor.FCKLang.DlgInfoTab);

// Get the selected flash embed (if available).
var oHyperlink = FCK.Selection.GetSelectedElement();

//if what is selected isn't a hyperlink set to null
if (oHyperlink&& oHyperlink.tagName != 'A') {
	oHyperlink = null;
}

function LoadSelection() {
	if (!oHyperlink) {
		return;
	}
	var url = oHyperlink.getAttribute('href');

	GetE('txtUrl').value = url;
	
}



//#### The OK button was hit.
function Ok() {
	var url = GetE('txtUrl').value;
	if (url.length === 0) {
		dialog.SetSelectedTab('Info');
		GetE('txtUrl').focus();
		return false;
	}

	oEditor.FCKUndo.SaveUndoStep();
	if (oHyperlink) {	// Modifying an existent link.
		oHyperlink.href = url;
	}
	else			// Creating a new link.
	{
		oHyperlink = oEditor.FCK.InsertElement('a');
		oHyperlink.href = url;
		oHyperlink.innerHTML = url;
	}
	return true;
}

//onload get the exising url and prepopulate the txt field
window.onload = function () {
	// Translate the dialog box texts.
	oEditor.FCKLanguageManager.TranslatePage(document);

	// Load the selected element information (if any).
	LoadSelection();


	dialog.SetAutoSize(true);
	// Activate the "OK" button.
	dialog.SetOkButton(true);
	SelectField('txtUrl');

};

$(function () {

	$("a.leaf").click(function (e) {
		$("#txtUrl").val($(this).attr('href'));
		return false;
	});
});