function showContact(contactId, readOnly) {
    callServer({
        type: "GET",
        url: `/Patient/EditPatientContactInfo?contactId=${contactId}&isReadOnlyViewMode=${readOnly}`,
        success: function (data) {
            $("#contactDetail").html(data);
            setCommonValidatorMethods();
            saveInitialFormData("#idPatientInfo");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.remove-contact', function (e) {
    e.preventDefault();
    e.stopPropagation();
    let contactRow = $(e.currentTarget).closest('tr');
    let data = {
        id: $(contactRow).attr('data-value'),
        rowVersion: $(contactRow).attr('data-version')
    }

    showDeleteModal(e, data.id, 'confirmDeletingContact', null, data.rowVersion);
});

function confirmDeletingContact() {
    let deteteSubmitButton = $('#buttonSubmitDelete');
    let requestData = {
        id: $(deteteSubmitButton).attr('data-id'),
        rowVersion: $(deteteSubmitButton).attr('data-state')
    }
    callServer({
        type: "DELETE",
        url: '/Patient/DeleteContact',
        data: requestData,
        success: function (data) {
            let contactRow = $(`.contact-entry[data-value=${requestData.id}]`);
            $(contactRow).remove();
            updateTableEntryFormData(contactRow, 'contacts', false);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.contact-entry', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var $closestRow = $(this).closest("tr");
    var contactId = $closestRow.attr("data-value");
    var readOnly = $closestRow.attr("data-readonly");
    showContact(contactId, readOnly);
});

function resetPatientContactForm() {
    $("#contactDetail").html("");
}

function getContacts() {
    let result = [];
    $(`#contacts table tbody tr`).each(function (index, contactRow) {
        var contact = getContact(contactRow);
        if (contact) {
            result.push(contact);
        }
    });
    return result;
}

function getContact(contactRow) {
    let dataSet = $(contactRow).attr("data-content");

    if (dataSet) {
        return JSON.parse(decodeURIComponent(removeEncodedPlusForWhitespace(dataSet)));
    } else {
        return "";
    }
}

function submitPatientContact() {
    updateDisabledOptions(false);
    let contactEntityForServer = getContactFromFormForServer();

    if (contactEntityForServer["patientId"] != '0') {
        callServer({
            type: "POST",
            url: "/Patient/CreateContactInfo",
            data: contactEntityForServer,
            success: function (data) {
                let patientContactId = data.id;
                getContactRow(patientContactId, +contactEntityForServer["Id"]);
                resetPatientContactForm();
                updateDisabledOptions(true);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    } else {
        let contactEntity = getContactFromFormForTable();
        let contactEntityEncoded = encodeURIComponent(JSON.stringify(contactEntityForServer));
        addContactToTable(contactEntity, contactEntityEncoded);
        submitParentForm();
    }
}

function getContactFromFormForServer() {
    var contact =
    {
        Id: getContactId(),
        NameGiven: $("#contactName").val(),
        NameFamily: $("#contactFamily").val(),
        BirthDate: toDateStringIfValue($("#contactBirthDate").val()),
        GenderId: $("#contactGender").val(),
        ContactRelationshipId: $("#contactRelationship").val(),
        ContactRoleId: $("#contactRole").val(),
        Addresses: getAddresses("patientContactAddresses"),
        Telecoms: getTelecoms('PatientContactTelecom'),
        patientId: getParentId()
    };
    
    return contact;
}

function getContactFromFormForTable() {
    var contactEntity =
    {
        Id: getContactId(),
    };

    contactEntity["NameGiven"] = {
        value: $('#contactName').val(),
        display: $('#contactName').val()
    };

    contactEntity["NameFamily"] = {
        value: $('#contactFamily').val(),
        display: $('#contactFamily').val()
    };

    contactEntity["ContactRelationship"] = {
        value: $("#contactRelationship").val(),
        display: $('#contactRelationship').find(":selected").text()
    };

    contactEntity["Gender"] = {
        value: $("#contactGender").val(),
        display: $('#contactGender').find(":selected").text()
    };

    return contactEntity;
}

function addContactToTable(contactEntity, contactEntityEncoded) {
    let nameGiven = addNewCell("NameGiven", contactEntity["NameGiven"], true);
    let nameFamily = addNewCell("NameFamily", contactEntity["NameFamily"]);
    let gender = addNewCell("Gender", contactEntity["Gender"]);
    let contactRelationship = addNewCell("ContactRelationship", contactEntity["ContactRelationship"]);

    let contact = document.createElement('tr');
    $(contact)
        .attr("data-value", contactEntity["Id"])
        .attr("data-content", contactEntityEncoded)
        .attr("data-readonly", "false")
        .addClass('contact-entry');

    $(contact)
        .append(nameGiven)
        .append(nameFamily)
        .append(gender)
        .append(contactRelationship)
        .append(createActionsCell("contact"));
    $(`#contacts tbody`).append(contact);
}

function getContactRow(contactId, isContactEdited) {
    let readOnly = false;

    callServer({
        type: "GET",
        url: `/Patient/GetPatientContactRow?contactId=${contactId}&isReadOnlyViewMode=${readOnly}`,
        success: function (contactHtmlData) {
            if (isContactEdited) {
                let contactRow = $("#contacts").find(`tr[data-value="${contactId}"]`);
                $(contactRow).replaceWith(contactHtmlData);
            } else {
                $(`#contacts tbody`).append(contactHtmlData);
            }
            saveInitialFormData("#idPatientInfo");
            //updateTableEntryFormData(contactHtmlData, 'contacts', true);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getContactId() {
    return $("#contactId").val();
}

function setContactIdAndReturn(addressEntity) {
    addressEntity["patientContactId"] = getContactId();
    return addressEntity;
}