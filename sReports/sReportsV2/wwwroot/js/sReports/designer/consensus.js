function renderViewWhenConsensusTabIsActive() {
    activateConsensusMode();
}

$(document).on('click', "#consensusBtn", function (e) {
    if ($(this).hasClass('active')) {
        deactivateConsensusMode();
    } else {
        activateConsensusMode();
    }
});

function deactivateConsensusMode() {
    const urlParams = new URLSearchParams(location.search);
    const thesaurusId = urlParams.get('thesaurusId');
    const versionId = urlParams.get('versionId');
    if (thesaurusId && versionId) {
        window.location.href = `/Form/Edit?thesaurusId=${thesaurusId}&versionId=${versionId}`;
    }
}

function activateConsensusMode() {
    $("#consensusBtn").addClass('active pressed');
    hideNonConsensusContainers();
    $('#formPreviewContainer').addClass('w-100');
    loadConsensusPartial();
}

function hideNonConsensusContainers() {
    $('.consensus-hidden').hide();
}

function loadConsensusTree() {
    let formId = $("#formId").val();

    callServer({
        method: 'get',
        url: `/FormConsensus/ReloadConsensusTree?formId=${formId}`,
        success: function (data) {
            $('#consensusTree').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(".consensus-checkbox").click(function () {
    let currentValue = $(this).is(':checked');
    let siblings;
    if ($(this).attr('name') === 'Form') {
        siblings = $(`.consensus-checkbox`);

    } else {
        siblings = $(`[name="${$(this).attr('name')}"]`);
        siblings.push($('[name="Form"]'));
    }

    $(siblings).each(function (index, element) {
        $(element).prop('checked', false);
    });

    $(this).prop('checked', currentValue);
});

function proceed() {
    let request = {};
    request['QuestionOccurences'] = [];
    $(".consensus-proceed").find('input[type=Radio]').each(function (index, element) {
        if ($(element).is(":checked")) {
            let parameter = {};
            parameter["Level"] = $(element).attr('name');
            parameter["Type"] = $(element).attr('data-question-type');
            request['QuestionOccurences'].push(parameter);
        }
    });
    if (request['QuestionOccurences'].length == 0) {
        if ($(".form-level").find('input[type=checkbox]').first().is(':checked')) {

            request['QuestionOccurences'].push({
                Level: 'Form',
                Type: 'Same'
            });
        } else {
            request['QuestionOccurences'].push({
                Level: 'Chapter',
                Type: 'Any'
            });

            request['QuestionOccurences'].push({
                Level: 'Page',
                Type: 'Any'
            });

            request['QuestionOccurences'].push({
                Level: 'FieldSet',
                Type: 'Any'
            });

            request['QuestionOccurences'].push({
                Level: 'Field',
                Type: 'Any'
            });

            request['QuestionOccurences'].push({
                Level: 'Fieldvalue',
                Type: 'Any'
            });
        }
    }


    request.FormId = $("#nestable").find(`li[data-itemtype='form']`).attr('data-id');
    request.ConsensusId = $("#consensusId").val();
    request.IterationId = $("#iterationId").val();

    callServer({
        method: 'post',
        data: request,
        url: `/FormConsensus/ProceedConsensus`,
        success: function (data) {
            $('#consensusTree').html(data);
            $('#proceedButtonContainer').remove();
            $('#terminateButtonContainer').show();
            $("#usersCosensusTab").removeClass("d-none");
            $("#trackerTab").removeClass("d-none");
            $('.consensus-decision-item').find('input').attr('disabled', 'disabled');
            $('.consensus-decision-item').find('.btn-question-occurence-item-reset').hide();
            $('.consensus-decision-item').addClass('started-iteration');
            toastr.success("Success")
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function openAddQuestion(id) {
    clearCreateQuestion();
    $('.tree-item').addClass('c-tree-item-margin');
    $('.question-create').hide();
    $(`#qc-${id}`).show();
    //scrollToElement($(`#qc-${id}`), 1000, 50);
    $('.question-preview').hide();
}

function adddNewAnswer(id) {
    let clone = $(`#answers-${id}`).find('.answer:last').clone();
    let answerCount = $(`#answers-${id}`).find('.answer').length;
    $(clone).find('.answer-label').text(`Answer ${++answerCount}:`)
    $(clone).find('.answer-value').val('');
    $(`#answers-${id}`).append(clone);
}

function finalizeQuestion(id) {
    let request = {};
    request["ItemRef"] = id;
    request["Question"] = $(`#qc-${id}`).find('.question-value:first').val();
    request["Options"] = [];

    $(`#qc-${id}`).find('.answer-value').each(function (index, element) {
        if ($(element).val()) {
            request["Options"].push($(element).val());
        }
    });


    if (request["Question"] && request["Options"].length > 0) {
        submitQuestion(request);
    } else {
        toastr.error("Question and answers are required!")
    }
    
    
}

function submitQuestion(request) {
    let formId = $("#nestable").find(`li[data-itemtype='form']`).attr('data-id');
    let itemId = request['ItemRef'];
    let itemType = $("#nestable").find(`li[data-id='${itemId}']`).attr('data-itemtype');
    request['Level'] = itemType;
    let iterationId = $("#iterationId").val();
    callServer({
        method: 'post',
        data: request,
        url: `/FormConsensus/AddQuestion?formId=${formId}&iterationId=${iterationId}`,
        success: function (data) {
            $('#consensusTree').html(data);
            toastr.success("Success");
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function clearCreateQuestion() {
    $('.tree-item').removeClass('c-tree-item-margin');
    $('.question-create').hide();
    $('.question-preview').show();

    $(`.qc-container`).each(function (ind, ele) {
        let isFirst = true;
        $(ele).find('.answer').each(function (index, element) {
            if (isFirst) {
                isFirst = false;
                $(element).find('.answer-value').val('');
            } else {
                $(element).remove();
            }
        });
    })

    $(`.qc-container`).find('.question-value').val('');

}

function openPopupComment(id) {
    $('.popuptext').each(function (index, element) {
        if ($(element).closest('.popup').find('.fa-comment:first').attr('data-itemref') !== id) {
            $(element).removeClass('show');
        }
    });
    var popup = document.getElementById(`popup-${id}`);
    if (!$(popup).hasClass('show')) {
        popup.classList.toggle("show");
    } else
    {
        $(popup).removeClass('show');
    }
}

function showConsensusFormPreview() {
    let formId = $("#formId").val();

    callServer({
        method: 'get',
        url: `/FormConsensus/GetConsensusFormPreview?formId=${formId}`,
        success: function (data) {
            let divWrapper = $('<div></div>')
                .addClass('consensus-questionnaire')
                .html(data);
            $("#consensusContainer").html(divWrapper);
            $(`#consensusContainer`).find('.form-instance-button-container').hide();
            $('#questionnaireSaveButton').hide();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showConsensusUsers() {
    callServer({
        method: 'get',
        url: `/FormConsensus/GetConsensusUsersPartial?consensusId=${$("#consensusId").val()}&readOnlyMode=${readOnlyViewModeViewBag}`,
        success: function (data) {
            $("#consensusContainer").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showConsensusTrackerData() {
    callServer({
        method: 'get',
        url: `/FormConsensus/GetTrackerData?consensusId=${$("#consensusId").val()}`,
        success: function (data) {
            $("#consensusContainer").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showConsensusQuestionnaire() {
    callServer({
        method: 'get',
        url: `/FormConsensus/GetQuestionnairePartial?${getConsensusInstanceUserQueryParams()}`,
        success: function (data) {
            $("#consensusContainer").html(data);
            if ($("#consensusContainer").val() === "true") {
                $('#questionnaireSaveButton').show();
            } else {
                $('#questionnaireSaveButton').hide();
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function setActiveTab(name, element) {
    switch (name) {
        case 'consensusFormPreview':
            showConsensusFormPreview();
            break;
        case 'consensusUsers':
            showConsensusUsers();
            break;
        case 'consensusTrackProcess':
            showConsensusTrackerData();
            break;
        case 'consensusQuestionnaire':
        default:
            showConsensusQuestionnaire();
    }
    $(".consensus-tab").removeClass('active-item');
    $(element).addClass('active-item');
}

function filterOrganizationHierarchy() {
    let name = $('#organizationName').val();
    let countries = [];
    $('.country-filter-element').each(function (index, element) {
        countries.push($(element).attr('data-value'));
    });

    callServer({
        method: 'post',
        data: {countries, name},
        url:`/FormConsensus/GetUserHierarchy`,
        success: function (data) {
            $("#organizationHierarchy").html(data);
            updateNumberOfSelectedUsers();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })


    $("#consensusUsers").find('.selected-users-container:first').remove();

}

$(document).on('click', ".organization-checkbox", function (e) {
    if ($(this).is(':checked')) {
        $(this).closest('.user-children').addClass('item-active');
    } else {
        $(this).closest('.user-children').removeClass('item-active');
    }

    filterUsers();
});

function filterUsers() {
    let organizationIds = [];
    $(".organization-checkbox:checked").each(function (index, element) {
        organizationIds.push($(element).val());
    });

    callServer({
        method: 'post',
        data: { organizationIds },
        url: `/FormConsensus/ReloadUsers?consensusId=${$("#consensusId").val()}`,
        success: function (data) {
            $("#consensusUsers").html(data);
            updateNumberOfSelectedUsers();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

$(document).on("click", ".consensus-organization-checkbox", function () {
    if ($(this).is(":checked")) {
        $(this).closest('.organization-with-users').find(".user-children").addClass('item-active');
    } else {
        $(this).closest('.organization-with-users').find(".user-children").removeClass('item-active');
    }
    $(this).closest('.organization-with-users').find(".consensus-user-checkbox").prop("checked", $(this).is(":checked"));

});

$(document).on("click", ".consensus-user-checkbox", function () {
    if ($(this).is(":checked")) {
        $(this).closest('.user-children').addClass('item-active');
    } else {
        $(this).closest('.user-children').removeClass('item-active');
    }

    let isEveryChecked = true;
    $(this).closest('.organization-with-users').find(".consensus-user-checkbox").each(function (index, element) {
        if (!$(element).is(":checked")) {
            isEveryChecked = false;
        }
    });

    $(this).closest(".organization-with-users").find(".consensus-organization-checkbox").prop("checked", isEveryChecked);


});

function submitOutsideUser(e) {
    e.stopPropagation();
    e.preventDefault();
    $("#addUserForm").validate({
        ignore: []
    });
    $("#addUserForm").validate();
    if ($("#addUserForm").valid()) {

        updateOutsideUser();
        return true;
    }
    return false;
}

function updateOutsideUser() {
    let user = getUserFromModal();

    callServer({
        method: 'post',
        data: user,
        url: `/FormConsensus/CreateOutsideUser`,
        success: function (data) {
            $("#usersOutsideSystem").html(data);
            $("#addUserFormModal").modal('hide');
            $("#userId").val('');
            updateNumberOfSelectedUsers();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getUserFromModal() {
    let user = {};
    user["Id"] = $("#addUserForm").find("#userId").val();
    user["FirstName"] = $("#addUserForm").find("#firstName").val();
    user["LastName"] = $("#addUserForm").find("#lastName").val();
    user["Email"] = $("#addUserForm").find("#email").val();
    user["Institution"] = $("#addUserForm").find("#institution").val();
    user["InstitutionAddress"] = $("#addUserForm").find("#institutionAddress").val();

    let address = {};
    address["City"] = $("#addUserForm").find("#city").val();
    address["CountryCD"] = $("#addUserForm").find("#countryCD").val();
    address["PostalCode"] = $("#addUserForm").find("#postalCode").val();
    address["Street"] = $("#addUserForm").find("#street").val();
    address["StreetNumber"] = $("#addUserForm").find("#streetNumber").val();

    user["Address"] = address;
    user["ConsensusRef"] = $("#consensusId").val();

    return user;
}

function deleteOutsideUser(id) {
    callServer({
        method: 'post',
        url: `/FormConsensus/DeleteOutsideUser?userId=${id}&consensusId=${$("#consensusId").val()}`,
        success: function (data) {
            $(`#${id}`).remove();
            updateNumberOfSelectedUsers();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });

}

function deleteInsideUser(id) {
    callServer({
        method: 'post',
        url: `/FormConsensus/DeleteInsideUser?userId=${id}&consensusId=${$("#consensusId").val()}`,
        success: function (data) {
            $(`#${id}`).remove();
            updateNumberOfSelectedUsers();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });

}

function openAddUserModal(e) {
    e.preventDefault();
    e.stopPropagation();
    $("#userId").val('');
    $("#addUserForm").find('input').val('');
    initCodeSelect2('', '', "countryCD", "country", "Country", 'addUserFormModal');
    $("#countryCD").val('').trigger("change");
    $("#addUserFormModal").modal('show');
}

function editOutsideUser(id) {
    $("#userId").val(id);
    $("#usersOutsideSystem").find(`#${id}`).find('.outside-user-item').each(function (index, element) {
        let inputId = $(element).attr('data-modal-id');
        let inputValue = $(element).attr('data-modal-value');
        if (inputId == 'countryCD') {
            initCodeSelect2(inputValue, inputValue, "countryCD", "country", "Country", 'addUserFormModal');
        } else {
            $(`#${inputId}`).val(inputValue);
        }
    });

    $("#addUserFormModal").modal('show');
}

function updateNumberOfSelectedUsers() {
    let sReportsUsersCount = $("#usersInsideSystem").find('.outside-user').length;
    let outsideUsersCount = $("#usersOutsideSystem").find(".outside-user").length;
    $("#numOfSelectedUsers").text(sReportsUsersCount + outsideUsersCount);
}

function startConsensusFindingProcess() {
    let usersIds = [];
    let numberOfSelectedReviewers = $("#numOfSelectedUsers").text();
    if (numberOfSelectedReviewers == 0) {
        toastr.warning("Please select at least one reviewer.");
        return;
    }

    $("#usersInsideSystem").find(".outside-user").each(function (index, element) {
        usersIds.push($(element).attr("id"));
    });

    let outsideUsersIds = [];
    $("#usersOutsideSystem").find(".outside-user").each(function (index, element) {
        outsideUsersIds.push($(element).attr("id"));
    });

    let request = {};
    request["UsersIds"] = usersIds;
    request["OutsideUsersIds"] = outsideUsersIds;
    request["ConsensusId"] = $("#consensusId").val();
    request["EmailMessage"] = $("#emailMessage").val();


    callServer({
        method: 'post',
        data: request,
        url: `/FormConsensus/StartConsensusFindingProcess`,
        success: function (data) {
            toastr.success(data.message);
            hideStartConsensusFindingProcessBtn();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function hideStartConsensusFindingProcessBtn() {
    $("#submitConsensusFindingProcess").addClass("d-none");
}

function saveSelectedUsers() {
    let newUsers = [];
    $(".consensus-user-checkbox:checked").each(function (index, element) {
        newUsers.push($(element).val());
    });

    callServer({
        method: 'post',
        data: { usersIds: newUsers },
        url: `/FormConsensus/SaveUsers?consensusId=${$("#consensusId").val()}`,
        success: function (data) {
            $("#usersInsideSystem").html(data);
            toastr.success("Success");
            updateNumberOfSelectedUsers();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function startNewIteration() {
    let formId = $("#nestable").find(`li[data-itemtype='form']`).attr('data-id');

    callServer({
        method: 'get',
        url: `/FormConsensus/StartNewIteration?consensusId=${$("#consensusId").val()}&formId=${formId}`,
        success: function (data) {
            loadConsensusPartial();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function terminateCurrentIteration() {
    callServer({
        method: 'get',
        url: `/FormConsensus/TerminateCurrentIteration?consensusId=${$("#consensusId").val()}`,
        success: function (data) {
            toastr.success('Current iteration is terminated');
            loadConsensusPartial();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function loadConsensusPartial() {
    let formId = $("#nestable").find(`li[data-itemtype='form']`).attr('data-id');

    callServer({
        method: 'get',
        url: `/FormConsensus/LoadConsensusPartial?formId=${formId}`,
        success: function (data) {
            $('#consensusPartialContainer').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function collapseIteration(header) {
    if ($(header).next().hasClass("show")) {
        $(header).children('.iteration-icon').removeClass('fa-angle-up');
        $(header).children('.iteration-icon').addClass('fa-angle-down');
    } else {                 
        $(header).children('.iteration-icon').removeClass('fa-angle-down');
        $(header).children('.iteration-icon').addClass('fa-angle-up');
    }
}

function remindUser(userId, isOutsideUser, iterationId) {

    callServer({
        method: 'get',
        url: `/FormConsensus/RemindUser?userId=${userId}&consensusId=${$("#consensusId").val()}&isOutsideUser=${isOutsideUser}&iterationId=${iterationId}`,
        success: function (data) {
            toastr.success('Successs');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('change', 'input[name=Form]', function (e) {
    if ($(this).is(':checked')) {
        $('.item-questions-container input[type=radio]').each(function () {
            this.checked = false;
        });
        $('.item-questions-container.consensus-decision-item').addClass('started-iteration');
        $('.item-questions-container input[type=radio]').attr('disabled', "disabled");
        $('.consensus-decision-item').find('.btn-question-occurence-item-reset').hide();
    } else {
        $('.item-questions-container input[type=radio]').removeAttr('disabled');
        $('.item-questions-container.consensus-decision-item').removeClass('started-iteration');

        $('.item-questions-container input[type=radio]').each(function () {
            if ($(this).attr('data-question-type') == 'Different') {
                this.checked = true;
            } else {
                this.checked = false;
            }
        });
        $('.consensus-decision-item').find('.btn-question-occurence-item-reset').show();
    }

});

$(document).on('click', '.btn-question-occurence-item-reset', function (e) {
    $(this).closest(".item-questions-container").find('input[type=Radio]').prop("checked", false);
});