function initDependencyComponent() {
    $(getVariablesSelector()).each(function () {
        getVariableTitle($(this));
    });

    $('#nestableDependencyVariable').nestable({
        group: 1,
        maxDepth: 5
    });
    $('#nestableDependencyVariable').nestable('collapseAll');
}

function getDependentOnData(formula) {
    let dependentOn = {
        formula: formula,
        dependentOnFieldInfos: []
    };

    $(getVariablesSelector()).each(function () {
        let dependentOnFieldInfo = {
            fieldId: $(this).attr('data-field-id'),
            fieldValueId: $(this).attr('data-field-value-id'),
            fieldType: $(this).attr('data-field-type'),
            variable: $(this)
                .find('input')
                .val(),
        }
        dependentOn.dependentOnFieldInfos.push(dependentOnFieldInfo);
    });

    return hasDependentOnData(dependentOn) ? dependentOn : null;
}

function getVariablesSelector() {
    return '#variables-body .field-variable-item';
}

function hasDependentOnData(dependentOn) {
    return dependentOn.formula || dependentOn.dependentOnFieldInfos.length > 0;
}

$(document).on('click', '.btn-add-variable', function () {
    removeHighlight();
    let depedencyVariable = {};
    let $btnAddVariable = $(this);
    $btnAddVariable.closest('.dd-item[data-itemtype]').each(function () {
        depedencyVariable['fieldId'] = $(this).attr('data-field-id');
        depedencyVariable['fieldValueId'] = $(this).attr('data-fieldvalue-id');
        depedencyVariable['fieldType'] = $(this).attr('data-field-type');
    });

    callServer({
        url: '/Form/AddDependentOnField',
        data: depedencyVariable,
        success: function (data) {
            let $newVariable = $(data);
            getVariableTitle($newVariable);
            $('#variables-body').append($newVariable);
            $(".dependent-on-fields-list").removeClass("d-none");

            $btnAddVariable.siblings('.btn-added-variable').removeClass('d-none');
            $btnAddVariable.addClass("d-none");
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
});

function getVariableTitle($variableRow) {
    let fieldId = $variableRow.attr('data-field-id');
    let fieldValueId = $variableRow.attr('data-field-value-id');
    let $fieldCell = $variableRow.find('.field-cell');
    let $treeItem = getLastTreeItemNode(fieldId, fieldValueId);
    let title = decodeToJsonOrText($treeItem.attr('data-title'));
    
    $fieldCell
        .attr('title', title)
        .text(title)
        ;
}

function getLastTreeItemNode(fieldId, fieldValueId) {
    let $fieldTreeItem = $('#nestableDependencyVariable').find(`li[data-itemtype=field][data-id="${fieldId}"]`);
    if (fieldValueId) {
        let $fieldValueTreeItem = $fieldTreeItem.find(`li[data-itemtype=fieldvalue][data-id=${fieldValueId}]`);
        return $fieldValueTreeItem;
    } else {
        return $fieldTreeItem;
    }
}

$(document).on('click', '.remove-variable-field', function (e) {
    removeHighlight();
    let $variableRow = $(e.currentTarget).closest('.field-variable-item');
    let fieldId = $variableRow.attr('data-field-id');
    let fieldValueId = $variableRow.attr('data-field-value-id');
    let $item = getLastTreeItemNode(fieldId, fieldValueId);
    let $btnAddVariable = $item.find('.btn-add-variable').removeClass("d-none");
    $btnAddVariable.siblings('.btn-added-variable').addClass('d-none');
    $variableRow.remove();
    resetTreeToDefault();
});

$(document).on('click', '.display-variable-field', function (e) {
    removeHighlight();
    let $variableRow = $(e.currentTarget).closest('.field-variable-item');
    let fieldId = $variableRow.attr('data-field-id');
    let fieldValueId = $variableRow.attr('data-field-value-id');
    let $treeItem = getLastTreeItemNode(fieldId, fieldValueId);

    resetTreeToDefault();
    displayVariableTreeItem($treeItem, true);

    $treeItem.parents('.dd-item[data-itemtype]').each(function () {
        displayVariableTreeItem($(this), false);
    });
});

function resetTreeToDefault() {
    $('#nestableDependencyVariable').nestable('collapseAll');
    $('#nestableDependencyVariable')
        .find(".dd-item[data-itemtype]")
        .removeClass('d-none');
}

function displayVariableTreeItem($treeItem, highlight) {

    $treeItem.expandItem('nestableDependencyVariable');
    let $elementsAtTheSameLevel = $treeItem
        .siblings('.dd-item[data-itemtype]')
        ;
    $elementsAtTheSameLevel.addClass("d-none");

    if (highlight) {
        $treeItem.children('.dd-handle').addClass('highlighted');
    }
}

function removeHighlight() {
    $('.dd-handle.highlighted').removeClass('highlighted');
}

$(document).ready(function () {
    addDependencyConfigurationValidationMethods();
});

function addDependencyConfigurationValidationMethods() {
    jQuery.validator.addMethod("validateIsRepetitive", function (value, element, params) {
        let valid = true;
        if (value == 'true') {
            let itemId = getOpenedElementId();
            let itemType = isFieldForm(element) ? 'field' : 'fieldset';
            let doesOccurredInDependency = getDoesOccurredInDependencyHandler(itemType);
            valid = !doesOccurredInDependency(itemId, itemType, params, 'repetitive');
        }
        return valid;
    }, jQuery.validator.format('{0}'));

    jQuery.validator.addMethod("formulaValidation", function (value, element, params) {
        let validationMessage = validateFormula(getDependentOnData(value.trim()));
        let valid = true;
        if (validationMessage) {
            params[0] = validationMessage;
            valid = false;
        }
        return valid;
    }, jQuery.validator.format('{0}'));
}

function isFieldForm(element) {
    return $(element).closest('form').attr('id') == 'fieldGeneralInfoForm';
}

function doesFieldValueOccurredInDependency(fieldValueId, itemType, params, action) {
    let occurrence = false;
    $('#nestable [data-dependenton]').each(function () {
        if (findParentFieldOccurrence($(this), params, f => f.fieldValueId == fieldValueId, itemType, action)) {
            occurrence = true;
            return false;
        }
    });
    return occurrence;
}

function doesFieldOccurredInDependency(fieldId, itemType, params, action) {
    let occurrence = false;
    $('#nestable [data-dependenton]').each(function () {
        if (findParentFieldOccurrence($(this), params, f => f.fieldId == fieldId, itemType, action)) {
            occurrence = true;
            return false;
        }
    });
    return occurrence;
}

function doesFieldSetOccurredInDependency(fieldSetId, itemType, params, action) {
    let occurrence = false;
    let fieldsIdsWithinFieldSet = $(`#nestable [data-itemtype="fieldset"][data-id="${fieldSetId}"] [data-itemtype="field"]`).map(function () {
        return $(this).attr('data-id');
    }).get();
    $(`#nestable [data-itemtype="fieldset"][data-id!="${fieldSetId}"] [data-dependenton]`).each(function () {
        if (findParentFieldOccurrence($(this), params, f => fieldsIdsWithinFieldSet.some(fieldIdFromCurrentFs => fieldIdFromCurrentFs == f.fieldId), itemType, action)) {
            occurrence = true;
            return false;
        }
    });
    return occurrence;
}

function findParentFieldOccurrence($fieldElement, params, query, itemType, action) {
    let findMatch = false;
    let dependentOn = JSON.parse(decodeURIComponent($fieldElement.attr('data-dependenton')));
    if (dependentOn) {
        let fieldIncludesInDepedency = dependentOn.dependentOnFieldInfos.some(query);
        if (fieldIncludesInDepedency) {
            let childFieldTitle = decode($fieldElement.attr('data-label'));
            params[0] = getDependencyValidationErrorMessage(itemType, childFieldTitle, action);
            findMatch = true;
        }
    }
    return findMatch;
}

function examineDependencyOnDelete(itemType) {
    return ['fieldset', 'field', 'fieldvalue'].includes(itemType);
}

function getDoesOccurredInDependencyHandler(itemType) {
    return {
        'fieldset': doesFieldSetOccurredInDependency,
        'field': doesFieldOccurredInDependency,
        'fieldvalue': doesFieldValueOccurredInDependency
    }[itemType];
}

function getDependencyValidationErrorMessage(itemType, childFieldTitle, action) {
    return {
        'fieldset': `Field from this fieldset has been included in dependency field for '${childFieldTitle}'. It can't be ${action}`,
        'field': `This field has been included in dependency for field '${childFieldTitle}'. It can't be ${action}.`,
        'fieldvalue': `This field option has been included in dependency for field '${childFieldTitle}'. It can't be ${action}.`
    }[itemType];
}

function validateFormula(validationObject) {
    let validationErrorMessage = '';
    callServer({
        type: 'POST',
        url: '/Form/CheckFormula',
        async: false,
        data: validationObject,
        success: function (data) {

        },
        error: function (xhr, textStatus, thrownError) {
            if (xhr.status == 500) {
                handleResponseError(xhr);
                validationErrorMessage = 'Unexpected validation error';
            } else {
                validationErrorMessage = getErrorMessage(xhr);
            }
        }
    });
    
    return validationErrorMessage;
}

$(document).on('keypress', '.search-field-variable-input', function (event) {
    if (event.keyCode === enter) {
        event.preventDefault();
        event.stopPropagation();
        searchFieldByName($(this));
    }
});

$(document).on('click', '.search-field-variable-button', function (event) {
    searchFieldByName($('.search-field-variable-input'));
});

function searchFieldByName($input) {
    let fieldName = $input.val().toLowerCase();
    let filteredFieldIds = [];
    if (fieldName) {
        filteredFieldIds = possibleParentFields.filter(f => f.title.toLowerCase().startsWith(fieldName)).map(x => x.id);
    }

    resetTreeToDefault();
    if (filteredFieldIds.length > 0) {
        $(`#nestableDependencyVariable li[data-itemtype]`).addClass("d-none");

        let filteredFields = $(getFieldTreeSelector()).filter(function () {
            return filteredFieldIds.indexOf($(this).attr('data-id')) !== -1;
        });

        filteredFields.each(function (ind, filteredField) {
            displayTreeItem($(filteredField));
            $(filteredField).find("li[data-itemtype='fieldvalue']").each(function (ind1, filteredFieldValue) {
                displayTreeItem($(filteredFieldValue));
            });
            $(filteredField).parents("li[data-itemtype]").each(function (ind2, filteredFieldParent) {
                displayTreeItem($(filteredFieldParent));
            });
        });
    }
}

function displayTreeItem($item) {
    $item.removeClass('d-none');
    $item.expandItem('nestableDependencyVariable');
}

function getPossibleParentFields() {
    let possibleParentFields = [];
    $(getFieldTreeSelector()).each(function () {
        possibleParentFields.push({
            title: decodeToJsonOrText($(this).attr('data-title')),
            id: $(this).attr('data-id')
        });
    });

    return possibleParentFields;
}

function getFieldTreeSelector() {
    return `#nestableDependencyVariable li[data-itemtype='field']`;
}