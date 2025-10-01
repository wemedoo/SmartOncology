function redirectToCreate() {
    window.location.href = `/Form/Create`;
}

function editEntity(event, thesaurusId, versionId) {
    event.preventDefault();
    window.location.href = `/Form/Edit?thesaurusId=${thesaurusId}&versionId=${versionId}`;
}

function viewEntity(event, thesaurusId, versionId) {
    event.preventDefault();
    window.location.href = `/Form/View?thesaurusId=${thesaurusId}&versionId=${versionId}`;
}

function generateReport(event, formId) {
    event.preventDefault();
    callServer({
        type: 'GET',
        url: '/Form/GenerateReport',
        data: {formId},
        success: function (data) {
            toastr.success('Thesaurus Code Report is successfully generated. Please have a look at your email.');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Title', $('#TitleTemp').val());
        addPropertyToObject(requestObject, 'ThesaurusId', $('#ThesaurusIdTemp').val());
        addPropertyToObject(requestObject, 'State', $('#StateTemp').val());
        addPropertyToObject(requestObject, 'Classes', $('#classes').val());
        addPropertyToObject(requestObject, 'ClassesOtherValue', $('#documentClassOtherInput').val());
        addPropertyToObject(requestObject, 'GeneralPurpose', $('#generalPurpose').val());
        addPropertyToObject(requestObject, 'ContextDependent', $('#contextDependent').val());
        addPropertyToObject(requestObject, 'ExplicitPurpose', $('#explicitPurpose').val());
        addPropertyToObject(requestObject, 'ScopeOfValidity', $('#scopeOfValidity').val());
        addPropertyToObject(requestObject, 'ClinicalDomain', $('#clinicalDomain').val());
        addPropertyToObject(requestObject, 'ClinicalContext', $('#clinicalContext').val());
        addPropertyToObject(requestObject, 'FollowUp', $('#followUp').val());
        addPropertyToObject(requestObject, 'AdministrativeContext', $('#administrativeContext').val());
        addPropertyToObject(requestObject, 'DateTimeTo', toLocaleDateStringIfValue($('#dateTimeTo').val()));
        addPropertyToObject(requestObject, 'DateTimeFrom', toLocaleDateStringIfValue($('#dateTimeFrom').val()));
    }

    return requestObject;
}

function getFilterParametersObjectForDisplay(filterObject) {
    getFilterParameterObjectForDisplay(filterObject, 'State');
    getFilterParameterObjectForDisplay(filterObject, 'Classes');
    getFilterParameterObjectForDisplay(filterObject, 'GeneralPurpose');
    getFilterParameterObjectForDisplay(filterObject, 'ContextDependent');
    getFilterParameterObjectForDisplay(filterObject, 'ExplicitPurpose');
    getFilterParameterObjectForDisplay(filterObject, 'ScopeOfValidity');
    getFilterParameterObjectForDisplay(filterObject, 'ClinicalDomain');
    getFilterParameterObjectForDisplay(filterObject, 'ClinicalContext');
    getFilterParameterObjectForDisplay(filterObject, 'FollowUp');
    getFilterParameterObjectForDisplay(filterObject, 'AdministrativeContext');
    return filterObject;
}

function reloadTable(initLoad) {
    let requestObject = applyActionsBeforeServerReload(['ThesaurusId', 'State', 'Title', 'page', 'pageSize']);
    requestObject.ClinicalDomain = $('#clinicalDomain').find(':selected').attr('id');

    callServer({
        type: 'GET',
        url: '/Form/ReloadTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function deleteFormDefinition(event) {
    event.preventDefault();
    var id = document.getElementById("buttonSubmitDelete").getAttribute('data-id');
    var lastUpdate = document.getElementById("buttonSubmitDelete").getAttribute('data-lastupdate');
    callServer({
        type: "DELETE",
        url: `/Form/Delete?formId=${id}&lastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success('Removed');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function advanceFilter() {
    $('#TitleTemp').val($('#title').val());
    $('#ThesaurusIdTemp').val($('#thesaurusId').val());
    $('#StateTemp').val($('#state').val()).change();
    
    filterData();
}

function mainFilter() {
    $('#title').val($('#TitleTemp').val());
    $('#thesaurusId').val($('#ThesaurusIdTemp').val());
    $('#state').val($('#StateTemp').val()).change();

    filterData();
}

function exportToQuestionnaire(event, formId) {
    event.preventDefault();
    callServer({
        url: `/Fhir/ExportFormToQuestionnaire?formId=${formId}`,
        success: function (data, status, xhr) {
            convertToBlobAndDownload(data, false, '', '', xhr.getResponseHeader('Original-File-Name'));
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}