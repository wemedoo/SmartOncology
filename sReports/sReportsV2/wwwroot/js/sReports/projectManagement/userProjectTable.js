
// ----- Table Actions -----

function viewProjectForms(event, id) {
    window.location.href = `/ProjectManagement/ProjectForms?projectId=${id}`;
    event.preventDefault();
}

// ----- Reload and Filtering -----

function reloadTable() {
    let requestObject = applyActionsBeforeServerReloadSimple();

    requestObject.ProjectType = $('#projectType').find(':selected').attr('id');

    callServer({
        type: 'GET',
        url: '/ProjectManagement/ReloadUserProjectTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data, "#trialManagementTableContainer");
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
        addPropertyToObject(result, 'ProjectName', $('#projectName').val());
        addPropertyToObject(result, 'ProjectType', $('#projectType').val());
    }

    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    return filterObject;
}

function advanceFilter() {
    filterData();
}

function mainFilter() {
    advanceFilter();
}