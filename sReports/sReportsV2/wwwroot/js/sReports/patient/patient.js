var PatientId;
var religionId;

addUnsavedChangesEventHandler("#idPatientInfo");

function submitForm(form) {
    updateDisabledOptions(false);
    $(form).validate({
        ignore: []
    });
    if ($(form).valid()) {

        var request = {};

        let patientId = getParentId();
        let action = patientId != 0 ? 'Edit' : 'Create';
        request['Id'] = patientId;
        request['Name'] = $("#name").val();
        request['FamilyName'] = $('#familyName').val();
        request['GenderCD'] = $("#gender").val();
        request['BirthDate'] = toDateStringIfValue($("#birthDate").val());
        request['Deceased'] = $("#deceased").is(":checked");
        request['DeceasedDateTime'] = toDateStringIfValue($("#deceasedDateTime").val());
        request['ReligionCD'] = $("#religion").val();
        request['CitizenshipCD'] = $("#citizenship").val();
        request['MaritalStatusCD'] = $("#maritalStatus").val();
        request['MultipleBirth'] = $("#multipleBirth").val();
        request['MultipleBirthNumber'] = $("#multipleBirthNumber").val();
        request['Addresses'] = getAddresses("patientAddresses");
        request['Contacts'] = getContacts();
        request['Telecoms'] = getTelecoms('PatientTelecom');
        request['Language'] = $("#language").val();
        request['UniqueMasterCitizenNumber'] = $("#umcn").val();
        request['Identifiers'] = getIdentifiers();
        request['Communications'] = getCommunications();

        callServer({
            type: "POST",
            url: `/Patient/${action}`,
            data: request,
            success: function (data) {
                $('#episodeOfCares').show();
                if (patientId == 0) {
                    toastr.options = {
                        timeOut: 100
                    }
                    toastr.options.onHidden = function () { window.location.href = `/Patient/EditPatientInfo?patientId=${data.id}&isReadOnlyViewMode=${false}`; }
                }
                updateDisabledOptions(true);
                saveInitialFormData("#idPatientInfo");
                toastr.success("Success");
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

    return false;
}

function selectChanged()
{
    if ($('#multipleBirth1').is(':visible')) {
        $('#multipleBirth1').hide();
        $('#multipleBirthNumber').val(1);
    }
    else
    {
        $('#multipleBirth1').show();
    }
}

function deceasedChanged(isDeceasedChecked) {
    var $deceasedDateTimeContainer = $("#deceasedDateTimeContainer");
    if (isDeceasedChecked) {
        $deceasedDateTimeContainer.show();
    } else {
        $deceasedDateTimeContainer.hide();
        $("#deceasedDateTime")
            .val("")
            .removeClass("error");
        $deceasedDateTimeContainer.find("label.error").remove();
    }
}

$(document).on("change", "#deceased", function () {
    deceasedChanged($(this).is(":checked"));
});

$(document).on("change", "#deceasedDateTime", function () {
    if ($(this).hasClass("error")) {
        var $deceasedDateTimeContainer = $("#deceasedDateTimeContainer");
        $(this).removeClass("error");
        $deceasedDateTimeContainer.find("label.error").remove();
    }
});

function goToAllPatient() {
    window.location.href = "/Patient/GetAll";
}

function cancelPatientEdit(event, readOnly) {
    event.preventDefault();
    if (PatientId) {
        let action = readOnly ? 'View' : 'Edit';
        if (!compareForms("#idPatientInfo")) {
            if (confirm("You have unsaved changes. Are you sure you want to cancel?")) {
                saveInitialFormData("#idPatientInfo");
                window.location.href = `/Patient/${action}?patientId=${PatientId}`;
            }
        } else {
            window.location.href = `/Patient/${action}?patientId=${PatientId}`;
        }
    } else {
        if (!compareForms("#idPatientInfo")) {
            if (confirm("You have unsaved changes. Are you sure you want to cancel?")) {
                saveInitialFormData("#idPatientInfo");
                window.location.href = "/Patient/GetAll";
            }
        } else {
            window.location.href = "/Patient/GetAll";
        }
    }
}

function getCommunications() {
    let result = [];
    let selected = $('input[name=radioPreferred]:checked').val();

    $('input[name=radioPreferred]').each(function (index, element) {
        result.push({
            preferred: $(element).val() == selected ? true : false,
            languageCD: $(element).val(),
            id: $(element).attr("data-id")
        })
    })

    return result;
}

$(document).ready(function () {
    setPatientId();
    renderInitialData();
    setCommonValidatorMethods();
    setValidationFunctions();
    saveInitialFormData("#idPatientInfo");
});

function setPatientId() {
    var url = new URL(window.location.href);
    var patientId = url.searchParams.get("patientId");

    if (patientId) {
        PatientId = patientId;
    } else {
        PatientId = null;
    }
}

function renderInitialData() {
    $("input[name=radioPreferred]").each(function () {
        if ($(this).is(":checked")) {
            let preferredText = document.createElement('span');
            $(preferredText).addClass("preferred-text-class");
            preferredText.innerHTML = " (Preferred)";
            $(this).closest('div').addClass("preferred-language-text-active").append(preferredText);;
        } else {
            $(this).closest('div').removeClass("preferred-language-text-active");
        }
    });
    deceasedChanged($("#deceased").is(":checked"));

    initCodeSelect2(hasSelectedReligion(), religionId, "religion", "religion", "ReligiousAffiliationType", '', "#idPatientInfo");
}

function hasSelectedReligion() {
    return religionId;
}

function setValidationFunctions() {
    $.validator.addMethod(
        "deceasedDateTime",
        function (value, element) {
            if ($("#deceased").is(":checked")) {
                return $(element).val();
            } else {
                return true;
            }
        },
        "Please enter deceased datetime."
    );
    $('#idPatientInfo').validate({});
    $('[name="deceasedDateTime"]').each(function () {
        $(this).rules('add', {
            deceasedDateTime: true
        });
    });
}

$(document).on('click', '.plus-button', function (e) {
    if (ValidateLanguage() && $('#language').val()) {
        let language = createLanguageElement();
        let input = createRadioInput();

        let removeButton = createRemoveLanguageButton();
        removeButton.id = "removeButtonId";

        let preferredText = createPreferredText();

        if ($('#tableBody').find('input:radio[name=radioPreferred]').length == 0) {
            $(input).attr('checked', true);
            $(language).addClass("preferred-language-text-active");
            $(language).append(preferredText);
            $(removeButton).addClass("preferred-language-text-active");
        }

        $(removeButton).addClass("right-remove-button");

        let preferred = createRadioField();

        let preferred2 = document.createElement('div');
        $(preferred2).addClass("preferred-language-group");

        let divElement = document.createElement('div');

        $(preferred).append(input);
        $(preferred2).append(preferred).append(language).append(removeButton)

        $("#tableBody").append(preferred2).append(divElement);

    }
});

function createLanguageElement() {
    let language = document.createElement('span');
    $(language).attr("data-property", 'language');
    $(language).attr("data-value", $('#language').val());
    $(language).addClass("preferred-language-text");
    language.id = "firstLanguage";
    $(language).html(getSelectedOptionLabel('language'));

    return language;
}

function createRadioInput() {
    let input = document.createElement('input');
    $(input).addClass("form-radio-field");
    $(input).attr("value", $('#language').val());
    $(input).attr("name", 'radioPreferred');
    $(input).attr("type", 'radio');
    $(input).attr("data-id", '0');
    $(input).attr("data-no-color-change", "true");

    return input;
}

function createPreferredText() {
    let preferredText = document.createElement('span');
    $(preferredText).addClass("preferred-text-class");
    preferredText.innerHTML = " (Preferred)";

    return preferredText;
}

function createRadioField() {
    let preferred = document.createElement('span');
    $(preferred).attr("data-property", 'preferred');
    $(preferred).attr("data-value", $('#preferred').val());
    $(preferred).addClass("radio-space");
    $(preferred).css("margin-right", "13px");

    return preferred;
}

function ValidateLanguage() {
    var isValid = true;
    let language = $("#language").val();

    $(`#tableBody > div`).each(function () {
        if ($(this).find("span:eq(1)").data('value') == language) {
            isValid = false;
            toastr.error(`This language already added`);
        }

    });
    return isValid;
}

function createRemoveLanguageButton() {
    let span = document.createElement('span');
    $(span).addClass('remove-language-button');

    let i = document.createElement('i');
    $(i).addClass('fas fa-times');

    $(span).append(i);
    return span;
}

$(document).on('click', '.remove-language-button', function (e) {
    var currentEl = e.currentTarget;
    $(currentEl).closest('div').remove();
});

function pushStateWithoutFilter(num) {
    if (PatientId) {
        history.pushState({}, '', `?patientId=${PatientId}&page=${num}&pageSize=${getPageSize()}`);
    } else {
        history.pushState({}, '', `?page=${num}&pageSize=${getPageSize()}`);
    }
}

function showGeneralInfo(event, element){
    event.stopPropagation();
    setTagActiveClass(element);
    setTagIconActiveClass("general-icon");

    $("#contactPersonPartial").hide();
    $("#patientAddressAndTelecomPartial").hide();
    $("#patientInfoPartial").show();
    resetPatientContactForm();
}

function showAddressAndTelecomInfo(event, element) {
    event.stopPropagation();
    setTagActiveClass(element);
    setTagIconActiveClass("telecom-icon");

    $("#contactPersonPartial").hide();
    $("#patientAddressAndTelecomPartial").show();
    $("#patientInfoPartial").hide();
    resetPatientContactForm();
}

function showContactPerson(event, element) {
    event.stopPropagation();
    setTagActiveClass(element);
    setTagIconActiveClass("contact-icon");

    $("#contactPersonPartial").show();
    $("#patientAddressAndTelecomPartial").hide();
    $("#patientInfoPartial").hide();
}

function setTagActiveClass(element) {
    $('.tab-item').removeClass('active');
    $(element).addClass('active');
}

function setTagIconActiveClass(iconId) {
    $('.tab-icon').removeClass('active');
    document.getElementById(iconId).classList.add('active');
}

$(document).on('click', '[name="radioPreferred"]', function () {
    let selected = $('input[name=radioPreferred]:checked').val();
    var element = document.getElementById("firstLanguage");
    element.classList.remove("preferred-language-text-active");
    var removeElement = document.getElementById("removeButtonId");
    if (removeElement) {
        removeElement.classList.remove("preferred-language-text-active");
        removeElement.classList.add("right-remove-button");
    }
    
    let preferredText = document.createElement('span');
    $(preferredText).addClass("preferred-text-class");
    preferredText.innerHTML = " (Preferred)";

    $(`#tableBody > div`).each(function () {
        $(this).removeClass("preferred-language-text-active");
        $(this).find(".preferred-text-class").remove();

        if ($(this).find("span:eq(1)").data('value') == selected) {
            $(this).find("span:eq(1)").append(preferredText);
            $(this).addClass("preferred-language-text-active");
        }
    });
});

function setParentIdAndReturn(identifierEntity) {
    identifierEntity["patientId"] = getParentId();
    return identifierEntity;
}

function getParentId() {
    return $("#patientId").val();
}

function submitParentForm() {
    return submitForm($("#idPatientInfo"));
}