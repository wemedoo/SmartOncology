function reloadTable() {
    $('#thesaurusFilterModal').modal('hide');
    let requestObject = applyActionsBeforeServerReloadSimple(true, false, { page: getPageNum(), doOrdering: true });
    isCustomState = true;

    callServer({
        type: 'GET',
        url: '/Code/ReloadTable',
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
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        if (isDefaultCodeFilter(result) && $("#showActive").length > 0) {
            $("#showActive").prop("checked", true);
            addPropertyToObject(result, 'ShowActive', true);
        }
        defaultFilter = null;
    }
    else {
        if ($('#id').val()) {
            addPropertyToObject(result, 'id', $('#id').val());
        }
        if ($('#codeDisplay').val()) {
            addPropertyToObject(result, 'codeDisplay', $('#codeDisplay').val());
        }
        addPropertyToObject(result, 'ShowActive', $("#showActive").is(":checked"));
        addPropertyToObject(result, 'ShowInactive', $("#showInactive").is(":checked"));

        addPropertyToObject(result, 'CodeSetId', $('#codeSetId').val());
        addPropertyToObject(result, 'CodeSetDisplay', encodeURIComponent($('#codeSetDisplay').val()));
    }

    return result;
}

function isDefaultCodeFilter(result) {
    let numOfProperties = Object.keys(result).length;
    return numOfProperties == 0 || ('CodeSetId' in result && 'CodeSetDisplay' in result && numOfProperties == 2);
}

function getFilterParametersObjectForDisplay(filterObject) {
    if (filterObject.ShowActive) {
        addPropertyToObject(filterObject, 'ShowActive', 'Active');
    } else {
        delete filterObject.ShowActive;
    }
    if (filterObject.ShowInactive) {
        addPropertyToObject(filterObject, 'ShowInactive', 'Inactive');
    } else {
        delete filterObject.ShowInactive;
    }

    delete filterObject.CodeSetId;
    delete filterObject.CodeSetDisplay;
    return filterObject;
}

function advanceFilter() {
    filterData();
}

function mainFilter() {
    reloadNomineeTable();
}


function removeCode(event, id) {
    event.preventDefault();
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/Code/Delete?Id=${id}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
            reloadNominatorTable();
            reloadNomineeTable();
            $('.code-action-btn').attr('disabled', true);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function pushCustomState() {
    var codeSetDisplay = encodeURIComponent($('#thesaurusSearchInputCode').val());
    return `?page=1&pageSize=${getPageSize()}&CodeSetId=${$('#newCodeSetNumberForCode').val()}&CodeSetDisplay=${codeSetDisplay}`;
}

function getCustomPageParams(number, pageSize) {
    var codeSetDisplay = encodeURIComponent($('#thesaurusSearchInputCode').val());
    return `?page=${number}&pageSize=${pageSize}&CodeSetId=${$('#codeSetNumberForCode').val()}&CodeSetDisplay=${codeSetDisplay}`;
}