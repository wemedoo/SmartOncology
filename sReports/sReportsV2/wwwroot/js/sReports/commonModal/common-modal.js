function addNewCell(cellName, cellObject, isFirstRow = false) {
    let cellClass = isFirstRow ? "custom-td-first" : "custom-td";

    let cellValue = cellObject["value"];
    let cellDisplayValue = cellObject["display"];
    let cellDisplayValueFormatted = displayCellValueOrNe(cellDisplayValue);

    let el = document.createElement('td');
    $(el).attr("data-property", cellName);
    $(el).attr("data-value", cellValue);
    $(el).addClass(cellClass);
    $(el).attr("title", cellDisplayValueFormatted);
    $(el).text(cellDisplayValueFormatted);
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
    return $(`#${inputId}`).val() ? readSelectedOptionLabel($(`#${inputId} option:selected`)) : '';
}

function readSelectedOptionLabel($input) {
    return $input.text().trim();
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

function destroyChartIfExist(chartId) {
    let existingChart = Chart.getChart(chartId);
    if (existingChart) {
        existingChart.destroy();
    }
  
    if (myChart) {
        myChart.destroy();
        myChart = null;
    }
}

$(document).on('click', 'a.pagination-item.disabled', function (e) {
    e.preventDefault();
});

function preventMultipleSubmit(buttonId) {
    $(`#${buttonId}`).prop("disabled", true);
}

function reAllowSubmit(buttonId) {
    $(`#${buttonId}`).prop("disabled", false);
}

function executeCallback(callback) {
    if (callback) {
        callback();
    }
}

function enableChangeTab(isEdit) {
    if (!isEdit) {
        $('.tab-disabled')
            .removeAttr('data-toggle')
            .removeAttr('data-original-title')
            .removeClass("tab-disabled");
    }
}

function arraysAreEqual(a, b) {
    if (a.length !== b.length) return false;
    return a.every((val, index) => {
        const bVal = b[index];

        if (typeof val === 'object' && val !== null &&
            typeof bVal === 'object' && bVal !== null) {
            return objectsAreEqual(val, bVal);
        }

        return val === bVal;
    });
}

function objectsAreEqual(obj1, obj2) {
    const keys1 = Object.keys(obj1);
    const keys2 = Object.keys(obj2);

    if (keys1.length !== keys2.length) return false;

    return keys1.every(key => obj1[key] === obj2[key]);
}

function logInfo(text) {
    //console.debug(text);
}

function logError(text) {
    console.error(text);
}