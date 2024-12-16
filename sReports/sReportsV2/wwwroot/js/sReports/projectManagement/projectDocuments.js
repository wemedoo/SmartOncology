
$(document).ready(function () {

    $('#DocumentTitleModal').initSelect2(
        getSelect2Object(
            {
                placeholder: '-',
                width: '100%',
                allowClear: true,
                url: `/ProjectManagement/GetDocumentsAutoCompleteName`,
                customAjaxData: function (params) {
                    clearNoResultContent();
                    return {
                        Term: params.term,
                        Page: params.page || 1,
                        projectId: $('#projectId').val(),
                    };
                }
            }
        )
    );

});

function clearNoResultContent() {
    var ulElement = document.getElementById("select2-DocumentTitleModal-results");
    var alertLiElement = ulElement.querySelector('li[role="alert"]');

    if (alertLiElement) {
        ulElement.removeChild(alertLiElement);
    }
}

function reloadTrialDocumentsTable() {

    setFilterFromUrl();
    let requestObject = getFilterParametersObject("#TrialDocumentsTable");
    setFilterTagsFromObj(requestObject, "TrialDocumentsTable");
    hideTrialIdFilterTag();

    setTableProperties(requestObject);
    addPropertyToObject(requestObject, 'IsReadOnly', $("#isReadOnly").val());

    callServer({
        type: 'GET',
        url: '/ProjectManagement/ReloadDocumentsTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data, "#TrialDocumentsTable");
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function addDocumentToProject() {

    let requestObject = {};
    requestObject['formId'] = $('#DocumentTitleModal').val();
    requestObject['projectId'] = $('#projectId').val();

    callServer({
        type: 'POST',
        url: '/ProjectManagement/AddDocumentToProject',
        data: requestObject,
        success: function (data) {
            reloadTrialDocumentsTable();
            toastr.success('Documentation Added');
            $('#DocumentTitleModal').val(null).trigger('change');
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

// ----- Modal -----

function showAddTrialDocumentsModal() {
    $('#AddTrialDocumentsModal').modal('show');
}

$(document).on('hidden.bs.modal', '#AddTrialDocumentsModal', function (event) {
    $('#DocumentTitleModal').val(null).trigger('change');
})


// ----- 3 Dots operations -----

function viewDocumentation(event, formId, versionId, thesaurusId, showUserProjects) {
    event.stopPropagation();
    event.preventDefault();
    if (showUserProjects != "")
        window.open(`/FormInstance/GetAllByUserProject?VersionId=${versionId}&FormId=${formId}&ThesaurusId=${thesaurusId}&ProjectId=${$('#projectId').val()}`, "newTab");
    else
        window.open(`/FormInstance/GetAllByProject?VersionId=${versionId}&FormId=${formId}&ThesaurusId=${thesaurusId}&ProjectId=${$('#projectId').val()}`, "newTab");

}

function removeDocumentation(event, formId) {
    event.stopPropagation();
    event.preventDefault();

    let requestObject = {};
    requestObject['formId'] = formId;
    requestObject['projectId'] = $('#projectId').val();

    callServer({
        type: "DELETE",
        url: `/ProjectManagement/RemoveDocumentFromProject`,
        data: requestObject,
        success: function (data) {
            $(`#row-${formId}`).remove();
            toastr.success('Removed');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}
