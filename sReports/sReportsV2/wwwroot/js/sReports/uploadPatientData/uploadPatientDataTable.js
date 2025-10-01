function downloadEntity(event, dataGuidName) {
    event.preventDefault();

    downloadResource(event, dataGuidName, '', 'uploadPatientData');
}

function showPrompt(event, prompt) {
    $('#prompt-text').text(prompt);
    $('#prompt-result').removeClass('d-none');
}

function proceedLLM(event, id) {
    event.preventDefault();

    callServer({
        type: 'GET',
        url: `/UploadPatientData/ProceedLLM?uploadPatientDataId=${id}`,
        success: function (data) {
            $('#llm-modal-body').html(data);
            $('#llmPreviewModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '#uploadPatientDataBtn', function (e) {
    $('#uploadPatientDataFileBtn').click();
});

$(document).on('change', '#uploadPatientDataFileBtn', function () {
    let $fileInput = $(this);
    let files = $fileInput.prop('files');
    if (files) {
        let filesData = [];
        for (let i = 0; i < files.length; i++) {
            filesData.push({
                content: files[i]
            });
        }
        
        sendFileData(filesData,
            undefined,
            function (resourceName) {
                window.location.reload();
            },
            getBinaryDomain('uploadPatientData'),
            '/UploadPatientData/UploadPatientData',
            true
        );
        $fileInput.val('');
    }
});

function reloadTable() {
    let requestObject = applyActionsBeforeServerReload(['NameGiven', 'NameFamily', 'DateTimeFrom', 'DateTimeTo', 'page', 'pageSize']);
    callServer({
        type: 'GET',
        url: '/UploadPatientData/ReloadTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getFilterParametersObject() {
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'DateTimeTo', toLocaleDateStringIfValue($('#dateTimeTo').val()));
        addPropertyToObject(result, 'DateTimeFrom', toLocaleDateStringIfValue($('#dateTimeFrom').val()));
        addPropertyToObject(result, 'NameGiven', $('#nameGiven').val());
        addPropertyToObject(result, 'NameFamily', $('#nameFamily').val());
    }

    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    if (filterObject.hasOwnProperty('PatientListId')) {
        if (filterObject.PatientListId == 0) {
            delete filterObject.PatientListName;
        }
        delete filterObject.PatientListId;
        delete filterObject.ListWithSelectedPatients;
    }

    return filterObject;
}

function mainFilter() {
    filterData();
}

function advanceFilter() {
    filterData();
}