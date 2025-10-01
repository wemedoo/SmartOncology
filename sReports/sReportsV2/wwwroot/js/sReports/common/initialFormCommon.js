var initialFormData;
var initialSecondFormData;

function addUnsavedChangesEventHandler(formSelector) {
    window.onbeforeunload = function () {
        if (!compareForms(formSelector)) {
            return "You have unsaved changes. Are you sure you want to leave?";
        }
    };
}

function addUnsavedSecondFormChangesEventHandler(formSelector, secondFormSelector) {
    window.onbeforeunload = function () {
        if (thereAreChanges(formSelector, secondFormSelector)) {
            return "You have unsaved changes. Are you sure you want to leave?";
        }
    };
}

function addUnsavedDesignerChangesEventHandler() {
    window.onbeforeunload = function () {
        if (editorTree && formDefinitionBefore && JSON.stringify(editorTree.get()) != formDefinitionBefore) {
            return "You have unsaved changes. Are you sure you want to leave?";
        }
    };
}

function saveInitialFormData(form) {
    initialFormData = getSerializedForm(form);
}

function getSerializedForm(form) {
    return form != "#fid" ? serializeForm(form) : serializeFormInstance(form);
}

function saveInitialSecondFormData(form) {
    initialSecondFormData = serializeForm(form);
}

function compareForms(form) {
    if ($(form).length > 0) {
        var currentFormData = getSerializedForm(form);
        return initialFormData === currentFormData;
    } else {
        return true;
    }
}

function compareSecondForms(form) {
    var currentFormData = serializeForm(form);
    return initialSecondFormData === currentFormData;
}

function thereAreChanges(formSelector, secondFormSelector) {
    return !compareForms(formSelector) || !compareSecondForms(secondFormSelector);
}

function serializeForm(form) {
    var formData = {};

    serializeInputs(form, formData);
    serializeSelects(form, formData);
    serializeTags(form, formData);
    serializeClinicals(form, formData);
    serializeGroupedData(formData);

    return JSON.stringify(formData);
}

function serializeFormInstance(form) {
    if (typeof tinymce !== 'undefined') {
        tinymce.triggerSave();
    }

    var formData = getFormInstanceSubmissionJson(false);
    return JSON.stringify(formData);
}

function serializeInputs(form, formData) {
    $(form).find(':input').each(function () {
        if (this.type === 'checkbox') {
            formData[this.id] = this.checked ? "true" : "false";
        } else if (this.type === 'radio') {
            if (this.checked) {
                formData[this.name] = $(this).val();
            }
            else if (!formData[this.name]) {
                formData[this.name] = "";
            }
        } else {
            formData[this.name] = $(this).val();
        }
    });
}

function serializeSelects(form, formData) {
    $(form).find('select').each(function () {
        formData[this.name] = $(this).val();
    });
}

function serializeTags(form, formData) {
    $(form).find('.tag-values .single-tag-value').each(function () {
        var tag = $(this).data('tag');
        var language = $(this).data('language');
        var tagId = $(this).attr('data-info').split('-')[2] + '-' + tag + '-' + language;
        formData[tagId] = $(this).text();
    });
}

function serializeClinicals(form, formData) {
    $(form).find('#clinicals .clinical').each(function () {
        formData['clinical_' + $(this).data('value')] = $(this).text();
    });
}

function serializeGroupedData(formData) {
    var telecomData = serializeData("telecomsForOrganizationTelecom", "telecom-entry");
    var telecomPatientData = serializeData("telecomsForPatientTelecom", "telecom-entry");
    var telecomPatientContactData = serializeData("telecomsForPatientContactTelecom", "telecom-entry");
    var identifierData = serializeData("identifierContainer", "identifier-entry");
    var addressPatientData = serializeData("patientAddresses", "address-entry");
    var addressPersonnelData = serializeData("personnelAddresses", "address-entry");
    var addressContactPerson = serializeData("patientContactAddresses", "address-entry");
    var contactData = serializeData("contacts", "contact-entry");
    var associationData = serializeAssociations("associationContainer", "association-entry");
    
    formData['telecom'] = telecomData;
    formData['telecomPatient'] = telecomPatientData;
    formData['telecomPatientContact'] = telecomPatientContactData;
    formData['identifier'] = identifierData;
    formData['addressesPersonnel'] = addressPersonnelData;
    formData['addressesPatient'] = addressPatientData;
    formData['addressesContactPerson'] = addressContactPerson;
    formData['contacts'] = contactData;
    formData['associations'] = associationData;
}

function serializeData(tableId, className) {
    var tableData = [];
    $('#' + tableId + ' tr.' + className).each(function () {
        tableData.push(serializeTableRow(this));
    });
    return tableData;
}

function serializeTableRow(tableRow) {
    var entryData = {};
    entryData['id'] = $(tableRow).data('value');
    $(tableRow).find('td:not(:last-child)').each(function () {
        var propertyName = $(this).data('property');
        if (propertyName) {
            entryData[propertyName] = $(this).text().trim();
        }
    });
    return entryData;
}

function serializeAssociations(tableId, className) {
    var telecomData = [];
    $('#' + tableId + ' .' + className).each(function () {
        var entryData = {};
        $(this).find('td:not(:last-child)').each(function () {
            var propertyName = $(this).data('property');
            if (propertyName) {
                entryData[propertyName] = $(this).text().trim();
            }
        });
        telecomData.push(entryData);
    });
    return telecomData;
}

function updateTableEntryFormData(tableRow, entriesFormName, createOrUpdateRow) {
    let formDataRaw = JSON.parse(initialFormData);
    let tableRowData = serializeTableRow(tableRow);
    let tableRowDataId = tableRowData.id;
    let formEntries = formDataRaw[entriesFormName] ?? [];
    let indexOfEditedEntry = formEntries.map(el => el.id).indexOf(tableRowDataId);
    if (createOrUpdateRow) {
        if (indexOfEditedEntry == -1) {
            formEntries.push(tableRowData);
        } else {
            formEntries[indexOfEditedEntry] = tableRowData;
        }
    } else {
        if (indexOfEditedEntry != -1) {
            formEntries.splice(indexOfEditedEntry, 1);
        }
    }

    initialFormData = JSON.stringify(formDataRaw);
}

function unsavedChangesCheck(formId, confirmCallBack, noChangesCallback) {
    let stopExecution = false;
    if (!compareForms(formId)) {
        if (confirm("You have unsaved changes. Are you sure you want to cancel?")) {
            saveInitialFormData(formId);
            executeCallback(confirmCallBack);
        } else {
            stopExecution = true;
        }
    } else {
        executeCallback(noChangesCallback);
    }
    return stopExecution;
}