
function reloadTable() {
    $('#thesaurusFilterModal').modal('hide');

    let requestObject = applyActionsBeforeServerReloadSimple(true, false, { page: getPageNum(), doOrdering: true });

    callServer({
        type: 'GET',
        url: '/CodeSet/ReloadTable',
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
        if (Object.keys(result).length == 0) {
            $("#showActive").prop("checked", true);
            addPropertyToObject(result, 'ShowActive', true);
        }
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'codeSetId', $('#codeSetId').val());
        addPropertyToObject(result, 'codeSetDisplay', $('#codeSetDisplay').val());
        addPropertyToObject(result, 'ShowActive', $("#showActive").is(":checked"));
        addPropertyToObject(result, 'ShowInactive', $("#showInactive").is(":checked"));
    }

    return result;
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
    return filterObject;
}

function advanceFilter() {
    filterData();
}

function removeCodeSet(event, id) {
    event.preventDefault();
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/CodeSet/Delete?Id=${id}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}