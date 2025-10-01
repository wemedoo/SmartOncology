var preventPushStateWhenReload;
$(document).ready(function () {
    reloadTable(true);
});

var currentPage = 1;
var tableConfigs = [];
var defaultFilter;
var columnName;
var switchcount = 0;
var isAscending = null;
var isModalTable = false;
var isPathwayModule = false;
var isCustomState = false;
var tableContainer = null;

$(document).on('keypress', '.filter-item input', function (e) {
    if (e.which === enter) {
        if (inputsAreInvalid()) {
            return;
        }
        try {
            pushState();
            callCorrespondingFilter($(this));

        } catch (error) {
            pushStateWithoutFilter(1);
        }

        currentPage = 1;
        reloadTable();
    }
});

function applyActionsBeforeServerReload(excludeAdvancedPropertyList, customizeTags = true, tableArguments = undefined) {
    hideAdvancedFilterModal();
    if (!customizeTags) {
        setFilterTagsFromUrl();
    }
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    if (customizeTags) {
        setFilterTagsFromObj(requestObject);
    }
    setAdvancedFilterBtnStyle(requestObject, excludeAdvancedPropertyList);
    checkUrlPageParams();
    setTableProperties(requestObject, tableArguments);

    return requestObject;
}

function applyActionsBeforeServerReloadSimple(applyFiltersFromUrl = true, checkPageParams = false, tableArguments = undefined, requestedTable = '') {
    if (applyFiltersFromUrl) {
        setFilterFromUrl();
    }
    let requestObject = getFilterParametersObject(requestedTable);
    setFilterTagsFromObj(requestObject, requestedTable);
    if (checkPageParams) {
        checkUrlPageParams();
    }
    setTableProperties(requestObject, tableArguments);

    return requestObject
}

function resetTableCommonVariables() {
    columnName = null;
    isAscending = null;
}

function callCorrespondingFilter($filterInput) {
    if (belongsToAdvanceFilter($filterInput)) {
        advanceFilter();
    } else {
        mainFilter();
    }
}

function belongsToAdvanceFilter($filterInput) {
    return $filterInput.parents('#advancedFilterForm').length == 1;
}

function changePage(num,e, url, container, pageNumIdentifier, preventPushHistoryState) {
    e.preventDefault();

    tableContainer = container;

    if (url) {
        changePageSecondary(num, e, url, container, pageNumIdentifier, preventPushHistoryState);
        return;
    }
    else {
        if (!preventPushHistoryState) {
            if (currentPage != num) {
                try {
                    let urlPageParams;
                    if (isCustomState)
                        urlPageParams = getCustomPageParams(num, getPageSize());
                    else
                        urlPageParams = `?page=${num}&pageSize=${getPageSize()}`;
                    let filter = getFilterParametersObject();
                    let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

                    history.pushState({}, '', fullUrlParams);

                } catch (error) {
                    pushStateWithoutFilter(num);
                }
            }
        }
    }

    currentPage = num;

    if (isModalTable)
        reloadModalTable();
    else
        reloadTable();
}

function filterData() {
    if (inputsAreInvalid()) {
        return;
    }
    pushState();
    currentPage = 1;
    reloadTable();
}

function addPropertyToObject(object, name, value) {
    if (value) {
        object[name] = value;
    }
}

function getPageSize() {
    let url = new URL(window.location.href);
    let pageSize = url.searchParams.get("pageSize");
    if (pageSize) {
        userPageSize = pageSize;
    }

    let targetPageSize = null;
    
    if (tableContainer && tableContainer !== "") {
        targetPageSize = $(`.pageSizeSelector[data-container=${tableContainer}]`).val();
    }
    else {
        targetPageSize = $("#pageSizeSelector").val();
    }

    return targetPageSize ? targetPageSize : userPageSize;
}

$(document).on('change', '.pageSizeSelector', function (e) {
    currentPage = 1;

    tableContainer = $(e.target).data('container');

    try {
        if (isPathwayModule) {
            pushState();
        }
        else {
            if (!isModalTable)
                pushState();
        }

    } catch (error) {
        pushStateWithoutFilter(1);
    }

    if (isModalTable)
        reloadModalTable();
    else
        reloadTable(true);

    tableConfigs.forEach(x => {
        reloadSecondaryTable(x.url, x.container, x.pageNumIdentifier);
    });

    updatePageSize(getPageSize());
});

function clickedRow(e, hasUpdatePermission, id, version, customClassNames) {
    if (canExecuteClickedRow($(e.target), customClassNames)) {
        if (hasUpdatePermission) {
            editEntity(e, id, version);
        } else {
            viewEntity(e, id);
        }
    }
}

function canExecuteClickedRow($target, customClassNames) {
    let tableElementsClass = [
        'dropdown-button', 'fa-bars', 'dropdown-item', 'dots', 'table-more'
    ];
    if (customClassNames) {
        tableElementsClass = tableElementsClass.concat(customClassNames.split(','));
    }
    for (let tableElementClass of tableElementsClass) {
        if ($target.hasClass(tableElementClass)) {
            return false;
        }
    }
    return true;
}

function getDefaultFilter(){
    let result = {};
    if (defaultFilter) {
        Object.keys(defaultFilter).forEach(x => {
            if (defaultFilter[x]) {
                let value = defaultFilter[x];
                if (isDateTimeFilter(x)) {
                    value = toValidTimezoneFormat(value);
                }
                result[x] = value;
            }
        });
    }

    return result;
}

function getPageNum() {
    var url = new URL(window.location.href);
    var page = url.searchParams.get("page");
    let pageNum;
    if (page) {
        pageNum = page;
    } else {
        pageNum = "1";
    }

    return pageNum;
}

function setTableProperties(requestObject, tableArguments) {
    delete requestObject.pageSize;
    delete requestObject.page;
    requestObject.Page = tableArguments?.page ?? currentPage;
    requestObject.PageSize = tableArguments?.pageSize ?? getPageSize();
    let doOrdering = tableArguments ? tableArguments.doOrdering: true;
    if (doOrdering) {
        requestObject.IsAscending = isAscending;
        requestObject.ColumnName = columnName;
    }
}

$(document).ready(function () {
    $('.tooltip-tipable').tooltip();
});

function advancedFilterModal(event) {
    event.preventDefault();
    event.stopPropagation();
    $("#advancedFilterModal").modal("show");
}

function hideAdvancedFilterModal() {
    $('#advancedFilterModal').modal('hide');
}

function thesaurusMoreModal(event, id) {
    event.preventDefault();
    event.stopPropagation();

    callServer({
        type: 'GET',
        url: `/ThesaurusEntry/ThesaurusMoreContent?id=${id}`,
        success: function (data) {
            $('#thesaurusMoreModalContent').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });

    $("#thesaurusMoreModal").modal("show");
}


function pushState() {
    if (!preventPushStateWhenReload) {
        let urlPageParams;
        if (isCustomState)
            urlPageParams = pushCustomState();
        else
            urlPageParams = `?page=1&pageSize=${getPageSize()}`;
        let filter = getFilterParametersObject();
        let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

        history.pushState({}, '', fullUrlParams);
    }
}

function pushStateWithoutFilter(num) {
    if (!preventPushStateWhenReload) {
        history.pushState({}, '', `?page=${num}&pageSize=${getPageSize()}`);
    }
}

function appendDeleteIcon() {
    let deleteElement = document.createElement('img');
    $(deleteElement).addClass("edit-svg-size");
    var deleteIcon = document.getElementById("deleteIcon").src;
    $(deleteElement).attr("src", deleteIcon);

    return deleteElement;
}

function appendEditIcon() {
    let editElement = document.createElement('img');
    $(editElement).addClass("edit-svg-size");
    var editIcon = document.getElementById("editIcon").src;
    $(editElement).attr("src", editIcon);

    return editElement;
}

function setFilterTagsFromUrl() {
    let url = new URL(window.location.href);
    let entries = url.searchParams.entries();
    let params = paramsToObject(entries);

    if (defaultFilter) {
        defaultFilter = params;
    }

    reloadTags(params);
}

function setFilterTagsFromObj(requestObject, dataContainer = null) {
    let requestObjectForDisplay = { ...requestObject };
    let params = getFilterParametersObjectForDisplay(requestObjectForDisplay);

    reloadTags(params, requestObject, dataContainer);
}

function setFilterTagsFromObjWithParams(requestObject, params, dataContainer = null) {
    reloadTags(params, requestObject, dataContainer);
}

function reloadTags(params, requestObject, dataContainer = null, removeAllFilterTags = true) {

    let filterTagsContainer = dataContainer ? $(`.filter-elements[data-container=${dataContainer}]`) : $('#filterElements');
    if (removeAllFilterTags) {
        $(filterTagsContainer).children('.filter-element').remove();
    } else {
        $(filterTagsContainer).children('.filter-element.static-element').remove();
    }

    for (let param in params) {
        if (params[param] && isNonPagingParam(param)) {
            $(filterTagsContainer).append(addFilterTag(params, param, requestObject, dataContainer, removeAllFilterTags));
        }
    }
}

function addFilterTag(params, param, requestObject, dataContainer, addTagCloseSign = true) {
    let element = document.createElement('div');
    $(element).addClass('filter-element');
    if (!addTagCloseSign) {
        $(element).addClass('static-element');
    }
    if (isDateTimeFilter(param)) {
        $(element).html(getDateTimeFilterTag(params, param));
    } else {
        $(element).html(params[param]);
    }

    let originalValue = requestObject ? requestObject[param] : params[param];
    if (addTagCloseSign) {
        $(element).append(getTagCloseSign(param, originalValue, dataContainer && dataContainer !== ""));
    }

    return $(element);
}

function getTagCloseSign(name, value, multiTable) {
    let x = document.createElement('img');
    $(x).attr('src', "/css/img/icons/Administration_remove.svg");
    $(x).addClass('ml-2');
    $(x).addClass(multiTable ? 'remove-multitable-filter cursor-pointer' : 'remove-filter');
    $(x).attr('name', name);
    $(x).css('font-size', '12px');
    $(x).css('width', '15px');
    $(x).css('padding', '5px');
    $(x).attr('data-value', value);

    return x;
}

function setAdvancedFilterBtnStyle(object, excludePropertiesList) {
    if (objectHasNoProperties(object, excludePropertiesList)) {
        $('#advancedId').children('div:first').removeClass('btn-advanced');
        $('#advancedId').find('button:first').addClass('btn-advanced-link');
        $('#advancedId').find('img:first').css('display', 'none');
    } else {
        $('#advancedId').children('div:first').addClass('btn-advanced');
        $('#advancedId').find('button:first').removeClass('btn-advanced-link');
        $('#advancedId').find('img:first').css('display', 'inline-block');
    }
}

function getFilterParameterObjectForDisplay(filterObject, attributeName) {
    if (filterObject.hasOwnProperty(attributeName)) {
        let attributeId = filterObject[attributeName];
        let attributeDisplay = $(`option#${attributeName}_${attributeId}`).attr('data-display');
        if (attributeDisplay) {
            addPropertyToObject(filterObject, attributeName, attributeDisplay);
        }
    }
}

function isNonPagingParam(param) {
    return param.toLowerCase() != 'page' && param.toLowerCase() != 'pagesize';
}

function sortTable(column) {
    if (switchcount == 0) {
        if (columnName == column)
            isAscending = checkIfAsc(isAscending);
        else
            isAscending = true;
        switchcount++;
    }
    else {
        if (columnName != column)
            isAscending = true;
        else
            isAscending = checkIfAsc(isAscending);
        switchcount--;
    }
    columnName = column;
    reloadTable(columnName, isAscending);
}

function checkIfAsc(isAscending) {
    return !isAscending;
}

function setTableContent(htmlData, containerId = '#tableContainer') {
    $(containerId).html(htmlData);
    addSortArrows();
}

function addSortArrows() {
    var element = document.getElementById(columnName);
    if (element != null) {
        element.classList.remove("sort-arrow");
        if (isAscending) {
            element.classList.remove("sort-arrow-desc");
            element.classList.add("sort-arrow-asc");
        }
        else {
            element.classList.remove("sort-arrow-asc");
            element.classList.add("sort-arrow-desc");
        }
    }
}