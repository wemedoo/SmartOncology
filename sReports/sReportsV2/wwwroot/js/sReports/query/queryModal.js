function showQueryModal(event, readOnly, fieldId, title, raiseNewQuery) {
    event.preventDefault();

    callServer({
        type: 'GET',
        url: `/QueryManagement/ShowQueryModal?readOnly=${readOnly}`,
        success: function (data) {
            $('#newQueryModal').remove();
            $('body').append('<div id="newQueryModal"></div>');
            $('#newQueryModal').html(data);
            $('#fieldId').val(fieldId); 
            $('#fieldTitle').text(title); 
            $('#raiseNewQuery').val(raiseNewQuery);
            $('#formInstanceId').val($('[name="formInstanceId"]').val()); 
            if (raiseNewQuery) {
                $('#queryHistoryModal').css({
                    'z-index': '1040'
                });
            }
            $('#queryModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showQueryHistoryModal(event, fieldId, readOnly) {
    event.preventDefault();

    callServer({
        type: 'GET',
        url: `/QueryManagement/ShowQueryHistoryModal?fieldId=${fieldId}&readOnly=${readOnly}`,
        success: function (data) {
            $('#queryHistory').html(data);
            $('#queryFieldId').val(fieldId);
            $('#queryReadOnly').val(readOnly);
            loadQueryHistoryTable(fieldId, 0, true, readOnly);
            $('#queryHistoryModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function createQuery(event) {
    event.preventDefault();

    $('#queryForm').validate();
    if ($('#queryForm').valid()) {
        var request = {};

        request['StatusCD'] = $("#status").val();
        request['ReasonCD'] = $("#reason").val();
        request['Title'] = "Manual Query Raised";
        request['Description'] = $("#description").val();
        request['FieldId'] = $("#fieldId").val();
        request['FormInstanceId'] = $("#formInstanceId").val();

        var raiseNewQuery = $("#raiseNewQuery").val() === "true";

        callServer({
            type: "POST",
            url: `/QueryManagement/Create`,
            data: request,
            success: function (data) {
                if (!raiseNewQuery) {
                    toastr.options = {
                        timeOut: 100
                    }
                    $('#queryModal').modal('hide');
                    toastr.success("Success");
                    location.reload(true);
                }
                else {
                    loadQueryHistoryTable($("#fieldId").val(), 0, true, false);
                    $('#queryModal').modal('hide');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function showQueriesContent(taskColumnName, taskIsAscending) {
    var fieldId = $("#queryFieldId").val();
    var request = $.param({
        'dataIn.FieldId': fieldId,
        'dataIn.ColumnName': taskColumnName,
        'dataIn.IsAscending': taskIsAscending,
        isFormInstanceMode: true
    });

    callServer({
        type: 'GET',
        url: `/QueryManagement/LoadQueryHistoryTable?${request}`,
        success: function (data) {
            $('#queryHistoryTableContainer').html(data);
            addQuerySortArrows();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function addQuerySortArrows() {
    var element = document.getElementById(queryColumnName);
    if (element != null) {
        element.classList.remove("sort-arrow");
        if (queryIsAscending) {
            element.classList.remove("sort-arrow-desc");
            element.classList.add("sort-arrow-asc");
        }
        else {
            element.classList.remove("sort-arrow-asc");
            element.classList.add("sort-arrow-desc");
        }
    }
}