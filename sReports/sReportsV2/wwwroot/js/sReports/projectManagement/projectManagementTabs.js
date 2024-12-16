$(document).ready(function () {
    preventPushStateWhenReload = true;  // Because of > 1 Tables
    if ($("#projectType").val() == $('#clinicalTrialTypeId').val())
        showTrialData();

    $('#projectType').on('change', function () {
        var selectedValue = $(this).val();

        if (selectedValue === $('#clinicalTrialTypeId').val())
            showTrialData();
        else
            hideTrialData();
    });
});

function showTrialData() {
    var trialDataDiv = document.getElementById("trialDataDiv");
    trialDataDiv.removeAttribute("hidden");
}

function hideTrialData() {
    var trialDataDiv = document.getElementById("trialDataDiv");
    trialDataDiv.setAttribute("hidden", true);
}

var trialEnv = {
    destinationTableSelector: null, 
    personnelHelperObj: { personnelIds: [], trialId: null }
}

$(document).on('click', '.so-tab', function (e) {
    var readOnly = ($("#readOnly").val() === "true");

    if (leavingMainTab() && !readOnly) {
        trySubmitClinicalTrial(e, $('#trialId').val(), this);
    }
    else {
        switchTab(this);
    }
});

function leavingMainTab() {
    return $('[data-id="TrialData"]').hasClass('active');
}

function switchTab(tabElement) {
    $('.so-tab').removeClass('active');
    $(tabElement).addClass('active');

    $('.so-tab-container').hide();

    let activeContainerId = $(tabElement).attr("data-id");

    trialEnv['destinationTableSelector'] = `#${activeContainerId}Table`;
    resetTableCommonVariables();

    $(`#${activeContainerId}`).show();
}

function reloadTable() {

    if (trialEnv['destinationTableSelector']) {
        if (trialEnv['destinationTableSelector'] === "#TrialPersonnelTable") {
            reloadTrialPersonnelTable();
        }
        else if (trialEnv['destinationTableSelector'] === "#AddTrialPersonnelTable") {
            reloadAddTrialPersonnelTable();
        }
        else if (trialEnv['destinationTableSelector'] === "#TrialDocumentsTable") {
            reloadTrialDocumentsTable();
        }
    }
    else {
        // Initialization
        reloadTrialPersonnelTable();
        reloadTrialDocumentsTable();
    }
}

// Common Filtering Functions

function getFilterParametersObject(requestedTable) {
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {

        addPropertyToObject(result, 'ProjectId', $('#projectId').val());
        addPropertyToObject(result, 'ClinicalTrialId', $('#trialId').val());

        if (requestedTable === "#TrialPersonnelTable") {
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
        }
        else if (requestedTable === "#AddTrialPersonnelTable") {
            if ($('#personnelIdModal').val()) {
                addPropertyToObject(result, 'PersonnelIdModal', $('#personnelIdModal').val());
            }
            if ($('#organizationIdModal').val()) {
                addPropertyToObject(result, 'OrganizationIdModal', $('#organizationIdModal').val());
            }
            if ($('#occupationCDModal').val()) {
                addPropertyToObject(result, 'OccupationCDModal', $('#occupationCDModal').val());
            }
            if ($('#personnelTeamIdModal').val()) {
                addPropertyToObject(result, 'PersonnelTeamIdModal', $('#personnelTeamIdModal').val());
            }
        }
        else if (requestedTable === "#TrialDocumentsTable") {
            if ($('#title').val()) {
                addPropertyToObject(result, 'Title', $('#title').val());
            }
            if ($('#classes').val()) {
                addPropertyToObject(result, 'Classes', $('#classes').val());
            }
            if ($('#explicitPurpose').val()) {
                addPropertyToObject(result, 'ExplicitPurpose', $('#explicitPurpose').val());
            }
            if ($('#clinicalContext').val()) {
                addPropertyToObject(result, 'ClinicalContext', $('#clinicalContext').val());
            }
        }
    }

    return result;
}


function getFilterParametersObjectForDisplay(filterObject) {

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

    // ---

    getFilterParameterObjectForDisplay(filterObject, 'OccupationCDModal');

    if (filterObject.hasOwnProperty('PersonnelIdModal')) {
        let parentDisplay = getSelectedSelect2Label("personnelIdModal");
        if (parentDisplay) {
            addPropertyToObject(filterObject, 'PersonnelIdModal', parentDisplay);
        }
    }

    if (filterObject.hasOwnProperty('OrganizationIdModal')) {
        let parentDisplay = getSelectedSelect2Label("organizationIdModal");
        if (parentDisplay) {
            addPropertyToObject(filterObject, 'OrganizationIdModal', parentDisplay);
        }
    }

    if (filterObject.hasOwnProperty('PersonnelTeamIdModal')) {
        let parentDisplay = getSelectedSelect2Label("personnelTeamIdModal");
        if (parentDisplay) {
            addPropertyToObject(filterObject, 'PersonnelTeamIdModal', parentDisplay);
        }
    }

    // ---

    getFilterParameterObjectForDisplay(filterObject, 'DocumentTitleFilter');
    getFilterParameterObjectForDisplay(filterObject, 'DocumentClassFilter'); 
    getFilterParameterObjectForDisplay(filterObject, 'DocumentExplicitPurposeFilter');
    getFilterParameterObjectForDisplay(filterObject, 'DocumentClinicalContextFilter');

    return filterObject;
}

function advanceFilter() {
    filterData();
}

function mainFilter() {
    advanceFilter();
}

// HELPERS

function hideTrialIdFilterTag() {
    $('.remove-multitable-filter[name=ClinicalTrialId]').closest('.filter-element').hide();
    $('.remove-multitable-filter[name=ProjectId]').closest('.filter-element').hide();
    $('.remove-multitable-filter[name=ShowUserProjects]').closest('.filter-element').hide();
}