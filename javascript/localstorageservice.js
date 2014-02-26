
var mjjames = mjjames || {};
mjjames.LocalStorageService = function () {
    var _fileInputSelector;
    var _fileSubmitSelector;
    var _mediaType;

    var FileUploadError = function () {
        SetUploadStatus("Error");
        alert("Sorry there has been a problem uploading your file, please try again");
    };

    var UserLoggedOutError = function () {
        SetUploadStatus("Error");
        alert("Sorry, you are currently logged out of the system, please login and try again");
    };

    var FileUploadComplete = function (fileName) {
        SetUploadStatus("Complete");
        //stash our file in our hidden input
        $(_fileInputSelector).closest("div.fileuploadWrapper").find(":hidden").val(fileName);
        Debug(fileName);
        Debug(String(fileName).substring(fileName.length - 3));
        switch (String(fileName).toLowerCase().substring(fileName.length - 3)) {
            case "jpg": case "png": case "gif":
                RenderPreviewImage(fileName);
                break;
            case "pdf":
                RenderPreviewImage("/admin/images/pdfpreview.png");
                break;
            default:
                break;
        }
    };

    var RenderPreviewImage = function (fileName) {

        var $parent = $(_fileInputSelector).closest("div.fileuploadWrapper");

        var $preview = $parent.find("img.previewImg");
        $preview.attr("src", fileName);
        $preview.attr("data-content", "<img src='" + fileName + "' />");

    };

    var InitUploader = function () {
        $(_fileSubmitSelector).click(function () {
            if ($(_fileInputSelector).val().length === 0) {
                alert("Please Ensure a File Is Provided Before Clicking Upload");
                return false;
            }
            return UploadFile();
        });
    };

    var UploadFile = function () {
        SetUploadStatus("Uploading");
        $("form").ajaxSubmit({
            url: '/admin/files/' + _mediaType + '/',
            method: 'post',
            beforeSend: function () {
                UploadPercent('0%');
            },
            uploadProgress: function (event, position, total, percentComplete) {
                UploadPercent(percentComplete + '%');
            },
            success: function (response) {
                //if we dont get any image locations back something has gone wrong its just not thrown a 500 error
                if (response.length === 0) {
                    FileUploadError();
                    return;
                }
                FileUploadComplete(response[0]);
            },
            error: function (result) {
                Debug(result.responseText);
                if (result.status === 401) {
                    UserLoggedOutError();
                } else {
                    FileUploadError();
                }
            }
        });

        return false;
    };

    var UploadPercent = function (percent) {
        var $bar = $(_fileInputSelector).siblings("div.progress").find("div.bar");
        $bar.css("width", percent);
    };

    var SetUploadStatus = function (status) {
        var $button = $(_fileSubmitSelector);
        switch (status) {
            case "Uploading":
                $button.val("Uploading...");
                $button.attr("disabled", "disabled");
                break;
            case "Complete":
                $button.val("Complete");
                setTimeout(function () {
                    SetUploadStatus("Reset");
                    $(_fileInputSelector).val('');
                }, 3000);
                break;
            case "Error":
                $button.val("Error Occurred");
                setTimeout(function () {
                    SetUploadStatus("Reset");
                }, 3000);
                break;
            case "Reset":
                $button.val("Upload");
                $button.removeAttr("disabled");
                UploadPercent('0%');
                break;
        }
    };

    var Debug = function (text) {
        if (console !== undefined) {
            console.log(text);
        }
    };

    return {
        Init: function (fileInputSelector, fileSubmitSelector, mediaType) {
            _fileInputSelector = fileInputSelector;
            Debug("File Selector: " + _fileInputSelector);
            _fileSubmitSelector = fileSubmitSelector;
            Debug("File Submit Selector: " + _fileSubmitSelector);
            _mediaType = mediaType;
            Debug("Media Type: " + _mediaType);
            InitUploader();
        }
    };
}();