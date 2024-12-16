var initialFormData;
var initialTrialFormData;

function addUnsavedChangesEventHandler(formSelector) {
    window.onbeforeunload = function () {
        if (!compareForms(formSelector)) {
            return "You have unsaved changes. Are you sure you want to leave?";
        }
    };
}

function addUnsavedTrialChangesEventHandler(formSelector, trialFormSelector) {
    window.onbeforeunload = function () {
        if (!compareForms(formSelector) || !compareTrialForms(trialFormSelector)) {
            return "You have unsaved changes. Are you sure you want to leave?";
        }
    };
}

function saveInitialFormData(form) {
    initialFormData = form != "#fid" ? serializeForm(form) : serializeFormInstance(form);
}

function saveInitialTrialFormData(form) {
    initialTrialFormData = serializeForm(form);
}

function compareForms(form) {
    var currentFormData = form != "#fid" ? serializeForm(form) : serializeFormInstance(form);
    return initialFormData === currentFormData;
}

function compareTrialForms(form) {
    var currentFormData = serializeForm(form);
    return initialTrialFormData === currentFormData;
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
    var formData = {};

    serializeFormInstanceInputs(form, formData);
    serializeSelects(form, formData);

    return JSON.stringify(formData);
}

function serializeInputs(form, formData) {
    $(form).find(':input').each(function () {
        if (this.type === 'checkbox') {
            formData[this.id] = this.checked ? "true" : "false";
        } else if (this.type === 'radio' && this.checked) {
            formData[$(this).val()] = $(this).val();
        } else {
            formData[this.name] = $(this).val();
        }
    });
}

function serializeFormInstanceInputs(form, formData) {
    $(form).find(':input').each(function () {
        var name = this.name;
        var originalName = name;

        if (($(this).is(':hidden') && !$(this).hasClass('audio-hid')) || $(this).attr('type') === 'submit' || this.id === 'lockDocument')
            return;

        if (formData.hasOwnProperty(name)) {
            var i = 1;
            while (formData.hasOwnProperty(name + '-' + i)) {
                i++;
            }
            name = name + '-' + i;
        }

        if (this.type === 'checkbox') {
            formData[name] = this.checked ? "true" : "false";
        } else if (this.type === 'radio' && this.checked) {
            formData[$(this).val()] = $(this).val();
        } else {
            if (name !== originalName && formData[originalName] !== "")
                formData[name] = formData[originalName];
            else
                formData[name] = $(this).val();
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
    let updatedFromEntries = formEntries.filter(el => el.id != tableRowDataId);
    if (createOrUpdateRow) {
        updatedFromEntries.push(tableRowData);
    }

    formDataRaw[entriesFormName] = updatedFromEntries;
    initialFormData = JSON.stringify(formDataRaw);
}