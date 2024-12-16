
// Shows and Resets PersonnelTeam Modal
function showPersonnelTeamModal(e) {
    e.stopPropagation();
    resetModal();

    getNewPersonnelTeamView();

    $('#personnelTeamModal').modal('show');
}

function getNewPersonnelTeamView() {
    callServer({
        type: 'GET',
        url: '/PersonnelTeam/GetNewPersonnelTeamView',
        success: function (data) {
            $("#modal-body-personnel-team").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function resetModal() {
    
    $('#personnelTeamModal').find('[data-property="filled"]').each(function () {
        $(this).parent().remove();
    });

    $('#personnel-team-name-input').val('').removeClass('error').trigger('change');
    $('#personnel-team-type-input').val('').removeClass('error').trigger('change');
    $('#modal-yes-radio-btn').prop("checked", true);

    $(".add-personnel-team-member-form").find('.filter-input').removeClass('error');

    $("#personnel-team-members-list").html("");
}

var leaderCodeId = 0;

$(document).ready(function () {
    callServer({
        type: 'GET',
        url: '/PersonnelTeamRelation/GetLeaderCodeId',
        success: function (data) {
            leaderCodeId = data;
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });

    jQuery.validator.addMethod("isPersonnelUnique", function (value, element, param) {
        return isSelectedValueUnique($('.new-team-member-name-select2'), value);
    }, "User already selected, please choose another.");

    jQuery.validator.addMethod("isPersonnelUniqueAjax", function (value, element, param) {
        let retVal = false;
        if (isPersonnelUniqueAjaxBool == "true")
            retVal = true;

        isPersonnelUniqueAjaxBool = "true"; // reset the value
        return retVal;
    }, "User already selected, please choose another.");

    jQuery.validator.addMethod("isLeaderUnique", function (value, element, param) {
        return isSelectedValueUnique($('.new-team-member-role-select2'), leaderCodeId);
    }, "Leader role already assigned, please choose another.");

    jQuery.validator.addMethod("isLeaderUniqueAjax", function (value, element, param) {
        let retVal = false;
        if (isLeaderUniqueAjaxBool == "true")
            retVal = true;

        isLeaderUniqueAjaxBool = "true"; // reset the value
        return retVal;
    }, "Leader role already assigned, please choose another.");
});

function isSelectedValueUnique(elements, value) {
    let isUniqueBool = true;
    let counter = 0;
    elements.each(function () {
        if ($(this).val() == value) {
            counter++;
            if (counter > 1) {
                isUniqueBool = false;
                return false;
            }
        }
    });
    return isUniqueBool;
}

var isLeaderUniqueAjaxBool = "true";
var isPersonnelUniqueAjaxBool = "true";

function addPersonnelTeamMember(elementAdded, personnelTeamId=0) {
    let form = $(elementAdded).closest('form'); 

    form.validate({
        rules: {
            newTeamMemberNameSelect2: {
                required: true,
                isPersonnelUniqueAjax: true,
                isPersonnelUnique: true,
            },
            newTeamMemberRoleSelect2: {
                required: true,
                isLeaderUniqueAjax: true,
                isLeaderUnique: true,
            }
        },
        errorElement: 'div',
        errorClass: 'error custom-error-div',
        errorPlacement: function (error, element) {
            //error.insertAfter(element.next('span'));
            error.appendTo(element.parent());
        }
    });

    if (personnelTeamId != 0 && form.find('.new-team-member-role-select2').val() != null) {
        isLeaderUniqueAjax(form, personnelTeamId)
            .then((dataFromFirstCall) => {
                isLeaderUniqueAjaxBool = dataFromFirstCall;
                return isPersonnelUniqueAjax(form, personnelTeamId);
            })
            .then((dataFromSecondCall) => {
                isPersonnelUniqueAjaxBool = dataFromSecondCall;
                isNewFormValid(elementAdded, form, personnelTeamId);
            })
            .catch((error) => {
                console.error(error);
            });
    }
    else {
        isNewFormValid(elementAdded, form);
    }

}

function isLeaderUniqueAjax(form, personnelTeamId) {

    let request = {
        newTeamMemberRoleSelect2: form.find('.new-team-member-role-select2').val(),
        personnelTeamId: personnelTeamId
    };

    return new Promise((resolve, reject) => {
        callServer({
            type: 'GET',
            url: '/PersonnelTeam/IsLeaderUnique',
            data: request,
            success: function (data) {
                resolve(data);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    });
}

function isPersonnelUniqueAjax(form, personnelTeamId) {
    let request = {
        newTeamMemberNameSelect2: form.find('.new-team-member-name-select2').val(),
        personnelTeamId: personnelTeamId
    };

    return new Promise((resolve, reject) => {
        callServer({
            type: 'GET',
            url: '/PersonnelTeam/IsPersonnelUnique',
            data: request,
            success: function (data) {
                resolve(data);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    });
}

function isNewFormValid(elementAdded, form, personnelTeamId) {

    if (form.valid()) {
        // Showing the Remove-Icon to delete the member (and hiding the Add-Icon)
        $(elementAdded).parent().find('.add-member').hide();
        $(elementAdded).parent().find('.remove-member').addClass('display-flex-important');

        // disabling inputs change after are selected
        form.find('.new-team-member-name-select2').prop('disabled', true);
        form.find('.new-team-member-role-select2').prop('disabled', true);

        // Setting Filled property to get filled forms later
        form.attr("data-property", 'filled');

        if (isLeader(form.find('.new-team-member-role-select2').val())) {
            appendOnTopOfList(form.parent());
        }

        getNewMemberEmptyForm(personnelTeamId);
    }
}

function getNewMemberEmptyForm(personnelTeamId = 0) {
    callServer({
        type: 'GET',
        url: `/PersonnelTeamRelation/GetNewMemberForm?personnelTeamId=${personnelTeamId}`,
        success: function (data) {
            $(".personnel-team-members-list").append(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function removeTeamMember(event) {
    $(event).closest('form').parent().remove();
}

function isLeader(roleCodeId) {
    let retVal = false;
    if (roleCodeId == leaderCodeId)
        retVal = true;

    return retVal;
}

function appendOnTopOfList(element) {
    $('#personnel-team-members-container').prepend(element);
}

function addNewPersonnelTeam(event, existingPersonnelTeamId = 0) {
    updateDisabledOptions(false);
    $('#personnel-team-form').validate({
        rules: {
            personnelTeamNameInput: {
                required: true,
                remote: {
                    async: false,
                    url: `/PersonnelTeam/IsNameNotUsedCheck`, // URL to check if the name exists
                    type: 'GET', // HTTP method to use for the AJAX request
                    data: {
                        // Pass any additional data to the server if needed
                        organizationId: function () {
                            return $('#organizationId').html();
                        },
                        personnelTeamId: function () {
                            return existingPersonnelTeamId;
                        }
                    },
                }
            }
        },
        errorElement: 'div',
        errorClass: 'error custom-error-div',
        errorPlacement: function (error, element) {
            if (element.next('span').length > 0)
                error.insertAfter(element.next('span'));
            else
                error.insertAfter(element);
        }
    });

    if ($('#personnel-team-form').valid()) {
        personnelTeamRequest = getNewPersonnelTeamData();
        personnelTeamRequest.PersonnelTeamId = existingPersonnelTeamId;
        callServer({
            type: 'POST',
            url: '/PersonnelTeam/CreateOrUpdate',
            data: personnelTeamRequest,
            success: function (data) {
                reloadPersonnelTeamTable();
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });

        $('#personnelTeamModal').modal('hide');
        showHidePersonnelTeamTable();
    }
}

function getNewPersonnelTeamData() {

    let personnelTeamRequest = {};
    let personnelTeamRelations = [];

    $('#personnelTeamModal').find('[data-property="filled"]').each(function () {
        personnelTeamRelations.push({
            RelationTypeCD: $(this).find('.new-team-member-role-select2').find(":selected").val(),
            UserId: $(this).find('.new-team-member-name-select2').find(":selected").val()
        });
    });

    personnelTeamRequest.PersonnelTeamOrganizationRelations = [ { OrganizationId : $('#organizationId').html() } ];
    personnelTeamRequest.TeamName = $('#personnel-team-name-input').val();
    personnelTeamRequest.TeamType = $('#personnel-team-type-input').find(":selected").val();
    personnelTeamRequest.Active = $('input[name="modal-active-radio-btn"]:checked').val();
    personnelTeamRequest.PersonnelTeamRelations = personnelTeamRelations;

    return personnelTeamRequest;
}

function showEditPersonnelTeamModal(event, personnelTeamId) {
    resetModal();

    callServer({
        type: 'GET',
        url: `/PersonnelTeam/GetEditView?personnelTeamId=${personnelTeamId}`,
        success: function (data) {
            $("#modal-body-personnel-team").html(data);
            $('#personnelTeamModal').modal('show');            
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showDeletePersonnelTeamModal(event, personnelTeamId) {
    resetModal();

    callServer({
        type: 'DELETE',
        url: `/PersonnelTeam/GetDeleteView?personnelTeamId=${personnelTeamId}`,
        success: function (data) {
            $("#modal-body-personnel-team").html(data);
            $('#personnelTeamModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}


function deletePersonnelTeam(event, personnelTeamId) {
    callServer({
        type: 'DELETE',
        url: `/PersonnelTeam/Delete?personnelTeamId=${personnelTeamId}`,
        success: function (data) {
            $('#personnelTeamModal').modal('hide');
            reloadPersonnelTeamTable();
            toastr.success(`Success`);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

// Care Team Members

function showAddMembersModal(personnelTeamId) {
    resetModal();

    callServer({
        type: 'GET',
        url: `/PersonnelTeamRelation/GetAddTeamMemberView?personnelTeamId=${personnelTeamId}`,
        success: function (data) {
            $("#modal-body-personnel-team").html(data);
            $('#personnelTeamModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showEditMemberModal(personnelTeamRelationId) {
    resetModal();

    callServer({
        type: 'GET',
        url: `/PersonnelTeamRelation/GetEditTeamMemberView?personnelTeamRelationId=${personnelTeamRelationId}`,
        success: function (data) {
            $("#modal-body-personnel-team").html(data);
            $(".remove-member").hide();
            $('#personnelTeamModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function showDeleteMemberModal(personnelTeamRelationId) {
    resetModal();

    callServer({
        type: 'GET',
        url: `/PersonnelTeamRelation/GetDeleteTeamMemberView?personnelTeamRelationId=${personnelTeamRelationId}`,
        success: function (data) {
            $("#modal-body-personnel-team").html(data);
            $('#personnelTeamModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function addMembers(event, personnelTeamId) {

    let personnelTeamRelationDataIns = [];

    $('#personnelTeamModal').find('[data-property="filled"]').each(function () {
        personnelTeamRelationDataIns.push({
            PersonnelTeamId: personnelTeamId,
            PersonnelTeamRelationId: 0,
            RelationTypeCD: $(this).find('.new-team-member-role-select2').find(":selected").val(),
            UserId: $(this).find('.new-team-member-name-select2').find(":selected").val(),
        });
    });

    if (personnelTeamRelationDataIns.length > 0) {
        callServer({
            type: 'POST',
            data: {
                'personnelTeamRelationDataIns': personnelTeamRelationDataIns
            },
            url: `/PersonnelTeamRelation/AddMembers`,
            success: function (data) {
                reloadSinglePersonnelTeamTable(event, personnelTeamId);
                reloadPersonnelTeamTable(1, false);
                toastr.success("Success");
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    $('#personnelTeamModal').modal('hide');
}

function editMember(event, personnelTeamRelationId, personnelTeamId) {
    updateDisabledOptions(false);
    form = $(event.target).parent().parent().find('form');
    form.validate({
        rules: {
            newTeamMemberNameSelect2: {
                required: true,
                isPersonnelUniqueAjax: true,
            },
            newTeamMemberRoleSelect2: {
                required: true,
                isLeaderUniqueAjax: true,
            }
        },
        errorElement: 'div',
        errorClass: 'error custom-error-div',
        errorPlacement: function (error, element) {
            error.insertAfter(element.next('span'));
        }
    });

    let personnelTeamRelationDataIn = {};
    personnelTeamRelationDataIn.PersonnelTeamId = personnelTeamId;
    personnelTeamRelationDataIn.PersonnelTeamRelationId = personnelTeamRelationId;
    personnelTeamRelationDataIn.RelationTypeCD = $('.new-team-member-role-select2').find(":selected").val();

    if (form.find('.new-team-member-role-select2').val() != null) {
        isLeaderUniqueAjax(form, personnelTeamId)
            .then((dataFromFirstCall) => {
                isLeaderUniqueAjaxBool = dataFromFirstCall;
                return isPersonnelUniqueAjax(form, personnelTeamId);
            })
            .then((dataFromSecondCall) => {
                isPersonnelUniqueAjaxBool = dataFromSecondCall;
                if (form.valid()) {
                    callServer({
                        type: 'POST',
                        data: {
                            'personnelTeamRelationDataIn': personnelTeamRelationDataIn
                        },
                        url: `/PersonnelTeamRelation/Edit`,
                        success: function (data) {
                            $('#personnelTeamModal').modal('hide');
                            reloadSinglePersonnelTeamTable(event, personnelTeamId);
                            toastr.success("Success");
                        },
                        error: function (xhr, textStatus, thrownError) {
                            handleResponseError(xhr);
                        }
                    });
                }
            })
            .catch((error) => {
                console.error(error);
            });
    }
}

function deleteMember(event, personnelTeamRelationId, personnelTeamId) {

    callServer({
        type: 'DELETE',
        url: `/PersonnelTeamRelation/Delete?personnelTeamRelationId=${personnelTeamRelationId}`,
        success: function (data) {
            $('#personnelTeamModal').modal('hide');
            reloadSinglePersonnelTeamTable(event, personnelTeamId);
            reloadPersonnelTeamTable(1, false);
            toastr.success("Success");
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}




