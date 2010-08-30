/// </// <reference path="jquery-1.3.2-vsdoc.js"/>

var mjjames = mjjames || {};
mjjames.StorageServices = function () {
	var _fileInputSelector;
	var serviceAlias;

	var FileUploadError = function () {
		alert("Sorry there has been a problem uploading your file, please try again");
	};

	var FileUploadComplete = function (fileName) {
		//stash our file in our hidden input
		$(_fileInputSelector).val(_serviceAlias + fileName);
		Debug(_serviceAlias + fileName);
		alert(fileName + " Upload Completed");
		Debug(String(fileName).substring(fileName.length - 3));
		switch (String(fileName).substring(fileName.length - 3)) {
			case "jpg":
				RenderPreviewImage();
				break;
			case "png":
				RenderPreviewImage();
				break;
			case "gif":
				RenderPreviewImage();
				break;
			default:
				RenderPreview();
		}
	};

	var RenderPreview = function () {
		var $fileInput = $(_fileInputSelector);

		//if we can't find a file input drop out
		if ($fileInput.length === 0) {
			return;
		}

		//if we have no file path no need to render anything
		if ($fileInput.val().length === 0) {
			return;
		}
		//find our file indicator text field
		var $storageservicefile = $fileInput.closest("div.row").find("input.storageservicefile");
		//if we dont have one create one
		if ($storageservicefile.length === 0) {
			$storageservicefile = $fileInput.closest("div")
										.before('<input readonly="readonly" class="storageservicefile field textbox" value="" />')
										.closest("div.row")
										.find("input.storageservicefile");
		}
		//set the textboxes value to the hidden input
		$storageservicefile.val($fileInput.val());
		//if someone clicks the clear button ensure our textbox value is also cleared
		$fileInput.siblings("input[type=submit]").click(function () {
			$storageservicefile.val("");
		});
	};

	var RenderPreviewImage = function () {
		var $fileInput = $(_fileInputSelector);

		//if we can't find a file input drop out
		if ($fileInput.length === 0) {
			return;
		}

		//if we have no file path no need to render anything
		if ($fileInput.val().length === 0) {
			return;
		}
		//find our file indicator text field
		var $storageservicefile = $fileInput.closest("div.row").find("img.storageservicefile");
		//if we dont have one create one
		if ($storageservicefile.length === 0) {
			$storageservicefile = $fileInput.closest("div")
										.before('<img class="storageservicefile previewImg" src="" alt="Preview" />')
										.closest("div.row")
										.find("img.storageservicefile");
		}
		//set the textboxes value to the hidden input
		$storageservicefile.attr("src", $fileInput.val());
		//if someone clicks the clear button ensure our textbox value is also cleared
		$fileInput.siblings("input[type=submit]").click(function () {
			$storageservicefile.attr("src", "");
		});
	};

	var Debug = function (text) {
		if (console !== undefined) {
			console.log(text);
		}
	};

	return {
		Init: function (fileInputSelector, serviceAlias) {
			_fileInputSelector = fileInputSelector;
			Debug("File Selector: " + _fileInputSelector);
			_serviceAlias = serviceAlias;
			Debug("Service Alias: " + _serviceAlias);
			RenderPreview();
		},
		SuccessfullUpload: function (response) {
			if (response.isError) {
				FileUploadError();
				return;
			}
			FileUploadComplete(response.name);
		},
		UploadComplete: function (sender, args) {
		   // FileUploadComplete(args.get_fileName());
		},
		FileStateChanged: function (uploader, file, httpStatus, isLast) {
			Flajaxian.fileStateChanged(uploader, file, httpStatus, isLast);
			//if we have an error state indicate this
			if (file.state === Flajaxian.File_Error) {
				FileUploadError();
				Debug("An Error Has Occurred");
				return;
			}
			//if we are still uploading log our current state
			if (file.state === Flajaxian.File_Uploading) {
				//we can only log if we have a console object
				Debug("bytes:" + file.bytes + " name:" + file.name + " state:" + file.state + " httpStatus:" + httpStatus + " isLast:" + isLast);
			}
			if (file.state > Flajaxian.File_Uploading && isLast) {
				FileUploadComplete(file.name);
			}
		}
	};
} ();