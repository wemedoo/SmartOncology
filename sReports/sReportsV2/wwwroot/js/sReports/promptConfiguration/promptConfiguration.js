$(document).on('click', '.edit-button', function (e) {
    e.stopPropagation();
    e.preventDefault();
    let element = $(e.currentTarget).closest('li.dd-item');
    let fieldId = $(element).attr('data-itemtype') == 'field' ? $(element).attr('data-id') : '';

    callServer({
        type: 'GET',
        url: '/PromptConfiguration/GetPrompt',
        data: getPromptRequestObject(fieldId),
        success: function (data) {
            $('#promptDisplayContainer').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
});

$(document).on('click', '#previewPrompts', function (e) {
    e.stopPropagation();
    e.preventDefault();

    callServer({
        type: 'GET',
        url: '/PromptConfiguration/PreviewPrompts',
        data: getPromptRequestObject(),
        success: function (data) {
            $('#prompt-form-modal-body').html(data);
            $('#promptPreviewModal').modal('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
});

$(document).on('click', '#addNewPromptVersion', function (e) {
    e.preventDefault();

    callServer({
        type: 'POST',
        url: '/PromptConfiguration/AddNewPromptVersion',
        data: getPromptRequestObject(),
        success: function (data) {
            toastr.success('Success');
            let version = `${data.major}.${data.minor}`;
            $('.version-link.selected').removeClass('selected');
            $('#prompt-current-version').text(version);
            let dropDownLink = `<a class="dropdown-item version-link selected" href="#" data-version-id="${data.id}">${version}</a>`;
            $('#version-dropdown').append($(dropDownLink));
            $('#promptDisplayContainer').html('');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
});

$(document).on('click', '.version-link', function (e) {
    e.preventDefault();
    let dropDownLink = $(this);
    let request = getPromptRequestObject();
    request.versionId = $(dropDownLink).attr('data-version-id');
    if (request.versionId) {
        callServer({
            type: 'POST',
            url: '/PromptConfiguration/SwitchPromptVersion',
            data: request,
            success: function (data) {
                toastr.success('Success');
                $('#prompt-current-version').text($(dropDownLink).text());
                $('.version-link.selected').removeClass('selected');
                $(dropDownLink).addClass('selected');
                $('#promptDisplayContainer').html('');
                if (data) {
                    $('#updatePrompt').removeClass('d-none');
                } else {
                    $('#updatePrompt').addClass('d-none');
                }
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
});

function getPromptRequestObject(fieldId, prompt) {
    var url = new URL(window.location.href);
    return {
        formId: url.searchParams.get("formId"),
        versionId: getActiveVersionId(),
        fieldId: fieldId,
        prompt: prompt,
        projectId: url.searchParams.get("projectId")
    }
}

$(document).on('click', '#updatePrompt', function (e) {
    e.preventDefault();

    callServer({
        type: 'POST',
        url: '/PromptConfiguration/UpdatePrompt',
        data: getPromptRequestObject($('#prompt-field-id').val(), $('#prompt-text').val()),
        success: function (data) {
            toastr.success('Success');
            if (!getActiveVersionId()) {
                $('.version-link.selected').attr('data-version-id', data);
                $('#addNewPromptVersion').removeClass('d-none');
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
});

function getActiveVersionId() {
    return $('.version-link.selected').attr('data-version-id')
}