$(document).on('change', '#type', function (e) {
    let selectedFieldType = $(this).val();
    if (selectedFieldType) {
        callServer({
            method: "post",
            url: '/Form/GetFieldInfoCustomForm',
            data: getDataForAjax(),
            contentType: 'application/json',
            success: function (data) {
                if (specificTypeFields.includes(selectedFieldType)) {
                    hideCommonElements();
                }
                else {
                    if (selectedFieldType == 'audio')
                        hideSomeCommonElements();
                    else {
                        showCommonElements();
                        showOrHideRepetitiveFields(stringFields.includes(selectedFieldType));
                    }
                }
                $('#customFields').html(data);
                unselectMissingValue();
                initializeValidator();
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        })
    }

});

function unselectMissingValue() {
    $('#nonObligatory').click();
    $('#isObligatory', '#saveWithValue, #allowSaveWithoutValue', '#saveWithValue').prop('checked', false);
    $('#additionalOptions').hide();
    $('.additionalCheckboxOptions').hide();
    $('.additional-checkbox').each(function () {
        $(this).prop('checked', false);
    });
}

$(document).on('click', '#submit-field-info', function (e) {
    if ($('#fieldGeneralInfoForm').valid()) {
        createNewThesaurusIfNotSelected();

        let label;
        let type = $('#type').find(":selected").val();
        let elementId = getOpenedElementId();
        let element = getElement('field', label);

        if (specificTypeFields.includes(type)) {
            label = document.getElementById(type).value;
            if (type === "paragraph" && label.trim() === "")
                label = "Paragraph";
        } else {
            label = $('#label').val();
        }

        if (element) {
            updateTreeIfFieldTypeIsChanged(element);
            updateElementAttributes(element, elementId, type);

            var layoutType = $('#layoutType').val();

            setHelp(element);
            setCustomFieldsByType(element, type);
            updateTreeItemTitle(element, label);

            if (layoutType == 'Matrix') {
                handleMatrixLayout(element, type);
            }

            closDesignerFormModal(true);
            clearErrorFromElement($(element).attr('data-id'));
        }
    } else {
        toastr.error("Field informations are not valid");
    }
});

function updateElementAttributes(element, elementId, type) {
    $(element).attr('data-id', encodeURIComponent(elementId));
    $(element).attr('data-label', encodeURIComponent($('#label').val()));
    $(element).attr('data-thesaurusid', encodeURIComponent($('#thesaurusId').val()));
    $(element).attr('data-description', encodeURIComponent($('#description').val()));
    $(element).attr('data-isbold', encodeURIComponent($('#isBold').is(":checked")));
    $(element).attr('data-isrequired', encodeURIComponent($('#isRequired').is(":checked")));
    $(element).attr('data-isreadonly', encodeURIComponent($('#isReadonly').is(":checked")));
    $(element).attr('data-isvisible', encodeURIComponent($('#isVisible').is(":checked")));
    $(element).attr('data-ishiddenonpdf', encodeURIComponent($('#isHiddenOnPdf').is(":checked")));
    $(element).attr('data-unit', encodeURIComponent($('#unit').val()));
    $(element).attr('data-type', encodeURIComponent(type));
    $(element).attr('data-allowsavewithoutValue', $('#isRequired').is(":checked") && ($('#allowSaveWithoutValue').is(":checked") || $('#saveWithValue').is(":checked"))
        ? encodeURIComponent($('#allowSaveWithoutValue').is(":checked")) : null);
    $(element).attr('data-nullflavors', encodeURIComponent("[" + getCheckedNullFlavor() + "]"));
    $(element).attr('data-formid', $("li[data-itemtype='form']:first").attr('data-id'));
    $(element).attr('data-dependenton', encodeURIComponent(JSON.stringify(getDependentOnData($("#dependentFormula").val()))));
    $(element).attr('data-parentid', encodeURIComponent(parentId));
    $(element).attr('data-maxitems', parseInt($('#maxItems').val(), 10));
}

function handleMatrixLayout(element, type) {
    removeAddNewFieldOption(parentId);
    var isUpdateField = $('#isUpdateField').val() == 'true';

    if (!isUpdateField) {
        var options = $('#fieldSetOptions').val();
        var optionsList = JSON.parse(options);
        var formFieldValues = [];

        optionsList.forEach(function (option) {
            formFieldValues.push({
                valuetype: type,
                id: generateUUIDWithoutHyphens(),
                label: option.Label,
                value: option.Value
            });
        });

        var jsonSerialize = JSON.stringify(formFieldValues);
        $(element).attr('data-values', jsonSerialize);
        let formDefinition = getNestableFullFormDefinition($("#nestable").find(`li[data-itemtype='form']`).first());
        getNestableTree(formDefinition);
        getNestableFormElements();
    }
}

function removeAddNewFieldOption(fieldSetId) {
    let mainListItem = document.querySelector(`.dd-item[data-itemtype="fieldset"][data-id="${fieldSetId}"]`);
    let fieldItems = mainListItem.querySelectorAll('li[data-itemtype="field"]');
    let maxItems = parseInt($('#maxItems').val(), 10);

    if (maxItems != 0 && fieldItems.length > maxItems) {
        let addNewButton = mainListItem.querySelector(`li.add-item-button.add-chapter-button.dd-nodrag[data-parentid="${fieldSetId}"]`);
        if (addNewButton) {
            addNewButton.remove();
        }
    }
}

function getCheckedNullFlavor() {
    var nullFlavor = [];
    $('.additional-checkbox').each(function () {
        if ($(this).is(':checked')) {
            nullFlavor.push($(this).val());
        }
    });

    return nullFlavor;
}

function updateTreeIfFieldTypeIsChanged(element) {
    let oldType = decode($(element).attr('data-type'));
    let newType = $('#type').find(":selected").val();
    if (oldType != newType) {
        if (selectableFields.includes(oldType) && !selectableFields.includes(newType)) {
            let ol = $(element).find('ol').first();
            $(ol).remove();
        } else if (!selectableFields.includes(oldType) && selectableFields.includes(newType)) {
            $(element).append(createDragAndDropOrderedlist('field', $(element).attr('data-id')));
        }
    }
}

function getDataForAjax() {
    let result = {
        id: getOpenedElementId(),
        formId: $("li[data-itemtype='form']:first").attr('data-id'),
        label: $('#label').val(),
        thesaurusId: $('#thesaurusId').val(),
        description: $('#description').val(),
        isbold: $('#isBold').is(":checked"),
        isRequired: $('#isRequired').is(":checked"),
        isReadonly: $('#isReadonly').is(":checked"),
        isVisible: $('#isVisible').is(":checked"),
        isHiddenOnPdf: $('#isHiddenOnPdf').is(":checked"),
        allowSaveWithoutValue : $('#isRequired').is(":checked") && ($('#allowSaveWithoutValue').is(":checked") || $('#saveWithValue').is(":checked"))
            ? encodeURIComponent($('#allowSaveWithoutValue').is(":checked")) : null,
        unit: $('#unit').val(),
        type: $('#type').find(":selected").val(),
        help: {
            content: CKEDITOR.instances.helpContent.getData(),
            title: $('#helpTitle').val()
        }
    };
    return result;
}

function setHelp(element) {
    $(element).attr('data-help', encodeURIComponent(JSON.stringify(getHelp(element))));
}

function getHelp(element) {
    let helpContent = CKEDITOR.instances.helpContent.getData();
    let helpTitle = $('#helpTitle').val();
    let help = null;
    if (helpTitle || helpContent) {
        help = getDataProperty(element, 'help') || {};
        help['content'] = helpContent;
        help['title'] = helpTitle;
    }
    return help;
}

function setCustomFieldsByType(element, type) {
    switch (type) {
        case 'calculative':
            setCustomCalculativeFields(element);
            break;
        case 'custom-button':
            setCustomCustomFieldButtonFields(element);
            break;
        case 'date':
            setCustomDateFields(element);
            break;
        case 'datetime':
            setCustomDatetimeFields(element);
            break;
        case 'email':
            setCommonStringFields(element);
            break;
        case 'file':
            setCommonStringFields(element);
            break;
        case 'long-text':
            setCustomLongTextFields(element);
            break;
        case 'number':
            setCustomNumberFields(element);
            break;
        case 'radio':
        case 'select':
        case 'checkbox':
            break;
        case 'regex':
            setCustomRegexFields(element);
            break;
        case 'text':
            setCustomTextFields(element);
            break;
        case 'coded':
            setCustomCodedFields(element);
            break;
        case 'paragraph':
            setCustomParagraphFields(element);
            break;
        case 'link':
            setCustomLinkFields(element);
            break;
        case 'audio':
            setCustomAudioFields(element);
            break;
    }
}

function setCommonStringFields(element) {
    if (element) {
        $(element).attr('data-isrepetitive', encodeURIComponent($('#isRepetitive').find(":selected").val()));
        $(element).attr('data-numberofrepetitions', encodeURIComponent($('#numberOfRepetitions').val() || 0));
    }
}

function showOrHideRepetitiveFields(flag) {
    if (flag) {
        $('.repetitive-field-group').show();
    } else {
        $('.repetitive-field-group').hide();
    }
}

function initializeValidator() {
    $('#fieldGeneralInfoForm').validate().destroy();

    var validationRules = {
        formula: {
            allVariablesAssignedToField: true,
            duplicateVariableAssignment: true,
            datetimeCalculationFormula: true
        },
        link: {
            required: true,
            isLinkAddress: true
        },
        calculationGranularityType: {
            granularityType: true
        },
        dependentFormula: {
            formulaValidation: []
        },
        isRepetitive: {
            validateIsRepetitive: []
        }
    };

    if (!$("#nonObligatory").is(":checked")) {
        validationRules.fieldCheckBoxGroup = {
            required: true,
            isAnyFieldNullFlavorChecked: true
        };
    }
    else {
        isAnyFieldNullFlavorChecked = true;
    }

    $('#fieldGeneralInfoForm').validate({
        rules: validationRules,
        messages: {
            fieldCheckBoxGroup: "Please select at least one missing value code!"
        },
        errorPlacement: function (error, element) {
            switch (element.attr("name")) {
                case "fieldCheckBoxGroup":
                    error.appendTo("#fieldCheckBoxGroup-error");
                    break;
                case "dependentFormula":
                    $('#formulaValidationErrors').html(error);
                    break;
                default:
                    error.insertAfter(element);
                    break;
            }
        },
        ignore: []
    });
}

function hideCommonElements() {
    toggleElement("commonFields", true);
    toggleElement("appendixNotes", true);
    toggleElement("thesaurusArea", true);
    toggleIsElementRequired("label", true);
    toggleIsElementRequired("numberOfRepetitions", true);
}

function hideSomeCommonElements() {
    showCommonElements();
    toggleElement("checkboxGroup", true);
    toggleElement("repetitiveDiv", true);
    toggleElement("numberOfRepetitionDiv", true);
    toggleElement("unitDiv", true);
}

function showCommonElements() {
    toggleElement("checkboxGroup", false);
    toggleElement("repetitiveDiv", false);
    toggleElement("numberOfRepetitionDiv", false);
    toggleElement("unitDiv", false);
    toggleElement("commonFields", false);
    toggleElement("appendixNotes", false);
    toggleElement("thesaurusArea", false);
    toggleIsElementRequired("label", false);
    toggleIsElementRequired("numberOfRepetitions", false);
}

function toggleElement(elementId, shouldHideElement) {
    var element = document.getElementById(elementId);
    if (element) {
        if (shouldHideElement) {
            element.setAttribute("hidden", true);
        } else {
            element.removeAttribute("hidden");
        }
    }
}

function toggleIsElementRequired(elementId, shouldRemoveRequiredAttribute) {
    var element = document.getElementById(elementId);
    if (element) {
        if (shouldRemoveRequiredAttribute) {
            element.removeAttribute("required");
        } else {
            element.setAttribute("required", true);
        }
    }
}

function validateLink() {
    $.validator.addMethod("isLinkAddress", function (value, element) {
        var urlPattern = /^(https?:\/\/)?([\w.]+)\.([a-z]{2,6}\.?)(\/[\w.]*)*\/?$/i;
        return this.optional(element) || urlPattern.test(value);
    }, "Please enter a valid link address.");
}

function validateIfAnyFieldNullFlavorChecked() {
    $.validator.addMethod("isAnyFieldNullFlavorChecked", function (value, element) {
        return $('input[name="fieldCheckBoxGroup"]:checked').length > 0;
    }, "Please select at least one missing value code!");
}

function validateIfHasError() {
    let fieldId = getOpenedElementId();
    if ($("#nestable").find(`[data-id='${fieldId}']`).children('.dd-handle').hasClass('nestable-error')) {
        $('#fieldGeneralInfoForm').valid();
    }
}

function fieldInfoTabClicked($clickedTab) {
    $('.field-info-tab').removeClass('active');
    $clickedTab.addClass('active');
    $('.field-info-cont').hide();
    let containerId = $clickedTab.attr("data-id");
    $(`#${containerId}`).show();
}