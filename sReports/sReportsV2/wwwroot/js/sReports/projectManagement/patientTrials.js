
function reloadPatientInfo() {
    let patientId = $("#patientId").val();
    callServer({
        type: "GET",
        url: `/Patient/GetPatientInfo?patientId=${patientId}&isReadOnlyViewMode=${false}`,
        success: function (data) {
            $('#trialsContainer').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

// ----- Remove Trial -----

$(document).on('click', '.ct-remove', function (e) {
    let id = $(this).attr('data-ctid');
    showDeleteModal(e, id, 'removePatientFromTrial', null, null);
});

function removePatientFromTrial(event) {
    event.stopPropagation();
    event.preventDefault();

    let projectId = $('#buttonSubmitDelete').attr('data-id');
    let patientId = $("#patientId").val();

    callServer({
        type: "DELETE",
        url: `/ProjectManagement/RemovePatientFromTrial?patientId=${patientId}&projectId=${projectId}`,
        success: function (data) {
            $(`#trialIdContainer-${projectId}`).remove();
            toastr.success('Trial Removed');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

// ----- Add Trials -----

function addTrials(patientId) {
    let requestObject = {};
    requestObject['patientProjects'] = getSelectedProjects(patientId);

    callServer({
        type: 'POST',
        url: '/ProjectManagement/AddPatientToTrials',
        data: requestObject,
        success: function (data) {
            reloadPatientInfo();
        },
        error: function (xhr, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getSelectedProjects(patientId) {
    let patientProjects = [];

    $('#searchedTrialsContainer').find(':checkbox').each(function () {
        if (this.checked) {
            let projectId = parseInt($(this).val());
            if (!isNaN(projectId)) {
                patientProjects.push({
                    projectId, patientId
                });
            }
        }
    });
    return patientProjects;
}


// ----- Go To Trial -----

function goToTrial(projectId) {
    window.open(`/ProjectManagement/Edit?projectId=${projectId}`, `newTab-${projectId}`);
}

// ----- Search Trial By Name -----

$(document).on('keyup', '#clinicalTrialsSearchInput', function (e) {
    e.stopImmediatePropagation();
    let textFilter = $('#clinicalTrialsSearchInput').val();
    textFilter = textFilter.normalize("NFD").replace(/\p{Diacritic}/gu, "")  // https://stackoverflow.com/questions/990904/remove-accents-diacritics-in-a-string-in-javascript

    if (textFilter) {
        searchTrialByTitle(textFilter, 1);
    }
    else {
        searchTrialByTitle("", 1);
    }
});

function searchTrialByTitle(title = "", page = 1, loadMore = false) {

    $('.no-trial-found').hide();
    let patientId = $("#patientId").val();

    callServer({
        type: "GET",
        data: {term: title, page: page, patientId: patientId},
        url: `/ProjectManagement/GetTrialAutoCompleteTitleForPatient`,
        success: function (data) {

            if (!loadMore) {
                $('#searchedTrialsContainer').html('');
            }
            else {
                $('.load-more-ct').closest('.trial-item').remove();
            }
            appendTrials(data.results);

            if (data.pagination.more) {
                appendLoadMoreBtnHtml(title, page + 1);
            }

            if (data.results.length == 0) {
                $('.no-trial-found').show();
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
            $('.no-trial-found').show();
        }
    });
}

function appendTrials(trials) {

    $(trials).each(function () {
        $('#searchedTrialsContainer').append(getTrialHtml(this.id, this.text));
    });
}

function getTrialHtml(id, title) {
    let trialHtml = `
        <div class="trial-item py-2 px-2">
            <label class="form-checkbox-label field-instance width-fit-content">
                <input class="checkbox-radio form-checkbox-field" type="checkbox" value="${id}" onclick="return true">
                <i class="form-checkbox-button table-checkbox dynamic-checkbox-size"></i>
            </label>
            <div class="trial-title">${title}</div>
        </div>`;

    return $('<div>').html(trialHtml);
}

function appendLoadMoreBtnHtml(title, newPage) {
    let loadMoreHtml = `
        <div class="trial-item py-2 px-2">
            <div class="trial-title m-auto cursor-pointer load-more-ct" onclick="searchTrialByTitle('${title}', ${newPage}, true)"> Load More </div>
        </div>`;

    $('#searchedTrialsContainer').append($('<div>').html(loadMoreHtml));
}

// ----- Helpers ------

$(document).on('click', '.trials-dropdown-menu', function (e) {
    e.stopPropagation();
});
