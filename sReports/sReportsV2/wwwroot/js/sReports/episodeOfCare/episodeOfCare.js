$(window).resize(function () {
    showArrows();
})

function showEncounterData(encTypeId, callback = null, isInit = false, resetForm = true) {
    var eocid = $("#encounterContainer").attr("data-episode-of-care");

    callServer({
        type: "GET",
        data: getPatientRequestObject({ episodeOfCareId: eocid }),
        url: '/EpisodeOfCare/EncounterData',
        success: function (data) {
            $("#encounterContainer").html(data);
            showEncounters(encTypeId, $("#activeEncounter").val(), isInit, resetForm);
            showArrows();

            if (callback) {
                callback();
            }
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showArrows() {
    var containerWidth = $("#test").width();
    var encountersWidth = $("#encounterTypesContainer").width();

    if (containerWidth - 9 > encountersWidth) {
        $('.left-arr-scr').hide();
        $('.right-arr-scr').hide();
    } else {
        $('.left-arr-scr').show();
        $('.right-arr-scr').show();
    }
}

$(document).ready(function () {
    setCommonValidatorMethods();

    var activeEncounterType = $("#activeEncounterType").val();

    var formInstanceId;
    var readOnly;
    if (hasParamInUrl('formInstanceId')) {
        formInstanceId = getParamFromUrl('formInstanceId');
        readOnly = getParamFromUrl('readOnly') == "true";
    }
    var resetForm = !hasParamInUrl('formInstanceId');
    showEncounterData(activeEncounterType, function () {
        if (formInstanceId) {
            setTimeout(function () {
                showFormInstanceDetails(formInstanceId, readOnly);
            }, 1000);
        }
    }, false, resetForm);
  
});

var globalEncType = 0;

function showEncounters(encounterTypeId, activeEncounterId, isInit, resetForm) {
    globalEncType = encounterTypeId;
    var count = $("#eocTypesCount").text();

    if (count != 0) {
        if (encounterTypeId == undefined || encounterTypeId == 0) {
            encounterTypeId = $(".single-encounter").last().attr("id");
            encounterTypeId = encounterTypeId == undefined ? 0 : encounterTypeId;
        }
        var episodeOfCareId = $("#encounterContainer").attr("data-episode-of-care");

        $(".single-encounter").removeClass("active-encounter");
        $(`#${encounterTypeId}`).addClass("active-encounter");

        callServer({
            type: 'GET',
            data: getPatientRequestObject({ encounterTypeId: encounterTypeId, episodeOfCareId: episodeOfCareId }),
            url: '/Encounter/ShowEncounterTypeEncounters',
            success: function (data) {
                $("#encountersContainer").html(data);
                var encounterId = $(".title-encounter").last().attr("id");
                if (activeEncounterId != undefined && !isInit && (globalEncType != "0" || encounterId == undefined))
                    encounterId = activeEncounterId;
                getActiveBreadcrumbValue(episodeOfCareId, encounterId);
                showDetails(encounterId, resetForm);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function getActiveBreadcrumbValue(episodeOfCareId, encounterId) {
    callServer({
        url: '/EpisodeOfCare/GetActiveBreadcrumbValue',
        method: 'GET',
        data: { episodeOfCareId: episodeOfCareId, encounterId: encounterId  },
        success: function (data) {
            $('.breadcrumb-active').html("<a>" + data + "</a>");
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function showSuggestions() {
    callServer({
        type: 'GET',
        url: `/Encounter/GetSuggestedForms`,
        success: function (data) {
            $("#suggestionsContainer").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showDetails(encounterId, resetForm = true) {
    if (!encounterId) return;
    var episodeOfCareId = $("#encounterContainer").attr("data-episode-of-care");
    var patientId = $(".eoc-encounter-documents-container").attr("data-patient");
    history.pushState({}, '', `?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}&encounterId=${encounterId}`);

    $(".encounter-div").removeClass("active-border");
    $(`#encounter-${encounterId}`).toggleClass("active-border");

    if (resetForm)
        cancelForm();

    callServer({
        type: 'GET',
        data: getPatientRequestObject({ encounterId: encounterId }),
        url: '/Encounter/ShowEncounterDetails',
        success: function (data) {
            $("#activeEncounter").val(encounterId);
            $("#encounterDocumentationContainer").html(data);
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function removeEncounter(event, id) {
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/Encounter/Delete?id=${id}`,
        success: function (data) {
            viewEpisodeOfCare($("#encounterContainer").attr("data-episode-of-care"));
            var encountersContainer = document.getElementById("encountersContainer");
            let numberOfDataEncounterIds = 0;

            if (encountersContainer) {
                var encounterDivs = encountersContainer.getElementsByClassName("encounter-div");

                for (const encounterDiv of encounterDivs) {
                    var dataEncounterId = encounterDiv.getAttribute("data-encounter-id");
                    if (dataEncounterId) {
                        numberOfDataEncounterIds++;
                    }
                }
            }
            if (numberOfDataEncounterIds <= 1)
                $("#activeEncounterType").val("0");

            $("#activeEncounter").val('');
            toastr.success(`Success`);
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showExtended(formInstanceId) {
    $(`#details-${formInstanceId}`).toggleClass("hidden-element");
    $(`#eye-${formInstanceId}`).toggleClass("blue-eye");
}

var formInstance;

function showFormInstanceDetails(formInstanceId, viewFormInstance = false) {
    $(document).off('click', '.dropdown-matrix');
    var formInstanceActive = getActiveFormInstanceId();

    if (formInstanceActive != formInstanceId) {
        deactivateSelectedFormInstance();
    }
    if (viewFormInstance) {
        $(`#edit-${formInstanceId}`).removeClass("blue-pencil");
        $(`#edit-${formInstanceId}`).toggleClass("view-form-instance");
    }
    else {
        $(`#edit-${formInstanceId}`).removeClass("view-form-instance");
        $(`#edit-${formInstanceId}`).toggleClass("blue-pencil");
    }

    var patientId = $(".eoc-encounter-documents-container").attr("data-patient");
    var episodeOfCareId = $("#encounterContainer").attr("data-episode-of-care")
    var encounterId = getActiveEncounterId();

    if ($('#formInstanceContainer').show() && !$(`#edit-${formInstanceId}`).hasClass("blue-pencil") && !viewFormInstance) {
        clearEncounterFormInstance(patientId, episodeOfCareId, encounterId);
    }
    else if (!$(`#edit-${formInstanceId}`).hasClass("view-form-instance") && viewFormInstance) {
        clearEncounterFormInstance(patientId, episodeOfCareId, encounterId);
    }
    else {
        submitNewEncounterDocument(formInstanceId, patientId, episodeOfCareId, encounterId, viewFormInstance);
    }
}

function submitNewEncounterDocument(formInstanceId, patientId, episodeOfCareId, encounterId, viewFormInstance, submitNew = false, callback = null) {
    var requestObject = {};
    requestObject['formInstanceId'] = formInstanceId;
    requestObject['setReferrableValues'] = submitNew;

    if (viewFormInstance) {
        requestObject['isReadOnlyViewMode'] = true;
    } else {
        requestObject = getPatientRequestObject(requestObject);
    }
    history.pushState({}, '', `?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}&encounterId=${encounterId}&formInstanceId=${formInstanceId}&readOnly=${viewFormInstance}`);
    callServer({
        type: 'GET',
        data: requestObject,
        url: '/DiagnosticReport/ShowFormInstanceDetails',
        success: function (data) {
            $("#formInstanceContainer").html(data);
            $("#formInstanceContainer").addClass("display-flex");
            if (submitNew)
                $(`#edit-${formInstanceId}`).toggleClass("blue-pencil");
            executeCallback(callback);
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr, true);
        }
    });
}

function clearEncounterFormInstance(patientId, episodeOfCareId, encounterId) {
    cancelForm();
    initialFormData = '{}';
    history.pushState({}, '', `?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}&encounterId=${encounterId}`);
}

function setEncounterTypeIfAny(isSelectedEncounterType) {
    if (isSelectedEncounterType) {
        var count = $("#eocTypesCount").text();
        if (count != 0) {
            var activeEncounterId = $(".single-encounter.active-encounter").attr("id");
            $("#type").val(activeEncounterId);
        }
    }
}

function clearEncounterModal() {
    $('#editEncounterId').val('');
    $('#status').val('');
    $('#classification').val('');
    $('#servicetype').val('');
    $('#periodStartDate').val('');
    $('#periodEndDate').val('');
}

function updateEncounterIdInUrl(savedEncounterId) {
    let activeEncounterId = $("#activeEncounter").val();
    if (activeEncounterId != savedEncounterId) {
        var searchParams = new URLSearchParams(window.location.search);
        searchParams.set("encounterId", savedEncounterId);

        let updatedSearchParams = searchParams.toString();
        if (updatedSearchParams) {
            history.pushState({}, '', `?${updatedSearchParams}`);
        }
    }
}

function getActiveEncounterId() {
    return $(".active-border").attr("data-encounter-id");
}

function getActiveFormInstanceId() {
    return $(".blue-pencil").attr("data-form-instance-id");
}

function isPatientModule() {
    return true;
}

function backToAllEocs(event, readOnly) {
    event.preventDefault();
    let patientId = $("#pId").val();
    window.location.href = `/Patient/${readOnly ? 'View' : 'Edit'}?patientId=${patientId}`;
}