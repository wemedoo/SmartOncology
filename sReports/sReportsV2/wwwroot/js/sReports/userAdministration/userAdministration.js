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

function submitForm(form, e) {
    e.preventDefault();
    e.stopPropagation();
    submitData();
}

function submitPersonalData() {
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
        if (!validateEmailInput(request, "email")) return false;
        if (!validateEmailInput(request, "personalEmail")) return false;

        request["MiddleName"] = $("#middleName").val();
        request["AcademicPositions"] = getSelectedAcademicPositions();
        request["Addresses"] = getAddresses("personnelAddresses");
        request["Identifiers"] = getIdentifiers();
        request["DayOfBirth"] = toDateStringIfValue($("#dayOfBirth").val());
        request["Roles"] = getUserRoles();
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
                enableChangeTab();
                if ($("#registrationType").val() == "Quick")
                    showUserBasicInfo(data.id, data.password);
                updateDisabledOptions(true);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
        return true;

    }
    var errors = $('.error').get();
    if (errors.length !== 0) {
        $.each(errors, function (index, error) {
            $(error).closest('.collapse').collapse("show");
        });
    };

    return false;
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
    institution["IsPracticioner"] = $(`#isPractitioner-${organizationId}:checked`).val();
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

function getUserRoles() {
    var chkArray = [];

    $("#roles option:selected").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function cancelUserEdit() {
    if (!compareForms("#idUserInfo")) {
        if (confirm("You have unsaved changes. Are you sure you want to cancel?")) {
            saveInitialFormData("#idUserInfo");
            window.location.href = isUserAdministration ? '/UserAdministration/GetAll' : '/Home/Index'
        }
    } else {
        window.location.href = isUserAdministration ? '/UserAdministration/GetAll' : '/Home/Index'
    }
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
    $.validator.addMethod("registeredEmail", function (value, element) {
        return emailExist(value);
    }, "This email is already associated with another user.");

    $.validator.addMethod("registeredUsername", function (value, element) {
        return usernameExist(value);
    }, "This username is already associated with another user.");

    $("#idUserInfo").validate({});
    $('[name="Email"]').each(function () {
        $(this).rules('add', {
            registeredEmail: true
        });
    });
    $('[name="Username"]').each(function () {
        $(this).rules('add', {
            registeredUsername: true
        });
    });
}

function removeCustomValidators() {
    $('[name="Email"]').each(function () {
        $(this).rules('remove', 'registeredEmail');
    });
    $('[name="Username"]').each(function () {
        $(this).rules('remove', 'registeredUsername');
    });
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@]+(\.[^<>()[\]\\.,;:\s@]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
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
    let isSuccess = submitData();
    if (isSuccess) {
        $('.personnel-tab').removeClass('active');

        $(this).addClass('active');
        $('.user-cont').hide();

        let containerId = $(this).attr("data-id");
        toggleSaveBtn(containerId);

        activeContainerId = containerId;
        $(`#${containerId}`).show();
        handleArrowVisibility(activeContainerId);
    }

});

function toggleSaveBtn(containerId) {
    if (containerId === "clinicalData") {
        $(`#buttonGroupPrimary`).hide();
    } else if (containerId === "institutionData" || containerId === "identifierData") {
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
        case "clinicalData": {
            $('.user-arrow-right').hide();
            $('.user-arrow-left').show();
            return true;
        }
        default:
    }
}

function submitData() {
    if (isReadOnly) return true;
    switch (activeContainerId) {
        case "personalData":
            saveInitialFormData("#idUserInfo");
            return submitPersonalData();
        case "institutionData":
            {
                $('#registrationTypeId').remove();
                return submitInstitutionalData();
            }
        case "identifierData":
            {
                $('#registrationTypeId').remove();
                saveInitialFormData("#idUserInfo");
                return submitPersonalData();
            }
        case "clinicalData":
            {
                $('#registrationTypeId').remove();
                return true;//submitClinicalData();
            }
        default:
        // code block
    }}



function submitInstitutionalData() {
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
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    
    return true;
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

$(document).on('change', '.ct-name', function (e) {
    $(this).closest('.single-ct').find('.institution-header-name').text($(this).val());
});

$(document).on('change', '.ct-role', function (e) {
    var text = $(this).find(":selected").data("display");
    $(this).closest('.single-ct').find('.header-role-value').text(text);
});

$(document).on('click', '.ct-status', function (e) {
    var text = $(this).data("display");
    $(this).closest('.single-ct').find('.ct-status-value').text(text);
});

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

function validateEmailInput(request, inputName, required = false) {
    if (validateEmail($(`#${inputName}`).val()) || !required) {
        request[`${capitalizeFirstLetter(inputName)}`] = $(`#${inputName}`).val();
        return true;
    }
    else {
        $(`#${inputName}`).addClass("error");
        $(`#${inputName}`).after(`<label id="${inputName}-error" class="error" for="${inputName}">Please enter a valid email address.</label>`);
        return false;
    }
}

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

function enableChangeTab() {
    $('[data-id="institutionData"]').removeAttr('data-toggle')
        .removeAttr('data-original-title').removeClass("personnel-tab-disabled").addClass('personnel-tab');
    $('[data-id="identifierData"]').removeAttr('data-toggle')
        .removeAttr('data-original-title').removeClass("personnel-tab-disabled").addClass('personnel-tab');
    $('[data-id="clinicalData"]').removeAttr('data-toggle')
        .removeAttr('data-original-title').removeClass("personnel-tab-disabled").addClass('personnel-tab');
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

//function impressumWordCounter()
$('.text-with-limit').on('keyup', function (e) {

    let targetId = e.target.id;
    let maxLength = $(`#${targetId}`).attr('maxLength');
    let charCount = $(`#${targetId}`).val().length;

    $(`#${targetId}`).siblings('.label').find('.char-limit-text').html(`${charCount}/${maxLength}`);
});

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
            toastr.success(`Success`);
            $("#institution-" + organizationId).remove();
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