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
        alert(fileName + " Upload Completed");
        RenderPreview();
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
            $storageservicefile = $fileInput.closest("div").before('<input readonly="readonly" class="storageservicefile field textbox" value="" />').closest("div.row").find("input.storageservicefile");
        }
        //set the textboxes value to the hidden input
        $storageservicefile.val($fileInput.val());
        //if someone clicks the clear button ensure our textbox value is also cleared
        $fileInput.siblings("input[type=submit]").click(function () {
            $storageservicefile.val("");
        });
    };

    return {
        Init: function (fileInputSelector, serviceAlias) {
            _fileInputSelector = fileInputSelector;
            _serviceAlias = serviceAlias;
            RenderPreview();
        },
        SuccessfullUpload: function (response) {
            if (response.isError) {
                FileUploadError();
                return;
            }
            FileUploadComplete(response.name);
        },
        FileStateChanged: function (uploader, file, httpStatus, isLast) {
            Flajaxian.fileStateChanged(uploader, file, httpStatus, isLast);
            //if we have an error state indicate this
            if (file.state === Flajaxian.File_Error) {
                FileUploadError();
                return;
            }
            //if we are still uploading log our current state
            if (file.state === Flajaxian.File_Uploading) {
                //we can only log if we have a console object
                if (console !== undefined) {
                    console.log("bytes:" + file.bytes + " name:" + file.name + " state:" + file.state + " httpStatus:" + httpStatus + " isLast:" + isLast);
                }
            }
            if (file.state > Flajaxian.File_Uploading && isLast) {
                FileUploadComplete(file.name);
            }
        }
    };
} ();