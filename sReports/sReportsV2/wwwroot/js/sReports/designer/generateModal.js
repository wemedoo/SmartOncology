function showGenerateModal(e, formId) {
    executeEventFunctions(e, true);
    callServer({
        type: "GET",
        url: "/Form/GetGenerateNewLanguage",
        data: { formId },
        success: function (data) {
            $('#generateModal')
                .html(data)
                .modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function generateNewLanguage() {
    if ($('#newGenerateForm').valid()) {
        var request = {
            formId: $('#generatedFormId').val(),
            language: $('#language').val()
        };

        callServer({
            type: "POST",
            url: "/Form/GenerateNewLanguage",
            data: request,
            success: function (data) {
                toastr.success(data.message);
                $('#generateModal').modal('hide');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }

    return false;
}