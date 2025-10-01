function reloadTable() {
    let requestObject = applyActionsBeforeServerReload(['Title', 'page', 'pageSize'], false);

    callServer({
        type: 'GET',
        url: '/DigitalGuideline/ReloadTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}



function redirectToCreate() {
    window.location.href = `/DigitalGuideline/Create`;
}


function editEntity(event, id) {
    window.location.href = `/DigitalGuideline/Edit?id=${id}`;
    event.preventDefault();
}

function removeEntry(event, id, lastUpdate) {
    event.stopPropagation();
    event.preventDefault();
    callServer({
        type: "DELETE",
        url: `/DigitalGuideline/Delete?id=${id}&&LastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function advanceFilter() {
    $('#TitleTemp').val($('#title').val());

    filterData();
}

function mainFilter() {
    $('#title').val($('#TitleTemp').val());

    filterData();
}

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Title', $('#title').val());
        addPropertyToObject(requestObject, 'Major', $('#major').val());
        addPropertyToObject(requestObject, 'Minor', $('#minor').val());
        addPropertyToObject(requestObject, 'DateTimeTo', toLocaleDateStringIfValue($('#dateTimeTo').val()));
        addPropertyToObject(requestObject, 'DateTimeFrom', toLocaleDateStringIfValue($('#dateTimeFrom').val()));
    }

    return requestObject;
}