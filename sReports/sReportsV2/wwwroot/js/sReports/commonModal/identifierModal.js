$(document).ready(function () {
    jQuery.validator.addMethod("duplicate", function (value, element) {
        return !identifierAlreadyExists();
    }, "Identifier already defined");

    jQuery.validator.addMethod("duplicateRemote", function (value, element) {
        return identifierAlreadyExistsRemote();
    }, "Duplicate identifier.");

    $("#identifierForm").validate({
        onkeyup: false,
        rules: {
            identifierValue: {
                required: true,
                maxlength: 128,
                duplicate: true,
                duplicateRemote: true
            }
        }
    });
});

function identifierAlreadyExistsRemote() {
    let result = false;
    let identifier = {
        'identifierTypeCD': $('#identifierTypeCD').val(),
        'identifierValue': $('#identifierValue').val(),
        'identifierUseCD': $('#identifierUseCD').val(),
        'id': $('#identifierEntityId').val()
    };
    callServer({
        type: 'GET',
        url: `/${identifierActiveEntity}/ExistIdentifier`,
        data: identifier,
        async: false,
        success: function (data) {
            result = data;
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });

    return result;
}

function identifierAlreadyExists() {
    let result = false;
    $('#identifierContainer').find('table tbody tr').each(function (index, element) {
        var identifierTypeCD = $(element).find('[data-property="identifierTypeCD"]').attr("data-value");
        var identifierValue = $(element).find('[data-property="identifierValue"]').attr("data-value");
        var identifierId = $(element).attr("data-value");

        if ($('#identifierTypeCD').val() == identifierTypeCD && identifierValue == $('#identifierValue').val() && identifierId != $("#identifierEntityId").val()) {
            result = true;
        }
    });
    return result;
}

function addNewIdentifier(e) {
    updateDisabledOptions(false);
    e.preventDefault();
    e.stopPropagation();
    if ($('#identifierForm').valid()) {
        let identifierEntity = getIdentifierFromForm();

        if (parentEntryExisting(getParentId())) {
            let isEdit = $('#identifierModal').attr("data-is-edit") === "true";
            callServer({
                type: "POST",
                url: `/${identifierActiveEntity}/${isEdit ? 'Edit' : 'Create'}Identifier`,
                data: getIdentifierForServer(identifierEntity),
                success: function (data) {
                    if (showIdentifierFeedbackMessage()) {
                        toastr.success('Identifier saved successfully!');
                    }
                    identifierEntity.identifierEntityVersion = data.rowVersion;
                    let identifierRow;
                    if (isEdit) {
                        identifierRow = editIdentifierInTable(identifierEntity);
                    } else {
                        identifierEntity.identifierEntityId = data.id;
                        identifierRow = addIdentifierToTable(identifierEntity);
                    }
                    updateTableEntryFormData(identifierRow, 'identifier', true);

                    handleModalAfterSubmitting("identifierContainer", 'identifier-entry', 'identifierModal');
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr);
                }
            });
        } else {
            addIdentifierToTable(identifierEntity);
            handleModalAfterSubmitting("identifierContainer", 'identifier-entry', 'identifierModal');
            submitParentForm();
        }
    }
}

function showIdentifierFeedbackMessage() {
    return identifierActiveEntity == 'UserAdministration';
}

function getIdentifierFromForm() {
    let identifierEntity = {
        identifierEntityId: $("#identifierEntityId").val(),
        identifierEntityVersion: $("#identifierEntityVersion").val()
    };

    identifierEntity["identifierTypeCD"] = {
        value: $('#identifierTypeCD').val(),
        display: getSelectedOptionLabel('identifierTypeCD')
    };

    identifierEntity["identifierValue"] = {
        value: $('#identifierValue').val().trim(),
        display: $('#identifierValue').val()
    };

    identifierEntity["identifierUseCD"] = {
        value: $('#identifierUseCD').val(),
        display: getSelectedOptionLabel('identifierUseCD')
    };

    return identifierEntity;
}

function getIdentifierForServer(identifierEntity) {
    let requestObj = {
        id: identifierEntity.identifierEntityId,
        rowVersion: identifierEntity.identifierEntityVersion,
        identifierTypeCD: identifierEntity.identifierTypeCD.value,
        identifierValue: identifierEntity.identifierValue.value,
        identifierUseCD: identifierEntity.identifierUseCD.value
    };
    return setParentIdAndReturn(requestObj);
}

function editIdentifierInTable(identifierEntity) {
    let identifierRow = $("#identifierContainer").find(`tr[data-value="${identifierEntity["identifierEntityId"]}"]`);
    $(identifierRow).attr('data-version', identifierEntity["identifierEntityVersion"]);
    $(identifierRow).children("[data-property]").each(function (index, identifierCell) {
        let propertyName = $(identifierCell).attr("data-property");
        let newPropertyValue = identifierEntity[propertyName];
        let cellDisplayValueFormatted = displayCellValueOrNe(newPropertyValue["display"]);
        $(identifierCell).attr("data-value", newPropertyValue["value"]);
        $(identifierCell).attr("title", cellDisplayValueFormatted);
        $(identifierCell).text(cellDisplayValueFormatted);
    });

    return identifierRow;
}

function addIdentifierToTable(identifierEntity) {
    let system = addNewCell("identifierTypeCD", identifierEntity["identifierTypeCD"], true);
    let value = addNewCell("identifierValue", identifierEntity["identifierValue"]);
    let use = addNewCell("identifierUseCD", identifierEntity["identifierUseCD"]);

    let identifierRow = document.createElement('tr');
    $(identifierRow)
        .attr("data-value", identifierEntity["identifierEntityId"])
        .attr("data-version", identifierEntity["identifierEntityVersion"])
        .addClass('tr edit-raw identifier-entry');

    $(identifierRow).append(system).append(value).append(use).append(createActionsCell("identifier", "identifier-actions-cell"));
    $('#identifierContainer tbody').append(identifierRow);

    return identifierRow;
}

function getIdentifiers() {
    let result = [];
    $('#identifierContainer table tbody tr').each(function (index, identifierRow) {
        let identifierEntity = getIdentifier(identifierRow);
        if (identifierEntity["identifierTypeCD"] && identifierEntity["identifierValue"]) {
            result.push(identifierEntity);
        }
    });

    return result;
}

function getIdentifier(identifierRow) {
    let identifierEntity = {
        Id: $(identifierRow).attr("data-value"),
        RowVersion: $(identifierRow).attr("data-version"),
    }

    $(identifierRow).children("[data-property]").each(function (index, identifierCell) {
        let propertyName = $(identifierCell).attr("data-property");
        let propertyValue = $(identifierCell).attr("data-value");
        identifierEntity[propertyName] = propertyValue;
    });
    return identifierEntity;
}

$(document).on('click', '.remove-identifier', function (e) {
    e.preventDefault();
    e.stopPropagation();
    let identifierRow = $(e.currentTarget).closest('tr');
    let data = {
        id: $(identifierRow).attr('data-value'),
        rowVersion: $(identifierRow).attr('data-version')
    }

    showDeleteModal(e, data.id, 'confirmDeletingIdentifier', null, data.rowVersion);
});

function confirmDeletingIdentifier() {
    let deteteSubmitButton = $('#buttonSubmitDelete');
    let requestData = {
        id: $(deteteSubmitButton).attr('data-id'),
        rowVersion: $(deteteSubmitButton).attr('data-state')
    }
    callServer({
        type: "DELETE",
        url: `/${identifierActiveEntity}/DeleteIdentifier`,
        data: requestData,
        success: function (data) {
            if (showIdentifierFeedbackMessage()) {
                toastr.success('Identifier removed successfully!');
            }
            let identifierRow = $(`.identifier-entry[data-value=${requestData.id}]`);
            $(identifierRow).remove();
            modifyTableBorder("identifierContainer", ".identifier-entry");
            updateTableEntryFormData(identifierRow, 'identifier', false);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.identifier-entry', function (e) {
    editIdentifier(e, $(this));
});

function editIdentifier(e, $el) {
    showIdentifierModal(e, "true");
    var identifierRow = $el.closest("tr");
    setIdentifierFormValues(identifierRow);
}

function showIdentifierModal(e, isEdit) {
    e.stopPropagation();
    resetIdentifierForm();
    var identifierModalTitle = document.getElementById("identifierModalTitle");
    if (isEdit == "true")
        identifierModalTitle.innerHTML = editIdentifierModalTitle;
    else
        identifierModalTitle.innerHTML = viewOrAddIdentifierModalTitle;
    $('#identifierModal')
        .attr("data-is-edit", isEdit)
        .modal('show');
}

function resetIdentifierForm() {
    resetValidation($('#identifierModal'));
    $('#identifierEntityId').val('0');
    $('#identifierEntityVersion').val('');
    $('#identifierValue').val('');
    $('#identifierTypeCD').val('');
    $('#identifierUseCD').val('');
    removeDisabledOption('identifierTypeCD');
    removeDisabledOption('identifierUseCD');
}

function setIdentifierFormValues(identifierRow) {
    $("#identifierEntityId").val($(identifierRow).attr("data-value"));
    $("#identifierEntityVersion").val($(identifierRow).attr("data-version"));
    $(identifierRow).children("[data-property]").each(function (index, addressCell) {
        let propertyName = $(addressCell).attr("data-property");
        let propertyValue = $(addressCell).attr("data-value");

        let inactiveType = inactiveIdentifierTypes.find(type => type.Id == propertyValue);
        let inactiveUseType = inactiveIdentifierUseTypes.find(useType => useType.Id == propertyValue);

        if (inactiveType)
            addInactiveOption($('#identifierTypeCD'), inactiveType.Id, inactiveType.PreferredTerm);

        if (inactiveUseType)
            addInactiveOption($('#identifierUseCD'), inactiveUseType.Id, inactiveUseType.PreferredTerm);

        $(`#${propertyName}`).val(propertyValue);
    });
}