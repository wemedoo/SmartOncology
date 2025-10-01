$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    saveInitialCodeFormData("#idCodes");
    window.onbeforeunload = function () {
        if (!compareCodeForms("#idCodes")) {
            return "You have unsaved changes. Are you sure you want to leave?";
        }
    };
});

function showAliases(event) {
    event.stopPropagation();
    document.getElementById("aliasesTab").classList.add("code-active-tab"); 
    document.getElementById("codeValueTab").classList.remove("code-active-tab");
    $("#showCodeValues").hide();
    $("#aliasTableContainer").show();
    reloadTable();
}

function reloadTable() {
    reloadCodeSetGroup();

    var element = document.getElementById('aliasesTab');
    if (element.classList.contains('code-active-tab')) {
        let requestObject = {};
        requestObject.Page = getPageNum();
        requestObject.PageSize = getPageSize();
        requestObject.IsAscending = isAscending;
        requestObject.ColumnName = columnName;
        requestObject.CodeId = $("#codeValue").val();
        requestObject.CodeDisplay = encodeURIComponent($("#codeValueDisplay").val());
        isCustomState = true;

        callServer({
            type: 'GET',
            url: '/Code/ReloadAliasTable',
            data: requestObject,
            success: function (data) {
                setTableContent(data, "#aliasTableContainer");
                saveInitialCodeFormData("#idCodes");
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function getFilterParametersObject() {
    let result = {};
    return result;
}

function pushCustomState() {
    var codeSetDisplay = encodeURIComponent($('#thesaurusSearchInputCode').val());
    var codeDisplay = encodeURIComponent($('#codeValueDisplay').val());
    return `?page=1&pageSize=${getPageSize()}&CodeId=${$('#codeValue').val()}&CodeDisplay=${encodeURIComponent(codeDisplay)}&CodeSetId=${$('#codeSetNumberForCode').val()}&CodeSetDisplay=${codeSetDisplay}`;
}

function getCustomPageParams(number, pageSize) {
    var codeSetDisplay = encodeURIComponent($('#thesaurusSearchInputCode').val());
    var codeDisplay = encodeURIComponent($('#codeValueDisplay').val());
    return `?page=${number}&pageSize=${pageSize}&CodeId=${$('#codeValue').val()}&CodeDisplay=${encodeURIComponent(codeDisplay)}&CodeSetId=${$('#codeSetNumberForCode').val()}&CodeSetDisplay=${codeSetDisplay}`;
}