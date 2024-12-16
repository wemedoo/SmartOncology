
// ----- reloadTable() gloabal func handling -----

var patientReloadTable = '';

$(document).ready(function () {
    patientReloadTable = reloadTable;
});

function resetReloadTable() {
    advanceFilter = patientTableFilter;
    reloadTable = patientReloadTable;
}

function overrideReloadTable() {
    advanceFilter = patientListPersonnelFilter;

    reloadTable = function (columnName, isAscending) {
        reloadPersonnelTable(true, 1, columnName, isAscending);
    };
}

$(document).on('hidden.bs.modal', '#patientListModal', function () {
    $('#patientListFormContainer').html('');
    resetReloadTable();
});

$(document).on('show.bs.modal', '#patientListModal', function () {
    overrideReloadTable();
});


// ----- Personnel Table (inside PatientList Modal) -----

function initSelect2PersonnelElements() {

    let selectsAndActions = [
        { class: ".personnel-select2", action: `/User/GetAutocompleteData?organizationId=${0}` },  // setting organizationId = 0 we'll get Personnel from every Org
        { class: ".organization-select2", action: `/Organization/GetAutocompleteData` },
        { class: ".personnelteam-select2", action: `/PersonnelTeam/GetNameAutocompleteData?organizationId=${0}` } // setting organizationId = 0 we'll get PersonnelTeams from every Org
    ];

    var placeholder = "-";

    $(selectsAndActions).each(function () {
        let classSelector = this.class;
        let action = this.action;
        $(classSelector).each(function () {
            $(this).initSelect2(
                getSelect2Object(
                    {
                        placeholder: placeholder,
                        width: '100%',
                        allowClear: true,
                        url: action
                    }
                )
            );
        });

    });
}

function reloadPersonnelTable(loadSelectedPersonnel = true, page = 1, columnName = "", isAscending = false) {

    let requestObject = getPatientListPersonnelFilterObject();
    let requestObjectForDisplay = { ...requestObject };
    let params = getPersonnelFilterParamsObjectForDisplay(requestObjectForDisplay);
    setFilterTagsFromObjWithParams(requestObject, params, "PatientListPersonnelTable");
    hidePatientListIdFilterTag();

    setTableProperties(requestObject);

    requestObject['LoadSelectedPersonnel'] = loadSelectedPersonnel;
    requestObject['PageSize'] = 5;
    requestObject['Page'] = page;
    requestObject['ColumnName'] = columnName;
    requestObject['IsAscending'] = isAscending;

    callServer({
        type: 'GET',
        url: '/PatientList/ReloadPersonnelTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data, "#PatientListPersonnelTable");
            initSelect2PersonnelElements();
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getPatientListPersonnelFilterObject() {
    let result = {};

    addPropertyToObject(result, 'PatientListId', $('#patientListId').val());

    if ($('#personnelId').val()) {
        addPropertyToObject(result, 'PersonnelId', $('#personnelId').val());
    }
    if ($('#organizationId').val()) {
        addPropertyToObject(result, 'OrganizationId', $('#organizationId').val());
    }
    if ($('#occupationCD').val()) {
        addPropertyToObject(result, 'OccupationCD', $('#occupationCD').val());
    }
    if ($('#personnelTeamId').val()) {
        addPropertyToObject(result, 'PersonnelTeamId', $('#personnelTeamId').val());
    }

    return result;
}

function clearPatientListFilters() {
    $('#personnelId').val('');
    $('#organizationId').val('');
    $('#occupationCD').val('');
    $('#personnelTeamId').val('');
}

function changePatientListPersonnelPage(num, e, url, container, pageNumIdentifier, preventPushHistoryState) {
    e.preventDefault();

    let tableToLoad = $('#patientListPersonnelTableId').val() == 'SelectedPersonnelTable'
    reloadPersonnelTable(tableToLoad, num);
}


function patientListPersonnelFilter() {
    let tableToLoad = $('#patientListPersonnelTableId').val() == 'SelectedPersonnelTable'
    reloadPersonnelTable(tableToLoad);
}

function getPersonnelFilterParamsObjectForDisplay(filterObject) {

    getFilterParameterObjectForDisplay(filterObject, 'OccupationCD');

    if (filterObject.hasOwnProperty('PersonnelId')) {
        let parentDisplay = getSelectedSelect2Label("personnelId");
        if (parentDisplay) {
            addPropertyToObject(filterObject, 'PersonnelId', parentDisplay);
        }
    }

    if (filterObject.hasOwnProperty('OrganizationId')) {
        let parentDisplay = getSelectedSelect2Label("organizationId");
        if (parentDisplay) {
            addPropertyToObject(filterObject, 'OrganizationId', parentDisplay);
        }
    }

    if (filterObject.hasOwnProperty('PersonnelTeamId')) {
        let parentDisplay = getSelectedSelect2Label("personnelTeamId");
        if (parentDisplay) {
            addPropertyToObject(filterObject, 'PersonnelTeamId', parentDisplay);
        }
    }

    return filterObject;
}

function hidePatientListIdFilterTag() {
    $('.remove-multitable-filter[name=PatientListId]').closest('.filter-element').hide();
}


// ----- Personnel Actions (Select,Add,Remove)-----

function showPersonnelToSelect() {
    clearPatientListFilters();
    $('.scroll-list-container').removeClass('d-none');
    $('#second-patientlist-modal-tab').trigger('click');
    $('#personnel-add-btn-group').addClass('d-none');
    $('#personnel-save-btn-group').removeClass('d-none');
    reloadPersonnelTable(false);
}

function closePersonnelSelection() {
    clearPatientListFilters();
    reloadPersonnelTable();
    $('#personnel-add-btn-group').removeClass('d-none');
    $('#personnel-save-btn-group').addClass('d-none');
}

$(document).on('change', '#PatientListPersonnelTable .form-checkbox-field', function () {

    let modalTablePageSize = 5;
    let c = countCheckedPersonnel();
    
    if (c == modalTablePageSize) {
        $('#checkAllBtn').addClass('d-none');
        $('#uncheckAllBtn').removeClass('d-none');
    }
    else if (c == 0) {
        $('#uncheckAllBtn').addClass('d-none');
        $('#checkAllBtn').removeClass('d-none');
    }
});

function checkAllPersonnels(event) {
    event.stopPropagation();
    event.preventDefault();
    $('#PatientListPersonnelTable').find(':checkbox').prop('checked', true);

    $('#checkAllBtn').addClass('d-none');
    $('#uncheckAllBtn').removeClass('d-none');
};

function uncheckAllPersonnels(event) {
    event.stopPropagation();
    event.preventDefault();
    $('#PatientListPersonnelTable').find(':checkbox').prop('checked', false);

    $('#uncheckAllBtn').addClass('d-none');
    $('#checkAllBtn').removeClass('d-none');
};


function countCheckedPersonnel() {
    let checkedPersonnelCount = 0;

    $('#PatientListPersonnelTable').find(':checkbox').each(function () {
        if ($(this).prop('checked')) {
            checkedPersonnelCount++;
        }
    });
    return checkedPersonnelCount;
}

function removePersonnelsFromPatientList(event, personnelId = null) {
    event.stopPropagation();
    event.preventDefault();

    let patientListId = $('#patientListId').val();
    let requestObject = {};
    requestObject['personnelId'] = personnelId;
    requestObject['patientListId'] = patientListId;

    callServer({
        type: "DELETE",
        url: `/PatientList/RemovePersonnel`,
        data: requestObject,
        success: function (data) {
            $(`#row-${personnelId}`).remove();
            toastr.success('Removed');
            decreasePersonnelCountHeader();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function decreasePersonnelCountHeader() {
    var currentNumber = parseInt($('#personnelHeaderCounter').text(), 10);
    var newNumber = currentNumber - 1;

    $('#personnelHeaderCounter').text(newNumber);
}

function addPersonnelToPatientList(event) {
    event.stopPropagation();
    event.preventDefault();
    clearPatientListFilters();

    let requestObject = {};
    requestObject['patientListPersonnelRelationDTOs'] = getPersonnelToAdd($('#patientListId').val());

    if (requestObject['patientListPersonnelRelationDTOs'] && requestObject['patientListPersonnelRelationDTOs'].length > 0) {
        callServer({
            type: 'POST',
            url: '/PatientList/AddPersonnels',
            data: requestObject,
            success: function (data) {
                toastr.success('Personnel Added');
                getPatientListModalContent("Edit", $('#patientListId').val(), false, undefined, true);
            },
            error: function (xhr, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function getPersonnelToAdd(patientListId) {
    let personnelToAdd = [];

    $('#PatientListPersonnelTable').find(':checkbox').each(function () {
        if (this.checked) {
            let personnelId = parseInt($(this).val());
            if (!isNaN(personnelId)) {
                personnelToAdd.push({
                    'PatientListId': patientListId,
                    'PersonnelId': personnelId
                });
            }
        }
    });
    return personnelToAdd;
}

