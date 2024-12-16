// ----- Arrows and DropDowns -----

$(window).resize(function () {
    showArrows();
});

function scrollArrowsByPixels(arrow, container, pixels) {
    if (arrow == 'left-arrow') {
        $(`#${container}`).animate({
            scrollLeft: `-=${pixels}px`
        }, "slow");
    } else if (arrow == 'right-arrow') {
        $(`#${container}`).animate({
            scrollLeft: `+=${pixels}px`
        }, "slow");
    }
}

function showArrows() {
    if ($("#scrollList").get(0).scrollWidth > $("#scrollList").get(0).clientWidth) { 
        $('.scroll-list-arrow-left').show();
        $('.scroll-list-arrow-right').show();
    } else {
        $('.scroll-list-arrow-left').hide();
        $('.scroll-list-arrow-right').hide();
    }
}

$(document).on('show.bs.dropdown', '.dropdown', function () {
    let dataId = $(this).attr('data-id');
    if (dataId) {
        $(`.item-spacer[data-id=${dataId}]`).show();
        let currentDropDown = this;
        $('body').append($(currentDropDown).css({
            position: 'absolute',
            left: $(currentDropDown).offset().left,
            top: $(currentDropDown).offset().top
        }).detach());
    }
});

$(document).on('hidden.bs.dropdown', '.dropdown', function () {
    let dataId = $(this).attr('data-id');
    if (dataId) {
        $(`.item-spacer[data-id=${dataId}]`).hide();
        let currentDropDown = this;
        $(`.scroll-list-item-dropdown-container[data-id=${dataId}]`).append($(currentDropDown).css({
            position: 'relative',
            left: '0px',
            top: '0px'
        }).detach());
    }
});

function showPatientListsContainer() {
    $('#patientListOuterContainer').removeClass('d-none');
}

// ----- Get Patient Lists -----

$(document).ready(function () {
    reloadPatientLists();
});


$(document).on('click', '.scroll-list-item:not(".add-scroll-list-item")', function () {
    selectPatientList(this);
})

function selectPatientList(element) {
    $('.scroll-list-item').removeClass('selected');
    $(element).addClass('selected');
    filterData();
}

function reloadPatientLists(page = 1, pageSize = 10, appendResults = false, listToSelectId = null, callback = null) {
    let requestObject = {};
    requestObject['Page'] = page;
    requestObject['PageSize'] = pageSize;
    requestObject['PatientListId'] = hasParamInUrl('PatientListId') ? getParamFromUrl('PatientListId') : 0;

    callServer({
        type: 'GET',
        url: '/PatientList/GetAll',
        data: requestObject,
        success: function (data) {
            if (appendResults) {
                $('#scrollList').append(data);
            }
            else {
                $('#scrollList').html(data);
            }

            let patientListCount = $('#scrollList').find('.scroll-list-item').length;
            if (patientListCount > 0) {
                showPatientListsContainer();
                $('[data-toggle="tooltip"]').tooltip({
                    placement: 'auto'
                });
                if (listToSelectId) {
                    selectPatientList($(`.scroll-list-item[data-patientlistid=${listToSelectId}]`));
                }
            }
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

function loadMorePatientLists(event, page, pageSize) {
    event.preventDefault();
    $('.load-more-scroll-list-items').remove();
    reloadPatientLists(page, pageSize, true);
}

function addAdditionalPatientListFilterTags(requestObject) {
    if (requestObject.PatientListId && requestObject.PatientListId != '0' && (!requestObject.ListWithSelectedPatients || requestObject.ListWithSelectedPatients == 'false')) {
        getPatientListModalContent('Edit', requestObject.PatientListId, false, function () {
            let patientListFilterTagsObject = {};

            addPropertyToObject(patientListFilterTagsObject, 'PersonnelTeam', getSelectedSelect2Label("personnelTeam"));
            addPropertyToObject(patientListFilterTagsObject, 'AttendingDoctor', getSelectedSelect2Label("attendingDoctor"));
            addPropertyToObject(patientListFilterTagsObject, 'EocType', getSelectedOptionLabel("eocType"));
            addPropertyToObject(patientListFilterTagsObject, 'EncounterStatus', getSelectedOptionLabel("encounterStatus"));
            addPropertyToObject(patientListFilterTagsObject, 'EncounterType', getSelectedOptionLabel("encounterType"));

            addPropertyToObject(patientListFilterTagsObject, 'DateTimeFrom', $('#periodStartDateTime').val());
            addPropertyToObject(patientListFilterTagsObject, 'DateTimeTo', $('#periodEndDateTime').val());
            addPropertyToObject(patientListFilterTagsObject, 'ExcludeDeceased', $('#excludeDeceased').is(':checked') ? $('#excludeDeceased').attr('data-label') : '');
            addPropertyToObject(patientListFilterTagsObject, 'IncludeDischarged', $('#includeDischarged').is(':checked') ? $('#includeDischarged').attr('data-label') : '');
            addPropertyToObject(patientListFilterTagsObject, 'ShowOnlyDischarged', $('#showOnlyDischarged').is(':checked') ? $('#showOnlyDischarged').attr('data-label') : '');

            if (patientListFilterTagsObject['DateTimeFrom']) {
                addPropertyToObject(patientListFilterTagsObject, 'DateTimeFrom', toValidTimezoneFormat(patientListFilterTagsObject['DateTimeFrom']));
            }
            if (patientListFilterTagsObject['DateTimeTo']) {
                addPropertyToObject(patientListFilterTagsObject, 'DateTimeTo', toValidTimezoneFormat(patientListFilterTagsObject['DateTimeTo']));
            }

            reloadTags(patientListFilterTagsObject, undefined, null, false);
        });
    }

}


// ----- Create / Edit / Delete Patient Lists -----

function showPatientListModal(event, action, patientListId = null) {
    event.preventDefault();
    getPatientListModalContent(action, patientListId, true);
}

function getPatientListModalContent(action, patientListId, showModal = false, callback = undefined, addToPatientList = false) {
    let requestObject = {};
    if (action == 'Edit') {
        requestObject['id'] = patientListId;
    }

    callServer({
        type: 'GET',
        url: `/PatientList/${action}`,
        data: requestObject,
        success: function (data) {
            $('#patientListFormContainer').html(data);
            initSelect2Elements();
            if (showModal) {
                $('#patientListModal').modal('show');
            }
            if (addToPatientList) {
                $('#second-patientlist-modal-tab').trigger('click');
            }
            if (callback) {
                callback();
            }
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function initSelect2Elements() {

    let selectsAndActions = [
        { class: "#attendingDoctor", action: `/UserAdministration/GetNameAutocompleteData?filterByDoctors=true`, placeholder: selectAttendingDoctorMsg() },
        { class: "#personnelTeam", action: `/PersonnelTeam/GetNameAutocompleteData?organizationId=${0}`, placeholder: selectPersonnelTeamMsg() } // setting organizationId = 0 we'll get PersonnelTeams form every Org
    ];

    $(selectsAndActions).each(function () {
        let classSelector = this.class;
        let action = this.action;
        let placeholder = this.placeholder;
        $(classSelector).each(function () {
            $(this).initSelect2(
                getSelect2Object(
                    {
                        placeholder: placeholder,
                        width: '100%',
                        allowClear: true,
                        url: action
                    }
                )
            );
        });
    });
}

function submitPatientListForm(action) {

    request = {};
    request['PatientListId'] = $("#patientListId").val();
    request['PatientListName'] = $("#listName").val();
    request['ActiveFrom'] = toDateStringIfValue($("#activeFrom").val());
    request['ActiveTo'] = toDateStringIfValue($("#activeTo").val());

    request['EpisodeOfCareTypeCD'] = $("#eocType").val();
    request['PersonnelTeamId'] = $("#personnelTeam").val();
    request['EncounterTypeCD'] = $("#encounterType").val();
    request['AttendingDoctorId'] = $("#attendingDoctor").val();

    request['ArePatientsSelected'] = $('#customListRadio').is(':checked');

    request['AdmissionDateFrom'] = $('#periodStartDateTime').val();
    request['DischargeDateTo'] = $('#periodEndDateTime').val();  
    request['EncounterStatusCD'] = $('#encounterStatus').val(); 

    request['ExcludeDeceasedPatient'] = $('#excludeDeceased').is(':checked');
    request['IncludeDischargedPatient'] = $('#includeDischarged').is(':checked');
    request['ShowOnlyDischargedPatient'] = $('#showOnlyDischarged').is(':checked');


    checkNameUniquenessAndTrySubmit(action, request, request['PatientListName'], request['PatientListId']);
}


function checkNameUniquenessAndTrySubmit(action, request, nameToCheck, currentId) {

    callServer({
        type: 'GET',
        url: '/PatientList/GetAutoCompleteName',
        data: {
            Term: nameToCheck,
            Page: 1,
        },
        success: function (data) {

            let isPatientListNameUnique = true;
            $(data.results).each(function () {
                if (this.text === nameToCheck && currentId !== this.id) {
                    isPatientListNameUnique = false;
                    return false;
                }
            });
            trySubmitPatientList(action, request, isPatientListNameUnique);
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function trySubmitPatientList(action, request, isPatientListNameUnique) {

    isPeriodValid = validateDateTimeSpan($('#periodStartDateTime').val(), $('#periodEndDateTime').val());

    if (isPatientListNameUnique && isPeriodValid) {
        let currentlySelectedListId = $('.scroll-list-item.selected').attr('data-patientlistid');
        if ($('#patientListForm').valid()) {
            callServer({
                type: "POST",
                url: `/PatientList/${action}`,
                data: request,
                success: function (data, textStatus, jqXHR) {
                    submissionSuccess(action, currentlySelectedListId);        
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }
    else {
        if (!isPatientListNameUnique) {
            triggerPatientListNameUniqueValidation(isPatientListNameUnique);
        }
        if (!isPeriodValid) {
            triggerAdmissionPeriodValidation();
        }
    }
    return false;
}

function submissionSuccess(action, currentlySelectedListId) {
    resetReloadTable();
    $('#patientListModal').modal('hide');
    if (action == 'Create') {
        patientListCreatedSuccess();
        $('.scroll-list-item').removeClass('selected');
        reloadPatientLists(1, 10, false, null, filterData);
    }
    else {
        patientListEditedSuccess();
        reloadPatientLists(undefined, undefined, false, currentlySelectedListId, filterData);
    }
}

// ----- Validation -----

function triggerPatientListNameUniqueValidation(isPatientListNameUnique) {
    if (!isPatientListNameUnique) {
        $('#nonUniqueListName-error').show();
        $('#listName').addClass('error');
    }
}

function resetPatientListNameUniqueValidation() {
    $('#nonUniqueListName-error').hide();
    $('#listName').removeClass('error');
}

$(document).on('keydown', '#listName', function () {
    resetPatientListNameUniqueValidation();
});

function validateDateTimeSpan(from, to) {
    return Date.parse(from) < Date.parse(to);
}

function triggerAdmissionPeriodValidation() {
    $('#admissionPeriod-error').show();
    $('#periodEndDate').addClass('error');
    $('#periodEndTime').addClass('error');
}

function resetAdmissionPeriodValidation() {
    $('#admissionPeriod-error').hide();
    $('#periodEndDate').removeClass('error');
    $('#periodEndTime').removeClass('error');
}

$(document).on('click', '#periodEndDate, #periodEndTime', function () { 
    resetAdmissionPeriodValidation();
});

// ----- Delete Patient Lists -----

function deleteScrollListItem(event) {
    event.stopPropagation();
    event.preventDefault();

    let patientListId = $('#buttonSubmitDelete').attr('data-id');

    callServer({
        type: "DELETE",
        url: `/PatientList/Delete?id=${patientListId}`,
        success: function (data) {
            $(`#scroll-list-item-${patientListId}`).closest('.scroll-list-item').remove();
            $(".dots.active").trigger("click");
            patientListRemovedSuccess();
            showArrows();
            filterData(); // patientTable.js
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}
