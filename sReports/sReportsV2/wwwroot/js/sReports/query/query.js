var queryColumnName;
var querySwitchCount = 0;
var queryIsAscending = null;

$(document).ready(function () {

    $(document).on('click', "tr[id^='row-']", function () {
        var queryId = $(this).attr("id").replace("row-", "");
        var detailsRow = $("#details-" + queryId);
        detailsRow.toggle();

        var icon = $("img.query-collapse-icon[data-queryid='" + queryId + "']");
        if (detailsRow.is(':visible')) {
            icon.attr('src', '/css/img/icons/query-collapse.svg');
        } else {
            icon.attr('src', '/css/img/icons/query-expand.svg');
        }
    });

    $(document).on('hide.bs.modal', '#queryModal', function () {
        if ($('#raiseNewQuery').val() === 'true') {
            $('#queryHistoryModal').css({
                'z-index': ''
            });
        }
    });

    $(document).on('input', '.filter-input', function () {
        var input = $(this);
        var queryId = input.data('queryid');
        var button = $('.send-update-btn[data-queryid="' + queryId + '"]');
        var cancelLink = $('#update-controls-' + queryId + ' .advanced-cancel');

        if (input.val().trim() === '') {
            button.prop('disabled', true);
            cancelLink.hide();
        } else {
            button.prop('disabled', false);
            cancelLink.show();
        }
    });
});

function loadQueryHistoryTable(fieldId, queryId, isFormInstanceMode, readOnly) {
    var request = $.param({
        'dataIn.FieldId': fieldId,
        'dataIn.QueryId': queryId,
        isFormInstanceMode: isFormInstanceMode,
        readOnly: readOnly
    });

    callServer({
        type: 'GET',
        url: `/QueryManagement/LoadQueryHistoryTable?${request}`,
        success: function (data) {
            $('#queryHistoryTableContainer').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function updateQuery(event) {
    event.preventDefault();

    var queries = [];

    $('#queryHistoryForm tbody tr.table-content-row').each(function () {
        var queryId = $(this).attr('id').replace('row-', '');

        var historyItems = [];
        $(`#details-${queryId} .query-update-history .query-history-area`).each(function () {
            var lastUpdate = $(this).find('span').attr('data-full-date');
            var itemDiv = $(this).find('.query-history-item');

            var isResolved = itemDiv.find('em').length > 0;
            var userText, commentText, statusCD;

            if (isResolved) {
                var el = itemDiv.find('em').parent();
                userText = itemDiv.find('em').text().trim();
                commentText = '';
                statusCD = el.data('status');
                historyId = el.data('history-id') || 0;
            } else {
                var el = itemDiv.find('div');
                userText = itemDiv.find('strong').text().trim();
                commentText = el.text().trim();
                statusCD = el.data('status');
                historyId = el.data('history-id') || 0;
            }

            historyItems.push({
                QueryHistoryId: historyId,
                QueryId: queryId,
                LastUpdate: lastUpdate,
                Comment: commentText,
                StatusCD: statusCD
            });
        });

        queries.push({
            QueryId: queryId,
            StatusCD: historyItems.length > 0 ? historyItems[historyItems.length - 1].StatusCD : $("#newStatusCD").val(),
            Comment: historyItems.length > 0 ? historyItems[historyItems.length - 1].Comment : "",
            History: historyItems
        });
    });

    callServer({
        type: "POST",
        url: "/QueryManagement/Edit",
        contentType: "application/json",
        data: JSON.stringify(queries),
        success: function () {
            toastr.options = { timeOut: 1000 };
            $('#queryHistoryModal').modal('hide');
            if (!$('[name="formInstanceId"]').length) {
                reloadTable();
            }
            else {
                location.reload(true);
            }
            toastr.success("Success!");
        },
        error: function (xhr) {
            handleResponseError(xhr);
        }
    });
}

function addHistoryItem(event, isResolved, queryId, userText, statusCD) {
    event.preventDefault();

    var commentInput = $(`#comment-${queryId}`);
    var comment = commentInput.val() || "";

    var container = $(`#details-${queryId} .query-update-history`);
    if (!container.length) return;

    var historyArea = $('<div>').addClass('query-history-area');

    var now = new Date();
    var dateSpan = $('<span>')
        .text(formatDateTimeWithSeconds(now))
        .attr('data-full-date', formatDateTimeWithSeconds(now))
        .addClass('padding-top-10'); 
    historyArea.append(dateSpan);

    var itemDiv = $('<div>').addClass('query-history-item');

    if (isResolved) {
        var resolvedDiv = $('<div>').attr('data-status', statusCD); 
        var em = $('<em>');
        var resolvedText = userText.replace(
            /Resolved/i,
            '<span class="span-resolved">Resolved</span>'
        );
        em.html(resolvedText); 
        resolvedDiv.append(em);
        itemDiv.append(resolvedDiv);
        $(`#update-controls-${queryId}`).hide();
    } else {
        var strongEl = $('<strong>').text(userText);
        var commentDiv = $('<div>').text(comment).attr('data-status', statusCD);
        itemDiv.append(strongEl).append(commentDiv);
    }

    historyArea.append(itemDiv);

    container.append(historyArea);

    clearQueryInput(queryId);
}

function formatDateTimeWithSeconds(date) {
    const d = date instanceof Date ? date : new Date(date);
    const pad = (n) => String(n).padStart(2, '0');

    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ` +
        `${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
}

function clearQueryInput(queryId) {
    var input = $('#comment-' + queryId);
    var button = $('.send-update-btn[data-queryid="' + queryId + '"]');
    var cancelLink = $('#update-controls-' + queryId + ' .advanced-cancel');

    input.val('');
    button.prop('disabled', true);
    cancelLink.hide();
}

function sortQueryTable(column) {
    if (querySwitchCount == 0) {
        if (queryColumnName == column)
            queryIsAscending = checkIfQueryAsc(queryIsAscending);
        else
            queryIsAscending = true;
        querySwitchCount++;
    }
    else {
        if (queryColumnName != column)
            queryIsAscending = true;
        else
            queryIsAscending = checkIfQueryAsc(queryIsAscending);
        querySwitchCount--;
    }
    queryColumnName = column;

    showQueriesContent(queryColumnName, queryIsAscending, true);
}

function checkIfQueryAsc(queryIsAscending) {
    return !queryIsAscending;
}