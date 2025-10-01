function reloadSecondaryTable(url, container, pageNumIdentifier) {
    //setFilterFromUrl();
    let requestObject = {};
    checkUrlPageParams();
    checkSecondaryPage();
    requestObject.Page = window[pageNumIdentifier];
    requestObject.PageSize = getPageSize();
    requestObject.IsAscending = isAscending;
    requestObject.ColumnName = columnName;

    if ($('#title').val()) {
        requestObject.Title = $('#title').val();
    }

    callServer({
        type: 'GET',
        url: `${url}`,
        data: requestObject,
        success: function (data) {
            setTableContent(data, `#${container}`);
            $(`#${container}`).show();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
            $(`#${container}`).hide();
        }
    });
}

function changePageSecondary(num, e, url, container, pageNumIdentifier, preventPushHistoryState) {
    e.preventDefault();
    formsCurrentPage = num;
    if (!preventPushHistoryState) {
        history.pushState({}, '', `?page=${currentPage}&pageSize=${getPageSize()}&secondaryPage=${num}`);
    }
    reloadSecondaryTable(url, container, pageNumIdentifier);
}

function clickedSimulatorRow(e, thesaurusId, versionId) {
    if (canExecuteClickedRow($(e.target), "")) {
        redirectToDistributionParams(e, thesaurusId, versionId);
    }
}

function redirectToDistributionParams(event, thesaurusId, versionId) {
    event.preventDefault();
    event.stopPropagation();
    window.location.href = `/FormDistribution/GetByThesaurusId?thesaurusId=${thesaurusId}&VersionId=${versionId}`;
}

function filter() {
    reloadSecondaryTable("/FormDistribution/ReloadForms", "formsTableContainer", "formsCurrentPage")
}
