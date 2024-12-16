function specialValueShouldBeSet($el) {
    return $el.attr("data-isrequired") === 'True' && $el.attr("data-allowsavewithoutvalue") != '';
}

function isSpecialValue($el) {
    return $el.attr("spec-value") != undefined;
}

function isSpecialValueSelected($fieldInput) {
    var $specialValueElement = getSpecialValueElement($fieldInput.attr("name"));
    return isSpecialValueSelectedTrue($specialValueElement);
}

function isSpecialValueSelectedTrue($specialValueElement) {
    return !$specialValueElement.attr('hidden') && $specialValueElement.is(":checked");
}

function getSpecialValueElement(inputName) {
    return $(`input[name="${inputName}"][spec-value]`);
}

function missingValueIsClicked($missingValueInput) {
    if ($missingValueInput.is(":checked")) {
        var fieldName = $missingValueInput.attr("name");
        setInputFieldToDefault($missingValueInput, fieldName, false);
    }
}

function showMessageAfterNEIsChecked() {
    let nEMessage = $('#ne-value-selected-message').text();
    if (nEMessage) {
        toastr.info(nEMessage);
    }
}

function getMissingValueInputByFieldInstanceId(fieldInstanceRepetitionId) {
    return $("input[spec-value][data-fieldinstancerepetitionid='" + fieldInstanceRepetitionId + "']");
}

function getMissingValueInputByFieldId(fieldId) {
    return $("input[spec-value][data-fieldid='" + fieldId + "']");
}

function unsetSpecialValueIfSelected($specialValueElement) {
    if (isSpecialValueSelectedTrue($specialValueElement)) {
        setMissingValueElementToDefault($specialValueElement);
    }
}

function setMissingValueElementToDefault($specialValueElement) {
    if ($specialValueElement.length > 0) {
        $specialValueElement.prop('disabled', 'false');
        $specialValueElement.prop("checked", false);
        $specialValueElement.prop('disabled', 'true');

        $specialValueElement.attr("data-isspecialvalue", "false");
        $specialValueElement.attr("value", '');
    }
}

function populateRequiredInputsIds() {
    var fieldSetContainer = document.querySelector('.form-accordion');
    let requiredInputsWithoutValues = Array.from(fieldSetContainer.querySelectorAll('input[data-isrequired="True"][data-allowsavewithoutvalue="False"], textarea[data-isrequired="True"][data-allowsavewithoutvalue="False"], select[data-isrequired="True"][data-allowsavewithoutvalue="False"]'));

    let requiredFieldIdsWithoutValues = [];
    let checkedSpecialValueFieldIds = [];

    requiredInputsWithoutValues.forEach(input => {
        if (isDependentFieldInstanceHidden(input) || isSpecialValue($(input))) {
            return;
        }

        var fieldInstanceRepetitionId = $(input).attr('data-fieldinstancerepetitionid');
        var $missingValueInput = getMissingValueInputByFieldInstanceId(fieldInstanceRepetitionId);
        if (isSpecialValueSelectedTrue($missingValueInput)) {
            checkedSpecialValueFieldIds.push(input.getAttribute('data-fieldid'));
            return;
        }

        let requiredFieldEmpty;
        if (isInputCheckboxOrRadio($(input))) {
            requiredFieldEmpty = $("input[data-fieldinstancerepetitionid='" + fieldInstanceRepetitionId + "']:checked").length == 0;
        } else {
            requiredFieldEmpty = $(input).val().trim() === '';
        }

        let fieldId = input.getAttribute('data-fieldid');
        if (requiredFieldEmpty && !checkedSpecialValueFieldIds.some(c => c == fieldId)) {
            requiredFieldIdsWithoutValues.push(fieldId);
        }
    });

    return requiredFieldIdsWithoutValues;
}

function showMissingValuesModal(event, fieldsIds, canSaveWithoutValue) {
    event.preventDefault();
    var thesaurusId = getFormInputValueByName("fid", "thesaurusId");
    var versionId = getFormInputValueByName("fid", "VersionId");
    callServer({
        type: 'GET',
        url: `/FormInstance/ShowMissingValuesModal`,
        data: {
            'thesaurusId': thesaurusId,
            'versionId': versionId,
            'fieldsIds': fieldsIds,
            'canSaveWithoutValue': canSaveWithoutValue
        },
        traditional: true,
        success: function (data) {
            $('#missingValueModal').html(data);
            if (canSaveWithoutValue) {
                let radioInput = $("input[spec-value][data-fieldid='" + fieldsIds + "']:checked");
                if (radioInput.length > 0) {
                    let value = radioInput.val();
                    radioInput = $("input[missing-code-value-option][value='" + value + "']");
                    radioInput.prop("checked", true);
                }
            }
            $('#missingValuesModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function populateMissingValues(event) {
    event.preventDefault();
    var modalForm = document.getElementById('missingValuesForm');
    modalForm.querySelectorAll('input[type="radio"]').forEach(function (radio) {
        if (radio.checked) {
            setMissingValueByField(radio.id, radio.value);
            showMessageAfterNEIsChecked();
        }
    });
    $('#missingValuesModal').modal('hide');
}

function setMissingValueByField(fieldId, value) {
    var $missingValueInput = getMissingValueInputByFieldId(fieldId);

    if ($missingValueInput.length > 0) {
        setMissingValue($missingValueInput, value);
    }
}

function setMissingValue($missingValueInput, value, triggerEvent = true) {
    $missingValueInput.prop('disabled', 'false');
    $missingValueInput.prop("checked", true);
    if (triggerEvent) {
        missingValueIsClicked($missingValueInput);
    }
    $missingValueInput.prop('disabled', 'true');

    $missingValueInput.attr("data-isspecialvalue", true);
    $missingValueInput.attr("value", value);
    removeSpecialAttributes($missingValueInput.attr('data-fieldinstancerepetitionid'));
}

function removeSpecialAttributes(fieldInstanceRepetitionId) {
    $('input[data-fieldinstancerepetitionid="' + fieldInstanceRepetitionId + '"]').each(function () {
        if ($(this).attr('type') === 'number') {
            $(this).removeAttr('min max');
        }
        else if ($(this).attr('type') === 'text') {
            $(this).removeAttr('minlength maxlength');
        }
        removeValidationMessages($(this));
    });
}