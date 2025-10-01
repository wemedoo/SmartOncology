function saveFormInstanceAndExtractAIData(event, fieldInstanceIdWithDataToExtract) {
    let formInstanceId = $(`#fid input[name=formInstanceId]`).val();
    let extractDataCallback = extractAIData.bind(null, formInstanceId, fieldInstanceIdWithDataToExtract);

    //save formInstance to not lose changes -> callback executed after saving
    clickedSubmit(event, extractDataCallback);
}

function extractAIData(formInstanceId, fieldInstanceIdWithDataToExtract) {

    callServer({
        url: '/FormInstance/GenerateAIExtraction',
        type: "Get",
        data: {
            FormInstanceId: formInstanceId,
            FieldInstanceIdWithDataToExtract: fieldInstanceIdWithDataToExtract
        },
        success: function (data, textStatus, xhr) {
            toastr.success("Data Extraction request successfully submitted!");
            window.location.reload();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}