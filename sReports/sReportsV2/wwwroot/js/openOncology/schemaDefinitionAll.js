function newEntity() {
    window.location.href = `/SmartOncology/CreateNewSchema`;
}

function editEntity(event, id) {
    event.preventDefault();
    window.location.href = `/SmartOncology/EditSchema/${id}`;
}

function viewEntity(event, id) {
    event.preventDefault();
    window.location.href = `/SmartOncology/PreviewSchema/${id}`;
}

function deleteEntity(event) {
    event.preventDefault();
    event.stopPropagation();

    var id = document.getElementById("buttonSubmitDelete").getAttribute('data-id')

    callServer({
        type: "DELETE",
        url: `/SmartOncology/DeleteSchema/${id}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
            $('#deleteModal').modal('hide');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function mainFilter() {
    $('#indication').val($('#indicationTemp').val());
    $('#name').val($('#nameTemp').val());
    filterData();
}

function advanceFilter() {
    $('#indicationTemp').val($('#indication').val());
    $('#nameTemp').val($('#name').val());
    filterData();
}

function reloadTable() {
    let requestObject = applyActionsBeforeServerReload(['Indication', 'Name', 'page', 'pageSize'], false);

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    callServer({
        type: 'GET',
        url: '/SmartOncology/ReloadSchemas',
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
    var indication = $("#indication").val();
    var name = $("#name").val();

    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'Indication', indication);
        addPropertyToObject(result, 'Name', name);
    }

    return result;
}