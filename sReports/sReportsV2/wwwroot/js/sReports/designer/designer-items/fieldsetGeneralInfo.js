$(document).on('click', '#submit-fieldset-info', function (e) {
    if ($('#fieldsetGeneralInfoForm').valid()) {
        createNewThesaurusIfNotSelected();

        let label = $('#label').val();
        let elementId = getOpenedElementId();
        let element = getElement('fieldset', label);

        if (element) {
            let matrixId = $('#fsMatrixId').val();

            if (matrixId !== "") {
                updateFieldsetInMatrix(matrixId, elementId, label);
            } else {
                updateFieldsetAttributes(element, elementId, label);
            }

            setFieldSetHelp(element);
            setFieldSetLayout(element, encodeURIComponent(elementId));
            updateTreeItemTitle(element, label);
            closDesignerFormModal(true);
            clearErrorFromElement($(element).attr('data-id'));
        }
    }
    else {
        toastr.error("Field set informations are not valid");
    }
})

function updateFieldsetInMatrix(matrixId, elementId, label) {
    let parentElement = document.querySelector(`li.dd-item[data-id='${matrixId}']`);
    let listOfFieldsets = $(parentElement).attr('data-listoffieldsets');

    if (listOfFieldsets) {
        let fieldsets = JSON.parse(decodeURIComponent(listOfFieldsets));

        let fieldset = fieldsets.find(f => f.id === elementId);
        if (fieldset) {
            fieldset.id = encodeURIComponent(elementId);
            fieldset.label = encodeURIComponent(label);
            fieldset.thesaurusId = encodeURIComponent($('#thesaurusId').val());
            fieldset.description = encodeURIComponent($('#description').val());
            fieldset.isBold = encodeURIComponent($('#isBold').is(":checked"));
            fieldset.isRepetitive = encodeURIComponent($('#isRepetitive').val());
            fieldset.numberOfRepetitions = encodeURIComponent($('#numberOfRepetitions').val());
            fieldset.help = getFieldSetHelp();
            $(parentElement).attr('data-listoffieldsets', JSON.stringify(fieldsets));
        }
    }
}

function updateFieldsetAttributes(element, elementId, label) {
    $(element).attr('data-id', encodeURIComponent(elementId));
    $(element).attr('data-label', encodeURIComponent(label));
    $(element).attr('data-thesaurusid', encodeURIComponent($('#thesaurusId').val()));
    $(element).attr('data-description', encodeURIComponent($('#description').val()));
    $(element).attr('data-isbold', encodeURIComponent($('#isBold').is(":checked")));
    $(element).attr('data-isrepetitive', encodeURIComponent($('#isRepetitive').val()));
    $(element).attr('data-numberofrepetitions', encodeURIComponent($('#numberOfRepetitions').val()));
}

function setFieldSetLayout(element, matrixId) {
    let layoutType = $('#layoutType').val();
    let layoutMaxItems = $('#layoutMaxItems').val();
    let matrixType = $('#matrixType').val();

    let layout = getLayoutData(element, layoutType, layoutMaxItems);

    if (layoutType === 'Matrix') {
        if (matrixType === 'FieldMatrix') {
            let options = collectOptions();
            updateFieldValues(element, options);
            $(element).attr('data-options', JSON.stringify(options));
        }
        else {
            let fieldSets = collectFieldsets(matrixId, matrixType);
            $(element).attr('data-listoffieldsets', JSON.stringify(fieldSets));
            $(element).attr('data-matrixtype', JSON.stringify(matrixType));
        }
    }

    let rawData = element.getAttribute('data-listoffieldsets') || "[]";
    let fieldsetids = JSON.parse(decodeURIComponent(rawData));

    if (fieldsetids.length > 0) {
        let uniqueIds = [...new Set(fieldsetids.map(item => item.Id))];
        $(`#nestable [data-itemtype="fieldset"]:not([data-fsmatrixid]):not(.add-item-button)`).each(function () {
            let fieldset = $(this);
            let fieldsetId = fieldset.attr("data-id");

            if (uniqueIds.includes(fieldsetId)) {
                fieldset.remove();
            }
        });
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

function collectFieldsets(matrixId, matrixType) {
    let fieldSets = [];
    let i = 0;
    $('.fieldSetLabel').each(function () {
        let id = $(this).data('id');
        let label = $(this).data('label');
        let description = $(this).data('description');
        let isBold = $(this).data('isbold');
        let thesaurusId = $(this).data('thesaurusid');
        let helpTitle = $(this).data('helptitle');
        let helpContent = $(this).data('helpcontent');
        let help = null;

        if (label) {
            if (helpTitle || helpContent) {
                help = {
                    Title: helpTitle || "",
                    Content: helpContent || ""
                };
            }
            let rawFields = $(this).data('fields');
            let fields = getFieldsForFieldSet(rawFields, i);

            let fieldSet = {
                Id: id,
                Label: label,
                MatrixId: matrixId,
                MatrixType: matrixType,
                Fields: fields,
                Description: description,
                IsBold: isBold,
                ThesaurusId: thesaurusId,
                Help: help
            };

            fieldSets.push(fieldSet);
            i++;
        }
    });

    return fieldSets;
}

function getFieldsForFieldSet(rawFields, index) {
    let fields = [];

    $('.fieldLabel').each(function (fieldIndex) {
        let fieldId = $(this).data('id')?.split(',')[index] ?? generateUUIDWithoutHyphens();

        let matchingField = rawFields?.find(field => field.id === fieldId);

        if (matchingField) {
            matchingField.label = $(this).data('label');
            fields.push(matchingField);
        } else {
            let label = $(this).data('label');

            if (label) {
                if (rawFields?.[fieldIndex]) {
                    fields.push(rawFields[fieldIndex]);
                } else {
                    fields.push(collectFields(label, fieldId));
                }
            }
        }
    });

    return fields;
}

function collectFields(label, id) {
    let field = {
        Id: id,
        Label: label,
        Type: $('#matrixFieldType').val()
    };

    return field;
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

    const matrixType = $("#matrixType").val();
    if (matrixType == 'FieldSetMatrix') {
        const fieldItems = mainListItem.querySelectorAll('li[data-itemtype="fieldset"]');
        return fieldItems.length;
    } else {
        const fieldItems = mainListItem.querySelectorAll('li[data-itemtype="field"]:not(.add-item-button)');
        return fieldItems.length;
    }
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
        optionGroup.classList.add('advanced-filter-item', 'margin-bottom-8', 'option-group', 'options-group');

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
        toggleOptionsVisibility();

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
    toggleOptionsVisibility();
});

function toggleFieldsetVisibility() {
    var layoutMaxItems = parseInt($('#layoutMaxItems').val());
    var fieldsetCount = $('.fieldSetLabel').length;

    if (fieldsetCount < layoutMaxItems) {
        $('#addFieldsetArea').removeClass('hide').addClass('show');
    } else {
        $('#addFieldsetArea').removeClass('show').addClass('hide');
    }
}

function toggleOptionsVisibility() {
    var layoutMaxItems = parseInt($('#layoutMaxItems').val());
    var fieldsetCount = $('.options-group').length;

    if (fieldsetCount < layoutMaxItems) {
        $('#optionGroup').removeClass('hide').addClass('show');
        $('#addOptionButton').removeClass('hide').addClass('show');
    } else {
        $('#optionGroup').removeClass('show').addClass('hide');
        $('#addOptionButton').removeClass('show').addClass('hide');
    }
}

function getFieldSetHelp() {
    let helpTitle = $('#helpTitle').val();
    let helpContent = $('#helpContent').val();
    let help = null;
    if (helpTitle || helpContent) {
        help = {
            Title: helpTitle || "",
            Content: helpContent || ""
        };
    }

    return help;
}

function handleLayoutTypeChange(value) {
    if (value === 'Matrix') {
        $('#layoutMaxItems').closest('.layout-maxitems').show();
        $('#matrixTypeContainer').show();
        $('#repetitive').hide();
        $('#numOfRepetition').hide();
        if ($('#matrixType').val() == 'FieldMatrix') {
            $('#matrixLayoutDiv').show();
            $('#fieldSetMatrixLayoutDiv').hide();
            $('#fieldTypeContainer').hide();
        }
        else {
            $('#matrixLayoutDiv').hide();
            $('#fieldSetMatrixLayoutDiv').show();
            $('#fieldTypeContainer').show();
        }
    } else {
        $('#layoutMaxItems').closest('.layout-maxitems').hide();
        $('#matrixTypeContainer').hide();
        $('#matrixLayoutDiv').hide();
        $('#fieldSetMatrixLayoutDiv').hide();
        $('#fieldTypeContainer').hide();
        $('#repetitive').show();
        $('#numOfRepetition').show();
    }
}

function handleMatrixTypeChange(value) {
    if ($('#layoutType').val() === 'Matrix') {
        if (value !== 'FieldMatrix') {
            $('#matrixLayoutDiv').hide();
            $('#fieldSetMatrixLayoutDiv').show();
            $('#fieldTypeContainer').show();
        } else {
            $('#matrixLayoutDiv').show();
            $('#fieldSetMatrixLayoutDiv').hide();
            $('#fieldTypeContainer').hide();
        }
    }
}

function addColumn(e) {
    e.stopPropagation();
    e.preventDefault();
    let rows = document.querySelectorAll(".matrix-table tr");
    let header = document.querySelector(".header-row");
    let columnCount = header.children.length - 1;
    let newColumnIndex = columnCount + 1;
    let newHeaderCell = document.createElement("th");
    newHeaderCell.setAttribute('onclick', 'toggleFieldsetDropdown(event, true)');

    newHeaderCell.innerHTML = `
                <input class="custom-input matrix-cell fieldLabel"
                       type="text"
                       placeholder="Enter Field"
                       title="Enter Field">
                <i class="remove-fieldset" onclick="removeColumn(this, ${newColumnIndex})"></i>
            `;
    header.appendChild(newHeaderCell);

    const dropdownDiv = document.createElement("div");
    dropdownDiv.className = "dropdown dropdown-center field-dropdown";
    dropdownDiv.style.display = "none";
    dropdownDiv.innerHTML = `
                    <div class="dropdown-menu  dropdown-menu-right dropleft matrix-fieldset-menu show" 
                         aria-labelledby="dropdownMenuButton-${columnCount}">
                        <input class="column-zoom-input"
                               onkeydown="handleFieldsetDropdownInput(event, this, '${columnCount}', true)"
                               type="text"
                               placeholder="Enter Field">
                    </div>
                `;
    newHeaderCell.appendChild(dropdownDiv);

    let input = newHeaderCell.querySelector("input");
    input.addEventListener("input", function () {
        let newValue = this.value.trim();
        if (newValue) {
            this.setAttribute("data-label", newValue);
        }
    });
    rows.forEach((row) => {
        if (row.classList.contains("add-row-row")) {
            let td = row.querySelector("td.full-width");
            td.colSpan += 1;
            return;
        }
        if (row.classList.contains("header-row")) return;
        let newCell = document.createElement("td");
        newCell.classList.add("cursor-default");
        let input = document.createElement("input");
        input.classList.add("custom-input", "matrix-cell");
        input.setAttribute("readonly", "");
        newCell.appendChild(input);
        row.appendChild(newCell);
    });

    adjustTableWidth();
}

function removeColumn(btn, colIndex) {
    let rows = document.querySelectorAll(".matrix-table tr");
    rows.forEach(row => {
        let cells = row.children;
        if (cells.length > colIndex) {
            cells[colIndex].remove();
        }
    });
    let footerTd = document.querySelector(".add-row-row td.full-width");
    if (footerTd && footerTd.colSpan > 1) {
        footerTd.colSpan -= 1;
    }
    let headerCells = document.querySelectorAll(".header-row th");
    headerCells.forEach((cell, index) => {
        let removeBtn = cell.querySelector(".remove-fieldset");
        if (removeBtn) {
            removeBtn.setAttribute("onclick", `removeColumn(this, ${index})`);
        }
    });

    removeStickyColumns();
    adjustTableWidth();
}

function addRow(e) {
    e.stopPropagation();
    e.preventDefault();
    let tbody = document.querySelector(".matrix-body");
    let rowCount = tbody.getElementsByTagName("tr").length;
    let row = document.createElement("tr");

    row.classList.add("matrix-row");
    row.dataset.rowIndex = `${rowCount}`;

    let firstCell = document.createElement("td");
    firstCell.setAttribute('onclick', 'toggleFieldsetDropdown(event, false)');
    firstCell.innerHTML = `
                <input class="custom-input matrix-cell fieldSetLabel"
                       type="text"
                       placeholder="Enter Fieldset"
                       title="Enter Fieldset">
                <i class="remove-fieldset" onclick="removeRow(this)"></i>
            `;
    row.appendChild(firstCell);

    const dropdownDiv = document.createElement("div");
    dropdownDiv.className = "dropdown dropdown-center fieldset-dropdown";
    dropdownDiv.style.display = "none";
    const datalistOptions = window.selectedFieldSets.map(fieldSet => `
        <option value="${fieldSet.Label}"
                data-id="${fieldSet.Id}"
                data-label="${fieldSet.Label}"
                data-description="${fieldSet.Description}"
                data-isbold="${fieldSet.IsBold}"
                data-thesaurusid="${fieldSet.ThesaurusId}"
                data-fields='${JSON.stringify(fieldSet.Fields)}'>
            ${fieldSet.Label}
        </option>
    `).join('');

    dropdownDiv.innerHTML = `
                    <div class="dropdown-menu  dropdown-menu-right dropleft matrix-fieldset-menu show" 
                         aria-labelledby="dropdownMenuButton-${rowCount}">
                        <input class="column-zoom-input"
                               list="fieldsetOptions"
                               onkeydown="handleFieldsetDropdownInput(event, this, '${rowCount}', true)"
                               type="text"
                               placeholder="Enter Fieldset">
                        <datalist id="fieldsetOptions-${rowCount}">
                            <option value="">Select Option</option>
                            ${datalistOptions}
                        </datalist>
                    </div>
                `;
    firstCell.appendChild(dropdownDiv);

    let columnCount = document.querySelector(".header-row").children.length - 1;
    for (let i = 0; i < columnCount; i++) {
        let numberCell = document.createElement("td");
        numberCell.classList.add("cursor-default");
        let input = document.createElement("input");
        input.classList.add("custom-input", "matrix-cell");
        input.setAttribute("readonly", "");
        numberCell.appendChild(input);
        row.appendChild(numberCell);
    }
    tbody.appendChild(row);
    toggleFieldsetVisibility();
    adjustTableWidth();
}

function removeRow(btn) {
    let row = btn.closest(".matrix-row");
    row.remove();
    let rows = document.querySelectorAll(".matrix-body .matrix-row");
    rows.forEach((row, index) => {
        row.dataset.rowIndex = index;
    });
    toggleFieldsetVisibility();
}

function updateLabel(input, isFieldset) {
    input.addEventListener("input", function () {
        let newValue = this.value.trim();
        if (newValue) {
            this.setAttribute("data-label", newValue);
            this.setAttribute("value", newValue);
            if (isFieldset) {
                this.setAttribute("data-id", generateUUIDWithoutHyphens());
            }
        }
    });
}

function adjustTableWidth() {
    const table = document.querySelector('.matrix-table');

    if (table) {
        const firstHeaders = table.querySelectorAll('tr th:first-child');
        const firstCells = table.querySelectorAll('tr td:first-child');
        table.style.width = '100%';
        if (table.scrollWidth <= table.clientWidth) {
            removeStickyColumns();
        } else {
            firstHeaders.forEach(th => {
                if (!th.classList.contains('sticky-column')) {
                    th.classList.add('sticky-column');
                }
            });
            firstCells.forEach(td => {
                if (!td.classList.contains('sticky-column')) {
                    td.classList.add('sticky-column');
                }
            });
        }
    }
}

function removeStickyColumns() {
    const table = document.querySelector('.matrix-table');
    if (table) {
        const firstHeaders = table.querySelectorAll('tr th:first-child');
        const firstCells = table.querySelectorAll('tr td:first-child');

        firstHeaders.forEach(th => th.classList.remove('sticky-column'));
        firstCells.forEach(td => td.classList.remove('sticky-column'));
    }
}

function toggleFieldsetDropdown(event, isField) {
    if (event.target.className === 'remove-fieldset') {
        return;
    }

    const element = event.currentTarget;
    const dropdown = isField ? element.querySelector('.field-dropdown') : element.querySelector('.fieldset-dropdown');
    const mainInputClass = isField ? '.fieldLabel' : '.fieldSetLabel';

    const parentChildren = Array.from(element.parentElement.children);
    const isFirstTh = parentChildren.length > 1 && parentChildren[1] === element; 
    const filteredChildren = parentChildren.slice(1);
    const isLastTh = filteredChildren.length > 0 && filteredChildren[filteredChildren.length - 1] === element;
    const isOnlyTh = parentChildren.length === 2;

    if (isField) {
        if (isLastTh || isOnlyTh) {
            dropdown.style.right = '-0px';
        } else if (isFirstTh) {
            dropdown.style.right = '-200px';
        }
    }

    if (dropdown.style.display === 'none' || dropdown.style.display === '') {
        dropdown.style.display = 'block';
        const dropdownInput = element.querySelector('.column-zoom-input');
        const mainInput = element.querySelector(mainInputClass);

        if (dropdownInput && mainInput) {
            const mainValue = mainInput.value.trim();
            if (mainValue) {
                dropdownInput.value = mainValue;
            } else {
                dropdownInput.value = '';
            }
            dropdownInput.focus();
        }

        setTimeout(() => {
            document.addEventListener('click', function closeDropdown(e) {
                if (!element.contains(e.target)) {
                    updateMainInput(element, isField);
                    dropdown.style.display = 'none';
                    document.removeEventListener('click', closeDropdown);
                }
            });
        }, 0);
    } else {
        updateMainInput(element, isField);
        dropdown.style.display = 'none';
    }
}

function handleFieldsetDropdownInput(event, inputElement, isField) {
    if (event.key === 'Enter') {
        event.preventDefault();
        const element = inputElement.closest('td') || inputElement.closest('th');
        const dropdown = isField ? element.querySelector('.field-dropdown') : element.querySelector('.fieldset-dropdown');

        updateMainInput(element, isField);

        if (dropdown) {
            dropdown.style.display = 'none';
        }
    }
}

function updateMainInput(element, isField) {
    const dropdownInput = element.querySelector('.column-zoom-input');
    const mainInputClass = isField ? '.fieldLabel' : '.fieldSetLabel';
    const mainInput = element.querySelector(mainInputClass);

    if (dropdownInput && mainInput) {
        const newValue = dropdownInput.value.trim();

        if (newValue) {

            if (isField) {
                const headerRow = document.querySelector('.header-row');
                const existingFields = headerRow.querySelectorAll('.fieldLabel');

                for (let field of existingFields) {
                    if (field !== mainInput &&
                        field.value.toLowerCase() === newValue.toLowerCase()) {
                        dropdownInput.value = '';
                        toastr.error("A field with the same name has already been added!");
                        return;
                    }
                }

                mainInput.value = newValue;
                dropdownInput.value = '';
                mainInput.setAttribute("data-label", newValue);
                mainInput.setAttribute("value", newValue);
                mainInput.setAttribute("title", newValue);
            } else {
                const matrixBody = document.querySelector('.matrix-body');
                const existingFields = matrixBody.querySelectorAll('.fieldSetLabel');

                for (let fieldSet of existingFields) {
                    if (fieldSet !== mainInput &&
                        fieldSet.value.toLowerCase() === newValue.toLowerCase()) {
                        dropdownInput.value = '';
                        toastr.error("A fieldset with the same name has already been added!");
                        return;
                    }
                }

                const datalist = document.getElementById('fieldsetOptions');
                const options = datalist?.getElementsByTagName('option');
                let selectedOption = null;

                if (options !== undefined) {
                    for (let option of options) {
                        if (option.value === newValue) {
                            selectedOption = option;
                            break;
                        }
                    }
                }

                if (selectedOption) {
                    mainInput.value = selectedOption.value;
                    mainInput.setAttribute("data-id", selectedOption.dataset.id);
                    mainInput.setAttribute("data-label", selectedOption.dataset.label);
                    mainInput.setAttribute("data-description", selectedOption.dataset.description);
                    mainInput.setAttribute("data-isbold", selectedOption.dataset.isbold);
                    mainInput.setAttribute("data-thesaurusid", selectedOption.dataset.thesaurusid);
                    mainInput.setAttribute("data-fields", selectedOption.dataset.fields);
                    mainInput.setAttribute("title", selectedOption.value);
                } else {
                    Array.from(mainInput.attributes)
                        .filter(attr => attr.name.startsWith('data-'))
                        .forEach(attr => mainInput.removeAttribute(attr.name));

                    mainInput.value = newValue;
                    dropdownInput.value = '';
                    mainInput.setAttribute("data-id", generateUUIDWithoutHyphens());
                    mainInput.setAttribute("data-label", newValue);
                    mainInput.setAttribute("value", newValue);
                    mainInput.setAttribute("title", newValue);
                }
            }
        }
    }
}

function removeFieldsetRow(e) {
    const submitButton = document.getElementById("buttonSubmitDelete");
    let rowId = submitButton.getAttribute('data-row-id');
    let row = document.querySelector(`.fs-row:nth-child(${parseInt(rowId) + 1})`);
    let fieldsetId = submitButton.getAttribute('data-target');
    let matrixId = submitButton.getAttribute('data-matrixid');

    if (row) {
        row.remove();
    } else {
        row = document.querySelector(`tr.fs-row[id="${fieldsetId}"]`);
        row.remove();
    }

    const rows = document.querySelectorAll(".matrix-body .matrix-row");
    rows.forEach((row, index) => {
        row.dataset.rowIndex = index;
    });

    removeFieldsetFromMatrix(matrixId, fieldsetId);
    deleteFormItem(e);
}

function removeFieldsetFromMatrix(matrixId, elementId) {
    let parentElement = document.querySelector(`li.dd-item[data-id='${matrixId}']`);
    let listOfFieldsets = $(parentElement).attr('data-listoffieldsets');

    if (listOfFieldsets) {
        let fieldsets = JSON.parse(decodeURIComponent(listOfFieldsets));

        fieldsets = fieldsets.filter(f => f.id !== elementId);

        $(parentElement).attr('data-listoffieldsets', JSON.stringify(fieldsets));
    }
}