function editEntity(event, id) {
    window.location.href = `/Patient/Edit?patientId=${id}`;
    event.preventDefault();
}

function viewEntity(event, id) {
    window.location.href = `/Patient/View?patientId=${id}`;
    event.preventDefault();
}

function createPatientEntry() {
    window.location.href = "/Patient/Create";
}

function removePatientEntry(event, id, rowVersion) {
    event.preventDefault();
    event.stopPropagation();
    let data = {
        id: id,
        rowVersion: rowVersion
    };
    callServer({
        type: "DELETE",
        url: `/Patient/Delete`,
        data: data,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function patientListsDisabled() {
    $('#scrollList').addClass('unclickable-div');
}

function patientListsEnabled() {
    $('#scrollList').removeClass('unclickable-div');
}

function reloadTable() {
    hideAdvancedFilterModal();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setFilterTagsFromObj(requestObject);
    setAdvancedFilterBtnStyle(requestObject, ['Given', 'Family', 'BirthDate', 'page', 'pageSize', 'PatientListId', 'PatientListName', 'ListWithSelectedPatients']);
    checkUrlPageParams();
    setTableProperties(requestObject);

    patientListsDisabled();
    callServer({
        type: 'GET',
        url: '/Patient/ReloadTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data);
            patientListsEnabled();
            addAdditionalPatientListFilterTags(requestObject);
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
        if ($('#identifierType').val()) {
            addPropertyToObject(result, 'IdentifierType', $('#identifierType').val());
        }
        if ($('#identifierValue').val()) {
            addPropertyToObject(result, 'IdentifierValue', $('#identifierValue').val());
        }
        if ($('#countryCD').val()) {
            addPropertyToObject(result, 'CountryCD', $('#countryCD').val());
        }
        if ($('#city').val()) {
            addPropertyToObject(result, 'City', $('#city').val());
        }
        if ($('#BirthDateTemp').val()) {
            addPropertyToObject(result, 'BirthDate', $('#birthDateDefault').val());
        }
        if ($('#GivenTemp').val()) {
            addPropertyToObject(result, 'Given', $('#GivenTemp').val());
        }
        if ($('#FamilyTemp').val()) {
            addPropertyToObject(result, 'Family', $('#FamilyTemp').val());
        }
        if ($('#postalCode').val()) {
            addPropertyToObject(result, 'PostalCode', $('#postalCode').val());
        }
        if ($('#entryDatetime').val()) {
            addPropertyToObject(result, 'EntryDatetime', $('#entryDatetimeDefault').val());
        }
        setPatientListFilter(result);
    }
    
    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    getFilterParameterObjectForDisplay(filterObject, 'IdentifierType');

    if (filterObject.hasOwnProperty('CountryCD')) {
        let countryNameByHidden = $('#countryName').val();
        if (countryNameByHidden) {
            addPropertyToObject(filterObject, 'CountryCD', countryNameByHidden);
        }
        let countryNameBySelect2 = getSelectedSelect2Label("countryCD");
        if (countryNameBySelect2) {
            addPropertyToObject(filterObject, 'CountryCD', countryNameBySelect2);
        }
    }

    if (filterObject.hasOwnProperty('PatientListId')) {
        if (filterObject.PatientListId == 0) {
            delete filterObject.PatientListName;
        }
        delete filterObject.PatientListId;
        delete filterObject.ListWithSelectedPatients;
    }

    return filterObject;
}

function advanceFilter() {
    patientTableFilter();
}

function patientTableFilter() {
    $('#FamilyTemp').val($('#family').val());
    $('#GivenTemp').val($('#given').val());
    $('#BirthDateTemp').val($('#birthDate').val());
    copyDateToHiddenField($("#BirthDateTemp").val(), "birthDateDefault");
    copyDateToHiddenField($("#entryDatetime").val(), "entryDatetimeDefault");

    $('#advancedId').children('div:first').addClass('btn-advanced');
    $('#advancedId').find('button:first').removeClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'inline-block');

    filterData();
    //clearFilters();
}

function mainFilter() {
    $('#family').val($('#FamilyTemp').val());
    $('#given').val($('#GivenTemp').val());
    $('#birthDate').val($("#BirthDateTemp").val());
    copyDateToHiddenField($("#birthDate").val(), "birthDateDefault");

    $('#advancedId').children('div:first').removeClass('btn-advanced');
    $('#advancedId').find('button:first').addClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'none');

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#family').val('');
    $('#given').val('');
    $('#birthDate').val('');
    $('#identifierType').val('');
    $('#identifierValue').val('');
    $('#city').val('');
    $('#country').val('');
    $('#postalCode').val('');
    $('#FamilyTemp').val('');
    $('#GivenTemp').val('');
    $('#BirthDateTemp').val('');
    $('#birthDateDefault').val('');
}

function setPatientListFilter(requestObject) {
    addPropertyToObject(requestObject, 'PatientListId', $('.scroll-list-item.selected').attr('data-patientlistid'));
    addPropertyToObject(requestObject, 'ListWithSelectedPatients', $('.scroll-list-item.selected').attr('data-patientselection') === 'True');
    addPropertyToObject(requestObject, 'PatientListName', $('.scroll-list-item.selected').find('.scroll-list-item-title').attr('data-title'));
}

function removePatientListFilter() {
    $(".scroll-list-item[data-patientlistid]").removeClass('selected');
    $(`[data-patientlistid="0"]`).addClass('selected');
}