function handleSuccessFormSubmitFromPatient(data, isEditForm) {
    if (isEditForm) {
        reloadAfterFormInstanceChange();
    }
    else {
        let activeEncounterId = getActiveEncounterId();
        showDetails(activeEncounterId, false);
        submitNewEncounterDocument(data.formInstanceId, $("#patientId").val(), $("#activeEOC").val(), activeEncounterId, false, true)
    }
}

function handleBackInFormAction() {
    showEncounterData(getEncounterTypeId());
}

function getEncounterTypeId() {
    return $('.single-encounter.active-encounter').first().attr("id");
}

function getPatientInfo(e, patientId, readOnly, activeEOC) {
    e.preventDefault();
    var action = readOnly ? "View" : "Edit";
    if (activeEOC != "0")
        window.location.href = `/Patient/${action}PatientInfo?patientId=${patientId}&isReadOnlyViewMode=${readOnly}&activeEOC=${activeEOC}`;
    else
        window.location.href = `/Patient/${action}PatientInfo?patientId=${patientId}&isReadOnlyViewMode=${readOnly}`;
}

function loadDynamicForm(e) {
    $("#formInstanceContainer").show();
    let selectedFormElement = $('.single-form-list-element.active').first();
    if (selectedFormElement.length === 0) {
        toastr.error('Please select a document');
        return;
    }
    let formId = $(selectedFormElement).data('id');

    createNewFormInstance(formId);
}

function createNewFormInstance(formId) {
    var episodeOfCareId = $('#episodeOfCareId').val();
    var patientId = $('#patientId').val();
    var encounterId = getActiveEncounterId();

    callServer({
        type: "GET",
        url: `/DiagnosticReport/CreateFromPatient?encounterId=${encounterId}&episodeOfCareId=${episodeOfCareId}&patientId=${patientId}&formId=${formId}&${getReferralsAsParameter()}`,
        data: {},
        success: function (data) {
            deactivateSelectedFormInstance();
            $("#formInstanceContainer").html(data);         
            $(".extended-div").addClass("hidden-element");
            closeCustomModal();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function deactivateSelectedFormInstance() {
    $(".edit").removeClass("blue-pencil");
}

$(document).on('click', '.show-form-referrals-button', function (e) {
    e.preventDefault();
    var episodeOfCareId = $('#episodeOfCareId').val();
    var encounterId = getActiveEncounterId();

    callServer({
        method: 'get',
        url: `/Encounter/ListReferralsAndForms?encounterId=${encounterId}&episodeOfCareId=${episodeOfCareId}`,
        success: function (data) {
            let modalMainContent = $("#customModalMainContent");
            modalMainContent.html(data);
            $('body').addClass('no-scrollable');
            $('.custom-modal').addClass('show');
            $('.custom-modal').trigger('lowZIndex');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    })
})

function addReferrals(e, id) {
    e.preventDefault();
    var element = $(`#${id}`);
    if (element.hasClass('active'))
        element.removeClass('active');
    else
        element.addClass("active");
}

$(document).on('click', '.single-form-list-element', function (e) {
    let unselect = $(this).hasClass('active');
    $('.single-form-list-element.active').removeClass('active');
    if (unselect) {
        $(this).removeClass('active');
    } else {
        $(this).addClass('active');
    }
});

var loadingForms;
let debounceTimer;

$(document).on('keyup', '#searchCondition', function (e) {
    clearTimeout(debounceTimer);

    const inputValue = $(this).val();
    if (typeof inputValue !== 'undefined') {
        debounceTimer = setTimeout(() => {
            if (loadingForms) {
                loadingForms.abort();
            }

            loadingForms = callServer({
                method: 'GET',
                url: `/Encounter/ListForms?condition=${inputValue}`,
                success: function (data) {
                    $('#formsContainer').html(data);
                    loadingForms = null;
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }, 300);
    }
});

function loadGuidelineInstanceTable() {
    let episodeOfCareId = $('#id').val();
    window.open(`/DigitalGuidelineInstance/GuidelineInstance?episodeOfCareId=${episodeOfCareId}`);
}

function getReferralsAsParameter() {
    let referralParams = [];
    let referrals = getSelectedReferrals();
    referrals.forEach(x => {
        if (x) {
            referralParams.push(`referrals=${x}`);
        }
    });

    return referralParams.join('&');
}

function getSelectedReferrals() {
    let result = [];
    $('.single-referral-item.active').each(function (index, element) {
        result.push($(this).data('id'));
    })

    return result;
}

function pinOrUnpinSuggestedForm(e, id) {
    if (e.currentTarget.classList.contains("pinned")) {
        e.currentTarget.classList.remove("pinned");
        removeSuggestedForm(e, id);
    }
    else {
        e.currentTarget.classList.add("pinned");
        callServer({
            method: 'PUT',
            url: `/User/AddSuggestedForm?formId=${id}`,
            success: function (data) {
                showSuggestions();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        })
    }
}

function removeSuggestedForm(e, id) {
    e.preventDefault();
    e.stopPropagation();
    callServer({
        method: 'PUT',
        url: `/User/RemoveSuggestedForm?formId=${id}`,
        success: function (data) {
            showSuggestions();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    })
}

/*Modal*/

$(document).on('click', '.custom-modal-body', function (e) {
    e.stopPropagation();
});

$(document).on('click', '.custom-modal', function (e) {
    closeCustomModal();
});

$(document).on('click', '.close-custom-modal-button', function (e) {
    closeCustomModal();
});

function getPatientId() {
    return $("#patientTree").data("patientid");
}

function closeCustomModal() {
    $('.custom-modal').removeClass('show');
    $('body').removeClass('no-scrollable');
    $('.custom-modal').trigger('defaultZIndex');
}

function showAddEocModal(event, id) {
    executeEventFunctions(event, true);
    var request = getPatientRequestObject({ episodeOfCareId: id ? id : 0 });
    let episodeAction;

    if (request.isReadOnlyViewMode) {
        episodeAction = 'ViewEpisodeOfCare';
    } else if (id) {
        episodeAction = 'UpdateEpisodeOfCare';
    } else {
        episodeAction = 'AddEpisodeOfCare';
    }

    callServer({
        type: "GET",
        data: request,
        url: `/EpisodeOfCare/${episodeAction}`,
        success: function (data) {
            $('#addEocModal').html(data);
            $('#addEocModal').modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, true);
        }
    });
}

function viewEpisodeOfCare(episodeOfCareId) {
    var patientId = $("#patientId").val();
    callServer({
        type: "POST",
        url: `/EpisodeOfCare/EditFromEOC?episodeOfCareId=${episodeOfCareId}`,
        success: function (data) {
            viewPatientInfo(patientId);
            $('#eocInfo').removeClass('eoc-info').addClass('edit-eoc-info');
            $("#eocInfo").html(data);
            history.pushState({}, '', `?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}`);
            viewPatientBasicInfo(patientId, episodeOfCareId);
            $('#episodeOfCareSelect option[value="' + episodeOfCareId + '"]').attr('selected', 'selected');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function viewEOCEncounters(readOnly, episodeOfCareId, event) {
    var patientId = $("#patientId").val();
    window.location.href = readOnly == "true" ? `/Patient/ViewPatientEncounters?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}` : `/Patient/EditPatientEncounters?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}`;
    event.preventDefault();
    event.stopPropagation();
}


function viewPatientInfo(patientId) {
    callServer({
        type: "POST",
        url: `/Patient/PatientInfo?patientId=${patientId}`,
        success: function (data) {
            $("#patientContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function viewPatientBasicInfo(patientId, episodeOfCareId) {
    callServer({
        type: "POST",
        url: `/Patient/PatientBasicInfo?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}&isReadOnlyViewMode=${readOnly}`,
        success: function (data) {
            $("#patientBasicInfo").html(data);
            showEocInfoAndHideButton();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showEocInfoAndHideButton() {
    var eocInfoDiv = document.querySelector('.eoc-info-div');
    eocInfoDiv.removeAttribute('hidden');
}

function removeEoc(event, id) {
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/EpisodeOfCare/DeleteEOC?eocId=${id}`,
        success: function (data) {
            reloadEpisodeOfCares();
            toastr.success(`Success`);        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function submitEOCForm() {
    updateDisabledOptions(false);
    $('#newEocForm').validate();
    if ($('#newEocForm').valid()) {
        var period = {
            StartDate: toDateStringIfValue($("#periodStartDate").val()),
            EndDate: toDateStringIfValue($("#periodEndDate").val())
        };

        var request = {};
        request['Id'] = $("#editEocId").val();
        request['StatusCD'] = $("#status").val();
        request['TypeCD'] = $("#type").val();
        request['Period'] = period;
        request['PatientId'] = $("#patientId").val();
        request['Description'] = $("#description").val();
        request['PersonnelTeamId'] = $('#careteam-name-select2 :selected').val();
        var action = request.Id ? "Edit" : 'Create';
        callServer({
            type: "POST",
            url: `/EpisodeOfCare/${action}`,
            data: request,
            success: function (data, textStatus, jqXHR) {
                $('#addEocModal').modal('hide');
                reloadEpisodeOfCares();
                toastr.success("Success");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }

        });

    }
    return false;
}

function reloadEpisodeOfCares() {
    var request = getPatientRequestObject({
        TypeCD : $("#EocTypeTemp").val(),
        StatusCD : $("#EocStatusTemp").val(),
        PatientId: $("#patientId").val()
    });

    callServer({
        type: 'GET',
        url: `/EpisodeOfCare/ReloadEOCFromPatient`,
        data: request,
        success: function (data) {
            $("#eocContainer").html(data);  
            type = $("#containerContentType").val();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getPatientRequestObject(requestObject) {
    setIsReadOnlyViewModeInRequest(requestObject);
    return requestObject;
}

$("#typeEOC").val();

var type = "";

function formatEpisodeOfCare(episodeOfCare) {
    if (!episodeOfCare.id) {
        return episodeOfCare.text;
    }
    var $episode = $('<span class="eoc-tt">' + episodeOfCare.text + '</span>');
    return $episode;
}

function showEpisodeOfCareContent() {
    callServer({
        type: "GET",
        data: getPatientRequestObject({}),
        url: '/EpisodeOfCare/ShowEpisodeOfCareContent',
        success: function (data) {
            showEpisodeOfCareFilterGroup();
            $("#eocInfo").html(data);
            reloadEpisodeOfCares();
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showEpisodeOfCareFilterGroup() {
    callServer({
        type: "GET",
        data: getPatientRequestObject({}),
        url: '/EpisodeOfCare/ShowEpisodeOfCareFilterGroup',
        success: function (data) {
            $("#filterGroup").html(data);
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function advanceFilter() {
    pushState();
    reloadEpisodeOfCares();
}

function getFilterParametersObject() {
    let result = {};
    if ($('#EocTypeTemp').val()) {
        addPropertyToObject(result, 'EOCType', $('#EocTypeTemp').val());
    }
    if ($('#EocStatusTemp').val()) {
        addPropertyToObject(result, 'Status', $('#EocStatusTemp').val());
    }

    return result;
}

function pushState() {
    let urlPageParams;
    var patientId = $("#patientId").val();
    urlPageParams = `?patientId=${patientId}`;
    let filter = getFilterParametersObject();
    let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

    history.pushState({}, '', fullUrlParams);
}

function addPropertyToObject(object, name, value) {
    if (value) {
        object[name] = value;
    }
}

function getFilterUrlParams(filter) {
    let result = "";
    for (const property in filter) {
        result = result.concat(`&${property}=${filter[property]}`);
    }
    return result;
}

function cancelForm() {
    noDocuments();
}

function noDocuments() {
    callServer({
        type: "GET",
        url: `/EpisodeOfCare/NoDocumentIsSelected`,
        data: {},
        success: function (data) {
            deactivateSelectedFormInstance();
            $("#formInstanceContainer").html(data);
            if ($(".no-result-content").length > 0) {
                $("#formInstanceContainer").removeClass("display-flex");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function toggleVisibility() {
    var showBtn = document.querySelector('.show-btn');
    var contentToToggle = document.getElementById("contentToToggle");
    if (showBtn.parentNode.classList.contains("hidden")) {
        showBtn.parentNode.classList.remove("hidden");
        contentToToggle.classList.add("hidden");
    } else {
        showBtn.parentNode.classList.add("hidden");
        contentToToggle.classList.remove("hidden");
        var patientContainer = document.querySelector(".patient-container");
        patientContainer.style.minWidth = "350px";
        patientContainer.style.maxWidth = "350px";
        var documentContainer = document.querySelector(".eoc-document");
        documentContainer.style.width = "unset";
    }
}

function hidePatientInfo() {
    var showBtn = document.querySelector('.show-btn');
    var contentToToggle = document.getElementById("contentToToggle");
    if (showBtn.parentNode.classList.contains("hidden")) {
        showBtn.parentNode.classList.remove("hidden");
        contentToToggle.classList.add("hidden");
        var patientContainer = document.querySelector(".patient-container");
        patientContainer.style.width = "auto";
        patientContainer.style.minWidth = "fit-content";
        var documentContainer = document.querySelector(".eoc-document");
        documentContainer.style.width = "calc(100% + 367px)";
    }
}