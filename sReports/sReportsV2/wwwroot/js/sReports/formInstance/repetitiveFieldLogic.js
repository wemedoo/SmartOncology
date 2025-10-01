//--------------------------- common repetitive logic functions ----------------------------------------------
function toggleRemoveRepetitiveButton(element, show) {
    if (show) {
        $(element).find('.remove-repetitive').parent().show();
    } else {
        $(element).find('.remove-repetitive').parent().hide();
    }
}

function cloneResetAndNeSection(clone, newFieldInstanceRepetitionId, newFieldSetInstanceRepetitionId) {
    $(clone).find(".ne-link").attr("data-field-name", newFieldInstanceRepetitionId);
    let $specialValueElement = $(clone).find(".ne-radio");

    setDataAttributesForNewRepetitiveField($specialValueElement, newFieldInstanceRepetitionId);
    unsetSpecialValueIfSelected($specialValueElement);
}

function resetDateFieldsBeforeClone(container) {
    $(container).find('.datetime-picker-container').each(function (index, value) {
        $(value).find('input:first').datepicker("destroy").removeAttr('id');
    });
}

function reinitDatePickerToStartingFieldsBeforeClone(container) {
    $(container).find('.datetime-picker-container').each(function (index, value) {
        $(value)
            .find('input:first')
            .initDatePicker(
                true,
                true,
                function (input, inst) {
                    $(document).off('focusin.bs.modal');
                },
                function () {
                    $(document).on('focusin.bs.modal');
                }
            );
    });
}

function doesRepetitionHaveSelect2(clone) {
    let ret = false;
    if ($(clone).children('.repetitive-field').children('.select2').length !== 0) {
        ret = true;
    }
    return ret;
}

function destroySelect2(closestFieldContainer) {
    $(closestFieldContainer).children('.repetitive-field').children('select').select2("destroy");
}

function getCodedFieldCodesetId(closestFieldContainer) {
    return $(closestFieldContainer).children('.repetitive-field').children('select').attr('data-codedfieldcodeset');
}

function generateNamesForNewRepetitiveFields(callback, numberOfFields = 1) {
    callServer({
        url: `/FormInstance/GetGuids?quantity=${numberOfFields}`,
        type: "GET",
        dataType: "json",
        success: function (data, textStatus, xhr) {
            callback(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function destroyFieldComponentsBeforeClone(closestFieldContainer) {
    resetDateFieldsBeforeClone(closestFieldContainer);

    let hasSelect2Elements = doesRepetitionHaveSelect2(closestFieldContainer);
    if (hasSelect2Elements) {
        destroySelect2(closestFieldContainer);  // destroying before .clone()
    }

    return hasSelect2Elements;
}

function initFieldComponentsAfterClone(hasSelect2Elements, closestFieldContainer) {
    if (hasSelect2Elements) {
        initializeCodedFieldsSelect2("select[data-codedfieldcodeset]", getCodedFieldCodesetId(closestFieldContainer));
    }
    reinitDatePickerToStartingFieldsBeforeClone(closestFieldContainer);
}

function configureClonedFieldFields(clone) {
    toggleFileNameContainer(clone, 'file');
}

function getSelectorForRepetitiveFieldsContainer() {
    return ".repetitive-values";
}

function cloneInput(clone, newFieldInstanceRepetitionId, additionalAttributesCallback) {
    let $inputElement = cloneInputCommon(clone, newFieldInstanceRepetitionId);

    $inputElement.each(function () {
        setDataAttributesForNewRepetitiveField($(this), newFieldInstanceRepetitionId);
        setInputToDefault(
            $(this),
            {
                revertToDefaultEditableState: true,
                setValue: true
            }
        );

        if (additionalAttributesCallback) {
            additionalAttributesCallback($(this), newFieldInstanceRepetitionId);
        }
    });
}

function cloneInputCommon(clone, newFieldInstanceRepetitionId, newFieldSetInstanceRepetitionId) {
    let $inputElement = getInput($(clone).children(getFieldChildrenSelector(clone)));
    cloneResetAndNeSection(clone, newFieldInstanceRepetitionId, newFieldSetInstanceRepetitionId);

    return $inputElement;
}

function getFieldChildrenSelector(clone) {
    if (isRadioClonedField(clone)) {
        return ".radio-container";
    } else if (isCheckboxClonedField(clone)) {
        return ".checkbox-container";
    } else {
        return ".repetitive-field"
    }
}

function getInput(fieldContainer) {
    let currentInput = $(fieldContainer).find(":input");
    return currentInput;
}

function setDataAttributesForNewRepetitiveField($input, newFieldInstanceRepetitionId) {
    if (isInputIncludedInSubmit($input)) {
        $input
            .attr("name", newFieldInstanceRepetitionId)
            .attr("data-fieldinstancerepetitionid", newFieldInstanceRepetitionId);
    }
}

function isInputIncludedInSubmit($input) {
    return $input.attr("data-fieldid");
}

function getAdditionalAttributesCallback(clone) {
    if (isFileClonedField(clone)) {
        configureClonedFieldFields(clone);
        return setAdditionalAttributesForFileField();
    } else if (isDateClonedField(clone)) {
        return setAdditionalAttributesForDateField();
    } else if (isDatetimeClonedField(clone)) {
        return setAdditionalAttributesForDatetimeField();
    } else if (isSelectClonedField(clone)) {
        return setAdditionalAttributesForSelectField();
    } else if (isRadioOrCheckboxClonedField(clone)) {
        return setAdditionalAttributesForRadioOrCheckboxField();
    } else {
        return null;
    }
}

function isFileClonedField($clone) {
    return $clone.find(".file-field").length > 0;
}

function isDatetimeClonedField($clone) {
    return $clone.hasClass('field-group-date-time');
}

function isDateClonedField($clone) {
    return $clone.find('[data-fieldtype="date"]').length > 0;
}

function isSelectClonedField(clone) {
    return clone.find('select[data-fieldid]').length > 0;
}

function isRadioClonedField(clone) {
    return $(clone).hasClass("form-radio");
}

function isCheckboxClonedField(clone) {
    return $(clone).hasClass("form-checkbox");
}

function isRadioOrCheckboxClonedField(clone) {
    return isRadioClonedField(clone) || isCheckboxClonedField(clone);
}

function setAdditionalAttributesForFileField() {
    return function ($input, newFieldInstanceRepetitionId) {
        if (isInputFile($input)) {
            $input.attr("id", "field-" + newFieldInstanceRepetitionId);
        } else {
            $input.attr("data-id", "field-" + newFieldInstanceRepetitionId);
            $input.attr("data-fileid", newFieldInstanceRepetitionId);
        }
    }
}

function setAdditionalAttributesForDatetimeField() {
    return function ($input, newFieldInstanceRepetitionId) {
        if ($input.hasClass('date-helper')) {
            let dateHandler = setAdditionalAttributesForDateField();
            dateHandler($input, newFieldInstanceRepetitionId);
        } else if ($input.hasClass('time-helper')) {
            $input.attr("name", newFieldInstanceRepetitionId);
            $input.attr("id", "time-field-id-" + newFieldInstanceRepetitionId);
        }
    }
}

function setAdditionalAttributesForDateField() {
    return function ($input, newFieldInstanceRepetitionId) {
        $input.attr("name", newFieldInstanceRepetitionId);
        $input.attr("id", "date-field-id-" + newFieldInstanceRepetitionId);
        $input.initDatePicker(
            true,
            true,
            function (input, inst) {
                $(document).off('focusin.bs.modal');
            },
            function () {
                $(document).on('focusin.bs.modal');
            }
        );
    }
}

function setAdditionalAttributesForSelectField() {
    return function ($input, newFieldInstanceRepetitionId) {
        $input.attr("id", newFieldInstanceRepetitionId);
    }
}

function setAdditionalAttributesForRadioOrCheckboxField() {
    return function ($input, newFieldInstanceRepetitionId) {
        let activeInputId = $input.attr("id");
        if (activeInputId) {
            let indexOfStaticPart = activeInputId.indexOf("-");
            if (indexOfStaticPart != -1) {
                let newInputId = `${newFieldInstanceRepetitionId}${activeInputId.substring(indexOfStaticPart)}`;
                $input.attr("id", newInputId);
            }
        }
    }
}

//--------------------------- repetitive logic for field ----------------------------------------------
$(document).on('click', '.button-plus-repetitive', function (event) {
    executeEventFunctions(event);

    let closestField = $(event.currentTarget).closest(".form-element");
    generateNamesForNewRepetitiveFields(handleFieldInstanceRepetition(closestField));
});

function handleFieldInstanceRepetition(closestField) {
    return function (guids) {
        let closestFieldContainer = $(closestField).children(getSelectorForRepetitiveFieldsContainer()).children(".field-group:last");
        
        let hasSelect2Elements = destroyFieldComponentsBeforeClone(closestFieldContainer);

        let clone = $(closestFieldContainer).clone();

        let newFieldInstanceRepetitionId = guids.pop();
       
        cloneInput(clone, newFieldInstanceRepetitionId, getAdditionalAttributesCallback(clone));

        clone.appendTo($(closestField).children(getSelectorForRepetitiveFieldsContainer()));
        handleRemoveRepetitiveFieldBtn($(closestField));

        initFieldComponentsAfterClone(hasSelect2Elements, closestFieldContainer);
    }
}

$(document).on('click', '.remove-repetitive', function (event) {
    executeEventFunctions(event);

    let closestFieldSet = $(event.currentTarget).closest(".form-element");
    $(event.target).closest(".field-group").remove();

    handleRemoveRepetitiveFieldBtn(closestFieldSet);
});

function handleRemoveRepetitiveFieldBtn(formElement) {
    let repetitiveCount = $(formElement).children(getSelectorForRepetitiveFieldsContainer()).children(".field-group").length;

    let showBtn = !(repetitiveCount && repetitiveCount === 1);
    $(formElement).children(getSelectorForRepetitiveFieldsContainer()).children(".field-group").each(function (index, element) {
        toggleRemoveRepetitiveButton(element, showBtn);
    });
}

//--------------------------- repetitive logic for fieldset ----------------------------------------------
$(document).on('click', '.button-fieldset-repetitive', function (event) {
    executeEventFunctions(event);
    handleFieldSetInstanceRepetition($(this));
});

function handleFieldSetInstanceRepetition($target) {
    let formFieldSetRepsContainer = $target.closest(".form-fieldset");
    let closestFsContainer = formFieldSetRepsContainer.children(".field-set-container");

    let fsNumsInRepetition = closestFsContainer.children('.field-set').length + 1;
    let oneFsRepetitionsContainer = closestFsContainer.closest('.oneFsRepetitionsContainer');
    let isLastFieldsetOnPage = oneFsRepetitionsContainer.next().length == 0;

    let request = {
        formId: getFormInputValueByName("fid", "formDefinitionId"),
        chapterId: formFieldSetRepsContainer.attr('chapter-id'),
        pageId: formFieldSetRepsContainer.attr('page-id'),
        fieldsetId: $target.attr('fieldset-id'),
        fsNumsInRepetition: fsNumsInRepetition,
        isLastFieldsetOnPage: isLastFieldsetOnPage,
        fieldInstances: getFormInstanceFieldsSubmissionJson(),
        hiddenFieldsShown: getIsHiddenFieldsShown()
    };
    callServer({
        url: `/FormInstance/AddFieldsetRepetition`,
        type: "POST",
        data: JSON.stringify(request),
        contentType: "application/json",
        success: function (data, textStatus, xhr) {
            let clone = $(data);
            clone.appendTo(closestFsContainer);
            handleRepetitiveFieldsetBtns(closestFsContainer);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.remove-field-set', function (event) {
    executeEventFunctions(event);

    let closestFsContainer = $(event.currentTarget).closest(".field-set-container");
    $(event.currentTarget).closest('.field-set').remove();
    handleRepetitiveFieldsetBtns(closestFsContainer);
});

function handleRepetitiveFieldsetBtns(closestFsContainer) {
    handleRemoveRepetitiveFieldsetBtn(closestFsContainer);
    handleAddNewRepetitiveFieldsetBtn(closestFsContainer);
}

function handleAddNewRepetitiveFieldsetBtn(closestFsContainer) {
    let numOfRepetitiveFieldSet = $(closestFsContainer).children(".field-set").length;
    $(closestFsContainer).children(".field-set").each(function (index, element) {
        let $addNewBtnInLastRepetitiveFieldset = $(element).children('div:first').find('.add-repetitive-btn-container');
        if (numOfRepetitiveFieldSet - 1 == index) {
            $addNewBtnInLastRepetitiveFieldset.removeClass('d-none');
        } else {
            $addNewBtnInLastRepetitiveFieldset.addClass('d-none');
        }
    });
}

function handleRemoveRepetitiveFieldsetBtn(closestFsContainer) {
    let numOfRepetitiveFs = $(closestFsContainer).children(".field-set").length;

    $(closestFsContainer).children(".field-set").each(function (index, element) {
        let $removeRepetitiveFieldsetBtn = $(element).children("div:first").find(".remove-repetitive-btn-container");
        if (numOfRepetitiveFs === 1) {
            $removeRepetitiveFieldsetBtn.addClass('d-none');
        } else {
            $removeRepetitiveFieldsetBtn.removeClass('d-none');
        }
    });
}

function initDatePickerInNewFieldSetRepetition(fieldSetInstanceRepetitionId) {
    $(`input[data-fieldsetinstancerepetitionid="${fieldSetInstanceRepetitionId}"]`).each(function () {
        let $dateInput;
        if (isDateOrTimeInput($(this))) {
            $dateInput = $(this);
        } else {
            $dateInput = $(this).siblings('[data-date-input]');
        }
        $dateInput.initDatePicker(
            true,
            true,
            function (input, inst) {
                $(document).off('focusin.bs.modal');
            },
            function () {
                $(document).on('focusin.bs.modal');
            }
        );
    });
}