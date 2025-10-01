var nominatorCodeId;
var nomineeCodeId;
var isChildToParent = true;
var nominatorColumnName;
var nominatorSwitchCount = 0;
var nominatorIsAscending = null;
var nomineeColumnName;
var nomineeSwitchCount = 0;
var nomineeIsAscending = null;
var nominatorAssociationColumnName;
var nominatorAssociationSwitchCount = 0;
var nominatorAssociationIsAscending = null;
var nomineeAssociationColumnName;
var nomineeAssociationSwitchCount = 0;
var nomineeAssociationIsAscending = null;

var associations = [];
var parentValue;
var childValue;
var associationsForDelete = [];

$(document).ready(function () {
    if ($('#viewAssociationPerm').val()) {
        reloadNominatorTable();
        reloadNomineeTable();
    }
});

function reloadNominatorTable() {
    let requestObject = getFilterParametersObject();
    setRequestObject(requestObject);
    requestObject.IsAscending = nominatorIsAscending;
    requestObject.ColumnName = nominatorColumnName;
    requestObject.CodeSetId = $('#codeSetId').val();
    requestObject.CodeSetDisplay = $('#codeSetDisplay').val();

    callServer({
        type: 'GET',
        url: '/Code/ReloadNominatorTable',
        data: requestObject,
        success: function (data) {
            $("#nominatorTableContainer").html(data);
            addNominatorSortArrows();
            reloadAssociationTable();
            reloadCodeSetGroup(true);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function reloadNomineeTable(codeSetId, nomineeCodeId, nomineeCodeDisplay) {
    let requestObject = getNomineeFilterParametersObject();
    setRequestObject(requestObject);

    if (codeSetId != null)
        requestObject.CodeSetId = codeSetId;

    requestObject.IsAscending = nomineeIsAscending;
    requestObject.ColumnName = nomineeColumnName;

    callServer({
        type: 'GET',
        url: '/Code/ReloadNomineeTable',
        data: requestObject,
        success: function (data) {
            $("#nomineeTableContainer").html(data);
            addNomineeSortArrows();
            if (nomineeCodeDisplay != null) {
                $("#nomineeCodeSetId").val(nomineeCodeId);
                $("#nomineeCodeSetDisplay").val(nomineeCodeDisplay);
            }
            selectActiveNomineeElement(nomineeCodeId);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getNomineeFilterParametersObject() {
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        if ($('#nomineeCodeSetId').val()) {
            addPropertyToObject(result, 'codeSetId', $('#nomineeCodeSetId').val());
        }
        if ($('#nomineeCodeSetDisplay').val()) {
            addPropertyToObject(result, 'codeSetDisplay', encodeURIComponent($('#nomineeCodeSetDisplay').val()));
        }
    }

    return result;
}

function setRequestObject(requestObject) {
    delete requestObject.CodeSetId;
    delete requestObject.CodeSetDisplay;
    requestObject.Page = getPageNum();
    requestObject.PageSize = getPageSize();
}

function selectNominatorCode(element, codeIdNominator, codeValue) {
    if (isChildToParent) {
        nominatorCodeId = codeIdNominator;
        parentValue = codeValue;
        $('.association-row').removeClass('active-code-tr');
        if (nomineeCodeId != null)
            document.getElementById("associateButton").removeAttribute("disabled");

        if (element.classList.contains("active-code-tr")) {
            deselectCodeAssociation(element, true);
        }
        else {
            $('.nominator-row').removeClass('active-code-tr');
            element.classList.add("active-code-tr");
        }

        if (nomineeCodeId != null && nominatorCodeId != null) {
            callServer({
                type: 'GET',
                url: `/Code/IsValidAssociation?parentId=${nominatorCodeId}&childId=${nomineeCodeId}`,
                success: function (data) {
                    if (!data && !associationAlreadyExists())
                        document.getElementById("associateButton").removeAttribute("disabled");
                    else
                        document.getElementById("associateButton").disabled = true;
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }
}

function associationAlreadyExists() {
    let result = false;
    $('#associationContainer').find('table tbody tr').each(function (index, element) {
        var parentId = $(element).attr("data-parentid");
        var childId = $(element).attr("data-childid");

        if (parentId == nominatorCodeId && childId == nomineeCodeId)
            result = true;
    });
    return result;
}

function selectNomineeCode(element, codeIdNominator, codeValue) {
    if (isChildToParent) {
        nomineeCodeId = codeIdNominator;
        childValue = codeValue;
        $('.association-row').removeClass('active-code-tr');
        if (nominatorCodeId != null)
            document.getElementById("associateButton").removeAttribute("disabled");

        if (element.classList.contains("active-code-tr")) {
            deselectCodeAssociation(element, false);
        }
        else {
            $('.nominee-row').removeClass('active-code-tr');
            element.classList.add("active-code-tr");
        }

        if (nomineeCodeId != null && nominatorCodeId != null) {
            callServer({
                type: 'GET',
                url: `/Code/IsValidAssociation?parentId=${nominatorCodeId}&childId=${nomineeCodeId}`,
                success: function (data) {
                    if (!data && !associationAlreadyExists())
                        document.getElementById("associateButton").removeAttribute("disabled");
                    else
                        document.getElementById("associateButton").disabled = true;
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }
}

function associate() {
    let codeValue = document.createElement('td');
    $(codeValue).attr("data-property", "codeId");
    $(codeValue).html(nomineeCodeId);
    let parent = document.createElement('td');
    $(parent).attr("data-property", "parent");
    $(parent).html(parentValue);
    let child = document.createElement('td');
    $(child).attr("data-property", "child");
    $(child).html(childValue);

    let removeIcon = document.createElement('td');
    $(removeIcon).addClass('position-relative');
    $(removeIcon).addClass('remove-code-association');
    let i = document.createElement('i');
    $(i).addClass('remove-association');
    $(removeIcon).append(i);

    let tr = document.createElement('tr');
    $(tr).addClass("table-content-row");
    $(tr).addClass("association-entry");

    $(tr).attr("data-parentid", nominatorCodeId);
    $(tr).attr("data-childid", nomineeCodeId);

    tr.append(codeValue);
    tr.append(parent);
    tr.append(child);
    tr.append(removeIcon);

    associations.push({ ParentId: nominatorCodeId, ChildId: nomineeCodeId });

    $("#associationTable tbody").append(tr); 
    document.getElementById("associateButton").disabled = true;

    if (document.getElementById("noContentAssociation") != null)
        document.getElementById("noContentAssociation").remove();

    document.getElementById("applyAssociationButton").removeAttribute("disabled");
    document.getElementById("saveAssociationButton").removeAttribute("disabled");
}

function countNewAssociations() {
    var result = 0;
    $('#associationContainer').find('table tbody tr').each(function (index, element) {
        if ($(element).attr("data-parentid") != null)
            result++;
    });
    return result;
}

$(document).on('click', '.remove-code-association', function (e) {
    var parentId = $(e.currentTarget).closest('tr').attr("data-parentid");
    var childId = $(e.currentTarget).closest('tr').attr("data-childid");

    for (var i = 0; i < associations.length; i++)
        if (associations[i].ParentId === parentId && associations[i].ChildId === childId)
            associations.splice(i, 1);

    $(e.currentTarget).closest('tr').remove();
    createNoResultContent();

    if (!associationAlreadyExists())
        document.getElementById("associateButton").removeAttribute("disabled");
    if (countNewAssociations() == 0) {
        document.getElementById("applyAssociationButton").disabled = true;
        document.getElementById("saveAssociationButton").disabled = true;
    }
});

function removeAssociationFromRow(e, associationId) {
    $(`#row-${associationId}`).remove();
    document.getElementById("applyAssociationButton").removeAttribute("disabled");
    document.getElementById("saveAssociationButton").removeAttribute("disabled");
    $('#nominatorAssociationTable tbody tr').removeClass('active-code-tr');
    $('#nomineeAssociationTable tbody tr').removeClass('active-code-tr');
    associationsForDelete.push(associationId);
}

function removeAssociations() {
    for (const association of associationsForDelete) {
        deleteAssociations(association);
    }
}

function deleteAssociations(associationId) {
    callServer({
        type: "DELETE",
        url: `/Code/DeleteCodeAssociation?associationId=${associationId}`,
        success: function (data) {
            $(`#row-${associationId}`).remove();
            createNoResultContent();
            toastr.success(`Success`);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function countAllAssociations() {
    var result = 0;
    $('#associationContainer').find('table tbody tr').each(function (index, element) {
        result++;
    });
    return result;
}

function createNoResultContent() {
    if (countAllAssociations() == 0) {
        let div = document.createElement('div');
        $(div).addClass("no-result-content");
        $(div).attr("id", "noContentAssociation");

        let img = document.createElement('img');
        $(img).attr("src", "/css/img/icons/no_results.svg");

        let br = document.createElement('br');

        let secondDiv = document.createElement('div');
        $(secondDiv).addClass("no-result-text");
        $(secondDiv).html("No result found!");

        div.append(img);
        div.append(br);
        div.append(secondDiv);

        document.getElementById("associationContainer").append(div);
    }
}

function createAssociation(isSaveAndExit) {
    var request = {};
    request['associations'] = associations;
    if (associations.length > 0) {
        callServer({
            type: "POST",
            url: "/Code/CreateCodeAssociation",
            data: request,
            success: function (data, textStatus, jqXHR) {
                removeAssociations();
                toastr.options = {
                    timeOut: 100
                }
                if (isSaveAndExit) {
                    saveInitialFormData("#codeSetsForm");
                    toastr.options.onHidden = function () {
                        window.location.href = `/CodeSet/GetAll`;
                    }
                }
                else {
                    reloadAssociationTable();
                    document.getElementById("associateButton").disabled = true;
                    associations = [];
                    document.getElementById("applyAssociationButton").disabled = true;
                    document.getElementById("saveAssociationButton").removeAttribute("disabled");
                    deselectCodeAssociation(null, true);
                    deselectCodeAssociation(null, false);
                    saveInitialFormData("#codeSetsForm");
                }
                toastr.success("Success");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    else if (associationsForDelete.length > 0) {
        removeAssociations();
        saveInitialFormData("#codeSetsForm");

        if (isSaveAndExit)
            window.location.href = `/CodeSet/GetAll`;
    }
    else
    {
        window.location.href = `/CodeSet/GetAll`;
    }
}

function reloadAssociationTable() {
    var request = {};
    request['ParentId'] = nominatorCodeId;
    request['IsChildToParent'] = isChildToParent;
    request.IsAscending = nominatorAssociationIsAscending;
    request.ColumnName = nominatorAssociationColumnName;
    request.CodeSetId = $('#codeSetId').val();
    request.IsReadOnly = $('#isReadOnly').val() === 'true' ? true : false;

    callServer({
        type: 'GET',
        url: `/Code/ReloadAssociationTable`,
        data: request,
        success: function (data) {
            $("#nominatorTable").html(data);
            addNominatorAssociationSortArrows();
            if (!isChildToParent)
                checkActiveAssociation('nominator');
            saveInitialFormData("#codeSetsForm");
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function parentToChild() {
    $('#nomineeCodeSetId').val('');
    reloadNomineeTable();
    deselectCodeAssociation(null, true);
    deselectCodeAssociation(null, false);
    isChildToParent = false;
    resetNominatorSortElement();
    reloadAssociationTable();
}

function childToParent() {
    $('#nomineeCodeSetId').val('');
    reloadNomineeTable();
    isChildToParent = true;
    resetNominatorSortElement();
    reloadAssociationTable();
    $('.nominator-row').removeClass('active-code-tr');
}

$(document).on('click', '#codeAssociationTable', function (e) {
    var arrowElement = document.getElementById("codeAssociationArrow");
    checkArrowClass(arrowElement);
});

function checkArrowClass(arrowElement) {
    if ($(arrowElement).hasClass("code-association-icon")) {
        arrowElement.classList.remove("code-association-icon");
        arrowElement.classList.add("code-association-icon-minus");
    }
    else {
        arrowElement.classList.remove("code-association-icon-minus");
        arrowElement.classList.add("code-association-icon");
    }
}

function checkActiveAssociation(tableId) {
    document.getElementById(`parentElement-${tableId}`).classList.add("association-table-content-grey");
    document.getElementById(`parentElement-${tableId}`).classList.add("child-to-parent");
    document.getElementById(`childElement-${tableId}`).classList.remove("association-table-content-grey");
    document.getElementById(`childElement-${tableId}`).classList.remove("parent-to-child");
}

function deselectCodeAssociation(element, isNominator) {
    if (element != null)
        element.classList.remove("active-code-tr");
    else
        if (isNominator)
            $.each($('#nominatorAssociationTable tbody tr'), function (idx, val) {
                $(this).removeClass('active-code-tr');
            });
        else
            $.each($('#nomineeAssociationTable tbody tr'), function (idx, val) {
                $(this).removeClass('active-code-tr');
            });

    checkIfNominatorOrNomineeDeselect(isNominator);
    document.getElementById("associateButton").disabled = true;
}

function checkIfNominatorOrNomineeDeselect(isNominator) {
    if (isNominator)
        nominatorCodeId = null;
    else
        nomineeCodeId = null;
}

function sortNominatorTable(column) {
    if (nominatorSwitchCount == 0) {
        if (nominatorColumnName == column)
            nominatorIsAscending = checkIfAsc(nominatorIsAscending);
        else
            nominatorIsAscending = true;
        nominatorSwitchCount++;
    }
    else {
        if (nominatorColumnName != column)
            nominatorIsAscending = true;
        else
            nominatorIsAscending = checkIfAsc(nominatorIsAscending);
        nominatorSwitchCount--;
    }
    nominatorColumnName = column;
    reloadNominatorTable();
}

function addNominatorSortArrows() {
    var element = document.getElementById(nominatorColumnName);
    if (element != null) {
        element.classList.remove("sort-arrow");
        if (nominatorIsAscending) {
            element.classList.remove("sort-arrow-desc");
            element.classList.add("sort-arrow-asc");
        }
        else {
            element.classList.remove("sort-arrow-asc");
            element.classList.add("sort-arrow-desc");
        }
    }
}

function sortNomineeTable(column) {
    if (nomineeSwitchCount == 0) {
        if (nomineeColumnName == column)
            nomineeIsAscending = checkIfAsc(nomineeIsAscending);
        else
            nomineeIsAscending = true;
        nomineeSwitchCount++;
    }
    else {
        if (nomineeColumnName != column)
            nomineeIsAscending = true;
        else
            nomineeIsAscending = checkIfAsc(nomineeIsAscending);
        nomineeSwitchCount--;
    }
    nomineeColumnName = column;
    reloadNomineeTable(nomineeColumnName, nomineeIsAscending);
}

function addNomineeSortArrows() {
    var element = document.getElementById(nomineeColumnName);
    if (element != null) {
        element.classList.remove("sort-arrow");
        if (nomineeIsAscending) {
            element.classList.remove("sort-arrow-desc");
            element.classList.add("sort-arrow-asc");
        }
        else {
            element.classList.remove("sort-arrow-asc");
            element.classList.add("sort-arrow-desc");
        }
    }
}
function sortAssociation(column){
    if (nominatorAssociationSwitchCount == 0) {
        if (nominatorAssociationColumnName == column)
            nominatorAssociationIsAscending = checkIfAsc(nominatorAssociationIsAscending);
        else
            nominatorAssociationIsAscending = true;
        nominatorAssociationSwitchCount++;
    }
    else {
        if (nominatorAssociationColumnName != column)
            nominatorAssociationIsAscending = true;
        else
            nominatorAssociationIsAscending = checkIfAsc(nominatorAssociationIsAscending);
        nominatorAssociationSwitchCount--;
    }
    nominatorAssociationColumnName = column;
    reloadAssociationTable();
}

function addNominatorAssociationSortArrows() {
    var element = document.getElementById(nominatorAssociationColumnName);
    if (element != null) {
        element.classList.remove("sort-arrow");
        if (nominatorAssociationIsAscending) {
            element.classList.remove("sort-arrow-desc");
            element.classList.add("sort-arrow-asc");
        }
        else {
            element.classList.remove("sort-arrow-asc");
            element.classList.add("sort-arrow-desc");
        }
    }
}

function resetNominatorSortElement() {
    nominatorAssociationColumnName = null;
    nominatorAssociationSwitchCount = 0;
    nominatorAssociationIsAscending = null;
}

function selectAssociation(event, element, codeSetId, nominatorCodeId, nomineeCodeId) {
    if (!event.target.classList.contains('remove-association'))
        associateCodes(element, codeSetId, nominatorCodeId, nomineeCodeId);
}

function associateCodes(element, codeSetId, nominatorCodeId, nomineeCodeId) {
    $('#nomineeCodeSetId').val('');
    $('.association-row').removeClass('active-code-tr');
    element.classList.add("active-code-tr");

    if (document.getElementById("associateButton") != null)
        document.getElementById("associateButton").disabled = true;

    selectActiveNominatorElement(nominatorCodeId);
    reloadNomineeTable(codeSetId, nomineeCodeId);
}

function selectActiveNominatorElement(nominatorCodeId) {
    var activeNominatorElement = document.getElementById("row_" + nominatorCodeId);
    $('.nominator-row').removeClass('active-code-tr');
    activeNominatorElement.classList.add("active-code-tr");
}

function selectActiveNomineeElement(nomineeCodeId) {
    var activeNomineeElement = document.getElementById("nominee-" + nomineeCodeId);
    if (activeNomineeElement != null)
        activeNomineeElement.classList.add("active-code-tr");
}

$(document).on('keyup', '#nomineeCodeSetDisplay', function (e) {
    if (isInputCharacter(e.which)) {
        page = 1;
        reloadCodeSets(false);
    }
});

function loadMoreCodeSets() {
    page++;
    reloadCodeSets(true);
}

function reloadCodeSets(loadMore) {
    let requestObject = {};
    requestObject.CodeSetDisplay = $('#nomineeCodeSetDisplay').val().toLowerCase();
    requestObject.Page = page;
    requestObject.PageSize = 20;

    if (requestObject.CodeSetDisplay.length > 2) {
        callServer({
            method: 'get',
            url: `/CodeSet/ReloadCodeSets`,
            data: requestObject,
            success: function (data) {
                if (loadMore) {
                    $('#codeSetOptions').append(data);
                    document.getElementById("nomineeCodeSetDisplay").focus();
                }
                else
                    $('#codeSetOptions').html(data);
                $('#loadCodeSets').remove();
                $('#codeSetOptions').show();
                if (data.trim()) {
                    if ($('#codeSetOptions').find(".option").length >= requestObject.PageSize * page) {
                        $('#codeSetOptions').append(appendLoadMore());
                    }
                }
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function appendLoadMore() {
    let divElement = document.createElement('div');
    $(divElement).addClass("load-more-button-container");
    $(divElement).addClass("load-more-umls");
    divElement.id = "loadCodeSets";
    let loadMoreElement = document.createElement('div');
    $(loadMoreElement).addClass("load-more-button");
    $(loadMoreElement).addClass("load-concepts");
    loadMoreElement.onclick = function () { loadMoreCodeSets() };
    var LoadMoreText = loadMore;
    var element = $(loadMoreElement).append(LoadMoreText)
    $(divElement).append(element);

    return divElement;
}

function filterCodeSetName(event, codeSetId, codeSetDisplay) {
    event.preventDefault();
    $('#nomineeCodeSetId').val('');
    reloadNomineeTable(codeSetId, codeSetId, codeSetDisplay);
}

var page = 1;