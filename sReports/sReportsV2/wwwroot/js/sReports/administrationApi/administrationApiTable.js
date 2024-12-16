function reloadTable() {
    hideAdvancedFilterModal();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setFilterTagsFromObj(requestObject);
    setAdvancedFilterBtnStyle(requestObject, ['ScreeningNumber', 'HttpStatusCode', 'RequestContains', 'ShowOnlyUnsuccessful', 'page', 'pageSize']);
    checkUrlPageParams();
    setTableProperties(requestObject);

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    callServer({
        type: 'GET',
        url: '/AdministrationApi/ReloadTable',
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
    let requestObject = {};

    if (defaultFilter) {
        requestObject = defaultFilter;
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'ApiRequestDirection', $('#apiRequestDirection').val());
        addPropertyToObject(requestObject, 'ApiName', $('#apiName').val());
        addPropertyToObject(requestObject, 'ScreeningNumber', $('#screeningNumber').val());
        addPropertyToObject(requestObject, 'RequestContains', $('#requestContains').val());
        addPropertyToObject(requestObject, 'HttpStatusCode', $('#httpStatusCode').val());
        addPropertyToObject(requestObject, 'RequestTimestampFrom', toLocaleDateStringIfValue($('#requestTimestampFrom').val()));
        addPropertyToObject(requestObject, 'RequestTimestampTo', toLocaleDateStringIfValue($('#requestTimestampTo').val()));
        addPropertyToObject(requestObject, 'ShowOnlyUnsuccessful', $('#showOnlyUnsuccessful').is(':checked'));
    }
    if (requestObject['RequestTimestampFrom']) {
        addPropertyToObject(requestObject, 'RequestTimestampFrom', toValidTimezoneFormat(requestObject['RequestTimestampFrom']));
    }
    if (requestObject['RequestTimestampTo']) {
        addPropertyToObject(requestObject, 'RequestTimestampTo', toValidTimezoneFormat(requestObject['RequestTimestampTo']));
    }

    return requestObject;
}

function editEntity(event, id) {
    viewEntity(event, id);
}


function viewEntity(event, id) {
    event.preventDefault();
    window.open(`/AdministrationApi/ViewLog?apiRequestLogId=${id}`);
}


function advanceFilter() {
    $('#screeningNumberTemp').val($('#screeningNumber').val());
    $('#requestContainsTemp').val($('#requestContains').val());
    $('#httpStatusCodeTemp').val($('#httpStatusCode').val());

    filterData();
}
function mainFilter() {
    $('#screeningNumber').val($('#screeningNumberTemp').val());
    $('#requestContains').val($('#requestContainsTemp').val());
    $('#httpStatusCode').val($('#httpStatusCodeTemp').val());

    filterData();
}

function getFilterParametersObjectForDisplay(filterObject) {
    getFilterParameterObjectForDisplay(filterObject, 'ApiRequestDirection');
    getFilterParameterObjectForDisplay(filterObject, 'HttpStatusCode');
    if (filterObject.ShowOnlyUnsuccessful) {
        addPropertyToObject(filterObject, 'ShowOnlyUnsuccessful', 'Show Only Unsuccessful');
    } else {
        delete filterObject.ShowOnlyUnsuccessful;
    }

    return filterObject;
}

function loadApiRequestAndResponse() {
    $('.json-editor').each(function () {
        $(this).html('');

        let json;
        try {
            json = JSON.parse($(this).attr('data-content'));
        } catch (e) {
            console.log('error while parsing api object');
            console.log('error: ' + e);
        }
        if (json) {
            let previewApiContentContainer = new JSONEditor(this, {
                mode: 'view'
            });
            previewApiContentContainer.set(json);
            previewApiContentContainer.expandAll();
        } else {
            $(this).html("No content");
        }
    });
}