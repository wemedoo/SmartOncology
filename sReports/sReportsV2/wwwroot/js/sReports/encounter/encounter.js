var encounterColumnName;
var encounterSwitchCount = 0;
var encounterIsAscending = null;

function openUpdateEncounterView(e, patientId, episodeOfCareId, encounterId, readOnly) {
    if (!$(e.target).hasClass('dropdown-button') && !$(e.target).hasClass('fa-bars') && !$(e.target).hasClass('dropdown-item') && !$(e.target).hasClass('dots') && !$(e.target).hasClass('table-more')) {
        updateEncounter(patientId, episodeOfCareId, encounterId, readOnly)
    }
}

function updateEncounter(patientId, episodeOfCareId, encounterId, readOnly) {
    window.location.href = `/Patient/${readOnly ? 'View' : 'Edit'}?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}&encounterId=${encounterId}`;
}

function deleteEncounter(event, id, reloadEncounterFromPatient) {
    event.preventDefault();
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/Encounter/Delete?id=${id}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
            if (!reloadEncounterFromPatient)
                reloadTable();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function viewEncounterInfo(event, encounterId) {
    event.preventDefault();
    event.stopPropagation();

    $("#encounterInfoEncounterId").text($(`#encounterId-${encounterId}`).text());
    $("#encounterInfoFamilyName").text($(`#familyName-${encounterId}`).text());
    $("#encounterInfoName").text($(`#name-${encounterId}`).text());
    $("#encounterInfoGender").text($(`#gender-${encounterId}`).text());
    $("#encounterInfoBirthDate").text($(`#birthDate-${encounterId}`).text());
    $("#encounterInfoPatientId").text($(`#patientId-${encounterId}`).text());
    $("#encounterInfoAdmissionDate").text($(`#admissionDate-${encounterId}`).text());
    $("#encounterInfoDischargeDate").text($(`#dischargeDate-${encounterId}`).text());

    $("#encounterInfoModal").modal("show");
}

function showEncounterContent(encounterColumnName, encounterIsAscending) {
    $("#encounterTab").addClass("code-active-tab");
    $("#eocTab").addClass("remove-eoc-tab");
    $("#taskTab").removeClass("code-active-tab");
    $("#eocTab").removeClass("code-active-tab");
    $('#patientId').data('value', $('#patientId').val());
    var request = {};
    request['PatientId'] = $('#patientId').val();
    request['IsAscending'] = encounterIsAscending;
    request['ColumnName'] = encounterColumnName;
    request['ReloadEncounterFromPatient'] = true;
    setIsReadOnlyViewModeInRequest(request);

    callServer({
        type: 'GET',
        url: '/Encounter/ReloadTable',
        data: request,
        success: function (data) {
            document.getElementById("filterGroup").innerHTML = "";
            $("#eocContainer").html(data);
            setTableMaxHeight("encounterTable", "encounterTableContent");
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function sortEncounterTable(event, column) {
    event.stopPropagation();
    if (encounterSwitchCount == 0) {
        if (encounterColumnName == column)
            encounterIsAscending = checkIfAsc(encounterIsAscending);
        else
            encounterIsAscending = true;
        encounterSwitchCount++;
    }
    else {
        if (encounterColumnName != column)
            encounterIsAscending = true;
        else
            encounterIsAscending = checkIfAsc(encounterIsAscending);
        encounterSwitchCount--;
    }
    encounterColumnName = column;

    if (showEncountersInPatientView())
        showEncounterContent(encounterColumnName, encounterIsAscending);
    else
        showOrderedEncounterContentForSelectedPatient($('#patientId').data('value'), encounterColumnName, encounterIsAscending);
}

function showEncountersInPatientView() {
    return $('#patientId').val() != "";
}

function checkIfAsc(isAscending) {
    return !isAscending;
}

function deleteDocumentFromEncounter(event) {
    event.stopPropagation();
    event.preventDefault();

    let formInstanceId = $('#buttonSubmitDelete').attr('data-id'); 
    let formInstanceLastUpdate = $('#buttonSubmitDelete').attr('data-lastupdate');
    let encounterId = $("#activeEncounter").val(); 

    callServer({
        type: "DELETE",
        url: `/DiagnosticReport/Delete?formInstanceId=${formInstanceId}&lastUpdate=${formInstanceLastUpdate}`,
        success: function (data) {
            $(`#id-${formInstanceId}`).remove();

            if ($('#fid').find('input[name="formInstanceId"]').val() == formInstanceId) {
                noDocuments();
            }
            if ($('.document-section').length === 0) {
                showDetails(encounterId);
            }
            initialFormData = '{}';
            toastr.success('Removed');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.add-new-doctor', function (event) {
    event.preventDefault();
    event.stopPropagation();

    addNewEncounterDoctor();
});

function addNewEncounterDoctor() {
    let doctorId = $("#doctor").val();
    let relationTypeId = $("#relationType").val();

    if (validateDoctorInput()) {
        let doctorName = getSelectedSelect2Label('doctor');
        let relationTypeName = getSelectedOptionLabel('relationType');

        $("#doctor").val('').trigger('change');
        $("#relationType").val('');

        let newDoctorRow = $("#encounter-doctor-pattern-row").clone();
        $(newDoctorRow).removeAttr("id");
        $(newDoctorRow).removeClass("d-none");
        $(newDoctorRow).addClass("d-flex");

        $(newDoctorRow).find('.encounter-doctor').html(getSelectedNewOption(doctorId, doctorName));
        $(newDoctorRow).find('.encounter-doctor-relation').html(getSelectedNewOption(relationTypeId, relationTypeName));

        $(".doctor-list").prepend($(newDoctorRow));
    }
}

function validateDoctorInput() {
    let valid = true;
    $(".encounter-doctor-input").each(function () {
        if (!$(this).val()) {
            valid = false;
            $(this)
                .removeClass('valid')
                .addClass('error');
            let id = $(this).attr("id");
            $(`#${id}-error`).remove();
            let validationError = `<label id="${id}-error" class="error" for="${id}">This field is required.</label>`;
            $(this).parent().append(validationError);
        }
    });
    return valid;
}

$(document).on('focus', '.encounter-doctor-input', function (event) {
    removeEncounterDoctorErrorValidation($(this))
});

function removeEncounterDoctorErrorValidation($input) {
    $input.removeClass("error");
    let id = $input.attr("id");
    $(`#${id}-error`).remove();
}

function removeEncounterDoctorErrorValidations() {
    $(".encounter-doctor-input").each(function () {
        removeEncounterDoctorErrorValidation($(this));
    });
}

$(document).on('click', '.delete-doctor', function (event) {
    event.preventDefault();
    event.stopPropagation();

    $(this).closest('.encounter-doctor-row').remove();
});

function getSelectedNewOption(value, label) {
    return $(`<option value="${value}" selected>${label}</option>`);
}

$(document).on('click', '.encounter-dropdown', function (event) {
    event.preventDefault();
    let $target = $(event.currentTarget);
    clickDropdownButton($target);
});

function clickDropdownButton($target) {
    let $dropdown = $target.closest('.dropdown');
    if ($dropdown.hasClass('show')) {
        dropdownIsHidding($target);
        hideOpenedDropdown($dropdown);
    } else {
        hideOpenedDropdowns();
        dropdownIsShowing($target);
        showDropdown($dropdown);
    }
}

function showDropdown($dropdown) {
    let $dropdownMenu = $dropdown.find('.dropdown-menu');
    let hasVerticalScroll = $dropdown.closest('.encounters-fix-head').hasScrollBar();
    $dropdown.addClass('show');
    $dropdownMenu.addClass('show');
    relocateDropdown($dropdown, $dropdownMenu, hasVerticalScroll);
}

function relocateDropdown($dropdown, $dropdownMenu, hasVerticalScroll) {
    let $td = $dropdown.closest('td');
    let offsets = getDropdownOffsets($td, $dropdownMenu, hasVerticalScroll);
    const { top, left } = getPosition($td);

    $dropdownMenu
        .css(
            "cssText",
            `left: ${left - offsets.leftOffset}px !important; 
             top: ${top + offsets.topOffset}px; 
             position: fixed
             `
        );
}

function getDropdownOffsets($td, $dropdownMenu, hasVerticalScroll) {
    let dropdownMenuWidth = getWidth($dropdownMenu);
    let tdWidth = getWidth($td);
    let elementsWidthDiff = dropdownMenuWidth - tdWidth;
    let staticLeftOffset = hasVerticalScroll ? 10 : 0;
    let leftOffset = elementsWidthDiff + staticLeftOffset;
    let topOffset = 15;

    return {
        leftOffset,
        topOffset
    }
}

function encounterTableIsScrolled() {
    hideOpenedDropdowns();
}

$(document).on('click', function (e) {
    clickOutsideDropdown($(e.target));
});

function clickOutsideDropdown($target) {
    if ($('.encounters-fix-head').length > 0 && !($target.hasClass('dots') && $target.parent().is('.dropdown-button'))) {
        hideOpenedDropdowns();
    }
}

function hideOpenedDropdowns() {
    $('.encounters-fix-head .dropdown.show').each(function () {
        dropdownIsHidding($(this).find('.dropdown-button'));
        hideOpenedDropdown($(this));
    });
}

function hideOpenedDropdown($dropdown) {
    $dropdown.removeClass('show');
    $dropdownMenu = $dropdown.children('.dropdown-menu');
    $dropdownMenu.removeClass('show');
    $dropdownMenu
        .css(
            "cssText",
            `left: ; 
                top: ; 
                position: 
                `
        );
}