
var mjjames = mjjames || {};
mjjames.LocalStorageService = function () {
    
    var FileUploadError = function (fileSubmitSelector, fileInputSelector) {
        SetUploadStatus("Error", fileSubmitSelector, fileInputSelector);
        alert("Sorry there has been a problem uploading your file, please try again");
    };

    var UserLoggedOutError = function (fileSubmitSelector, fileInputSelector) {
        SetUploadStatus("Error", fileSubmitSelector, fileInputSelector);
        alert("Sorry, you are currently logged out of the system, please login and try again");
    };

    var FileUploadComplete = function (fileName, fileInputSelector, fileSubmitSelector) {
        SetUploadStatus("Complete", fileSubmitSelector, fileInputSelector);
        //stash our file in our hidden input
        $(fileInputSelector).closest("div.fileuploadWrapper").find(":hidden").val(fileName);
        Debug(fileName);
        Debug(String(fileName).substring(fileName.length - 3));
        switch (String(fileName).toLowerCase().substring(fileName.length - 3)) {
            case "jpg": case "png": case "gif":
                RenderPreviewImage(fileName, fileInputSelector);
                break;
            case "pdf":
                RenderPreviewImage("/admin/images/pdfpreview.png", fileInputSelector);
                break;
            default:
                break;
        }
    };

    var RenderPreviewImage = function (fileName, fileInputSelector) {

        var $parent = $(fileInputSelector).closest("div.fileuploadWrapper");

        var $preview = $parent.find("img.previewImg");
        $preview.attr("src", fileName);
        $preview.attr("data-content", "<img src='" + fileName + "' />");

    };

    var InitUploader = function (fileInputSelector, fileSubmitSelector, mediaType) {
        $(fileSubmitSelector).click(function () {
            if ($(fileInputSelector).val().length === 0) {
                alert("Please Ensure a File Is Provided Before Clicking Upload");
                return false;
            }
            return UploadFile(mediaType, fileInputSelector, fileSubmitSelector);
        });
    };

    var UploadFile = function (mediaType, fileInputSelector, fileSubmitSelector) {
        SetUploadStatus("Uploading", fileSubmitSelector, fileInputSelector);
        $("form").ajaxSubmit({
            url: '/admin/files/' + mediaType + '/',
            method: 'post',
            beforeSend: function () {
                UploadPercent('0%', fileInputSelector);
            },
            uploadProgress: function (event, position, total, percentComplete) {
                UploadPercent(percentComplete + '%', fileInputSelector);
            },
            success: function (response) {
                //if we dont get any image locations back something has gone wrong its just not thrown a 500 error
                if (response.length === 0) {
                    FileUploadError(fileSubmitSelector, fileInputSelector);
                    return;
                }
                FileUploadComplete(response[0],fileInputSelector, fileSubmitSelector);
            },
            error: function (result) {
                Debug(result.responseText);
                if (result.status === 401) {
                    UserLoggedOutError(fileSubmitSelector, fileInputSelector);
                } else {
                    FileUploadError(fileSubmitSelector, fileInputSelector);
                }
            }
        });

        return false;
    };

    var UploadPercent = function (percent, fileInputSelector) {
        var $bar = $(fileInputSelector).siblings("div.progress").find("div.bar");
        $bar.css("width", percent);
    };

    var SetUploadStatus = function (status, fileSubmitSelector, fileInputSelector) {
        var $button = $(fileSubmitSelector);
        switch (status) {
            case "Uploading":
                $button.val("Uploading...");
                $button.attr("disabled", "disabled");
                break;
            case "Complete":
                $button.val("Complete");
                setTimeout(function () {
                    SetUploadStatus("Reset", fileSubmitSelector, fileInputSelector);
                    $(fileInputSelector).val('');
                }, 3000);
                break;
            case "Error":
                $button.val("Error Occurred");
                setTimeout(function () {
                    SetUploadStatus("Reset", fileSubmitSelector, fileInputSelector);
                }, 3000);
                break;
            case "Reset":
                $button.val("Upload");
                $button.removeAttr("disabled");
                UploadPercent('0%', fileInputSelector);
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
            Debug("File Selector: " + fileInputSelector);
            Debug("File Submit Selector: " + fileSubmitSelector);
            Debug("Media Type: " + mediaType);
            InitUploader(fileInputSelector, fileSubmitSelector, mediaType);
        }
    };
}();