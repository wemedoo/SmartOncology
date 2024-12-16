function addNewCell(cellName, cellObject, isFirstRow = false) {
    let cellClass = isFirstRow ? "custom-td-first" : "custom-td";

    let cellValue = cellObject["value"];
    let cellDisplayValue = cellObject["display"];

    let el = document.createElement('td');
    $(el).attr("data-property", cellName);
    $(el).attr("data-value", cellValue);
    $(el).addClass(cellClass);
    $(el).text(displayCellValueOrNe(cellDisplayValue));
    $(el).attr("title", cellDisplayValue);
    $(el).tooltip();

    return el;
}

function createActionsCell(entityName, additionalTdClass = '') {
    let div = document.createElement('td');
    $(div).addClass(`custom-td-last position-relative ${additionalTdClass}`);

    let removeEntityIcon = document.createElement('i');
    $(removeEntityIcon).addClass(`remove-table-row-icon remove-${entityName}`);

    let editEntityIcon = document.createElement('img');
    $(editEntityIcon)
        .addClass(`${entityName}-entry`)
        .attr("src", "/css/img/icons/editing.svg");

    $(div)
        .append(editEntityIcon)
        .append(removeEntityIcon)
        ;
    return div;
}

function displayCellValueOrNe(cellValue) {
    return cellValue ? cellValue : getSpecialValueString();
}

function modifyTableBorder(activeTableContainerId, tableEntryClassSelector) {
    if (hasNoRow(activeTableContainerId, tableEntryClassSelector)) {
        $(`#${activeTableContainerId}`).addClass("identifier-line-bottom");
    } else {
        $(`#${activeTableContainerId}`).removeClass("identifier-line-bottom");
    }
}

function hasNoRow(activeTableContainerId, tableEntry) {
    return $(`#${activeTableContainerId} tbody`).children(tableEntry).length == 0;
}

function resetValidation($form) {
    $form.find("input.error, select.error").each(function () {
        $(this).removeClass("error");
        let inputId = $(this).attr("id");
        $(`#${inputId}-error`).remove();
    });
}

function executeEventFunctions(event, shouldPreventDefault) {
    if (shouldPreventDefault) {
        event.preventDefault();
    }
    event.stopPropagation();
    event.stopImmediatePropagation();
}
function removeEncodedPlusForWhitespace(value) {
    return value ? value.replace(/\+/g, ' ') : '';
}

$.fn.hasScrollBar = function () {
    let plainHtml = this.get(0);
    return plainHtml.scrollHeight > plainHtml.clientHeight;
}

function getPosition(el) {
    var position = $(el).offset();
    return {
        left: position.left,
        top: position.top - window.scrollY
    };
}

function getWidth(el) {
    return $(el).width();
}

function getHeight(el) {
    return $(el).height();
}

function needToScrollToTheRight(elLeftPosition, threshold) {
    return $(document).width() - elLeftPosition < threshold;
}

function getOverflowDifference(elLeftPosition) {
    return Math.abs($(document).width() - elLeftPosition);
}

function getSpecialValueString() {
    return "N/E";
}

function getSelectedOptionLabel(inputId) {
    return $(`#${inputId}`).val() ? $(`#${inputId} option:selected`).text().trim() : '';
}

function addInactiveOption(selectElement, id, term) {
    let option = `<option value="${id}" class="option-disabled" disabled selected>
                    ${term}
                  </option>`;
    selectElement.append(option);
}

function updateDisabledOptions(disabled) {
    $(".option-disabled").prop("disabled", disabled);
}

function removeDisabledOption(inputId) {
    $(`#${inputId} option.option-disabled`).remove();
}

function getActiveContainer(el, entityName) {
    return $(el).closest(`.${entityName}-container-wrapper`).attr("id")
}

function parentEntryExisting(parentId) {
    return parentId != '0';
}

function handleModalAfterSubmitting(tableContainerName, tableRowClassName, modalId, callback) {
    modifyTableBorder(tableContainerName, `.${tableRowClassName}`);
    updateDisabledOptions(true);
    $(`#${modalId}`).modal('hide');
    if (callback) {
        callback();
    }
}