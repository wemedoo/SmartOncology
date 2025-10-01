$(document).ready(function (e) {
    if ($("#search").val()) {
        reloadTable();
    }
});

function reloadTable() {
    hideAdvancedFilterModal();
    let filter = applyActionsBeforeServerReloadSimple(false, true, { pageSize: 20, doOrdering: false });

    callServer({
        type: "get",
        data: filter,
        url: `/ThesaurusGlobal/ReloadThesaurus`,
        success: function (data, textStatus, jqXHR) {
            $("#browseResults").html(data);
            $("#pageSizeSelector").hide();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getFilterParametersObjectForDisplay(filterObject) {
    return filterObject;
}

$('#search').keypress(function (e) {
    if (e.which == enter) {
        $("#searchModal").val($(this).val());
        filterData();
    }
});

$('#search').change(function (e) {
    $("#searchModal").val($(this).val());
});

$('#searchModal').change(function (e) {
    $("#search").val($(this).val());
});

$('#searchIcon').on("click", function (e) {
    filterData();
});

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Term', $('#searchModal').val());
        addPropertyToObject(requestObject, 'Language', $('#language').val());
        addPropertyToObject(requestObject, 'Author', $('#author').val());
        addPropertyToObject(requestObject, 'License', $('#license').val());
        addPropertyToObject(requestObject, 'TermIndicator', $('input[name="exact"]:checked').val());
    }

    return requestObject;
}

function advanceFilter() {
    filterData();
}

function previewThesaurus(o4mtid) {
    window.location.href = `/ThesaurusGlobal/PreviewThesaurus?Id=${o4mtid}`;
}