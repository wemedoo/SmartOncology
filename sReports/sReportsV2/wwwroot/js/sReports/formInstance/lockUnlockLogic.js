function showLockUnlockDocumentModal(event, formInstanceNextState) {
    executeEventFunctions(event, true);

    callServer({
        type: "GET",
        url: '/FormInstance/GetLockUnlockDocumentModel',
        data: getLockUnlockDocumentModalRequest(formInstanceNextState),
        success: function (data) {
            showLockUnlockDocumentModalContent(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getLockUnlockDocumentModalRequest(formInstanceNextState) {
    var request = {};
    request['formInstanceNextState'] = formInstanceNextState;
    return request;
}

function showLockUnlockDocumentModalContent(data) {
    $("#lockUnlockDocumentModalFormContainer").html(data);
    $("#lockUnlockDocumentModal").attr("data-reload-partially", "false");
    $("#lockUnlockDocumentModal").modal("show");
}

function hideLockUnlockDocumentModalContentAfterConfirmation() {
    emptyLockUnlockDocumentModalContent();
    $("#lockUnlockDocumentModal").attr("data-reload-partially", "true");
    $("#lockUnlockDocumentModal").modal("hide");
}

function emptyLockUnlockDocumentModalContent() {
    $("#lockUnlockDocumentModalFormContainer").html("");
}

$(document).on("keypress", "#lockUnlockDocumentPassword", function (e) {
    if (e.which === enter) {
        executeEventFunctions(e, true);
        $('#lockUnlockButton').trigger('click');
    }
})

function lockOrUnlockDocument() {
    callServer({
        type: "POST",
        url: '/FormInstance/LockOrUnlockDocument',
        data: getLockUnlockDocumentRequest(),
        success: function (data) {
            if (isLockUnlockDocumentValid(data)) {
                handleAfterLockConfirmation(data);
            } else {
                showLockUnlockDocumentModalContent(data);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getLockUnlockDocumentRequest() {
    var request = {};
    request["formInstanceId"] = getFormInstanceId();
    request['password'] = $("#lockUnlockDocumentPassword").val();
    request['formInstanceNextState'] = $("#FormInstanceNextState").val();
    request['lastUpdate'] = getLastUpdate();
    return request;
}

$(document).on("focus", "#lockUnlockDocumentPassword", function () {
    if ($(this).hasClass("error")) {
        $(this).removeClass("error");
        $("#lock-unlock-password-error").html('');
    }
})

function isLockUnlockDocumentValid(data) {
    try {
        return $(data).find('input.error').length == 0;
    } catch (e) {
        return true;
    }
}

function showLockOrUnlockPartialConfirmationModal(event, chapterId, pageId, isLockAction) {
    executeEventFunctions(event, true);

    callServer({
        type: "GET",
        url: `/FormInstance/${getLockOrUnlockActionName(pageId, isLockAction)}`,
        data: getLockModalRequest(chapterId, pageId),
        success: function (data) {
            showLockUnlockDocumentModalContent(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getLockOrUnlockActionName(pageId, isLockAction) {
    if (isLockAction) {
        return pageId ? "GetLockPageInstancePartially" : "GetLockChapterInstancePartially";
    } else {
        return pageId ? "GetUnLockPageInstancePartially" : "GetUnLockChapterInstancePartially";
    }
}

$(document).on('click', '#lockUnlockButton', function (event) {
    executeEventFunctions(event, true);
    window[$(this).attr('data-action')]();
});

function lockOrUnlockPartially() {
    let request = getLockModalRequest($("#ChapterId").val(), $("#PageId").val());
    request['password'] = $("#lockUnlockDocumentPassword").val();
    request['chapterPageNextState'] = $("#ChapterPageNextState").val();

    callServer({
        type: "POST",
        url: $("#ActionUrl").val(),
        data: request,
        success: function (data) {
            if (isLockUnlockDocumentValid(data)) {
                handleAfterLockConfirmation(data);
            } else {
                showLockUnlockDocumentModalContent(data);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getLockModalRequest(chapterId, pageId) {
    var request = {};
    request['pageId'] = pageId;
    request['chapterId'] = chapterId;
    request['formInstanceId'] = getFormInstanceId();
    request['lastUpdate'] = getLastUpdate();
    return request;
}

function handleAfterLockConfirmation(responseMessage) {
    toastr.success(responseMessage);
    hideLockUnlockDocumentModalContentAfterConfirmation();
}

$(document).on('hidden.bs.modal', '#lockUnlockDocumentModal', function (e) {
    executeEventFunctions(e);
    emptyLockUnlockDocumentModalContent();
    if ($("#lockUnlockDocumentModal").attr("data-reload-partially") == "true") {
        reloadAfterFormInstanceChange();
    }
});