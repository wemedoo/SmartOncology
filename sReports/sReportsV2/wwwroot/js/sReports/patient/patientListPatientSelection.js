
// ----- Add Patients By Filtering or Selecting -----

$(document).on("click", ".segmented-btn", function () {
    $(".segmented-btn").removeClass("pressed");
    $('.patientlist-form-tab').addClass("d-none");
    $(this).addClass("pressed");

    let containerId = $(this).attr('data-container');
    $(`#${containerId}`).removeClass('d-none');
});

$(document).on("change", "[name=addPatientsBy]", function () {
    $('.patientlist-form-tab').addClass("d-none");
    let containerId = $(this).attr('data-container');
    $(`#${containerId}`).removeClass('d-none');
});

// -----  Add / Remove Patient From List -----

function addPatientToList(event, patientId, patientListId) {
    event.stopPropagation();
    event.preventDefault();

    requestObject = {};
    requestObject['patientId'] = patientId;
    requestObject['patientListId'] = patientListId;

    callServer({
        type: 'POST',
        url: '/PatientList/AddPatient',
        data: requestObject,
        success: function (data) {
            toastr.success('Patient Added');
            $(".dropdown-menu").dropdown('hide');
            filterData();
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function removePatientFromList(event, patientId, patientListId) {
    event.stopPropagation();
    event.preventDefault();

    requestObject = {};
    requestObject['patientId'] = patientId;
    requestObject['patientListId'] = patientListId;

    callServer({
        type: 'DELETE',
        url: '/PatientList/RemovePatient',
        data: requestObject,
        success: function (data) {
            $(`#row-${patientId}`).remove();
            toastr.success('Patient Removed');
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    })
}

$(document).on('mouseover', '.nested-dropdown', function () {
    if ($(this).attr('aria-expanded') == "false") {
        $(this).trigger('click');
    }
});