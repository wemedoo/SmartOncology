function reloadTable(initLoad) {
    hideAdvancedFilterModal();
    setFilterTagsFromUrl();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setAdvancedFilterBtnStyle(requestObject, ['Title', 'State', 'ThesaurusId', 'page', 'pageSize']);
    checkUrlPageParams();
    setTableProperties(requestObject);
    requestObject.ClinicalDomain = $('#clinicalDomain').find(':selected').attr('id');

    callServer({
        type: 'GET',
        url: '/Form/ReloadByFormThesaurusTable',
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
    let requestObject = {};

    if (defaultFilter) {
        requestObject = defaultFilter;
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Content', $('#ContentTemp').val());
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

function loadFormInstances(event, formId, thesaurusId, versionId) {
    var url = `/FormInstance/GetAllByFormThesaurus?VersionId=${versionId}&FormId=${formId}&ThesaurusId=${thesaurusId}`;
    window.location.href = url;
}


function downloadPdfs(event) {
    event.preventDefault();
    event.stopPropagation();
    var chkArray = getCheckedRows();

    if (chkArray.length === 0) {
        toastr.warning("Please select at least one document to download.");
        return;
    }

    var numOfSelectedDocuments = chkArray.length;
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');
        getDocument('/Pdf/GetPdfForFormId', formTitle, '.pdf', { formId }, { lastFile: i === numOfSelectedDocuments - 1 });
    }
}

function downloadCSVs(event) {
    event.stopPropagation();
    event.preventDefault();
    let checkedRows = getCheckedRows();
    var formDataArray = [];

    var numOfSelectedDocuments = checkedRows.length;
    if (numOfSelectedDocuments === 0) {
        toastr.warning("Please select at least one document to download.");
        return;
    }
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(checkedRows[i]).val();
        var formTitle = $(checkedRows[i]).data('title');
        var formData = {
            formId: formId,
            formTitle: formTitle
        };

        formDataArray.push(formData);
    } 

    sendFilesToEmail('/FormInstance/ExportToCSV', formDataArray);
}

function downloadXLSXs(event) {
    event.stopPropagation();
    event.preventDefault();
    let checkedRows = getCheckedRows();
    var formDataArray = [];

    var numOfSelectedDocuments = checkedRows.length;
    if (numOfSelectedDocuments === 0) {
        toastr.warning("Please select at least one document to download.");
        return;
    }
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(checkedRows[i]).val();
        var formTitle = $(checkedRows[i]).data('title');
        var formData = {
            formId: formId,
            formTitle: formTitle
        };

        formDataArray.push(formData);
    }
    sendFilesToEmail('/FormInstance/ExportToXLSX', formDataArray);

}

function getCheckedRows() {
    var result = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        result.push(element);
    });
    return result;
}

function sendFilesToEmail(url, formDataArray) {
    var data = {
        formInstancesForDownload: formDataArray,
    };
    callServer({
        type: "POST",
        url: url,
        data: data,
        success: function (response) {
            reloadFormInstances(`The results will be sent to your email address when ready!`);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function reloadFormInstances(successMsg) {
    toastr.options = {
        timeOut: 2000
    }
    toastr.options.onHidden = function () {
        window.location.href = '/FormInstance/GetAllFormDefinitions';
    }
    toastr.success(successMsg);
}

$(document).on('change', '#selectAllCheckboxes', function () {
        var c = this.checked;
        $(':checkbox').prop('checked', c);
});

function advanceFilter() {
    $('#TitleTemp').val($('#title').val());
    $('#ThesaurusIdTemp').val($('#thesaurusId').val());
    $('#StateTemp').val($('#state').val()).change();

    filterData();
    //clearFilters();
}

function mainFilter() {
    $('#content').val($('#ContentTemp').val());
    $('#title').val($('#TitleTemp').val());
    $('#thesaurusId').val($('#ThesaurusIdTemp').val());
    $('#state').val($('#StateTemp').val()).change();

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#content').val('');
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
}
