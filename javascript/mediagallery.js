/// </// <reference path="jquery-1.3.2-vsdoc.js"/>

var mjjames = mjjames || {};
mjjames.MediaGallery = function () {
	var _accordianSelector;
	var _uploadPath = "";
	var SetupVideoPreview = function () {

	};

	return {
		Init: function (accordianSelector, uploadPath) {
			_accordianSelector = accordianSelector;
			_uploadPath = uploadPath; 
			SetupVideoPreview();
		}
	};
} ();