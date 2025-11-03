function onImageAdded() {
    let fileInput = document.getElementById("adminImageInput");
    var fileGiven = fileInput.files[0];

    $.ajax({
        url: '/Admin/ProposeImage',
        type: 'POST',
        data: {
            contentTypeString: fileGiven.type
        },
        success: function () {
            var fr = new FileReader();

            fr.onload = function (e) {
                document.getElementById("adminImgPreview").src = e.target.result;

                document.getElementById("removeImgBtn").disabled = false;
                document.getElementById("imgResetBtn").disabled = false;
            }
            fr.readAsDataURL(fileGiven);
        }
    });
 }

function onProductEdit() {

    var formData = new FormData($('#editingForm')[0]);
    if (formData == null) { return; }

    let dataUrl = document.getElementById("adminImgPreview").src;
    formData.append("dataUrl", dataUrl);

    $.ajax({
        url: '/Admin/Edit',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {

            document.open();
            document.write(response);
            document.close();
        }
    });
}

function onImageCancelOrDelete(shouldDelete) {

    var formData = new FormData($('#editingForm')[0]);
    formData.append("shouldDelete", shouldDelete);

    if (formData == null) { return; }

    $.ajax({
        url: '/Admin/CancelOrDeleteImage',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {

            document.getElementById("adminImgPreview").src = response.pathForJs;
            document.getElementById("removeImgBtn").disabled = !response.canRemove;
            document.getElementById("adminImageInput").value = null;
        },
        error: function () {

            return;
        }
    });
}