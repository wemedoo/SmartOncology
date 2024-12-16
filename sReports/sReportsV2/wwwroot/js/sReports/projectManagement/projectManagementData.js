$(document).ready(function () {
    validateProjects();
    triggerTimeOnChange("#projectDataForm");
    saveInitialTrialFormData("#trialDataForm");
    saveInitialFormData("#projectDataForm");
});

addUnsavedTrialChangesEventHandler("#projectDataForm", "#trialDataForm");

function validateProjects() {
    destroyValidator();
    validateTrial();
    $.validator.addMethod("validateCodeActiveFromTo", function (value, element) {
        return compareActiveDateTime("activeFromDate", "activeToDate", "activeFromTime", "activeToTime", "activeToTimeWrapper");
    }, "Active From shouldn't be greater than Active To!");

    $.validator.addMethod("dateInputValidation", function (value, element) {
        return validateDateInput($(element));
    }, `Please put your date in [${getDateFormatDisplay()}] format.`);

    $('#projectDataForm').validate({
        rules: {
            activeToDate: {
                validateCodeActiveFromTo: true,
                dateInputValidation: true
            },
            activeFromDate: {
                dateInputValidation: true
            },
        }
    });
}

function validateTrial() {
    $.validator.addMethod("isTitleUnique", function (value, element) {
        return checkUniqueTitle($(`#trialTitle`).val());
    }, "This title is already used. Please choose another.");

    $('#trialDataForm').validate({
        rules: {
            trialTitle: {
                required: true,
                isTitleUnique: true
            }
        }
    });
}

$("#activeToDate, #activeFromDate").on('change', function () {
    $("#projectDataForm").validate().element("#activeToDate");
    $("#projectDataForm").validate().element("#activeFromDate");
});

$("#trialTitle").on('change', function () {
    $("#trialDataForm").validate().element("#trialTitle");
});

// -----

function trySubmitClinicalTrial(event, id, tabToSwitchTo = null) {
    event.preventDefault();
    event.stopPropagation();
    if ($('#trialDataForm').valid()) {
        checkUniqueTitleAndSubmit(event, id, tabToSwitchTo);
    }
}

function checkUniqueTitleAndSubmit(event, id, tabToSwitchTo = null) {
    submitClinicalTrial(event, id, tabToSwitchTo);
    saveInitialFormData("#projectDataForm");
}

function checkUniqueTitle(title) {
    let isTitleUnique = true;

    callServer({
        type: 'GET',
        url: '/ProjectManagement/GetTrialAutoCompleteName',
        data: {
            Term: title,
            Page: 1,
        },
        success: function (data) {
            $(data.results).each(function () {
                if (this.text === title && $('#trialId').val() !== this.id) {
                    isTitleUnique = false;
                    return false;
                }
            });
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
            isTitleUnique = false;
        }
    });

    return isTitleUnique;
}

function submitClinicalTrial(event, id, tabToSwitchTo = null) {
    updateDisabledOptions(false);
    var request = {};
    if ($('#projectDataForm').valid()) {
        request['ProjectId'] = $("#projectId").val();
        request['ProjectName'] = $("#projectName").val();
        request['ProjectTypeCD'] = $('#projectType').val();
        request['ProjectStartDateTime'] = calculateDateTimeWithOffset("#activeFromDate", "#activeFromTime");
        request['ProjectEndDateTime'] = calculateDateTimeWithOffset("#activeToDate", "#activeToTime");
        request['ClinicalTrial'] = getClinicalTrial();

        if ($('#trialDataForm').valid()) {
            callServer({
                type: "POST",
                url: id > 0 ? `/ProjectManagement/Edit` : `/ProjectManagement/Create`,
                data: request,
                success: function (data, textStatus, xhr) {
                    updateDisabledOptions(true);
                    if ($('#projectId').val() > 0) {
                        toastr.success("Project updated successfully!");
                        if (tabToSwitchTo) {
                            switchTab(tabToSwitchTo);
                        }
                    }
                    else {
                        toastr.success("Project created successfully!");
                        setTimeout(function () {
                            window.location.href = `/ProjectManagement/Edit?projectId=${data}`;
                        }, 1000); 
                    }
                    saveInitialTrialFormData("#trialDataForm");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }
}

function getClinicalTrial() {
    request = {};
    request['ClinicalTrialId'] = $("#trialId").val();
    var trialDataDiv = document.getElementById("trialDataDiv");
    if (trialDataDiv && !trialDataDiv.hasAttribute('hidden')) {
        request['ClinicalTrialTitle'] = $(`#trialTitle`).val();
        request['ClinicalTrialAcronym'] = $(`#acronym`).val();
        request['ClinicalTrialDataManagementProvider'] = $(`#datamanagement-provider-id`).val();
        request['ClinicalTrialDataProviderIdentifier'] = $(`#dataprovider-id`).val();
        request['ClinicalTrialSponsorName'] = $(`#sponsor-name`).val();
        request['ClinicalTrialSponsorIdentifier'] = $(`#sponsor-id`).val();
        request['ClinicalTrialSponsorIdentifierTypeCD'] = $(`#sponsor-id-type`).val();
        request['ClinicalTrialIdentifier'] = $(`#clinicaltrial-id`).val();
        request['ClinicalTrialIdentifierTypeCD'] = $(`#clinicaltrial-id-type`).val();
        request['ClinicalTrialRecruitmentStatusCD'] = $(`#status:checked`).val();
        request['PersonnelId'] = $("#userId").val();
    }
    else {
        clearTrialData();
    }
    return request;
}

function clearTrialData() {
    $(`#trialTitle`).val('');
    $(`#acronym`).val('');
    $(`#datamanagement-provider-id`).val('');
    $(`#dataprovider-id`).val('');
    $(`#sponsor-name`).val('');
    $(`#sponsor-id`).val('');
    $(`#sponsor-id-type`).val('');
    $(`#clinicaltrial-id`).val('');
    $(`#clinicaltrial-id-type`).val('');
    $("input[name^='status']").prop('checked', false);
    $("#userId").val('');
    $("#trialId").val(0);
}