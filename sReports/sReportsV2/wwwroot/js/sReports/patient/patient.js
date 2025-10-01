var PatientId;
var religionId;

addUnsavedChangesEventHandler("#idPatientInfo");

function submitPatientForm(form, callback) {
    updateDisabledOptions(false);
    $(form).validate({
        ignore: []
    });
    if ($(form).valid()) {
        var request = {};

        let patientId = getParentId();
        let action = patientId != 0 ? 'Edit' : 'Create';
        request['Id'] = patientId;
        request['GivenName'] = $("#name").val();
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
        if (!validateCommunication(request['Communications'])) { return false; }

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
                resetCommunication(patientId);
                updateDisabledOptions(true);
                saveInitialFormData("#idPatientInfo");
                executeCallback(callback);
                toastr.success("Patient data have been saved successfully");
                enableChangeTab(+request["Id"]);
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

$(document).on('change', '#multipleBirth', function (e) {
    var isMultipleBirth = $(this).val();
    if (isMultipleBirth === "true") {
        $('#multipleBirthNumberContainer').removeClass('d-none');
    } else {
        resetMultipleBirthNumber();
    }
});

function resetMultipleBirthNumber() {
    $('#multipleBirthNumberContainer').addClass('d-none');
    $('#multipleBirthNumber').val('1');
}

$(document).on('change', '#gender', function (e) {
    var gender = $(this).val();
    if (gender === $('#femaleCodeId').val()) {
        $('#multipleBirthContainer').removeClass('d-none');
    } else {
        $('#multipleBirthContainer').addClass('d-none');
        $('#multipleBirth').val('false');
        resetMultipleBirthNumber();
    }
});

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
        unsavedChangesCheck("#idPatientInfo",
            function () {
                window.location.href = `/Patient/${action}?patientId=${PatientId}`;
            },
            function () {
                window.location.href = `/Patient/${action}?patientId=${PatientId}`;
            }
        );
    } else {
        unsavedChangesCheck("#idPatientInfo",
            function () {
                window.location.href = "/Patient/GetAll";
            },
            function () {
                window.location.href = "/Patient/GetAll";
            }
        );
    }
}

$(document).ready(function () {
    setPatientId();
    renderInitialData();
    setCommonValidatorMethods();
    setValidationFunctions();
    saveInitialFormData("#idPatientInfo");
    setPatientLanguages();
    $('[data-toggle="tooltip"]').tooltip();
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

function pushStateWithoutFilter(num) {
    if (PatientId) {
        history.pushState({}, '', `?patientId=${PatientId}&page=${num}&pageSize=${getPageSize()}`);
    } else {
        history.pushState({}, '', `?page=${num}&pageSize=${getPageSize()}`);
    }
}

$(document).on('click', '.patient-tab-item', function (e) {
    if ($(this).hasClass('tab-disabled')) return;

    let $el = $(this);
    savePatientIfThereAreChanges(function () {
        setTagActiveClass($el);
        setTagIconActiveClass($el);

        $('.patient-partial').addClass('d-none');
        let containerId = $el.attr('data-id');
        $(containerId).removeClass('d-none');

        if (containerId != '#contactPersonPartial') {
            resetPatientContactForm();
        }
    });
});

function savePatientIfThereAreChanges(callback) {
    if (compareForms("#idPatientInfo")) {
        executeCallback(callback);
    } else {
        submitPatientForm($("#idPatientInfo"), callback);
    }
}

function setTagActiveClass(element) {
    $('.patient-tab-item').removeClass('active');
    $(element).addClass('active');
}

function setTagIconActiveClass(element) {
    $('.tab-icon').removeClass('active');
    $(element).find('i').addClass('active');
}

function setParentIdAndReturn(identifierEntity) {
    identifierEntity["patientId"] = getParentId();
    return identifierEntity;
}

function getParentId() {
    return $("#patientId").val();
}

function submitParentForm() {
    return submitPatientForm($("#idPatientInfo"));
}