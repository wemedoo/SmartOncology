
$(document).ready(function () {
    initSelect2Elements();
});


function initSelect2Elements() {

    let selectsAndActions = [
        { class: ".personnel-select2", action: `/User/GetAutocompleteData?organizationId=${$("#activeOrganizationId").val()}` },
        { class: ".organization-select2", action: `/Organization/GetAutocompleteData` },
        { class: ".personnelteam-select2", action: `/PersonnelTeam/GetNameAutocompleteData?organizationId=${0}` } // setting organizationId = 0 we'll get PersonnelTeams form every Org
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


// ----- Selecting / Unselecting Users -----

function checkAllPersonnels(event) {
    event.stopPropagation();
    event.preventDefault();
    var allChecked = true;

    $('#AddTrialPersonnelTable').find(':checkbox').each(function () {
        if (!$(this).prop('checked')) {
            allChecked = false;
            return false; 
        }
    });

    if (allChecked) {
        $('#AddTrialPersonnelTable').find(':checkbox').prop('checked', false);
    } else {
        $('#AddTrialPersonnelTable').find(':checkbox').prop('checked', true);
    }
};

function removePersonnelsFromTrial(event) {
    event.stopPropagation();
    event.preventDefault();

    let projectId = $('#projectId').val();
    let personnelId = $('#buttonSubmitDelete').attr('data-id');

    let requestObject = {};
    requestObject['personnelId'] = personnelId;
    requestObject['projectId'] = projectId;

    callServer({
        type: "DELETE",
        url: `/ProjectManagement/RemovePersonnelFromTrial`,
        data: requestObject,
        success: function (data) {
            $(`#row-${personnelId}`).remove();
            toastr.success('Removed');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function addPersonnelsToTrial(event) {
    event.stopPropagation();
    event.preventDefault();

    let requestObject = {};
    requestObject['personnelProjects'] = getPersonnelCheckboxValues($('#projectId').val());

    if (requestObject['personnelProjects'] && requestObject['personnelProjects'].length > 0) {
        callServer({
            type: 'POST',
            url: '/ProjectManagement/AddPersonnels',
            data: requestObject,
            success: function (data) {
                reloadAddTrialPersonnelTable();
                toastr.success('Personnel Added');
                reloadTrialPersonnelTable();
            },
            error: function (xhr, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function getPersonnelCheckboxValues(projectId) {
    let personnelProjects = [];

    $('#AddTrialPersonnelTable').find(':checkbox').each(function () {
        if (this.checked) {
            let personnelId = parseInt($(this).val());
            if (!isNaN(personnelId)) {
                personnelProjects.push({
                    projectId, personnelId
                });
            }
        }
    });
    return personnelProjects;
}

// ---

function reloadTrialPersonnelTable() {

    let requestObject = getFilterParametersObject("#TrialPersonnelTable");
    setFilterTagsFromObj(requestObject, "TrialPersonnelTable");
    hideTrialIdFilterTag();

    setTableProperties(requestObject);

    addPropertyToObject(requestObject, 'tableContainer', "TrialPersonnelTable");
    addPropertyToObject(requestObject, 'ShowAddedPersonnels', true);
    addPropertyToObject(requestObject, 'ActiveOrganizationId', $("#activeOrganizationId").val());
    addPropertyToObject(requestObject, 'IsReadOnly', $("#isReadOnly").val());

    callServer({
        type: 'GET',
        url: '/ProjectManagement/ReloadPersonnelTable',
        data: removeModalNamesFromRequest(requestObject),
        success: function (data) {
            setTableContent(data, "#TrialPersonnelTable");
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function reloadAddTrialPersonnelTable() {

    let requestObject = getFilterParametersObject("#AddTrialPersonnelTable");
    setFilterTagsFromObj(requestObject, "AddTrialPersonnelTable");
    hideTrialIdFilterTag();

    setTableProperties(requestObject);

    addPropertyToObject(requestObject, 'tableContainer', "AddTrialPersonnelTable");
    addPropertyToObject(requestObject, 'ShowAddedPersonnels', false);
    addPropertyToObject(requestObject, 'ActiveOrganizationId', $("#activeOrganizationId").val());
    addPropertyToObject(requestObject, 'IsReadOnly', $("#isReadOnly").val());

    callServer({
        type: 'GET',
        url: '/ProjectManagement/ReloadPersonnelTable',
        data: removeModalNamesFromRequest(requestObject),
        success: function (data) {
            setTableContent(data, "#AddTrialPersonnelTable");
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function removeModalNamesFromRequest(requestObject) {
    if (trialEnv['destinationTableSelector'] === "#AddTrialPersonnelTable") {
        requestObject["PersonnelId"] = requestObject["PersonnelIdModal"];
        requestObject["OrganizationId"] = requestObject["OrganizationIdModal"];
        requestObject["OccupationCD"] = requestObject["OccupationCDModal"];
        requestObject["PersonnelTeamId"] = requestObject["PersonnelTeamIdModal"];
    }
    return requestObject;
}

function showAddTrialPersonnelModal() {
    $('#AddTrialPersonnelModal').modal('show');
    tableContainer = "AddTrialPersonnelTable";
    currentPage = 1;
    trialEnv['destinationTableSelector'] = "#AddTrialPersonnelTable";
    reloadAddTrialPersonnelTable();
}

$(document).on('hidden.bs.modal', '#AddTrialPersonnelModal', function (event) {
    trialEnv['destinationTableSelector'] = "#TrialPersonnelTable";

    $('#personnelIdModal').val(null).trigger('change');
    $('#organizationIdModal').val(null).trigger('change');
    $('#occupationCDModal').val(null).trigger('change');
    $('#personnelTeamIdModal').val(null).trigger('change');
})
