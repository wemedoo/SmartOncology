
var validator;
$(document).ready(function () {

    validator = $('#csvUploadForm').validate({
        rules: {
            codesetName: {
                required: true,
                remote: {
                    async: false,
                    url: `/CodeSet/IsCodeSetAvailableByPreferredTerm`,
                    type: 'GET',
                    data: {
                        codesetName: function () {
                            return $('#codesetName').val();
                        }
                    },
                }
            }
        },
    });

    $(document).on('hide.bs.modal', '#uploadModal', function (e) {  // on modal closing
        validator.resetForm();  // resets error messages
        resetCsvForm();
    })

});

$("#csvUploadForm").on("submit", function (e) {
    e.preventDefault();  // REQUIRED !!!
    uploadCSV();
})

function uploadCSV() {
    let fd = new FormData();
    let myFile = $("#file").prop('files')[0];
    let codesetName = $('#codesetName').val();
    let applicableInDesigner = $('#applicableInDesignerCsv-yes').is(':checked'); 

    fd.append('file', myFile);
    fd.append('codesetName', codesetName);
    fd.append('applicableInDesigner', applicableInDesigner);
    
    if ($('#csvUploadForm').valid()) {
        callServer({
            url: `/CodeSet/ImportAsCsv`,
            data: fd,
            processData: false,
            contentType: false,
            type: 'POST',
            success: function (data) {
                codesetCreatedSuccess();
                $('#uploadModal').modal('hide');
                reloadTable();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    return false;
}

function resetCsvForm() {
    $('#codesetName').val("");
    $('#file').val("");
    $(".remove-file").trigger("click");
    $('input[name="applicableInDesignerCsv"]').prop('checked', false);
}