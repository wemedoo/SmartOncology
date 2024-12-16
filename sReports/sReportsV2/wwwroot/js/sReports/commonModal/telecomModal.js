var activeTelecomContainer = '';

function setActiveTelecomContainer(containerId) {
    activeTelecomContainer = containerId;
}
function showTelecomModal(e, container, isEdit) {
    e.stopPropagation();
    setActiveTelecomContainer(container);
    var telecomModalTitle = document.getElementById("telecomModalTitle");
    resetTelecomForm();
    if (isEdit == "true")
        telecomModalTitle.innerHTML = editTelecomModalTitle;
    else
        telecomModalTitle.innerHTML = viewOrAddTelecomModalTitle;
    $('#telecomModal')
        .attr("data-is-edit", isEdit)
        .modal('show');
}

function resetTelecomForm() {
    resetValidation($('#newTelecomForm'));
    $('#telecomEntityId').val('0');
    $('#telecomEntityVersion').val('');
    $('#system').val('');
    $('#value').val('');
    $('#use').val('');
    removeDisabledOption('use');
    removeDisabledOption('system');
}

function addNewTelecom(e) {
    updateDisabledOptions(false);

    e.preventDefault();
    e.stopPropagation();

    if ($('#newTelecomForm').valid()) {
        let telecomEntity = getTelecomFromForm();

        if (parentEntryExisting(getTelecomParentId())) {
            let isEdit = $('#telecomModal').attr("data-is-edit") === "true";

            callServer({
                type: "POST",
                url: `/${telecomActiveEntity}/${getSubmitTelecomEndpoint(isEdit)}`,
                data: getTelecomForServer(telecomEntity),
                success: function (data) {
                    telecomEntity.telecomEntityVersion = data.rowVersion;
                    let telecomRow;
                    if (isEdit) {
                        telecomRow = editTelecomInTable(telecomEntity);
                    } else {
                        telecomEntity.telecomEntityId = data.id;
                        telecomRow = addNewTelecomToTable(telecomEntity);
                    }
                    updateTableEntryFormData(telecomRow, getFormTrackingTelecomPropertyName(), true);

                    handleModalAfterSubmitting(activeTelecomContainer, 'edit-raw', 'telecomModal', function () {
                        setActiveTelecomContainer('');
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr);
                }
            });

        } else {
            addNewTelecomToTable(telecomEntity);
            let parentFormHandler = submitTelecomParentFormHandler();
            handleModalAfterSubmitting(activeTelecomContainer, 'edit-raw', 'telecomModal', function () {
                setActiveTelecomContainer('');
            });
            parentFormHandler();
        }
    }
}

function addNewTelecomToTable(telecomEntity) {
    let system = addNewCell("system", telecomEntity["system"], true);
    let value = addNewCell("value", telecomEntity["value"]);
    let use = addNewCell("use", telecomEntity["use"]);

    let telecomRow = document.createElement('tr');
    $(telecomRow).addClass('edit-raw');
    $(telecomRow)
        .attr("data-value", telecomEntity["telecomEntityId"])
        .attr("data-version", telecomEntity["telecomEntityVersion"])
        .addClass('tr edit-raw telecom-entry');
    $(telecomRow).append(system).append(value).append(use).append(createActionsCell("telecom", "telecom-actions-cell"));
    $(`#${activeTelecomContainer} tbody`).append(telecomRow);

    return telecomRow;
}

function editTelecomInTable(telecomEntity) {
    let telecomRow = $(`#${activeTelecomContainer}`).find(`tr[data-value="${telecomEntity["telecomEntityId"]}"]`);
    $(telecomRow)
        .attr("data-version", telecomEntity["telecomEntityVersion"]);
    $(telecomRow).children("[data-property]").each(function (index, telecomCell) {
        let propertyName = $(telecomCell).attr("data-property");
        let newPropertyValue = telecomEntity[propertyName];
        $(telecomCell).attr("data-value", newPropertyValue["value"]);
        $(telecomCell).text(displayCellValueOrNe(newPropertyValue["display"]));
    });

    return telecomRow;
}

function getTelecomFromForm() {
    let telecomEntity = {
        telecomEntityId: $("#telecomEntityId").val(),
        telecomEntityVersion: $("#telecomEntityVersion").val()
    };

    telecomEntity["system"] = {
        value: $('#system').val(),
        display: getSelectedOptionLabel('system')
    };

    telecomEntity["value"] = {
        value: $('#value').val(),
        display: $('#value').val()
    };

    telecomEntity["use"] = {
        value: $('#use').val(),
        display: getSelectedOptionLabel('use')
    };

    return telecomEntity;
}

function getTelecomForServer(telecomEntity) {
    let requestObj = {
        id: telecomEntity.telecomEntityId,
        rowVersion: telecomEntity.telecomEntityVersion,
        systemCD: telecomEntity.system.value,
        value: telecomEntity.value.value,
        useCD: telecomEntity.use.value
    };
    return setTelecomParentIdAndReturn(requestObj);
}

function getTelecoms(container) {
    let result = [];
    $(`#telecomsFor${container} table tr`).each(function (index, element) {
        let telecomEntity = getTelecom(element);
        if (telecomEntity["systemCD"] && telecomEntity["value"]) {
            result.push(telecomEntity);
        }
    });

    return result;
}

function getTelecom(telecomRow) {
    let telecomEntity = {
        Id: $(telecomRow).attr("data-value"),
        RowVersion: $(telecomRow).attr("data-version")
    }

    $(telecomRow).children("[data-property]").each(function (index, telecomCell) {
        let propertyName = $(telecomCell).attr("data-property");
        let propertyValue = $(telecomCell).attr("data-value");
        if (propertyName == "system")
            propertyName = "systemCD";
        else if (propertyName == "use")
            propertyName = "useCD";
        telecomEntity[propertyName] = propertyValue;
    });
    return telecomEntity;
}

$(document).on('click', '.remove-telecom', function (e) {
    e.preventDefault();
    e.stopPropagation();

    let telecomRow = $(e.currentTarget).closest('tr');
    let data = {
        id: $(telecomRow).attr('data-value'),
        rowVersion: $(telecomRow).attr('data-version')
    }
    var containerId = getActiveContainer(this, 'telecom');
    setActiveTelecomContainer(containerId);
    showDeleteModal(e, data.id, 'confirmDeletingTelecom', null, data.rowVersion);
});

function confirmDeletingTelecom() {
    let deteteSubmitButton = $('#buttonSubmitDelete');
    let requestData = {
        id: $(deteteSubmitButton).attr('data-id'),
        rowVersion: $(deteteSubmitButton).attr('data-state')
    }
    callServer({
        type: "DELETE",
        url: `/${telecomActiveEntity}/${getDeleteTelecomEndpoint()}`,
        data: requestData,
        success: function (data) {
            let telecomRow = $(`#${activeTelecomContainer} .telecom-entry[data-value=${requestData.id}]`);
            $(telecomRow).remove();
            modifyTableBorder(activeTelecomContainer, ".edit-raw");
            updateTableEntryFormData(telecomRow, getFormTrackingTelecomPropertyName(), false);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.telecom-entry', function (e) {
    editTelecom(e, $(this));
});

function editTelecom(e, telecomEntity) {
    var telecomRow = telecomEntity.closest("tr");
    var containerId = getActiveContainer(telecomRow, 'telecom')
    showTelecomModal(e, containerId, "true");
    setTelecomFormValues(telecomRow);
}

function setTelecomFormValues(telecomRow) {
    $("#telecomEntityId").val($(telecomRow).data("value"));
    $("#telecomEntityVersion").val($(telecomRow).attr("data-version"));
    $(telecomRow).children("[data-property]").each(function (index, telecomCell) {
        let propertyName = $(telecomCell).data("property");
        let propertyValue = $(telecomCell).data("value");

        let inactiveSystem = inactiveTelecomSystems.find(system => system.Id == propertyValue);
        let inactiveUseType = inactiveTelecomUseTypes.find(useType => useType.Id == propertyValue);

        if (inactiveSystem)
            addInactiveOption($('#system'), inactiveSystem.Id, inactiveSystem.PreferredTerm);

        if (inactiveUseType)
            addInactiveOption($('#use'), inactiveUseType.Id, inactiveUseType.PreferredTerm);

        $(`#${propertyName}`).val(propertyValue);
    });
}

function isContactTelecomForm() {
    return activeTelecomContainer.toLowerCase().includes('contact');
}

function setTelecomParentIdAndReturn(telecomEntity) {
    return isContactTelecomForm() ? setContactIdAndReturn(telecomEntity) : setParentIdAndReturn(telecomEntity);
}

function getTelecomParentId() {
    return isContactTelecomForm() ? getContactId() : getParentId();
}

function submitTelecomParentFormHandler() {
    return isContactTelecomForm() ? submitPatientContact : submitParentForm;
}

function getSubmitTelecomEndpoint(isEdit) {
    return `${isEdit ? 'Edit' : 'Create'}${isContactTelecomForm() ? 'Contact' : ''}Telecom`
}

function getDeleteTelecomEndpoint() {
    return `Delete${isContactTelecomForm() ? 'Contact' : ''}Telecom`
}

function getFormTrackingTelecomPropertyName() {
    return {
        "telecomsForOrganizationTelecom": "telecom",
        "telecomsForPatientTelecom": "telecomPatient",
        "telecomsForPatientContactTelecom": "telecomPatientContact",
    }[activeTelecomContainer];
}