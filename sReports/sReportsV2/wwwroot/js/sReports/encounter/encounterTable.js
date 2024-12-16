function reloadTable() {
    hideAdvancedFilterModal();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setFilterTagsFromObj(requestObject);
    setAdvancedFilterBtnStyle(requestObject, ['Given', 'Family', 'BirthDate', 'page', 'pageSize']);
    setCodeValues(requestObject);
    checkUrlPageParams();
    setTableProperties(requestObject);

    callServer({
        type: 'GET',
        url: '/Encounter/ReloadTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data, "#encounterTableContainer");
        },
        error: function (xhr, thrownError) {
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
        if ($('#FamilyTemp').val()) {
            addPropertyToObject(result, 'Family', $('#FamilyTemp').val());
        }
        if ($('#GivenTemp').val()) {
            addPropertyToObject(result, 'Given', $('#GivenTemp').val());
        }
        if ($('#BirthDateTemp').val()) {
            addPropertyToObject(result, 'BirthDate', $('#birthDateDefault').val());
        }
        if ($('#gender').val()) {
            addPropertyToObject(result, 'Gender', $("#gender").val());
        }
        if ($('#typeCD').val()) {
            addPropertyToObject(result, 'TypeCD', $("#typeCD").val());
        }
        if ($('#statusCD').val()) {
            addPropertyToObject(result, 'StatusCD', $("#statusCD").val());
        }
        if ($('#episodeOfCareTypeCD').val()) {
            addPropertyToObject(result, 'EpisodeOfCareTypeCD', $("#episodeOfCareTypeCD").val());
        }
        if ($('#admissionDate').val()) {
            addPropertyToObject(result, 'AdmissionDate', $('#admissionDateDefault').val());
        }
        if ($('#dischargeDate').val()) {
            addPropertyToObject(result, 'DischargeDate', $('#dischargeDateDefault').val());
        }
    }

    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    return filterObject;
}

function advanceFilter() {
    encounterTableFilter();
}

function encounterTableFilter() {
    $('#FamilyTemp').val($('#family').val());
    $('#GivenTemp').val($('#given').val());
    $('#BirthDateTemp').val($('#birthDate').val());
    copyDateToHiddenField($("#BirthDateTemp").val(), "birthDateDefault");
    copyDateToHiddenField($("#admissionDate").val(), "admissionDateDefault");
    copyDateToHiddenField($("#dischargeDate").val(), "dischargeDateDefault");

    $('#advancedId').children('div:first').addClass('btn-advanced');
    $('#advancedId').find('button:first').removeClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'inline-block');

    filterData();
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
}

function setCodeValues(requestObject) {
    requestObject.Gender = $('#gender').find(':selected').attr('id');
    requestObject.TypeCD = $('#typeCD').find(':selected').attr('id');
    requestObject.StatusCD = $('#statusCD').find(':selected').attr('id');
    requestObject.EpisodeOfCareTypeCD = $('#episodeOfCareTypeCD').find(':selected').attr('id');
}