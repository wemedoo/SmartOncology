function fetchFormInstance(url, callback = null) {
    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            var existingDiv = document.getElementById('temporalFormInstanceDiv');
            existingDiv.innerHTML = html;
            submitForm(callback, true);
        })
        .catch(error => {
            console.error('Error loading content:', error);
        });
}

function clickedSubmit(event, callback = null) {
    event.preventDefault();
    var fieldsIds = getRequiredInputsIds();
    if (fieldsIds.length != 0) {
        showMissingValuesModal(event, fieldsIds, false);
    }
    else {
        $('#fid').find('input').each(function (index, element) {
            validateInput(element, event);
        });
        reAllowSubmit('submitBtn');

        preventMultipleSubmit('submitBtn');
        submitForm(callback);
        return false;
    }
}

function submitForm(callback = null, isCreate = false) {
    includeDisabledInputsInSubmit();
    let jsonData = getFormInstanceSubmissionJson(isCreate);
    callServer({
        url: $('#fid').attr('action'),
        type: "POST",
        data: jsonData,
        contentType: "application/json",
        success: function (data, textStatus, xhr) {
            handleSuccessFormSubmit(data, $("#projectId").val(), $("#showUserProjects").val() == "true", !isCreate, callback);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
            reAllowSubmit('submitBtn');
        }
    });

    return false;
}

function includeDisabledInputsInSubmit() {
    includeDisabledSelect2InSubmit();

    $("#fid").find("fieldset[disabled]").each(function () {
        $(this).removeAttr("disabled");
    });
}

function includeDisabledSelect2InSubmit() {
    if ($("#fid").find("fieldset[disabled]").find('select').hasClass('select2-hidden-accessible')) {
        $("#fid").find("fieldset[disabled]").find('select').prop("disabled", false);
    }
}

function handleSuccessFormSubmit(data, projectId, showUserProjects, isEditForm, callback) {
    toastr.success(data.message ? data.message : 'Success');
    if ($("#eocId").val() != undefined) {
        handleSuccessFormSubmitFromPatient(data, isEditForm, callback);
    } else {
        handleSuccessFormSubmitFromEngine(data, projectId, showUserProjects, callback);
    }
}

function getFormInstanceSubmissionJson(isCreate) {

    formInstanceData = {};

    let formInstanceState = getFormSelectValueByName("fid", "FormState");

    formInstanceData.FormInstanceId = getFormInputValueByName("fid", "formInstanceId");
    formInstanceData.FormDefinitionId = getFormInputValueByName("fid", "formDefinitionId");
    formInstanceData.ThesaurusId = getFormInputValueByName("fid", "thesaurusId");
    formInstanceData.Notes = getFormTextAreaValueByName("fid", "Notes");
    formInstanceData.FormState = modifyStateIfRequiresTransition(formInstanceState);
    formInstanceData.Date = getFormInputValueByName("fid", "Date");
    formInstanceData.LastUpdate = getFormInputValueByName("fid", "LastUpdate");
    formInstanceData.Referrals = getFormInputArrayValuesByName("fid", "referrals");
    formInstanceData.VersionId = getFormInputValueByName("fid", "VersionId");
    formInstanceData.EditVersionId = getFormInputValueByName("fid", "EditVersionId");
    formInstanceData.Language = getFormInputValueByName("fid", "language");
    formInstanceData.EncounterId = getFormInputValueByName("fid", "encounterId");
    formInstanceData.ProjectId = $("#formInstanceProjectId").val();

    formInstanceData.FieldInstances = getFormInstanceFieldsSubmissionJson(isCreate ? undefined : getFieldInstancesFromActivePage());

    return formInstanceData;
}

function getFieldInstancesFromActivePage() {
    return getPreviousActivePage().find(getFormInstanceFieldsSelector());
}

function modifyStateIfRequiresTransition(formInstanceState) {
    return formInstanceState == 'Unlocked' ? 'OnGoing' : formInstanceState;
}

function getFormInstanceFieldsSubmissionJson(filteredFieldInstanceElements) {
    let formFields = [];
    let fieldInstanceElements = filteredFieldInstanceElements ? filteredFieldInstanceElements : getAllFieldInstanceElements();

    fieldInstanceElements.each(function (index) {
        let fields = [];

        $(this).find(":input[data-fieldid]", ":textarea[data-fieldid]").each(function () {
            let field = {};
            const $input = $(this);
            const element = this;

            setCommonFieldProperties(field, $input);

            if (isInputCheckboxOrRadio($input)) {
                addValue(field, $input.is(':checked') ? $input.val() : null);
                field.FlatValueLabel = $input.is(':checked') ? $input.attr('data-value') : null;

            } else if (isInputFile($input) || isInputAudio($input)) {
                const value = $input.val();
                const separator = isInputFile($input) ? '_' : '/';
                const valueLabel = getFileName(value, separator);

                addValue(field, value);
                field.FlatValueLabel = valueLabel;
            } else if ($input.attr('data-fieldtype') == 'connected') {
                field.ConnectedFieldInstanceRepetitionId = $input.val();
                let $selectedConnectedField = $input.find(':selected');
                let selectedConnectedFieldValue = $selectedConnectedField.attr("data-value");
                field.FlatValueLabel = $selectedConnectedField.attr("data-label");
                field.FlatValues = selectedConnectedFieldValue ? selectedConnectedFieldValue.split(",") : [];
            } else if (element .type == "select-one" || element .type == "select-multiple") {
                addValue(field, $input.val());
                if (isSelect2Component(element )) {
                    field.FlatValueLabel = readSelectedOptionLabel($input);
                } else {
                    field.FlatValueLabel = $input.find(':selected').attr("title");
                }
            } else {
                let value = getValue($input);
                
                addValue(field, value);
                field.FlatValueLabel = value;
            }

            if (!$input.valid()) {
                field.ValidationError = {
                    FieldId: $input.data("fieldinstancerepetitionid") || null,
                    Title: "Automatic Query Raised",
                    Description: $input.attr('data-fieldtype') == 'regex' ? "Regex input does not match the required pattern." : element .validationMessage
                };
            } else if ($input.data("allowsavewithoutvalue") == 'True' && (getValue($input) == '' || field.FlatValues.length == 0)) {
                field.ValidationError = {
                    FieldId: $input.data("fieldinstancerepetitionid") || null,
                    Title: "Automatic Query Raised",
                    Description: "Mandatory field is empty."
                };
            } else {
                field.ValidationError = null;
            }

            fields.push(field);
        });

        formFields = formFields.concat(filterObjectsByFieldInstanceRepetitionId(fields));
    });

    return formFields.filter(item => item !== undefined);
}

function getAllFieldInstanceElements() {
    return $("#fid").find(getFormInstanceFieldsSelector());
}

function setCommonFieldProperties(field, $input) {
    field.FieldSetId = $input.attr('data-fieldsetid');
    field.FieldSetInstanceRepetitionId = $input.attr('data-fieldsetinstancerepetitionid');
    field.FieldId = $input.attr('data-fieldid');
    field.FieldInstanceRepetitionId = $input.attr('data-fieldinstancerepetitionid');
    field.Type = $input.attr('data-fieldtype');
    field.ThesaurusId = $input.attr('data-thesaurusid');
    field.IsSpecialValue = $input.attr('data-isspecialvalue');
    field.FlatValues = [];
}

function filterObjectsByFieldInstanceRepetitionId(arr) {
    const mapping = {};

    $(arr).each(function (index, obj) {
        const { FieldInstanceRepetitionId, FlatValues } = obj;

        if (!(FieldInstanceRepetitionId in mapping)) {
            // If Field instance is not in the mapping, add the current object
            mapping[FieldInstanceRepetitionId] = obj;
        } else {
            const existingObj = mapping[FieldInstanceRepetitionId];

            if (existingObj.Type == 'checkbox') {
                filterCheckboxObjectsByFieldInstanceRepetitionId(mapping, obj);
                return;
            }

            // Check if the current object has any FlatValues, or if the existing object's FlatValues has no FlatValues
            if (hasAnyValue(FlatValues) || !hasAnyValue(existingObj.FlatValues)) {
                // Replace the existing object with the current object
                mapping[FieldInstanceRepetitionId] = obj;
            }
            // If both FlatValues and existing object's FlatValues are not empty, we don't do anything.
        }
    });

    // Convert the updated mapping back into an array to get the final result
    const result = Object.values(mapping);

    return result;
}

function filterCheckboxObjectsByFieldInstanceRepetitionId(mapping, obj) {
    const { FieldInstanceRepetitionId, FlatValues, FlatValueLabel, IsSpecialValue } = obj;
    const existingObj = mapping[FieldInstanceRepetitionId];

    if (hasAnyValue(FlatValues) && IsSpecialValue) {
        mapping[FieldInstanceRepetitionId] = obj;
    } else {
        if (hasAnyValue(FlatValues)) {
            if (hasAnyValue(existingObj.FlatValues)) {
                addValue(existingObj, ...FlatValues);
                existingObj.FlatValueLabel += "," + FlatValueLabel;
            } else {
                existingObj.FlatValues = FlatValues;
                existingObj.FlatValueLabel = FlatValueLabel;
            }
            mapping[FieldInstanceRepetitionId] = existingObj;
        }
    }
}

// ---------- Helpers ----------
function addValue(fieldInstanceObj, value) {
    if (value) {
        fieldInstanceObj.FlatValues.push(value);
    }
}

function hasAnyValue(values) {
    return values?.length > 0;
}

function getFormInputValueByName(formId, inputName) {
    return $(`#${formId} input[name=${inputName}]`).val();
}

function getFormInputArrayValuesByName(formId, inputName) {
    return $(`#${formId} input[name=${inputName}]`).map(function () {
        return $(this).val();
    }).get();
}

function getFormSelectValueByName(formId, inputName) {
    return $(`#${formId} select[name=${inputName}]`).find(':selected').val();
}

function getFormTextAreaValueByName(formId, inputName) {
    return $(`#${formId} textarea[name=${inputName}]`).val();
}

function getFileName(url, separator) {
    let fileName = '';

    if (!url) return fileName;

    let lastIndexOfSlash = url.lastIndexOf('/');
    if (lastIndexOfSlash > 0) {
        let urlGUID = url.substring(lastIndexOfSlash + 1);
        let firstIndexOfDash = urlGUID.indexOf(separator);
        fileName = urlGUID.substring(firstIndexOfDash + 1);
    }

    return fileName;
}

function getValue($input) {
    let value = $input.val();
    if ($input.attr("data-fieldtype") == 'date' && value) {
        value = toDateISOStringIfValue(value);
    }

    return value;
}