var selectionUpperLimitSurpassed = false;
var selectionLowerLimitSurpassed = false;
var selectionUpperLimit = 7;
var selectionLowerLimit = 2;

var lastSavedHeaderLabels = [];

$(document).ready(function () {

    lastSavedHeaderLabels = getSelectedHeaderLabels();
    updateHeaderCounter();

    $(document).on('keyup', '#CustomHeadersSearchInput', function (e) {
        e.stopImmediatePropagation();
        let textFilter = $('#CustomHeadersSearchInput').val();
        textFilter = textFilter.normalize("NFD").replace(/\p{Diacritic}/gu, "")  // https://stackoverflow.com/questions/990904/remove-accents-diacritics-in-a-string-in-javascript

        $("#CustomHeaderSelectionTable tbody tr").each(function () {
            let text = $(this).find("td").text();

            if (!text.toLowerCase().includes(textFilter.toLowerCase()))
                $(this).hide();
            else
                $(this).show();
        });
    });

    $(document).on("click", '.custom-header-selection-row', function (e) {
        e.stopImmediatePropagation();

        if (!selectionUpperLimitSurpassed) {
            selectHeader(this);
            updateHeaderCounter();
        }
        else {
            toastr.error("Reached max number of selections!");
        }
        
    });

    $(document).on('click', '.remove-header', function (e) {
        e.stopImmediatePropagation();

        if (!selectionLowerLimitSurpassed) {
            removeHeader($(this).siblings('.selected-header-input'));
            triggerSearchInput();
            updateHeaderCounter();
        }
        else {
            toastr.error("Reached min number of selections!");
        }
    });

    $('#previewContainer').sortable({
        axis: "x",
    });

    $(document).on('click', '.rename-header', function (e) {
        e.stopImmediatePropagation();
        saveHeaderLabelOnRenameForRollback($(this).siblings('.selected-header-input'));
        enableHeaderInputToRename($(this));
    });

    $(document).on('focusout', '.selected-header-input', function (e) {
        e.stopImmediatePropagation();
        $(this).prop('disabled', true); 
        updateHeaderLabelAfterRename($(this));
    });

});

// -----

function enableHeaderInputToRename(renameHeader) {
    $('.selected-header-input').prop('disabled', true);
    renameHeader.siblings('.selected-header-input').prop('disabled', false);
}

function saveHeaderLabelOnRenameForRollback(selectedHeaderInput){
    previousLabel = selectedHeaderInput.val();
}

function updateHeaderLabelAfterRename(selectedHeaderInput) {
    let dataId = selectedHeaderInput.attr('data-id');
    let newLabel = selectedHeaderInput.val();

    if (newLabel == "") {
        rollbackPreviousHeaderLabel(selectedHeaderInput);
    }
    else {
        updateHeaderLabelPreviewAfterRename(dataId, newLabel);
    }
}

function rollbackPreviousHeaderLabel(selectedHeaderInput) {
    selectedHeaderInput.val(previousLabel);
    toastr.error("Header Labels cannot be empty!");
}

function updateHeaderLabelPreviewAfterRename(dataId, newLabel) {
    $('.custom-header-preview-element').each(function () {
        if ($(this).attr('data-id') == dataId) {
            $(this).attr('data-customlabel', newLabel);
            $(this).find('nobr').html(newLabel.toUpperCase());
        }
    });
}

// -----

function selectHeader(selectedHeaderRow) {
    let headerId = $(selectedHeaderRow).find("td").attr("data-id");
    let headerLabel = $(selectedHeaderRow).find("td").attr("data-label");
    let defaultHeaderCode = $(selectedHeaderRow).find("td").attr("data-defaultheadercode");

    if (typeof headerId !== 'undefined' && headerId != "") {
        selectCustomHeader(headerId, headerLabel);
        removeRowFromSelectionTable(selectedHeaderRow);
    } else if (typeof defaultHeaderCode !== 'undefined' && defaultHeaderCode != "") {  
        selectDefaultHeader(defaultHeaderCode, headerLabel);
        removeRowFromSelectionTable(selectedHeaderRow);
    }
}

function selectCustomHeader(headerId, headerLabel) {
    let selectedListElement =
        '<div class="selected-header d-flex">' +
        `<input disabled class="selected-header-input form-control" data-id="${headerId}" data-label="${headerLabel}" value="${headerLabel}" />` +
        '<div class="rename-header" title="Rename Header Label"><img class="rename-header-image" src="/css/img/icons/rename_gray.svg"></div > ' +
        '<div class="remove-header" title="Remove Header"><img src="/css/img/icons/remove_simulator.svg"></div > ' +
        '</div >';

    $('#selectedFieldsContainer').append(selectedListElement);

    let previewElement = `<div data-id="${headerId}" data-label="${headerLabel}" class="custom-header-preview-element" title="Drag and Drop to change the order"><nobr>${headerLabel.toUpperCase()}</nobr></div>`;
    $('#previewContainer').append(previewElement);
}

function selectDefaultHeader(defaultHeaderCode, headerLabel) {
    let selectedListElement =
        '<div class="selected-header d-flex">' +
        `<input disabled class="selected-header-input form-control" data-defaultheadercode="${defaultHeaderCode}" data-label="${headerLabel}" value="${headerLabel}" />` +
        '<div class="remove-header remove-default-header" title="Remove Header"><img src="/css/img/icons/remove_simulator.svg"></div > ' +
        '</div >';

    $('#selectedFieldsContainer').append(selectedListElement);

    let previewElement = `<div data-defaultheadercode="${defaultHeaderCode}" data-label="${headerLabel}" class="custom-header-preview-element" title="Drag and Drop to change the order"><nobr>${headerLabel.toUpperCase()}</nobr></div>`;
    $('#previewContainer').append(previewElement);
}

function removeRowFromSelectionTable(selectedHeaderRow) {
    $(selectedHeaderRow).remove();
}

// -----

function removeHeader(selectedHeaderInput) {
    let headerId = selectedHeaderInput.attr("data-id");
    let headerLabel = selectedHeaderInput.attr("data-label");
    let defaultHeaderCode = selectedHeaderInput.attr("data-defaultheadercode");
    let headerOrder = selectedHeaderInput.attr("data-order");

    removeHeaderFromSelectedContainer(selectedHeaderInput);

    if (typeof headerId !== 'undefined' && headerId != "") {
        removeCustomHeader(headerId);
        appendHeaderOnSelectionTable(headerLabel, headerId, "data-id");
    }
    else if (typeof defaultHeaderCode !== 'undefined' && defaultHeaderCode != "") { 
        removeDefaultHeader(defaultHeaderCode);
        appendHeaderOnSelectionTable(headerLabel, defaultHeaderCode, "data-defaultheadercode");
    }
    else if (typeof headerOrder !== 'undefined' && headerOrder != "") {
        removeOrder(headerOrder);
        appendHeaderOnSelectionTable(headerLabel, headerOrder, "data-order");
    }
}

function removeHeaderFromSelectedContainer(selectedHeaderInput) {
    selectedHeaderInput.parent('.selected-header').remove();
}

function removeCustomHeader(headerId) {
    $('.custom-header-preview-element').each(function () {
        if ($(this).attr("data-id") == headerId) {
            $(this).remove();
        }
    });   
}

function removeDefaultHeader(defaultHeaderCode) {
    $('.custom-header-preview-element').each(function () {
        if ($(this).attr("data-defaultheadercode") == defaultHeaderCode) {
            $(this).remove();
        }
    });
}

function removeOrder(order) {
    $('.custom-header-preview-element').each(function () {
        if ($(this).attr("data-order") == order) {
            $(this).remove();
        }
    });
}

function appendHeaderOnSelectionTable(headerLabel, defaultCodeOrId, dataAttributeName){
    $('#CustomHeaderSelectionTable tbody').prepend(`<tr class="custom-header-selection-row"><td class="custom-header-selection-cell" ${dataAttributeName}="${defaultCodeOrId}" data-label="${headerLabel}">${headerLabel} (Default)</td></tr>`);
}

// -----

function updateHeaderCounter() {
    let count = $('.selected-header').length;
    $('#headerCounter').html(`${count}/${selectionUpperLimit}`);

    if (count >= selectionUpperLimit) {
        selectionUpperLimitSurpassed = true;
        $('#headerCounter').css('color', 'red');
    }
    else if (count <= selectionLowerLimit){
        selectionLowerLimitSurpassed = true;
        $('#headerCounter').css('color', 'red');
    }
    else {
        selectionLowerLimitSurpassed = false;
        selectionUpperLimitSurpassed = false;
        $('#headerCounter').css('color', 'black');
    }
}


function resetDefaultHeaders(formId) {
    callServer({
        method: 'GET',
        url: `/Form/ResetCustomHeadersView?formId=${formId}`,
        success: function (data) {
            $('#CustomFieldHeaders').html(data);
            updateHeaderCounter();
            $('#previewContainer').sortable({
                axis: "x",
            });
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function insertOrUpdateCustomHeaders(formId) {
    callServer({
        method: 'POST',
        data: { formId: formId, customHeaders: getSelectedCustomHeaderFields() },
        url: `/Form/InsertOrUpdateCustomHeaders`,
        success: function (data) {
            toastr.success("Headers updated successfully!");
            window.location.href = "#";
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getSelectedCustomHeaderFields() {
    let customHeaderFields = []

    $('.custom-header-preview-element').each(function (index) {
        customHeaderFields.push(
            {
                FieldId: $(this).attr("data-id"),
                Label: $(this).attr("data-label"),
                CustomLabel: $(this).attr("data-customlabel"),
                Order: index,
                DefaultHeaderCode: $(this).attr("data-defaultheadercode"),
            });
    });
    return customHeaderFields;
}

function triggerSearchInput() {
    $('#CustomHeadersSearchInput').trigger('keyup');
}

// -----

function getSelectedHeaderLabels() {

    let selectedHeaderLabels = [];

    $('.custom-header-preview-element').each(function (index) {
        let customLabel = $(this).attr("data-customlabel");

        if (typeof customLabel !== 'undefined' && customLabel != "") {
            selectedHeaderLabels.push(customLabel);
        }
        else {
            selectedHeaderLabels.push($(this).attr("data-label"));
        }
    });
    return selectedHeaderLabels;
}

function hasSelectedHeadersChanged() {
    let currentHeaderLabels = getSelectedHeaderLabels();

    if (lastSavedHeaderLabels.length !== currentHeaderLabels.length) {
        return true;
    }

    for (var i = 0; i < lastSavedHeaderLabels.length; i++) {
        if (lastSavedHeaderLabels[i] !== currentHeaderLabels[i]) {
            return true;
        }
    }

    return false;
}

function checkSelectedHeaderUnsavedChanges() {
    if ($('#CustomFieldHeaders').is(':hidden')) {
        if (hasSelectedHeadersChanged()) {
            toastr.warning('You have unsaved changes in Custom Header tab !');
            $('.unsaved-custom-headers-star').show();
        }
        else {
            $('.unsaved-custom-headers-star').hide();
        }
    }
}