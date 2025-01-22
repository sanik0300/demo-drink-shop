function onImageAdded()
{
    var formData = new FormData($('#editingForm')[0]);
    if (formData == null) { return; }

    $.ajax({
        url: '/Admin/ProposeImage',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function ()
        {
            var fr = new FileReader();

            fr.onload = function (e)
            {
                document.getElementById("adminImgPreview").src = e.target.result;

                document.getElementById("removeImgBtn").disabled = false;
                document.getElementById("imgResetBtn").disabled = false;
            }
            var fileInput = document.getElementById("adminImageInput");
            fr.readAsDataURL(fileInput.files[0]);
        },
        error: function () {

            return;
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