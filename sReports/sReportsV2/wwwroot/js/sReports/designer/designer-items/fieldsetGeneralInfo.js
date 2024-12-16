$(document).on('click', '#submit-fieldset-info', function (e) {
    if ($('#fieldsetGeneralInfoForm').valid()) {
        createNewThesaurusIfNotSelected();

        let label = $('#label').val();
        let elementId = getOpenedElementId();
        let element = getElement('fieldset', label);
        if (element) {
            $(element).attr('data-id', encodeURIComponent(elementId));
            $(element).attr('data-label', encodeURIComponent(label));
            $(element).attr('data-thesaurusid', encodeURIComponent($('#thesaurusId').val()));
            $(element).attr('data-description', encodeURIComponent($('#description').val()));
            $(element).attr('data-isbold', encodeURIComponent($('#isBold').is(":checked")));
            $(element).attr('data-isrepetitive', encodeURIComponent($('#isRepetitive').val()));
            $(element).attr('data-numberofrepetitions', encodeURIComponent($('#numberOfRepetitions').val()));

            setFieldSetHelp(element);
            setFieldSetLayout(element);

            updateTreeItemTitle(element, label);
            clearErrorFromElement($(element).attr('data-id'));
        }
    }
    else {
        toastr.error("Field set informations are not valid");
    }
})

function setFieldSetLayout(element) {
    let layoutType = $('#layoutType').val();
    let layoutMaxItems = $('#layoutMaxItems').val();

    let layout = getLayoutData(element, layoutType, layoutMaxItems);

    if (layoutType === 'Matrix') {
        let options = collectOptions();
        updateFieldValues(element, options);
        $(element).attr('data-options', JSON.stringify(options));
    }

    saveLayoutToElement(element, layout);
    updateFormDefinition();
}

function getLayoutData(element, layoutType, layoutMaxItems) {
    let layout = getDataProperty(element, 'layoutstyle') || {};
    layout['layoutType'] = layoutType;
    layout['maxItems'] = layoutType === 'Matrix' ? layoutMaxItems : null;
    return layout;
}

function collectOptions() {
    let options = [];
    $('#tableBody .optionLabel').each(function () {
        let id = $(this).data('id');
        let label = $(this).data('label');
        let value = $(this).data('value');

        let option = {
            Id: id,
            Label: label,
            Value: value
        };

        options.push(option);
    });
    return options;
}

function updateFieldValues(element, options) {
    $(element).find('li.dd-item[data-itemtype="field"]').each(function () {
        let formFieldValues = getExistingFieldValues($(this));
        let existingLabels = formFieldValues.map(item => item.Label);
        let childTitles = collectChildTitles($(this));
        let type = $(this).attr("data-type");

        formFieldValues = formFieldValues.filter(item => options.includes(item.Label));

        options.forEach(option => {
            if (!existingLabels.includes(option.Label) && !childTitles.includes(option.Label)) {
                formFieldValues.push(createFieldValue(option, type));
            }
        });

        saveFieldValues($(this), formFieldValues);
    });
    removeItemsNotInOptions(element, options);
}

function removeItemsNotInOptions(element, options) {
    $(element).find('li.dd-item[data-itemtype="fieldvalue"]').each(function () {
        let label = $(this).find('.dd-handle').text().trim();
        let labelExists = options.some(option => option.Label === label);
        if (!labelExists) {
            $(this).remove();
        }
    });
}

function getExistingFieldValues(fieldElement) {
    let fieldValueElements = fieldElement.find('[data-itemtype="fieldvalue"]');
    let values = [];

    fieldValueElements.each(function () {
        let existingData = {
            Label: $(this).attr('data-label'),
        };

        values.push(existingData);
    });

    return values;
}

function collectChildTitles(fieldElement) {
    return fieldElement.find('li.dd-item[data-itemtype="fieldvalue"] .dd-handle')
        .map(function () {
            return $(this).text().trim();
        }).get();
}

function createFieldValue(option, type) {
    return {
        valuetype: type,
        id: generateUUIDWithoutHyphens(),
        label: option.Label,
        value: option.Value
    };
}

function saveFieldValues(fieldElement, formFieldValues) {
    let jsonSerialize = JSON.stringify(formFieldValues);
    fieldElement.attr('data-values', jsonSerialize);
}

function saveLayoutToElement(element, layout) {
    $(element).attr('data-layoutstyle', encodeURIComponent(JSON.stringify(layout)));
}

function updateFormDefinition() {
    $('.designer-form-modal').removeClass('show');
    $('body').removeClass('no-scrollable');
    let formDefinition = getNestableFullFormDefinition($("#nestable").find(`li[data-itemtype='form']`).first());
    getNestableTree(formDefinition, true);
    getNestableFormElements();
}

function setFieldSetHelp(element) {
    let helpContent = CKEDITOR.instances.helpContent.getData();
    let helpTitle = $('#helpTitle').val();
    let help = null;
    if (helpTitle || helpContent) {
        help = getDataProperty(element, 'help') || {};
        help['content'] = helpContent;
        help['title'] = helpTitle;
    }

    $(element).attr('data-help', encodeURIComponent(JSON.stringify(help)));
}

$(document).on('mouseover', '.fieldset-custom-dd-handle', function (e) {
    $(e.target).closest('li').children('button').addClass('white');
});

$(document).on('mouseout', '.fieldset-custom-dd-handle', function (e) {
    $(e.target).closest('li').children('button').removeClass('white');
});

function initializeFieldSetValidator() {
    let numberOfElements = getNumberOfElements();

    $.validator.addMethod("minValue", function (value, element, min) {
        return value >= numberOfElements;
    }, $.validator.format("Cannot set a smaller value. This fieldset already has {0} fields."));

    var validationRules = {
        isRepetitive: {
            validateIsRepetitive: []
        },
        layoutMaxItems: {
            required: {
                depends: function (element) {
                    return $(element).is(':visible');
                }
            },
            minValue: {
                param: numberOfElements,
                depends: function (element) {
                    return $(element).is(':visible');
                }
            }
        }
    };

    $('#fieldsetGeneralInfoForm').validate({
        rules: validationRules,
        messages: {
            numberOfElements: {
                required: "This field is required.",
                minValue: $.validator.format("Cannot set a smaller value. This fieldset already has {0} fields.")
            }
        },
        ignore: []
    });
}

function getNumberOfElements() {
    var fieldSetId = $("#elementId").val();
    const mainListItem = document.querySelector(`.dd-item[data-itemtype="fieldset"][data-id="${fieldSetId}"]`);
    if (!mainListItem)
        return 0;

    const fieldItems = mainListItem.querySelectorAll('li[data-itemtype="field"]:not(.add-item-button)');
    return fieldItems.length;
}

function createNewOption(e) {
    e.preventDefault();
    e.stopPropagation();

    // Get the current values from the existing inputs
    let existingLabel = document.querySelector('input[name="optionLabel"]').value;

    if (existingLabel != "") {
        let existingValue = document.querySelector('input[name="optionValue"]').value;

        // Create a new div for the option group
        let optionGroup = document.createElement('div');
        optionGroup.classList.add('advanced-filter-item', 'margin-bottom-8', 'option-group');

        // Create the input for option label
        let optionLabelInput = document.createElement('input');
        optionLabelInput.type = 'text';
        optionLabelInput.classList.add('filter-input', 'fs-item-title', 'margin-right-8', 'option-filter', 'optionLabel');
        optionLabelInput.setAttribute('data-label', existingLabel);
        optionLabelInput.setAttribute('data-id', generateUUIDWithoutHyphens());
        optionLabelInput.value = existingLabel;
        optionLabelInput.disabled = true;

        // Create the input for option value
        let optionValueInput = document.createElement('input');
        optionValueInput.type = 'text';
        optionValueInput.classList.add('filter-input', 'fs-item-title', 'margin-right-8', 'option-filter');
        optionValueInput.setAttribute('data-value', existingValue);
        optionValueInput.value = existingValue;
        optionLabelInput.setAttribute('data-value', existingValue);
        optionValueInput.disabled = true;

        // Create the span element for the remove button
        let removeSpan = document.createElement('span');
        removeSpan.classList.add('remove-option');

        // Create the remove button image
        let removeImage = document.createElement('img');
        removeImage.classList.add('remove-option-btn');
        removeImage.src = '/css/img/icons/close_black.svg';

        // Append the image to the span
        removeSpan.appendChild(removeImage);

        // Append the input fields and remove button to the option group div
        optionGroup.appendChild(optionLabelInput);
        optionGroup.appendChild(optionValueInput);
        optionGroup.appendChild(removeSpan);

        // Append the new option group to the table body before the existing empty option group
        document.getElementById('tableBody').insertBefore(optionGroup, document.querySelector('.advanced-filter-item.margin-bottom-16.option-group'));

        // Clear the existing inputs for the next entry
        document.querySelector('input[name="optionLabel"]').value = '';
        document.querySelector('input[name="optionValue"]').value = '';
    }
}

function createRemoveOptionButton() {
    let span = document.createElement('span');
    $(span).addClass('remove-language-button');

    let i = document.createElement('i');
    $(i).addClass('fas fa-times');

    $(span).append(i);
    return span;
}

$(document).on('click', '.remove-option', function (e) {
    $(this).closest('.option-group').remove();
});