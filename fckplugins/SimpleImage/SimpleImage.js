﻿/* MJJames SimpleImage - provides a cut down version of the FCK Image Dialog */

/*
* FCKeditor - The text editor for Internet - http://www.fckeditor.net
* Copyright (C) 2003-2007 Frederico Caldeira Knabben
*
* == BEGIN LICENSE ==
*
* Licensed under the terms of any of the following licenses at your
* choice:
*
*  - GNU General Public License Version 2 or later (the "GPL")
*    http://www.gnu.org/licenses/gpl.html
*
*  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
*    http://www.gnu.org/licenses/lgpl.html
*
*  - Mozilla Public License Version 1.1 or later (the "MPL")
*    http://www.mozilla.org/MPL/MPL-1.1.html
*
* == END LICENSE ==
*
* Scripts related to the SimpleImage dialog window (see SimpleImage.html).
*/

var dialog		= window.parent ;
var oEditor		= dialog.InnerDialogLoaded() ;
var FCK			= oEditor.FCK ;
var FCKLang		= oEditor.FCKLang ;
var FCKConfig	= oEditor.FCKConfig ;
var FCKDebug	= oEditor.FCKDebug ;
var FCKTools	= oEditor.FCKTools ;

//#### Dialog Tabs

// Set the dialog tabs.
window.parent.AddTab('Info', FCKLang.DlgImgInfoTab);
window.parent.AddTab('Link', FCKLang.DlgImgLinkTab);


// Function called when a dialog tag is selected.
function OnDialogTabChange(tabCode) {
	ShowE('divInfo', (tabCode == 'Info'));
	ShowE('divLink', (tabCode == 'Link'));
}

// Get the selected image (if available).
var oImage = dialog.Selection.GetSelectedElement() ;

if (oImage && oImage.tagName != 'IMG')
	oImage = null;

// Get the active link.
var oLink = dialog.Selection.GetSelection().MoveToAncestorNode( 'A' ) ;

var oImageOriginal ;

function UpdateOriginal( resetSize )
{
	if ( !eImgPreview )
		return ;

	if ( GetE('txtUrl').value.length == 0 )
	{
		oImageOriginal = null ;
		return ;
	}

	oImageOriginal = document.createElement( 'IMG' ) ;	// new Image() ;

	if ( resetSize )
	{
		oImageOriginal.onload = function()
		{
			this.onload = null ;
			ResetSizes() ;
		}
	}

	oImageOriginal.src = eImgPreview.src ;
}

var bPreviewInitialized ;

window.onload = function () {
	// Translate the dialog box texts.
	oEditor.FCKLanguageManager.TranslatePage(document);

	GetE('btnLockSizes').title = FCKLang.DlgImgLockRatio;
	GetE('btnResetSize').title = FCKLang.DlgBtnResetSize;

	// Load the selected element information (if any).
	LoadSelection();

	UpdateOriginal();

	window.parent.SetAutoSize(true);

	// Activate the "OK" button.
	dialog.SetOkButton( true ) ;

	SelectField( 'txtUrl' ) ;
}

function LoadSelection() {
	if (!oImage) return;

	var sUrl = oImage.getAttribute('_fcksavedurl');

	if (sUrl == null) {
		sUrl = GetAttribute(oImage, 'src', '');
	}

	GetE('txtUrl').value = sUrl;

	GetE('txtAlt').value = GetAttribute(oImage, 'alt', '');

	var iWidth, iHeight ;

	var regexSize = /^\s*(\d+)px\s*$/i ;

	if ( oImage.style.width )
	{
		var aMatchW  = oImage.style.width.match( regexSize ) ;
		if ( aMatchW )
		{
			iWidth = aMatchW[1] ;
			oImage.style.width = '' ;
			SetAttribute( oImage, 'width' , iWidth ) ;
		}
	}

	if ( oImage.style.height )
	{
		var aMatchH  = oImage.style.height.match( regexSize ) ;
		if ( aMatchH )
		{
			iHeight = aMatchH[1] ;
			oImage.style.height = '' ;
			SetAttribute( oImage, 'height', iHeight ) ;
		}
	}

	GetE('txtWidth').value	= iWidth ? iWidth : GetAttribute( oImage, "width", '' ) ;
	GetE('txtHeight').value	= iHeight ? iHeight : GetAttribute( oImage, "height", '' ) ;

	// Get Advances Attributes
	//	GetE('txtAttId').value			= oImage.id ;
	//	GetE('cmbAttLangDir').value		= oImage.dir ;
	//	GetE('txtAttLangCode').value	= oImage.lang ;
	GetE('txtAttTitle').value = oImage.title;
	//	GetE('txtLongDesc').value		= oImage.longDesc ;

	//	if ( oEditor.FCKBrowserInfo.IsIE )
	//	{
	//		GetE('txtAttClasses').value = oImage.className || '' ;
	//		GetE('txtAttStyle').value = oImage.style.cssText ;
	//	}
	//	else
	//	{
	//		GetE('txtAttClasses').value = oImage.getAttribute('class',2) || '' ;
	//		GetE('txtAttStyle').value = oImage.getAttribute('style',2) ;
	//	}

	if (oLink) {
		var sLinkUrl = oLink.getAttribute('_fcksavedurl');
		if (sLinkUrl == null)
			sLinkUrl = oLink.getAttribute('href', 2);

		GetE('txtLnkUrl').value = sLinkUrl;
		GetE('cmbLnkTarget').value = oLink.target;
	}

	UpdatePreview();
}

//#### The OK button was hit.
function Ok()
{
	if ( GetE('txtUrl').value.length == 0 )
	{
		dialog.SetSelectedTab( 'Info' ) ;
		GetE('txtUrl').focus() ;

		alert( FCKLang.DlgImgAlertUrl ) ;

		return false ;
	}

	var bHasImage = (oImage != null);

	oEditor.FCKUndo.SaveUndoStep();
	if (!bHasImage) {
		oImage = FCK.InsertElement('img');
	}

	UpdateImage(oImage);

	var sLnkUrl = GetE('txtLnkUrl').value.Trim();

	if (sLnkUrl.length == 0) {
		if (oLink)
			FCK.ExecuteNamedCommand('Unlink');
	}
	else {
		if (oLink)	// Modifying an existent link.
			oLink.href = sLnkUrl;
		else			// Creating a new link.
		{
			if (!bHasImage)
				oEditor.FCKSelection.SelectNode(oImage);

			oLink = oEditor.FCK.CreateLink(sLnkUrl)[0];

			if (!bHasImage) {
				oEditor.FCKSelection.SelectNode(oLink);
				oEditor.FCKSelection.Collapse(false);
			}
		}

		SetAttribute(oLink, '_fcksavedurl', sLnkUrl);
		SetAttribute(oLink, 'target', GetE('cmbLnkTarget').value);
	}

	return true;
}

function UpdateImage(e, skipId) {
	e.src = GetE('txtUrl').value;
	SetAttribute(e, "_fcksavedurl", GetE('txtUrl').value);

	SetAttribute(e, "alt", GetE('txtAlt').value);
	SetAttribute(e, "width", GetE('txtWidth').value);
	SetAttribute(e, "height", GetE('txtHeight').value);


	// Advances Attributes
	SetAttribute(e, 'title', GetE('txtAttTitle').value);
}

var eImgPreview ;
var eImgPreviewLink ;

function SetPreviewElements( imageElement, linkElement )
{
	eImgPreview = imageElement ;
	eImgPreviewLink = linkElement ;

	UpdatePreview() ;
	UpdateOriginal() ;

	bPreviewInitialized = true ;
}

function UpdatePreview()
{
	if ( !eImgPreview || !eImgPreviewLink )
		return ;

	if ( GetE('txtUrl').value.length == 0 )
		eImgPreviewLink.style.display = 'none' ;
	else
	{
		UpdateImage( eImgPreview, true ) ;

		if ( GetE('txtLnkUrl').value.Trim().length > 0 )
			eImgPreviewLink.href = 'javascript:void(null);' ;
		else
			SetAttribute( eImgPreviewLink, 'href', '' ) ;

		eImgPreviewLink.style.display = '' ;
	}
}

var bLockRatio = true ;

function SwitchLock( lockButton )
{
	bLockRatio = !bLockRatio ;
	lockButton.className = bLockRatio ? 'BtnLocked' : 'BtnUnlocked' ;
	lockButton.title = bLockRatio ? 'Lock sizes' : 'Unlock sizes' ;

	if ( bLockRatio )
	{
		if ( GetE('txtWidth').value.length > 0 )
			OnSizeChanged( 'Width', GetE('txtWidth').value ) ;
		else
			OnSizeChanged( 'Height', GetE('txtHeight').value ) ;
	}
}

// Fired when the width or height input texts change
function OnSizeChanged( dimension, value )
{
	// Verifies if the aspect ration has to be maintained
	if ( oImageOriginal && bLockRatio )
	{
		var e = dimension == 'Width' ? GetE('txtHeight') : GetE('txtWidth') ;

		if ( value.length == 0 || isNaN( value ) )
		{
			e.value = '' ;
			return ;
		}

		if ( dimension == 'Width' )
			value = value == 0 ? 0 : Math.round( oImageOriginal.height * ( value  / oImageOriginal.width ) ) ;
		else
			value = value == 0 ? 0 : Math.round( oImageOriginal.width  * ( value / oImageOriginal.height ) ) ;

		if ( !isNaN( value ) )
			e.value = value ;
	}

	UpdatePreview() ;
}

// Fired when the Reset Size button is clicked
function ResetSizes()
{
	if ( ! oImageOriginal ) return ;
	if ( oEditor.FCKBrowserInfo.IsGecko && !oImageOriginal.complete )
	{
		setTimeout( ResetSizes, 50 ) ;
		return ;
	}

	GetE('txtWidth').value  = oImageOriginal.width ;
	GetE('txtHeight').value = oImageOriginal.height ;

	UpdatePreview() ;
}

function BrowseServer() {
	OpenServerBrowser(
		'Image',
		FCKConfig.ImageBrowserURL,
		FCKConfig.ImageBrowserWindowWidth,
		FCKConfig.ImageBrowserWindowHeight);

}

function LnkBrowseServer() {
	OpenServerBrowser(
		'Link',
		FCKConfig.LinkBrowserURL,
		FCKConfig.LinkBrowserWindowWidth,
		FCKConfig.LinkBrowserWindowHeight);
}

function OpenServerBrowser(type, url, width, height) {
	sActualBrowser = type;
	OpenFileBrowser(url, width, height);
}

var sActualBrowser;

function SetUrl(url, width, height, alt) {
	if (sActualBrowser == 'Link') {
		GetE('txtLnkUrl').value = url;
		UpdatePreview();
	}
	else {
		GetE('txtUrl').value = url;
		GetE('txtWidth').value = width ? width : '';
		GetE('txtHeight').value = height ? height : '';

		if (alt)
			GetE('txtAlt').value = alt;

		UpdatePreview();
		UpdateOriginal(true);
	}

	window.parent.SetSelectedTab('Info');
}
