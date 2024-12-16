function showEncounterModal(event, isSelectedEncounterType, id, readOnly = false, fromEncounterTable = false) {
    event.preventDefault();
    event.stopPropagation();
    let encounterId = id ? id : 0;
    let encounterAction;

    if (readOnly) {
        encounterAction = 'ViewEncounter';
    } else if (id) {
        encounterAction = 'EditEncounter';
    } else {
        encounterAction = 'AddEncounter';
    }

    if (encounterId) {
        $("#activeEncounter").val(encounterId);
    }

    if (fromEncounterTable) {
        let request = {};
        request['EncounterId'] = id;
        request['isReadOnlyViewMode'] = readOnly;
    }

    callServer({
        type: 'GET',
        data: fromEncounterTable ? request : getPatientRequestObject({ encounterId: encounterId }),
        url: `/Encounter/${encounterAction}`,
        success: function (data) {
            $('#addEncounterModal').html(data);
            if (!fromEncounterTable)
                setEncounterTypeIfAny(isSelectedEncounterType);
            $('#addEncounterModal').modal('show');
            $("#fromEncounterTable").val(fromEncounterTable);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, true);
        }
    });
}

function submitEncounterForm(event) {
    event.preventDefault();
    event.stopPropagation();
    removeEncounterDoctorErrorValidations();
    $("#newEncForm").validate();
    updateDisabledOptions(false);
    if ($("#newEncForm").valid()) {
        var period = {
            StartDate: $("#periodStartDateTime").val(),
            EndDate: $("#periodEndDateTime").val()
        };

        var request = {};
        request['EpisodeOfCareId'] = $("#encounterContainer").attr("data-episode-of-care");
        request['Id'] = $("#editEncounterId").val();
        request['StatusCD'] = $("#status").val();
        request['ClassCD'] = $("#classification").val();
        request['TypeCD'] = $("#type").val();
        request['ServiceTypeCD'] = $("#servicetype").val();
        request['PatientId'] = $("#patientId").val();
        request['Period'] = period;
        request['Doctors'] = getDoctors();

        var fromEncounterTable = ($("#fromEncounterTable").val() === "true");
        var action = request.Id && request.Id != '0' ? "Edit" : "Create";

        callServer({
            type: 'POST',
            url: `/Encounter/${action}`,
            data: request,
            success: function (data, jqXHR) {
                $('#addEncounterModal').modal('hide');
                resetFormFields();
                if (!fromEncounterTable) {
                    updateEncounterIdInUrl(data.id);
                    showEncounterData(request.TypeCD, null, true);
                }
                toastr.success("Success");
            },
            error: function (xhr, thrownError) {
                handleResponseError(xhr);
            }
        });

        if (fromEncounterTable) {
            reloadTable();
        }
    }
    return false;
}

function getDoctors() {
    let doctors = [];

    $(".doctor-list .encounter-doctor-row").each(function () {
        let doctorId = $(this).find('.encounter-doctor').val();
        let relationTypeId = $(this).find('.encounter-doctor-relation').val();
        doctors.push({
            Id: $(this).attr('data-id'),
            RelationTypeId: relationTypeId,
            DoctorId: doctorId
        });
    });

    return doctors;
}

function resetFormFields() {
    $("#servicetype").val("");
    $("#status").val("");
    $("#classification").val("");
    $("#periodStartDate").datepicker('setDate', null);
}

function setTableMaxHeight(tableId, contentId) {
    var table = document.getElementById(tableId);
    var windowHeight = window.innerHeight;
    var tableOffset = table.offsetTop;
    var maxHeight = windowHeight - tableOffset - 70;

    var tableContent = document.getElementById(contentId);
    tableContent.style.maxHeight = maxHeight + "px";
}