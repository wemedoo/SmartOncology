var activeAddressContainer = '';

function setActiveAddressContainer(containerId) {
    activeAddressContainer = containerId;
}

function addAddress(e, containerId) {
    showAddressModal(e, containerId, "false");
    initCountryComponent();
}

function showAddressModal(e, containerId, isEdit) {
    e.stopPropagation();
    setActiveAddressContainer(containerId);
    resetAddressForm();
    var addressModalTitle = document.getElementById("addressModalTitle");
    if (isEdit == "true")
        addressModalTitle.innerHTML = editAddressModalTitle;
    else
        addressModalTitle.innerHTML = viewOrAddAddressModalTitle;
    $('#addressModal')
        .attr("data-is-edit", isEdit)
        .modal('show');
}

function resetAddressForm() {
    resetValidation($('#newAddressForm'));
    $('#addressEntityId').val('0');
    $('#addressEntityVersion').val('');
    $('#city').val('');
    $('#postalCode').val('');
    $('#state').val('');
    $('#country').val('');
    $('#street').val('');
    $('#addressType').val('');
    $('#countryCD').val('').trigger("change");
    removeDisabledOption('addressType');
}

function submitAddress(e) {
    updateDisabledOptions(false);
    e.preventDefault();
    e.stopPropagation();
    if ($('#newAddressForm').valid()) {
        let addressEntity = getAddressFromForm();

        if (parentEntryExisting(getAddressParentId())) {
            let isEdit = $('#addressModal').attr("data-is-edit") === "true";

            callServer({
                type: "POST",
                url: `/${addressActiveEntity}/${getSubmitAddressEndpoint(isEdit)}`,
                data: getAddressForServer(addressEntity),
                success: function (data) {
                    addressEntity.addressEntityVersion = data.rowVersion;
                    let addressRow;
                    if (isEdit) {
                        addressRow = editAddressInTable(addressEntity);
                    } else {
                        addressEntity.addressEntityId = data.id;
                        addressRow = addAddressToTable(addressEntity);
                    }
                    updateTableEntryFormData(addressRow, getFormTrackingAddressPropertyName(), true);
                    
                    handleModalAfterSubmitting(activeAddressContainer, 'address-entry', 'addressModal', function () {
                        setActiveAddressContainer('');
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr);
                }
            });
        } else {
            addAddressToTable(addressEntity);
            let parentFormHandler = submitAddressParentFormHandler();
            handleModalAfterSubmitting(activeAddressContainer, 'address-entry', 'addressModal', function () {
                setActiveAddressContainer('');
            });
            parentFormHandler();
        }
    }
}

function addAddressToTable(addressEntity) {
    let cityEl = addNewCell("city", addressEntity["city"], true);
    let postalCodeEl = addNewCell("postalCode", addressEntity["postalCode"]);
    let stateEl = addNewCell("state", addressEntity["state"]);
    let countryEl = addNewCell("country", addressEntity["country"]);
    let streetEl = addNewCell("street", addressEntity["street"]);

    let addressRow = document.createElement('tr');
    $(addressRow)
        .attr("data-value", addressEntity["addressEntityId"])
        .attr("data-version", addressEntity["addressEntityVersion"])
        .attr("data-addressType", addressEntity["addressType"]["value"])
        .attr("data-countryCD", addressEntity["country"]["value"])
        .addClass('address-entry');

    $(addressRow)
        .append(cityEl)
        .append(postalCodeEl)
        .append(stateEl)
        .append(countryEl)
        .append(streetEl)
        .append(createActionsCell("address"));
    $(`#${activeAddressContainer} tbody`).append(addressRow);

    return addressRow;
}

function editAddressInTable(addressEntity) {
    let addressRow = $(`#${activeAddressContainer}`).find(`tr[data-value="${addressEntity["addressEntityId"]}"]`);
    $(addressRow)
        .attr("data-version", addressEntity["addressEntityVersion"])
        .attr("data-addressType", addressEntity["addressType"]["value"])
        .attr("data-countryCD", addressEntity["country"]["value"])
    $(addressRow).children("[data-property]").each(function (index, addressCell) {
        let propertyName = $(addressCell).attr("data-property");
        let newPropertyValue = addressEntity[propertyName];
        $(addressCell).attr("data-value", newPropertyValue["value"]);
        $(addressCell).text(displayCellValueOrNe(newPropertyValue["display"]));
    });

    return addressRow;
}

function getAddressFromForm() {
    let addressEntity = {
        addressEntityId: $("#addressEntityId").val(),
        addressEntityVersion: $("#addressEntityVersion").val(),
    };

    addressEntity["city"] = {
        value: $('#city').val(),
        display: $('#city').val()
    };

    addressEntity["postalCode"] = {
        value: $('#postalCode').val(),
        display: $('#postalCode').val()
    };

    addressEntity["state"] = {
        value: $('#state').val(),
        display: $('#state').val()
    };

    addressEntity["street"] = {
        value: $('#street').val(),
        display: $('#street').val()
    };

    addressEntity["addressType"] = {
        value: $('#addressType').val(),
        display: $('#addressType').val()
    };

    addressEntity["country"] = {
        value: $('#countryCD').val(),
        display: getSelectedSelect2Label("countryCD")
    };

    return addressEntity;
}

function getAddressForServer(addressEntity) {
    let requestObj = {
        id: addressEntity.addressEntityId,
        rowVersion: addressEntity.addressEntityVersion,
        city: addressEntity.city.value,
        postalCode: addressEntity.postalCode.value,
        state: addressEntity.state.value,
        street: addressEntity.street.value,
        addressTypeCD: addressEntity.addressType.value,
        countryCD: addressEntity.country.value
    };
    return setAddressParentIdAndReturn(requestObj);
}

function getAddresses(targetAddressContainer) {
    let result = [];
    $(`#${targetAddressContainer} table tbody tr`).each(function (index, addressRow) {
        result.push(getAddress(addressRow));
    });
    return result;
}

function getAddress(addressRow) {
    let addressEntity = {
        Id: $(addressRow).attr("data-value"),
        RowVersion: $(addressRow).attr("data-version"),
        CountryCD: $(addressRow).attr("data-countryCD"),
        AddressTypeCD: $(addressRow).attr("data-addressType")
    }

    $(addressRow).children("[data-property]").each(function (index, addressCell) {
        let propertyName = $(addressCell).attr("data-property");
        let propertyValue = $(addressCell).attr("data-value");
        addressEntity[propertyName] = propertyValue;
    });
    return addressEntity;
}

$(document).on('click', '.remove-address', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var containerId = getActiveContainer(this, 'address');
    setActiveAddressContainer(containerId);

    let addressRow = $(this).closest('tr');
    let data = {
        id: $(addressRow).attr('data-value'),
        rowVersion: $(addressRow).attr('data-version')
    }

    showDeleteModal(e, data.id, 'confirmDeletingAddress', null, data.rowVersion);
});

function confirmDeletingAddress() {
    let deteteSubmitButton = $('#buttonSubmitDelete');
    let requestData = {
        id: $(deteteSubmitButton).attr('data-id'),
        rowVersion: $(deteteSubmitButton).attr('data-state')
    }
    callServer({
        type: "DELETE",
        url: `/${addressActiveEntity}/${getDeleteAddressEndpoint()}`,
        data: requestData,
        success: function (data) {
            let addressRow = $(`#${activeAddressContainer} .address-entry[data-value=${requestData.id}]`);
            $(addressRow).remove();
            modifyTableBorder(activeAddressContainer, ".address-entry");
            updateTableEntryFormData(addressRow, getFormTrackingAddressPropertyName(), false);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.address-entry', function (e) {
    editAddress(e, $(this));
});

function editAddress(e, $el) {
    var containerId = getActiveContainer($el, 'address');
    showAddressModal(e, containerId, "true");
    var addressRow = $el.closest("tr");
    setAddressFormValues(addressRow);
}

function setAddressFormValues(addressRow) {
    $("#addressEntityId").val($(addressRow).attr("data-value"));
    $("#addressEntityVersion").val($(addressRow).attr("data-version"));
    $("#addressType").val($(addressRow).attr("data-addressType"));
    initCountryComponent($(addressRow).attr("data-countryCD"));

    $(addressRow).children("[data-property]").each(function (index, addressCell) {
        let propertyName = $(addressCell).attr("data-property");
        let propertyValue = $(addressCell).attr("data-value");
        $(`#${propertyName}`).val(propertyValue);
    });

    let propertyValue = $(addressRow).attr("data-addressType");
    let inactiveType = inactiveAddressTypes.find(type => type.Id == propertyValue);
    if (inactiveType)
        addInactiveOption($('#addressType'), inactiveType.Id, inactiveType.PreferredTerm);
}

function initCountryComponent(countryCD) {
    initCodeSelect2(countryCD, countryCD, "countryCD", "country", "Country", "addressModal");
}

function isContactAddressForm() {
    return activeAddressContainer.toLowerCase().includes('contact'); 
}

function setAddressParentIdAndReturn(addressEntity) {
    return isContactAddressForm() ? setContactIdAndReturn(addressEntity) : setParentIdAndReturn(addressEntity);
}

function getAddressParentId() {
    return isContactAddressForm() ? getContactId() : getParentId();
}

function submitAddressParentFormHandler() {
    return isContactAddressForm() ? submitPatientContact : submitParentForm;
}

function getSubmitAddressEndpoint(isEdit) {
    return `${isEdit ? 'Edit' : 'Create'}${isContactAddressForm() ? 'Contact' : ''}Address`
}

function getDeleteAddressEndpoint() {
    return `Delete${isContactAddressForm() ? 'Contact' : ''}Address`
}

function getFormTrackingAddressPropertyName() {
    return {
        "patientAddresses": "addressesPatient",
        "patientContactAddresses": "addressesContactPerson",
        "personnelAddresses": "addressesPersonnel",
    }[activeAddressContainer];
}