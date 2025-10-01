function reloadTable() {
    let requestObject = applyActionsBeforeServerReload(['FieldId', 'FieldLabel', 'ReasonCD', 'StatusCD', 'page', 'pageSize']);
    setCodeValues(requestObject);

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    callServer({
        type: 'GET',
        url: '/QueryManagement/ReloadTable',
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
    var fieldId = $("#fieldId").val();
    var fieldLabel = $("#fieldLabel").val();
    var reason = $("#reasonCD").val();
    var status = $("#statusCD").val();
    var title = $("#title").val();
    var description = $("#description").val();

    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'FieldId', fieldId);
        addPropertyToObject(result, 'FieldLabel', fieldLabel);
        addPropertyToObject(result, 'ReasonCD', reason);
        addPropertyToObject(result, 'StatusCD', status);
        addPropertyToObject(result, 'Title', title);
        addPropertyToObject(result, 'Description', description);
    }

    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {

    return filterObject;
}

function mainFilter() {
    $('#reasonCD').val($('#queryReason').val());
    $('#statusCD').val($('#queryStatus').val());
    $('#fieldLabel').val($('#fieldLabelTemp').val());

    filterData();
}

function advanceFilter() {
    $('#queryReason').val($('#reasonCD').val());
    $('#queryStatus').val($('#statusCD').val());
    $('#fieldLabelTemp').val($('#fieldLabel').val());

    filterData();
}

function setCodeValues(requestObject) {
    requestObject.ReasonCD = $('#reasonCD').find(':selected').attr('id');
    requestObject.StatusCD = $('#statusCD').find(':selected').attr('id');
}

function removeQueryEntry(event, id) {
    event.preventDefault();
    event.stopPropagation();

    callServer({
        type: "DELETE",
        url: `/QueryManagement/Delete?id=${id}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function clickedQueryRow(event, hasUpdatePermission, queryId, fieldId) {
    if (canExecuteClickedRow($(event.target))) {
        if (hasUpdatePermission) {
            editOrViewEntity(event, queryId, fieldId, false);
        }
    }
}

function editOrViewEntity(event, queryId, fieldId, readOnly) {
    event.preventDefault();

    var action = readOnly ? "ViewQuery" : "EditQuery"

    callServer({
        type: 'POST',
        url: `/QueryManagement/${action}?queryId=${queryId}`,
        success: function (data) {
            $('#queryHistory').html(data);
            loadQueryHistoryTable(fieldId, queryId, false, readOnly);
            $('#queryHistoryModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showQueriesContent() {
    addQuerySortArrows();
}

function addQuerySortArrows() {
    var headers = document.querySelectorAll("th.sort-arrow, th.sort-arrow-asc, th.sort-arrow-desc");
    headers.forEach(function (h) {
        h.classList.remove("sort-arrow-asc", "sort-arrow-desc");
        h.classList.add("sort-arrow");
    });

    var element = document.getElementById(queryColumnName);
    if (element) {
        element.classList.remove("sort-arrow");
        if (queryIsAscending) {
            element.classList.add("sort-arrow-asc");
        } else {
            element.classList.add("sort-arrow-desc");
        }
    }
}
