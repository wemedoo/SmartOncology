
function reloadTable() {
    reloadTrialDocumentsTable();
}

// Common Filtering Functions

function getFilterParametersObject() {
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'ProjectId', $('#projectId').val());
        addPropertyToObject(result, 'ShowUserProjects', true);
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

    return result;
}


function getFilterParametersObjectForDisplay(filterObject) {
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