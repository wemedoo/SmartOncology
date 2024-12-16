$(document).click(function () {
    if ($('#comments-btn').hasClass('active')) {
        redrawLines();
    }
});

$(document).on('click', ".comment-button", function (e) {

    e.stopPropagation();
    e.preventDefault();
    setCanvasSize();

    if (!$(this).hasClass('active')) {
        renderViewWhenCommentsTabIsActive(0);
    } else {
        $(this).removeClass('active pressed');
        removeCommentSection();
        showNestableContainer();
    }
}); 

function renderViewWhenCommentsTabIsActive(taggedCommentId) {
    var $this = $(".comment-button");
    var formId = $this.attr('data-value');

    $this.addClass('active pressed');
    reloadComments(formId, taggedCommentId);
    showCommentSection();
    hideNestableContainer();
}

function isCommentSectionDisplayed() {
    let displayCommentSectionProp = $("#commentSection").css('display');
    return displayCommentSectionProp !== 'none';
}

function submitCommentForm(form, e) {
    $(form).validate({
        ignore: []
    });

    if ($(form).valid()) {
        let commentId = $(form).attr('data-id');
        let newComment = +commentId ? getReplyCommentObject(form) : getCommentObject(form);

        callServer({
            type: 'post',
            url: '/Form/AddComment',
            data: newComment,
            success: function (data) {
                updateCommentSection(data);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    return false;
}

function getCommentObject(form) {
    let commentId = $(form).attr('data-id');
    var comment = {
        Value: $(`#commentText_${commentId}`).html(),
        FormRef: $("li[data-itemtype='form']:first").attr('data-id'),
        ItemRef: $("#itemRef").val(),
        TaggedUsers: getTaggedUsers(commentId),
        CommentStateCD: $(`#commentText_${commentId}`).attr('data-commentstate')
    };

    return comment;
}

function getReplyCommentObject(form) {
    let commentId = $(form).attr('data-id');
    var commText = $(form).find(`#commentText_${commentId}`).html();
    var formRef = $("li[data-itemtype='form']:first").attr('data-id');
    var itemRef = $(form).find(`#idItemRefReplay_${commentId}`).val();

    var replyComment = {
        Value: commText,
        CommentRef: commentId,
        FormRef: formRef,
        ItemRef: itemRef,
        TaggedUsers: getTaggedUsers(commentId),
        CommentStateCD: $(`#commentText_${commentId}`).attr('data-commentstate')
    };

    return replyComment;
}

function replayComment(id) {
    $(`#${id}`).find('.replay-container:first').css('display', 'block');
    redrawLines();
}

function cancelReplay(id) {
    $(`#${id}`).find('.replay-container:first').css('display', 'none');
    redrawLines();
}

function cancelNewComment() {
    var $div = $('div#newComment');
    $div.css('display', 'none');
}

function sendCommentStatus(id, stateCD) {

    var dataToSend = {
        commentId: id,
        stateCD: stateCD
    };

    callServer({
        method: 'post',
        url: '/Form/SendCommentStatus',
        data: dataToSend,
        success: function (data) {
            updateCommentSection(data);
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error("Your account cannot change the status of comments");
        }
    });
}

function drawCommentLines(){
    $('.comment-section').each(function (index, element) {
        let targetId = $(element).attr('data-field-id');
        let targetItem = $(`#${targetId}`);
        drawLine(targetItem, element);
    });
}

function drawLine(item, comment) {
    if ($('#comments-btn')?.hasClass('active')) {
        let canvasXoffset = $("#designerCanvas").offset().left;
        let canvasYoffset = $("#designerCanvas").offset().top;

        let itemPaddingLR = parseInt($(item).css('padding-right'));
        let itemPaddingBT = parseInt($(item).css('padding-top'));

        let x1 = 0;
        let y1 = 0;
        if ($(item).length) {
            x1 = $(item).offset().left - canvasXoffset + $(item).width() + itemPaddingLR * 2;
            y1 = $(item).offset().top - canvasYoffset + ($(item).height() + itemPaddingBT * 2) / 2;
        }

        let x2 = $(comment).offset().left - canvasXoffset;
        let y2 = $(comment).find('.comm-user-icon:first').offset().top - canvasYoffset + $(comment).find('.comm-user-icon:first').height() / 2;

        let c = document.getElementById("designerCanvas");
        let ctx = c.getContext("2d");
        ctx.beginPath();
        ctx.strokeStyle = '#d93d3d';
        ctx.moveTo(x1, y1);
        ctx.quadraticCurveTo(x1 + (x2 - x1) / 4, y1, x1 + (x2 - x1) / 2, y1 + (y2 - y1) / 2);
        ctx.stroke();

        ctx.moveTo(x1 + (x2 - x1) / 2, y1 + (y2 - y1) / 2);
        ctx.quadraticCurveTo(x1 + 3 * (x2 - x1) / 4, y2, x2, y2);
        ctx.stroke();
    }
}

function reloadComments(formId, taggedCommentId = 0) {
    let urlParameter = taggedCommentId > 0 ? `?formId=${formId}&taggedCommentId=${taggedCommentId}` : `?formId=${formId}`;
    callServer({
        method: 'post',
        url: `/Form/GetAllCommentsByForm${urlParameter}`,
        success: function (data) {
            updateCommentSection(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function clearLines() {
    let c = document.getElementById("designerCanvas");
    let ctx = c.getContext("2d");
    ctx.clearRect(0, 0, c.width, c.height);
}

function updateCommentSection(htmlData) {
    $('#commentSection').html(htmlData);
    redrawLines();
}

function showCommentSection() {

    $("#nestableFormElements").hide();
    $('#dd-btn').removeClass('expanded');


    $('.icon-container .edit-button').css('display', 'none');
    $('.item-settings-button .edit-button').addClass('d-none');
    $('.icon-container .remove-button').css('display', 'none');

    $('#nestableFormPartial .drag-item-icon').addClass('pe-none');
    $('.add-comment-link').removeClass('comment-block-hide');
    $('.add-comment-link').addClass('comment-block-show');

    $("#commentSection").show();
}

function removeCommentSection() {
    $('.icon-container .edit-button').css('display', 'flex');
    $('.item-settings-button .edit-button').removeClass('d-none');
    $('.icon-container .remove-button').css('display', 'flex');

    $('#nestableFormPartial .drag-item-icon').removeClass('pe-none');
    $('.add-comment-link').addClass('comment-block-hide');
    $('.add-comment-link').removeClass('comment-block-show');

    $("#commentSection").hide();

    clearLines();
    removeTargetItemActive();
}

function addComment(id) {
    var dataToSend = { fieldId: id };
    callServer({
        method: 'post',
        url: '/Form/AddCommentSection',
        data: dataToSend,
        success: function (data) {
            $('#newComment').html(data);  
            redrawLines();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
    $('#newComment').css('display', 'flex');
}

function redrawLines() {
    setTargetItemActive();
    setCanvasSize();
    clearLines();
    drawCommentLines();
}

function setTargetItemActive() {
    $('.comment-section').each(function (index, element) {
        let targetId = $(element).attr('data-field-id');
        $(`#${targetId}`).addClass('active');
    });
}

function removeTargetItemActive() {
    $('.comment-section').each(function (index, element) {
        let targetId = $(element).attr('data-field-id');
        $(`#${targetId}`).removeClass('active');
    });
}
