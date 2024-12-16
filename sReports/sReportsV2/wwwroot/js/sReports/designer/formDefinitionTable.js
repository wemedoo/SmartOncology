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

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        requestObject = defaultFilter;
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
        addPropertyToObject(requestObject, 'FollowUp', $('#documentFollowUpSelect').val());
        addPropertyToObject(requestObject, 'AdministrativeContext', $('#administrativeContext').val());
        addPropertyToObject(requestObject, 'DateTimeTo', toLocaleDateStringIfValue($('#dateTimeTo').val()));
        addPropertyToObject(requestObject, 'DateTimeFrom', toLocaleDateStringIfValue($('#dateTimeFrom').val()));
    }
    if (requestObject['DateTimeFrom']) {
        addPropertyToObject(requestObject, 'DateTimeFrom', toValidTimezoneFormat(requestObject['DateTimeFrom']));
    }
    if (requestObject['DateTimeTo']) {
        addPropertyToObject(requestObject, 'DateTimeTo', toValidTimezoneFormat(requestObject['DateTimeTo']));
    }

    return requestObject;
}

function reloadTable(initLoad) {
    hideAdvancedFilterModal();
    setFilterTagsFromUrl();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    checkUrlPageParams();
    setAdvancedFilterBtnStyle(requestObject, ['ThesaurusId', 'State', 'Title', 'page', 'pageSize']);
    setTableProperties(requestObject);
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
    //clearFilters();
}

function mainFilter() {
    $('#title').val($('#TitleTemp').val());
    $('#thesaurusId').val($('#ThesaurusIdTemp').val());
    $('#state').val($('#StateTemp').val()).change();

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#title').val('');
    $('#thesaurusId').val('');
    $('#state').val('');
    $('#TitleTemp').val('');
    $('#ThesaurusIdTemp').val('');
    $('#StateTemp').val('');
    $('#classes').val('');
    $('#generalPurpose').val('');
    $('#documentClassOtherInput').val('');
    $('#contextDependent').val('');
    $('#explicitPurpose').val('');
    $('#scopeOfValidity').val('');
    $('#clinicalDomain').val('');
    $('#clinicalContext').val('');
    $('#administrativeContext').val('');
    $('#documentFollowUpSelect').val('');
    $('#dateTimeTo').val('');

}

