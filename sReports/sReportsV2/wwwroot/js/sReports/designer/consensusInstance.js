function saveConsensusInstance(event) {
    submitConsensusInstance($(event.target).data('userid'), $(event.target).data('consensusid'), $(event.target).data('isoutsideuser'));
}

function submitConsensusInstance(userId, consensusId, isOutsideUser, autosave = false) {
    let consensusInstance = {};
    consensusInstance["ConsensusRef"] = consensusId;
    consensusInstance["UserRef"] = userId;
    consensusInstance["IsOutsideUser"] = isOutsideUser;
    consensusInstance["Questions"] = getQuestions();
    consensusInstance["Id"] = $("#consensusInstanceId").val();
    consensusInstance["IterationId"] = $("#iterationId").val();

    callServer({
        method: 'post',
        data: consensusInstance,
        url: `/FormConsensus/CreateConsensusInstance`,
        success: function (data) {
            if (autosave) {
                $('#consensusInstanceId').val(data.id);
            } else {
                location.reload();
                toastr.success("Success");
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function getQuestions() {
    let questions = [];
    $(".question-preview").each(function (index, element) {
        let question = {};
        question["ItemRef"] = $(element).data('id');
        question["Options"] = [];
        $(element).find(".consensus-radio").each(function (i, el) {
            question["Options"].push($(el).val());
        });
        question["Value"] = $(element).find(".consensus-radio:checked").val();
        question["Question"] = $(element).find('.qp-question').attr("data-value");
        question["Comment"] = $(`#qcomment-${$(element).data('id')}`).val();

        questions.push(question);
    });

    return questions;
}

function loadConsensusInstanceTree() {
    callServer({
        method: 'get',
        url: `/FormConsensus/ReloadConsensusInstanceTree?${getConsensusInstanceUserQueryParams()}`,
        success: function (data) {
            $('#consensusTree').html(data);
            $('.consensus-visible').show();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showQuestionnaireSaveModal() {
    $("#saveModal").modal('show');
}

function questionnaireSaveModalDecision(event, decision) {
    event.preventDefault();
    $("#saveModal").modal('hide');

    if (decision == 'yes') {
        submitConsensusInstance($('#questionnaireSaveButton').data('userid'), $('#questionnaireSaveButton').data('consensusid'), $('#questionnaireSaveButton').data('isoutsideuser'), true);
    }

    $(".consensus-tab").removeClass('active-item');
    $("#consensusFormPreviewTab").addClass('active-item');
    showConsensusFormPreview();
}

function getConsensusInstanceUserQueryParams() {
    return $.param({
        formId: $("#formId").val(),
        consensusInstanceId: $("#consensusInstanceId").val(),
        viewType: $("#viewType").val(),
        iterationId: $("#iterationId").val(),
        isOutsideUser: $("#isOutsideUser").val(),
        userId: $("#userId").val(),
        showQuestionnaireType: $("#showQuestionnaireType").val()
    });
}