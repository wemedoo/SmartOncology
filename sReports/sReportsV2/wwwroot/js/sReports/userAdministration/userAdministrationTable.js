function reloadTable() {
    let requestObject = applyActionsBeforeServerReload(['Given', 'Family', 'Username', 'RoleCD', 'OrganizationId', 'ShowUnassignedUsers', 'page', 'pageSize']);
    setCodeValues(requestObject);

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    callServer({
        type: 'GET',
        url: '/UserAdministration/ReloadTable',
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
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        if ($('#birthDate').val()) {
            addPropertyToObject(requestObject, 'BirthDate', $('#birthDateDefault').val());
        }

        setOrganizationId(requestObject);
        addPropertyToObject(requestObject, 'ShowUnassignedUsers', $('#showUnassignedUsers').is(":checked"));
        addPropertyToObject(requestObject, 'RoleCD', $('#roleCD').val());
        addPropertyToObject(requestObject, 'PersonnelTypeCD', $('#personnelTypeCD').val());
        addPropertyToObject(requestObject, 'BusinessEmail', $('#businessEmail').val());
        addPropertyToObject(requestObject, 'Username', $('#username').val());
        addPropertyToObject(requestObject, 'Given', $('#given').val());
        addPropertyToObject(requestObject, 'Family', $('#family').val());
        addPropertyToObject(requestObject, 'IdentifierValue', $('#identifierValue').val());
        addPropertyToObject(requestObject, 'IdentifierType', $('#identifierType').val());
        addPropertyToObject(requestObject, 'CountryCD', $('#countryCD').val());
        addPropertyToObject(requestObject, 'City', $('#city').val());
        addPropertyToObject(requestObject, 'Street', $('#street').val());
        addPropertyToObject(requestObject, 'PostalCode', $('#postalCode').val());
    }
  
    return requestObject;
}

function setOrganizationId(requestObject) {
    addPropertyToObject(requestObject, 'OrganizationId', $('#organizationId').val());
    addPropertyToObject(requestObject, 'OrganizationId', $('#organizationTempId').val());
}

function createUserEntry() {
    window.location.href = "/UserAdministration/Create";
}

function editEntity(event, id) {
    window.location.href = `/UserAdministration/Edit?userId=${id}`;
    event.preventDefault();
}

function viewEntity(event, id) {
    window.location.href = `/UserAdministration/View?userId=${id}`;
    event.preventDefault();
}

function removeUserEntry(event, id) {
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/UserAdministration/Delete?userId=${id}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function setUserState(event, id, organizationId, state) {
    event.stopPropagation();
    event.preventDefault();

    callServer({
        type: "PUT",
        url: `/UserAdministration/SetUserState?userId=${id}&organizationId=${organizationId}&newState=${state}`,
        success: function (data) {
            toastr.success(`Success`);
            reloadTable();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on("change", "#showUnassignedUsers", function () {
    filterData();
});

function advanceFilter() {
    $('#FamilyTemp').val($('#family').val());
    $('#GivenTemp').val($('#given').val());
    $('#UsernameTemp').val($('#username').val());
    $('#RoleTempId').val($('#roleCD').val());
    syncOrganizationValues('organizationId', 'organizationTempId');
    copyDateToHiddenField($("#birthDate").val(), "birthDateDefault");

    filterData();
}

function mainFilter() {
    $('#family').val($('#FamilyTemp').val());
    $('#given').val($('#GivenTemp').val());
    $('#username').val($('#UsernameTemp').val());
    syncOrganizationValues('organizationTempId', 'organizationId');
    $('#roleCD').val($('#RoleTempId').val());
    
    filterData();
}

function getFilterParametersObjectForDisplay(filterObject) {
    getFilterParameterObjectForDisplay(filterObject, 'IdentifierType');
    getFilterParameterObjectForDisplay(filterObject, 'PersonnelTypeCD');

    if (filterObject.hasOwnProperty('ShowUnassignedUsers')) {
        delete filterObject.ShowUnassignedUsers;
    }

    if (filterObject.hasOwnProperty('OrganizationId')) {
        let organizationDisplay = getSelectedSelect2Label("organizationId");
        if (!organizationDisplay) {
            organizationDisplay = getSelectedSelect2Label("organizationTempId");
        }
        if (organizationDisplay) {
            addPropertyToObject(filterObject, 'OrganizationId', organizationDisplay);
        }
    }

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

    return filterObject;
}

function setCodeValues(requestObject) {
    requestObject.RoleCD = $('#roleCD').find(':selected').attr('id');
    requestObject.PersonnelTypeCD = $('#personnelTypeCD').find(':selected').attr('id');
}

function syncOrganizationValues(sourceId, targetId) {
    let selectedValue = $(`#${sourceId}`).val();
    $(`#${targetId}`).empty().append($(`#${sourceId} option`).clone());
    $(`#${targetId}`).val(selectedValue).trigger('change');
}
