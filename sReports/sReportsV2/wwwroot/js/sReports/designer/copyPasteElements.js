
//#region ---------- Selection Logic ----------

var itemTypesToIndexMap = {
    "chapter": 1,
    "page": 2,
    "fieldset": 3,
    "field": 4,
}

var indexToItemTypesMap = {
    1: "chapter",
    2: "page",
    3: "fieldset",
    4: "field",
}

var selectedItems = [];
var selectedTypeIndex = 0;

function resetCopyPasteData() {
    selectedItems = [];
    selectedTypeIndex = 0;
    $(".selectable-item").removeClass('selected-item');
}

$(document).on("click", ".selectable-item", function (event) {
    if ($(event.target).data('action') === "collapse" || $(event.target).data('action') === "expand" || isCommentSectionDisplayed()) {
        return false;
    }

    let element = $(this).closest('.dd-item');
    let isSelected = $(this).hasClass('selected-item');
    let id = $(element).attr('data-id');
    let itemType = $(element).attr('data-itemtype');

    commonSelectionLogic(element, id, itemType, isSelected, event);
});

function commonSelectionLogic(element, id, itemType, isSelected, event) {

    let itemTypeIndex = itemTypesToIndexMap[itemType];

    if (isSelected) {
        removeSelectedItem(id, element);
    }
    else {
        if (selectedItems.length == 0) {
            selectedTypeIndex = itemTypeIndex;
            addSelectedItem(id, element);
        }
        else {
            if (validationRule(element, id, itemTypeIndex)) {
                addSelectedItem(id, element);
            }
            else {
                if (!isElementContainedInSelected(element, id)) {
                    incompatibleTypeWarning();
                }
            }
        }
    }

    event.stopPropagation();
}

function addSelectedItem(id, element) {
    $(element).findAndSelf(".selectable-item").addClass('selected-item');
    selectedItems.push({ id: id, element: $(element).clone() });
}

function removeSelectedItem(id, element) {
    selectedItems = $.grep(selectedItems, function (e) {
        return e.id != id;
    });
    $(element).findAndSelf(".selectable-item").each(function () {
        $(this).removeClass('selected-item');
    });

    removeElementIfContainedInSelected(id);
}

function validationRule(element, id, itemTypeIndex) {

    if (selectedTypeIndex == itemTypeIndex) {
        if (itemTypeIndex == itemTypesToIndexMap["chapter"]) {
            return true;
        }
        else {
            return IsSiblingElement(id);
        }
    }
    else {
        return containsSelectedElements(element, itemTypeIndex);
    }
}

function isElementContainedInSelected(element, id) {
    let isElementContained = false;

    $(element).parents('.dd-item').each(function () {
        if ($(this).hasClass('selected-item')) {
            let parentId = $(this).attr('data-id');
            $(selectedItems).each(function () {
                if (this.id == parentId) {
                    $(element).findAndSelf(".selectable-item").addClass('selected-item');

                    $(this.element).find(`.dd-item[data-id=${id}]`).find(".selectable-item").addClass('selected-item');

                    isElementContained = true;
                    return true;
                }
            });
        }
        else {
            return false;
        }
    });

    return isElementContained;
}

function removeElementIfContainedInSelected(idToRemove) {
    $(selectedItems).each(function () {
        $(this.element).find(`.dd-item[data-id=${idToRemove}]`).removeClass('selected-item'); // If not marked as selected, it will not be pasted
    });
}

function IsSiblingElement(id) {
    let IsSiblingElement = false;
    $(selectedItems).each(function () {
        if ($(`.selectable-item[data-id=${this.id}]`).closest('.dd-list').is($(`.selectable-item[data-id=${id}]`).closest('.dd-list'))) {
            IsSiblingElement = true;
            return true;
        }
    });
    return IsSiblingElement;
}

function containsSelectedElements(element, itemTypeIndex) {
    let result = false;
    $(selectedItems).each(function () {

        if ($(element).find(`.dd-item[data-id=${this.id}]`).length > 0) {
            result = true;
            removeSelectedItem(this.id, this.element);
        }
    });

    if (result) {
        selectedTypeIndex = itemTypeIndex;
    }
    return result;
}

//#endregion

//#region ---------- Copy Actions ----------

$(document).on("click", "#copy-designer-items", copySelectedElements);

function copySelectedElements(event) {

    if (selectedItems.length == 0) {
        noSelectedElementsWarning();
        return null;
    }

    $(selectedItems).each(function () { 
        this.element = removeUnselectedSubElements(this.element);
    })
    insertSelectedItemsInStorage();
    resetCopyPasteData();
    hidePasteButtonAndDestinations();
}

//#endregion

//#region ---------- Paste Actions ----------

$(document).on("click", "#paste-designer-items", enableDisablePaste);

function enableDisablePaste(event) {
    event.stopPropagation();

    if (!localStorage["copiedItems"] || !localStorage["copiedTypeIndex"] || JSON.parse(localStorage.getItem("copiedItems")).length == 0) {
        nothingWasCopiedWarning();
        return null;
    }

    if ($('#paste-designer-items').hasClass('pressed')) {
        hidePasteButtonAndDestinations();
    }
    else {
        $('#paste-designer-items').addClass('pressed');
        let itemType = indexToItemTypesMap[localStorage.getItem("copiedTypeIndex")];

        hidePastableDestinations();
        showPastableDestinations(itemType);
    }
}

function showPastableDestinations(itemType) {
    $(`#formPreviewContainer`).find(`.dd-item[data-itemtype="${itemType}"]:not(.dd-item-placeholder)`).each(function (index) {
        createPastableDestination(this, itemType);
    });

    $(`#formPreviewContainer .dd-list:has(> .dd-item-placeholder[data-itemtype='${itemType}']:only-child)`).each(function () {
        $(this).find('.dd-item-placeholder').first().addClass('temporary-hidden');
        createPastableDestination(this, itemType, true);
    })


    $(".after-pastable-destination").next(".before-pastable-destination").remove();
}

function createPastableDestination(element, itemType, emptyParent=false) {
    let targetId = '';
    let elementId = emptyParent ? "first" : $(element).attr('data-id');
    switch (itemType) {
        case "chapter":
            targetId = elementId;
            break;
        case "page":
            targetId = $(element).closest('.dd-item[data-itemtype="chapter"]').attr('data-id') + '_'
                + elementId;
            break;
        case "fieldset":
            targetId = $(element).closest('.dd-item[data-itemtype="chapter"]').attr('data-id') + '_'
                + $(element).closest('.dd-item[data-itemtype="page"]').attr('data-id') + '_'
                + elementId;
            break;
        case "field":
            targetId = $(element).closest('.dd-item[data-itemtype="chapter"]').attr('data-id') + '_'
                + $(element).closest('.dd-item[data-itemtype="page"]').attr('data-id') + '_'
                + $(element).closest('.dd-item[data-itemtype="fieldset"]').attr('data-id') + '_'
                + elementId;
            break;
    }
    if (!emptyParent) {
        $(element).before(buildPastePlaceholderElement(itemType, targetId, true));
        $(element).after(buildPastePlaceholderElement(itemType, targetId));
    }
    else {
        $(element).prepend(buildPastePlaceholderElement(itemType, targetId, true));
    }
}

$(document).on("click", ".after-pastable-destination", function (event) {
    pasteSelectedElements(event, "after");
});

$(document).on("click", ".before-pastable-destination", function (event) {
    pasteSelectedElements(event, "before");
});

function pasteSelectedElements(event, insertMethod) {
    event.stopPropagation();

    let formTreePositionTargetId = $(event.target).attr('data-targetid');
    let pasteAfterDestination = insertMethod === "after";
    let formId = $("#nestable").find(`li[data-itemtype='form']`).first().attr('data-id');
    formId = formId == 'formIdPlaceHolder' ? '' : formId;

    let { getElementWithData, action } = selectElementWithDataFuncAndAction();

    const itemsToAppend = JSON.parse(localStorage.getItem("copiedItems"));
    let elementsToSubmit = [];
    $(itemsToAppend).each(function () {
        let elementToSubmit = getElementWithData($(this.element));
        elementsToSubmit.push(elementToSubmit);
    });

    if (formTreePositionTargetId) {
        hidePasteButtonAndDestinations();
        pleaseWaitInfo();

        callServer({
            method: 'POST',
            data: elementsToSubmit,
            url: `/Form/${action}?destinationFormId=${formId}&destinationElementId=${formTreePositionTargetId}&afterDestination=${pasteAfterDestination}`,
            contentType: 'application/json',
            success: function (data) {
                updateFormWithLastUpdate(data);
                reloadFormTreeAfterPaste("#formTreeNestable", "ReloadFormTreeNestable", formId);
                elementsPastedSuccess();
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    else {
        noTargetIdFoundError();
        hidePasteButtonAndDestinations();
    }
}

function selectElementWithDataFuncAndAction() {
    let action = '';
    let getElementWithData = () => { }; 

    let storedIndex = localStorage.getItem("copiedTypeIndex");
    let storedType = indexToItemTypesMap[storedIndex];
    switch (storedType) {
        case "chapter":
            getElementWithData = getChapterWithData;
            action = 'PasteChapters';
            break;
        case "page":
            getElementWithData = getPageWithData;
            action = 'PastePages';
            break;
        case "fieldset":
            getElementWithData = getFieldSetWithData;
            action = 'PasteFieldSets';
            break;
        case "field":
            getElementWithData = getFieldWithData;
            action = 'PasteFields';
            break;
    }
    return {
        getElementWithData: getElementWithData,
        action: action
    };
}

function reloadFormTreeAfterPaste(destinationContainerId, action, formId) {
    callServer({
        method: 'GET',
        url: `/Form/${action}?formId=${formId}`,
        success: function (data) {
            $(destinationContainerId).html(data);
            reloadFormPreviewAfterPaste("#formPreviewContainer", "ReloadFormPreviewContainer", formId);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
            location.reload();
        }
    });
}

function reloadFormPreviewAfterPaste(destinationContainerId, action, formId) {
    callServer({
        method: 'GET',
        url: `/Form/${action}?formId=${formId}`,
        success: function (data) {
            $(destinationContainerId).html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
            location.reload();
        }
    });
}

//#endregion

//#region ---------- Helpers ----------

function insertSelectedItemsInStorage() {
    let itemsHtml = [];
    $(selectedItems).each(function () {
        let html = $(this.element).get(0).outerHTML;
        itemsHtml.push({ element: html });
    });

    localStorage.setItem("copiedItems", JSON.stringify(itemsHtml));
    localStorage.setItem("copiedTypeIndex", selectedTypeIndex);
    elementsCopiedSuccess();
}

function removeUnselectedSubElements(element) {

    let modifiedElement = $(element);
    $(modifiedElement).find('.selectable-item').each(function () {
        if (!$(this).hasClass('selected-item')) {
            $(this).closest('.dd-item').remove();
        }
    })
    return modifiedElement;
}

function hidePasteButtonAndDestinations() {
    $('#paste-designer-items').removeClass('pressed');
    hidePastableDestinations();
}

function hidePastableDestinations() {
    $('.dd-item-placeholder').removeClass('temporary-hidden');
    $('.after-pastable-destination').remove();
    $('.before-pastable-destination').remove();
}

/**
 * Includes in the result the current element matching the selector (find() takes only descendants)
 * @param {any} selector
 */
jQuery.fn.findAndSelf = function (selector) {
    return this.find(selector).addBack(selector);
};

$("#nestableFormElements").on("mouseup", function () {  // hide pastable destination when user Drag/Drops PlaceHolder Items
    hidePasteButtonAndDestinations();
});

// Hides Pastable Destionations when Tab is not visible
$(document).on("visibilitychange", function () {
    if (document.hidden) {  // Browser tab is hidden
        if ($('#paste-designer-items').hasClass('pressed')) { 
            hidePasteButtonAndDestinations();
        }
    } 
});

function storageAvailable(type) {
    let storage;
    try {
        storage = window[type];
        const x = "__storage_test__";
        storage.setItem(x, x);
        storage.removeItem(x);
        return true;
    } catch (e) {
        return (
            e instanceof DOMException &&
            // everything except Firefox
            (e.code === 22 ||
                // Firefox
                e.code === 1014 ||
                // test name field too, because code might not be present
                // everything except Firefox
                e.name === "QuotaExceededError" ||
                // Firefox
                e.name === "NS_ERROR_DOM_QUOTA_REACHED") &&
            // acknowledge QuotaExceededError only if there's something already stored
            storage &&
            storage.length !== 0
        );
    }
}

$(document).ready(function () {
    let formId = $("#nestable").find(`li[data-itemtype='form']`).first().attr('data-id');

    if (storageAvailable("localStorage") && formId !== "formIdPlaceHolder") {
        console.log("localStorage available And Form Ok");
    } else {
        console.log("localStorage not available OR New Form");
        $("#copy-designer-items").remove(); // removing Copy button
        $("#paste-designer-items").remove(); // removing Paste button
        commonSelectionLogic = function () { return false; };
    }
});

//#endregion
