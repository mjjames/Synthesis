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
var oHyperLink = dialog.Selection.GetSelection().MoveToAncestorNode('A');
if (oHyperLink)
	FCK.Selection.SelectNode(oHyperLink);

//if what is selected isn't a hyperlink set to null
if (oHyperLink && oHyperLink.tagName != 'A') {
    oHyperLink = null;
}

function LoadSelection() {
    if (!oHyperLink) {
		return;
	}
    var url = oHyperLink.getAttribute('href');

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
	// If no link is selected, create a new one (it may result in more than one link creation - #220).
	var aLinks = oHyperLink ? [oHyperLink] : oEditor.FCK.CreateLink(url, true);

	// If no selection, no links are created, so use the uri as the link text (by dom, 2006-05-26)
	var aHasSelection = (aLinks.length > 0);
	if (!aHasSelection) {
	    sInnerHtml = url;
        // Create a new (empty) anchor.
	    aLinks = [oEditor.FCK.InsertElement('a')];
	}
	var sInnerHtml;

	for (var i = 0; i < aLinks.length; i++) {
	    oHyperLink = aLinks[i];

	    if (aHasSelection)
	        sInnerHtml = oHyperLink.innerHTML; 	// Save the innerHTML (IE changes it if it is like an URL).

	    oHyperLink.href = url;
	    SetAttribute(oHyperLink, '_fcksavedurl', url);

	    oHyperLink.innerHTML = sInnerHtml; 	// Set (or restore) the innerHTML
	}

	// Select the (first) link.
	oEditor.FCKSelection.SelectNode(aLinks[0]);
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