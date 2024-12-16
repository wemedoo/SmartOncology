function reloadTable() {
    hideAdvancedFilterModal();
    setFilterTagsFromUrl();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setAdvancedFilterBtnStyle(requestObject, ['Title', 'page', 'pageSize']);
    checkUrlPageParams();
    setTableProperties(requestObject);

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
    //clearFilters();
}

function mainFilter() {
    $('#title').val($('#TitleTemp').val());

    filterData();
    //clearFilters();
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
    if (requestObject['DateTimeFrom']) {
        addPropertyToObject(requestObject, 'DateTimeFrom', toValidTimezoneFormat(requestObject['DateTimeFrom']));
    }
    if (requestObject['DateTimeTo']) {
        addPropertyToObject(requestObject, 'DateTimeTo', toValidTimezoneFormat(requestObject['DateTimeTo']));
    }

    return requestObject;
}