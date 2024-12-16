
function saveFormInstanceAndExtractData(event, fieldIdWithDataToExtract, fieldInstanceIdWithDataToExtract) {

    let textToExtract = $(`textarea[data-fieldinstancerepetitionid=${fieldInstanceIdWithDataToExtract}]`).val();

    if (textToExtract.trim() === '') {
        toastr.warning("Data Extraction not possible beacuse Text is empty");
        return false;
    }

    let formInstanceId = $(`#fid input[name=formInstanceId]`).val();
    let extractDataCallback = extractData.bind(null, formInstanceId, fieldIdWithDataToExtract, fieldInstanceIdWithDataToExtract);

    //save formInstance to not lose changes -> callback executed after saving
    clickedSubmit(event, extractDataCallback);
}

function extractData(formInstanceId, fieldIdWithDataToExtract, fieldInstanceIdWithDataToExtract) {

    callServer({
        url: '/Fhir/GenerateDocumentReferenceForDataExtraction',
        type: "POST",
        data: {
            FormInstanceId: formInstanceId,
            FieldIdWithDataToExtract: fieldIdWithDataToExtract,
            FieldInstanceIdWithDataToExtract: fieldInstanceIdWithDataToExtract
        },
        success: function (data, textStatus, xhr) {
            toastr.success("Data Extraction request successfully submitted!");
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });

}

