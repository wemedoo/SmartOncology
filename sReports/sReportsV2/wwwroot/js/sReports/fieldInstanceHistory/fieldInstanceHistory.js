var defaultPageSize = 15;

function showFormInstanceHistory(formInstanceId, fieldInstanceRepetitionId) {
    let data = {
        FormInstanceId: formInstanceId,
        Page: 1,
        PageSize: defaultPageSize,
        ColumnName: "FieldSetLabel",
        IsAscending: true,
        FieldInstanceRepetitionId: fieldInstanceRepetitionId
    };

    reloadTable(data);
}

function getFilter(Page=1) {
    filter = {};
    filter['FormInstanceId'] = $('#filterByFormInstanceId').val();
    filter['UserId'] = $('#FieldInstanceHistoryUserSelect').find(":selected").val();
    filter['FieldLabel'] = $('#FieldInstanceHistorySearchInput').val();
    filter['FieldInstanceRepetitionId'] = $('#filterByFieldInstanceRepetitionId').val();

    filter['Page'] = Page;
    filter['PageSize'] = getPageSize() ?? defaultPageSize;

    let sortBy = $('#FieldInstanceHistorySortSelect').val(); 
    if (sortBy == "ascending") {
        filter['ColumnName'] = "FieldSetLabel";
        filter['IsAscending'] = true;
    }
    else if (sortBy == "descending") {
        filter['ColumnName'] = "FieldSetLabel";
        filter['IsAscending'] = false;
    }
    return filter;
}

function reloadTable(filterData) {
    callServer({
        type: 'GET',
        url: `/FieldInstanceHistory/ReloadData`,
        data: filterData,
        success: function (data) {
            console.log("History retrieved");
            $("#form-instance-history-container").html(data);
            $('#formInstanceHistoryModal').modal('show');
            initializeFieldInstanceHistorySelect2();
            prepareHistoryModal(filterData);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}
    
$(document).on('click', '#FieldInstanceHistoryFilter', function () {
    reloadTable(getFilter());
});

function prepareHistoryModal(filterData) {
    if (filterData.fieldInstanceRepetitionId) {
        $(".global-form-history-filters").addClass("d-none");
        $(".field-history-label-btn").trigger("click");
    } else {
        $(".global-form-history-filters").removeClass("d-none");
    }
}

// ----- Pagination Overloads -----

function changePage(num, e, url, container, pageNumIdentifier, preventPushHistoryState) {
    e.preventDefault();

    reloadTable(getFilter(num));
}

function getPageSize() {
    return $('#pageSizeSelector').val();
}

$(document).on('change', '.pageSizeSelector', function () {

    pageNum = $('.page-item.active').find('.pagination-item-num').text();
    reloadTable(getFilter(pageNum ?? 1));
});


// ----- Select2 Initialization -----

function initializeFieldInstanceHistorySelect2() {

    var placeholder = "Select User";

    $("#FieldInstanceHistoryUserSelect").initSelect2(
        getSelect2Object(
            {
                placeholder: placeholder,
                width: '100%',
                modalId: 'formInstanceHistoryModal',
                url: `/User/GetAutocompleteDataAsync`,
                customAjaxData: function (params) {
                    return {
                        Term: params.term,
                        Page: params.page || 1,
                    };
                }
            }
        )
    );
}
