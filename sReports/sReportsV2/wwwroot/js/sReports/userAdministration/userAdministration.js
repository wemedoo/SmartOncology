var activeContainerId = "personalData";
var isUserAdministration;
var isReadOnly;
var userCountryId;

function setUserAdministration(userAdministration) {
    isUserAdministration = userAdministration;
}

function setReadOnly(readOnly) {
    isReadOnly = readOnly;
}

function submitUserForm(form, e) {
    e.preventDefault();
    e.stopPropagation();
    submitData();
}

function submitPersonalData(callback) {
    updateDisabledOptions(false);
    let form = $("#idUserInfo");
    $(form).validate({
        ignore: []
    });

    if ($(form).valid() && $("#userInfo").find('.fa-times-circle').length === 0) {
        var request = {};

        let userId = getParentId();
        let action = userId != 0 ? 'Edit' : 'Create';
        request['Id'] = userId;
        request['Username'] = $("#username").val();
        request['FirstName'] = $("#firstName").val();
        request['LastName'] = $("#lastName").val();
        request['PrefixCD'] = $('#prefix').val();
        request['PersonnelTypeCD'] = $('#personnelType').val();
        request['Email'] = $('#email').val();
        request['PersonalEmail'] = $('#personalEmail').val();

        request["MiddleName"] = $("#middleName").val();
        request["AcademicPositions"] = getSelectedAcademicPositions();
        request["Addresses"] = getAddresses("personnelAddresses");
        request["Identifiers"] = getIdentifiers();
        request["DayOfBirth"] = toDateStringIfValue($("#dayOfBirth").val());
        request["PersonnelPositions"] = getPersonnelPositions();
        request["PersonnelOccupation"] = getPersonnelOccupations();

        removeCustomValidators();

        callServer({
            type: "POST",
            url: `/UserAdministration/${isUserAdministration ? action : 'UpdateUserProfile'}`,
            data: request,
            success: function (data) {
                updateAfterNewEntryIsCreated(request, data.id);
                updateIdAndRowVersion(data);
                toastr.success(data.message);
                enableChangeTab(+request["Id"]);
                if ($("#registrationType").val() == "Quick")
                    showUserBasicInfo(data.id, data.password);
                updateDisabledOptions(true);
                validateCustomUserInfo();
                userDataAreSaved(callback);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    var errors = $('.error').get();
    if (errors.length !== 0) {
        $.each(errors, function (index, error) {
            $(error).closest('.collapse').collapse("show");
        });
    };
}

function updateAfterNewEntryIsCreated(request, systemAndUserId) {
    let isEdit = +request["Id"];
    if (!isEdit) {
        $("#systemId").val(systemAndUserId);
        $("#systemIdInput").removeClass("d-none");
        history.replaceState({}, '', `/UserAdministration/Edit?userId=${systemAndUserId}`);
        $('.breadcrumb-active').html(`<a>${request['Username']}</a>`);
    }
}

function getOrganizations() {
    let institutions = [];
    $("#institutions").find('.institution-container').each(function (index, element) {
        let organizationId = $(element).attr('id').split('-')[1];
        institutions.push(getOrganization(organizationId));
    });

    return institutions;
}

function getOrganization(organizationId) {
    let institution = {};
    institution["IsPracticioner"] = $(`[name=isPractitioner-${organizationId}]:checked`).val();
    institution["Qualification"] = $(`#qualification-${organizationId}`).val();
    institution["SeniorityLevel"] = $(`#seniority-${organizationId}`).val();
    institution["Speciality"] = $(`#speciality-${organizationId}`).val();
    institution["SubSpeciality"] = $(`#subspeciality-${organizationId}`).val();
    institution["OrganizationId"] = organizationId;
    institution["StateCD"] = $(`#organizationState-${organizationId}`).val();

    return institution;
}

function getSelectedAcademicPositions() {
    var chkArray = [];

    $("#academicPosition option:selected").each(function () {
        chkArray.push({
            "Id": $(this).attr("data-id"),
            "AcademicPositionId": $(this).val()
        });
    });

    return chkArray;
}

function getPersonnelPositions() {
    var chkArray = [];

    $("#roles option:selected").each(function () {
        chkArray.push({
            PositionCD: $(this).val()
        });
    });

    return chkArray;
}

function cancelUserEdit() {
    unsavedChangesCheck("#idUserInfo",
        function () {
            window.location.href = isUserAdministration ? '/UserAdministration/GetAll' : '/Home/Index';
        },
        function () {
            window.location.href = isUserAdministration ? '/UserAdministration/GetAll' : '/Home/Index';
        }
    );
}

$(document).ready(function () {
    validateCustomUserInfo();
    $('.vertical-line-user').each(function (index, element) {
        let count = $(element).closest('.child').children('.child').length;
        if (count == 1) {
            $(element).css('height', '26px');
        }
    });

    $('#registrationType').change(function () {
        var selectedOption = $(this).val();

        if (selectedOption === 'Quick') {
            $('#email').prop('disabled', true).removeAttr('required');
            $('#emailRequired').hide();
            $('#emailValid').remove();
            $('#email-error').remove();
            $('#email').removeClass('error');
        } else {
            $('#email').prop('disabled', false).attr('required', 'required');
            $('#emailRequired').show();
        }
    });

    $('.sreports-select2-multiple').initSelect2(
        getSelect2Object({
            width: '100%',
            allowClear: false,
            minimumInputLength: 0
        })
    );
    initializeAcademicPositions();
    initializeInactiveAcademicPositions();
    initializeRoles();
    initializeInactiveRoles()
    reloadChildren();

    var personnelSeniorityField = $('#personnelSeniority');
    var requiredDiv = document.createElement("div");
    requiredDiv.className = "label-required";
    requiredDiv.textContent = "*";

    if ($("#occupationSubCategory").val() == $('#medicalDoctorCodeId').val())
        setPersonnelSeniorityToRequired(personnelSeniorityField, requiredDiv);

    $('#occupationSubCategory').on('change', function () {
        var selectedValue = $(this).val();

        if (selectedValue === $('#medicalDoctorCodeId').val())
            setPersonnelSeniorityToRequired(personnelSeniorityField, requiredDiv);
        else
            hidePersonnelSeniority(personnelSeniorityField);
    });

    saveInitialFormData("#idUserInfo");
    addUnsavedChangesEventHandler("#idUserInfo");
});

function validateCustomUserInfo() {
    $.validator.addMethod("validEmailFormat", function (value, element) {
        return isValidEmailFormat(value);
    }, "Please enter a valid email address.");

    $.validator.addMethod("registeredEmail", function (value, element) {
        if (!isValidEmailFormat(value)) {
            return true;
        }
        return emailExist(value);
    }, "This email is already associated with another user.");

    $.validator.addMethod("registeredUsername", function (value, element) {
        return usernameExist(value);
    }, "This username is already associated with another user.");

    $("#idUserInfo").validate({});
    $('[name="Email"]').each(function () {
        $(this).rules('add', {
            registeredEmail: true,
            validEmailFormat: true
        });
    });

    $('[name="PersonalEmail"]').each(function () {
        $(this).rules('add', {
            validEmailFormat: true
        });
    });

    $('[name="Username"]').each(function () {
        $(this).rules('add', {
            registeredUsername: true
        });
    });
}

function isValidEmailFormat(value) {
    value = value.trim().toLowerCase();
    return value === '' ||
        (/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(value) &&
            value.length <= 254 && value.split('@')[1].includes('.'));
}

function removeCustomValidators() {
    $('[name="Email"]').each(function () {
        $(this).rules('remove', 'registeredEmail');
    });
    $('[name="Username"]').each(function () {
        $(this).rules('remove', 'registeredUsername');
    });
}

function emailExist(email) {
    let result = false;
    let request = {
        email: email,
        userId: getParentId()
    };
    callServer({
        type: 'GET',
        data: request,
        url: `/UserAdministration/CheckEmail`,
        async: false,
        success: function (data) {
            $("#emailValid").addClass("fa-check-circle");
            $("#emailValid").removeClass("fa-times-circle");
            result = true;
        },
        error: function (xhr, textStatus, thrownError) {
            $("#emailValid").addClass("fa-times-circle");
            $("#emailValid").removeClass("fa-check-circle");
        }
    });
    return result;
}

function usernameExist(username) {
    let result = false;
    let request = {
        username: username,
        userId: getParentId()
    };
    callServer({
        type: 'GET',
        data: request,
        url: `/UserAdministration/CheckUsername`,
        async: false,
        success: function (data) {
            $("#usernameValid").addClass("fa-check-circle");
            $("#usernameValid").removeClass("fa-times-circle");
            result = true;
        },
        error: function (xhr, textStatus, thrownError) {
            $("#usernameValid").addClass("fa-times-circle");
            $("#usernameValid").removeClass("fa-check-circle");
        }
    });
    return result;
}

$(document).on('click', '.personnel-tab', function (e) {
    if ($(this).hasClass('tab-disabled')) return;

    let $el = $(this);
    saveIfThereAreChanges(function () {
        $('.personnel-tab').removeClass('active');

        $el.addClass('active');
        $('.user-cont').hide();

        let containerId = $el.attr("data-id");
        toggleSaveBtn(containerId);

        activeContainerId = containerId;
        $(`#${containerId}`).show();
        handleArrowVisibility(activeContainerId);
    });
});

function saveIfThereAreChanges(callback) {
    if (compareForms("#idUserInfo")) {
        executeCallback(callback);
    } else {
        submitData(callback);
    }
}

function toggleSaveBtn(containerId) {
    if (containerId === "institutionData" || containerId === "identifierData") {
        $(`#buttonGroupPrimary`).show();
        if (isUserAdministration) {
            $(`#buttonGroupPrimary`).find("button").show();
        } else {
            $(`#buttonGroupPrimary`).find("button").hide();
        }
    } else {
        $(`#buttonGroupPrimary`).show();
        $(`#buttonGroupPrimary`).find("button").show();
    }
}

function handleArrowVisibility(activeContainerId) {
    switch (activeContainerId) {
        case "personalData": {
            $('.user-arrow-right').show();
            $('.user-arrow-left').hide();
            return true;
        }
        case "identifierData": 
        case "institutionData": {
            $('.user-arrow-right').show();
            $('.user-arrow-left').show();
            return true;
        }
        default:
    }
}

function submitData(callback) {
    if (isReadOnly) return;
    switch (activeContainerId) {
        case "personalData":
            return submitPersonalData(callback);
        case "institutionData":
            $('#registrationTypeId').remove();
            return submitInstitutionalData(callback);
        case "identifierData":
            $('#registrationTypeId').remove();
            return submitIdentifierData(callback);
        default:
            break;
    }
}

function submitInstitutionalData(callback) {
    if (isUserAdministration) {
        var request = {};

        request['Id'] = getParentId();
        request['RowVersion'] = $("#RowVersion").val();
        request["UserOrganizations"] = getOrganizations();

        callServer({
            type: "POST",
            url: "/UserAdministration/UpdateUserOrganizations",
            data: request,
            success: function (data) {
                updateIdAndRowVersion(data);
                toastr.success(data.message);
                userDataAreSaved(callback);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function submitIdentifierData(callback) {
    toastr.success(document.getElementById("msgEdit").value);
    userDataAreSaved(callback);
}

function userDataAreSaved(callback) {
    saveInitialFormData("#idUserInfo");
    executeCallback(callback);
}

function updateIdAndRowVersion(data) {
    $("#userId").val(data["id"]);
    $("#RowVersion").val(data["rowVersion"]);
}

function collapseChapter(element) {
    let id = $(element).data('target');
    if ($(`${id}`).hasClass('show')) {
        $(`${id}`).collapse('hide');
        $(element).children('.institution-icon').removeClass('fa-angle-up');
        $(element).children('.institution-icon').addClass('fa-angle-down');

    } else {
        $(`${id}`).collapse('show');
        $(element).children('.institution-icon').removeClass('fa-angle-down');
        $(element).children('.institution-icon').addClass('fa-angle-up');
    }

}

function openInstitutionModal(e) {
    e.stopPropagation();
    e.preventDefault();

    $('#newOrganization').val('').trigger('change');
    $('#institutionModal').modal('show');
}

function addNewOrganizationData() {
    let organizationId = $("#newOrganization").val();
    if (organizationId) {
        let organizationIds = [];
        $('.institution-container').each(function (index, element) {
            organizationIds.push($(element).attr('id').split('-')[1]);
        });
        request = {};
        request["OrganizationsIds"] = organizationIds;
        request["OrganizationId"] = organizationId;

        callServer({
            type: "post",
            url: `/UserAdministration/LinkOrganization`,
            data: request,
            success: function (data) {
                $("#institutions").find(".no-result-content").hide();
                $("#institutions").append(data);
                $('#institutionModal').modal('hide');
                submitData();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    } else {
        toastr.warning("You have no organization selected yet!")
    }
}

$(document).on('click', '.user-arrow-left', function (e) {
    $('.personnel-tab').each(function (index, element) {
        if ($(element).hasClass('active')) {
            $(element).prev().click();
            return false;
        }
    });
});

$(document).on('click', '.user-arrow-right', function (e) {
    $('.personnel-tab').each(function (index, element) {
        if ($(element).hasClass('active')) {
            $(element).next().click();
            return false;
        }
    });
});

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

function showUserBasicInfo(userId, password) {
    var activeElement = document.querySelector('.personnel-tab.active');
    var containerId = activeElement.getAttribute('data-id');
    if (containerId == "personalData") {
        callServer({
            type: 'GET',
            url: `/UserAdministration/ShowUserBasicInfo?userId=${userId}`,
            success: function (data) {
                $('#userInfoModal').html(data);
                document.getElementById("userPassword").textContent = password;
                $('#userInfoModal').modal('show');
                $('#registrationTypeId').remove();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, true);
            }
        });
    }
}


function copyValue(icon) {
    var valueElement = icon.parentNode;
    var value = valueElement.innerText;

    var textarea = document.createElement("textarea");
    textarea.value = value;
    document.body.appendChild(textarea);
    textarea.select();

    document.execCommand("copy");
    document.body.removeChild(textarea);

    var copiedMessage = document.createElement("span");
    copiedMessage.innerText = "Copied!";
    copiedMessage.classList.add("copied-message");

    valueElement.insertBefore(copiedMessage, icon.nextSibling);
    setTimeout(function () {
        valueElement.removeChild(copiedMessage);
    }, 2000);
}

function removePersonnelFromOrganization(event) {
    event.stopPropagation();
    event.preventDefault();
    var id = getParentId();
    var organizationId = document.getElementById("buttonSubmitDelete").getAttribute('data-id');
    var state = document.getElementById("buttonSubmitDelete").getAttribute('data-state');
    callServer({
        type: "PUT",
        url: `/UserAdministration/SetUserState?userId=${id}&organizationId=${organizationId}&newState=${state}`,
        success: function (data) {
            toastr.success(`Successfully removed user from organizatoin`);
            $("#institution-" + organizationId).remove();
            userDataAreSaved();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getPersonnelOccupations() {
    let request = {};
    request['OccupationCategoryCD'] = $('#occupationCategory').val();
    request['OccupationSubCategoryCD'] = $('#occupationSubCategory').val();
    request['OccupationCD'] = $('#occupation').val();
    request['PersonnelSeniorityCD'] = $('#personnelSeniority').val();

    if (request['OccupationCategoryCD'] || request['OccupationSubCategoryCD'] || request['OccupationCD'] || request['PersonnelSeniorityCD'])
        return request;
    else
        return null;
}

function initializeAcademicPositions() {
    var academicPositionSelect = $("#academicPosition");
    academicPositionSelect.empty();

    for (const position of academicPositions) {
        const preferredTerm = findPreferredTermByLanguage(position, activeLanguage);
        academicPositionSelect.append($('<option>', {
            value: position.Id,
            text: preferredTerm
        }));
    }
    academicPositionSelect.trigger('change');

    if (userAcademicPositionsCount > 0) {
        for (const position of selectedPositionIds) {
            academicPositionSelect.find('option[value="' + position.AcademicPositionId + '"]').prop('selected', true).attr("data-id", position.Id);
        }
    }
}

function initializeInactiveAcademicPositions() {
    var academicPositionSelect = $("#academicPosition");

    for (const position of inactiveAcademicPositions) {
        const preferredTerm = findPreferredTermByLanguage(position, activeLanguage);

        const isSelected = selectedPositionIds.some(item => item.AcademicPositionId === position.Id);

        if (isSelected) {
            academicPositionSelect.append($('<option>', {
                value: position.Id,
                text: preferredTerm,
                disabled: true
            }));
        }
    }
    academicPositionSelect.trigger('change');

    if (userAcademicPositionsCount > 0) {
        for (const selectedPosition of selectedPositionIds) {
            academicPositionSelect
                .find('option[value="' + selectedPosition.AcademicPositionId + '"]')
                .prop('selected', true)
                .attr("data-id", selectedPosition.Id);
        }
    }
}

function initializeRoles() {
    var rolesSelect = $("#roles");
    rolesSelect.empty();

    for (const position of roles) {
        const preferredTerm = findPreferredTermByLanguage(position, activeLanguage);

        rolesSelect.append($('<option>', {
            value: position.Id,
            text: preferredTerm
        }));
    }
    rolesSelect.trigger('change');

    if (userRolesCount > 0) {
        for (const roleId of selectedRolesIds) {
            rolesSelect
                .find(`option[value="${roleId}"]`)
                .prop('selected', true);
        }
    }

}

function initializeInactiveRoles() {
    var rolesSelect = $("#roles");

    for (const position of inactiveRoles) {
        const preferredTerm = findPreferredTermByLanguage(position, activeLanguage);

        const isSelected = selectedRolesIds.some(item => item === position.Id);

        if (isSelected) {
            rolesSelect.append($('<option>', {
                value: position.Id,
                text: preferredTerm,
                disabled: true
            }));
        }
    }
    rolesSelect.trigger('change');

    if (userRolesCount > 0) {
        for (const roleId of selectedRolesIds) {
            rolesSelect
                .find(`option[value="${roleId}"]`)
                .prop('selected', true);
        }
    }
}

function findPreferredTermByLanguage(position, language) {
    for (const translation of position.Thesaurus.Translations) {
        if (translation.Language === language) {
            return translation.PreferredTerm;
        }
    }
    return '';
}

function setPersonnelSeniorityToRequired(personnelSeniorityField, requiredDiv) {
    var seniorityDiv = document.getElementById("seniorityDiv");
    seniorityDiv.removeAttribute("hidden");
    personnelSeniorityField.attr('required', 'required');
    $("#seniorityLabel").append(requiredDiv);
}

function hidePersonnelSeniority(personnelSeniorityField) {
    personnelSeniorityField.removeAttr('required');
    personnelSeniorityField.removeClass('error');
    $("#seniorityLabel .label-required").remove();
    $('#personnelSeniority-error').remove();
    var seniorityDiv = document.getElementById("seniorityDiv");
    seniorityDiv.setAttribute("hidden", true);
    removeSelectedSeniority();
}

function reloadChildren() {
    const selectElements = document.querySelectorAll('[data-codesetid]');
    selectElements.forEach(selectElement => {
        const elementId = selectElement.getAttribute('id');
        reloadCodeSetChildren(elementId);
    });
}

function removeSelectedSeniority() {
    var selectElement = document.getElementById("personnelSeniority");
    var selectedOption = selectElement.querySelector("option[selected]");
    if (selectedOption) {
        selectedOption.removeAttribute("selected");
    }
}

function setParentIdAndReturn(identifierEntity) {
    identifierEntity["personnelId"] = getParentId();
    return identifierEntity;
}

function getParentId() {
    return $("#userId").val();
}

function submitParentForm() {
    return submitPersonalData();
}