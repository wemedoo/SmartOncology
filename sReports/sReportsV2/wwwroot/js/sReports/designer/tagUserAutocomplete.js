$(document).on('keydown', '.comment-text', function (e) {
    if (e.which === enter) {
        let $commentTextInput = $(this);
        var commentId = $commentTextInput.attr('data-id');
        if (isUserProposalSelected($commentTextInput)) {
            e.preventDefault();
        } else {
            handleRegularEnterPress(e, commentId);
        }
    }
});

$(document).on('keyup', '.comment-text', function (e) {
    let $commentTextInput = $(this);
    var commentId = $commentTextInput.attr('data-id');

    let tagIndex = tagIndexes[commentId];
    let commentText = $commentTextInput.html();
    let lastCharacterIndex = getLastCharacterIndex(commentText);

    if (isCommentTyping(e.which)) {
        let lastCharacter = commentText[lastCharacterIndex];
        if (isDeleteAction(tagIndex, lastCharacterIndex)) {
            tagIndex = -1;
        }
        handleSearchUserProposal(commentId, lastCharacterIndex, lastCharacter, tagIndex, commentText, $commentTextInput);
    }

    if (e.which === backSpace) {
        if (isDeleteAction(tagIndex, lastCharacterIndex)) {
            let userOptions = getUserOptionsElement($commentTextInput);
            $(userOptions).hide();
        }

        let shouldUntagUser = decideIfUserShouldBeUntaged(commentId, lastCharacterIndex);
        if (shouldUntagUser) {
            let indexBeforeTagForDeleting = commentText.lastIndexOf("<a");
            let indexAfterTagForDeleting = commentText.lastIndexOf("</a>");

            let textBeforeTagForDeleting = commentText.substring(0, indexBeforeTagForDeleting);
            let textAfterTagForDeleting = commentText.substring(indexAfterTagForDeleting + closingATagOffset);

            $commentTextInput.html(
                textBeforeTagForDeleting +
                textAfterTagForDeleting
            );
            setCursorAfter(commentId, null);

            resetCommentMetadata(commentId);
            $commentTextInput.removeData((lastCharacterIndex - closingATagOffset - minSearchLength).toString());
        }
    }
});

$(document).on('keydown', '.search-user', function (e) {
    handleSearchUserTagging(e);
});

$(document).on("click", '.sidebar-shrink', function (e) {
    if (isCommentSectionDisplayed()) {
        //hide user proposals if click is outside comment
        $('.user-options').hide();
        handleIfClickIsInsideComment(e.target);
    }
});

function isUserProposalSelected($commentInput) {
    var $userOptionForCurrentComment = getUserOptionsElement($commentInput);
    return !$userOptionForCurrentComment.attr("style");
}

function handleRegularEnterPress(e, commentId) {
    let div = document.createElement("div");
    let br = document.createElement("br");
    div.append(br);
    let commentTextInput = e.currentTarget;
    commentTextInput.append(div);
    setCursorAfter(commentId, 0);

    e.preventDefault();
    e.stopImmediatePropagation();
}

function setCursorAfter(commentId, offset) {
    var el = document.getElementById(`commentText_${commentId}`);
    var sel = window.getSelection();
    let lastChild = getLastChildByDepth(el);
    if (lastChild != null) {
        sel.setPosition(lastChild, offset == null ? lastChild.length : offset);
    }
}

function getLastChildByDepth(el) {
    let lastChild = getLastChild(el);
    let hasChild = getLastChild(lastChild);
    while (hasChild) {
        lastChild = getLastChild(lastChild);
        hasChild = getLastChild(lastChild);
    }
    return lastChild;
}

function getLastChild(el) {
    let lastChild = null;
    for (let child of el.childNodes) {
        if (child.tagName != "BR") {
            lastChild = child;
        }
    }
    return lastChild;
}

function getLastCharacterIndex(commentText) {
    let lastCharacterIndex = commentText.length - indexOfset;
    if (commentText.endsWith('<br></div>')) {
        lastCharacterIndex -= (brTagOffset + closingDivTagOffset);
    } else if (commentText.endsWith('</div>')) {
        lastCharacterIndex -= closingDivTagOffset;
    } else if (commentText.endsWith('<br>')) {
        lastCharacterIndex -= brTagOffset;
    }
    return lastCharacterIndex;
}

function isCommentTyping(inputKeyCode) {
    return inputKeyCode !== downArrow && inputKeyCode !== upArrow && inputKeyCode !== enter;
}

function isDeleteAction(tagIndex, lastCharacterIndex) {
    return tagIndex - lastCharacterIndex >= 1;
}

function handleSearchUserProposal(commentId, lastCharacterIndex, lastCharacter, tagIndex, commentText, $commentInput) {
    log('**********************', "");
    log('commentText', commentText);
    log('tagIndex', tagIndex);
    log('lastCharterIndex', lastCharacterIndex);
    log('lastCharter', lastCharacter);

    if (lastCharacter === '@') {
        searchWords[commentId] = "";
        tagIndexes[commentId] = lastCharacterIndex;
    } else if (tagIndex !== -1 && tagIndex != undefined) {
        let searchWord = commentText.substring(tagIndex + 1, lastCharacterIndex + 1);
        log('search word', searchWord);
        searchWords[commentId] = searchWord;
        if (searchWord.length >= minSearchLength) {
            if (isHtmlCharacter(searchWord)) {
                searchWords[commentId] = '';

                let userOptions = getUserOptionsElement($commentInput);
                $(userOptions).html('');
                $(userOptions).show();
            } else {
                callServer({
                    method: 'get',
                    data: { searchWord, commentId },
                    url: `/Form/RetrieveUser`,
                    success: function (data) {
                        let userOptions = getUserOptionsElement($commentInput);
                        $(userOptions).html(data);
                        $(userOptions).show();
                    },
                    error: function (xhr, textStatus, thrownError) {
                        handleResponseError(xhr);
                    }
                });
            }
        } else {
            hideUsersProposalIfShown($commentInput);
        }
    }
}

function isHtmlCharacter(commentText) {
    return commentText.includes("&gt;") || commentText.includes("&lt;");
}

function getUserOptionsElement($element) {
    return $element.closest("div.search-user").siblings(".user-options");
}

function hideUsersProposalIfShown($commentInput) {
    let userOptions = getUserOptionsElement($commentInput);
    if ($(userOptions).css('display') !== 'none') {
        $(userOptions).hide();
    }
}

function decideIfUserShouldBeUntaged(commentId, lastCharacterIndex) {
    let shouldUntagUser = false;
    let taggedUsers = getTaggedUsersFromHtml(commentId);
    for (let columnPosition in taggedUsers) {
        if (parseInt(columnPosition) + closingATagOffset + minSearchLength == lastCharacterIndex) {
            shouldUntagUser = true;
        }
    }
    return shouldUntagUser;
}

function resetCommentMetadata(commentId) {
    searchWords[commentId] = "";
    tagIndexes[commentId] = -1;
}

function handleSearchUserTagging(e) {
    let next;
    if (e.which === downArrow) {
        if (liSelected) {
            $(liSelected).removeClass('selected');
            next = $(liSelected).next();
            if (next.length > 0) {
                liSelected = $(next).addClass('selected');
            } else {
                liSelected = $('.option').eq(0).addClass('selected');
            }
        } else {
            liSelected = $('.option').eq(0).addClass('selected');
        }
    } else if (e.which === upArrow) {
        if (liSelected) {
            $(liSelected).removeClass('selected');
            next = $(liSelected).prev();
            if (next.length > 0) {
                liSelected = $(next).addClass('selected');
            } else {
                liSelected = $('.option').last().addClass('selected');
            }
        } else {
            liSelected = $('.option').last().addClass('selected');
        }
    } else if (e.which === enter) {
        $(liSelected).click();
    }

    e.stopImmediatePropagation();
}

function userOptionClicked(e, userIdentifier, name, commentId) {
    addTaggedUser(userIdentifier, decodeLocalizedString(name), commentId);
}

function addTaggedUser(userIdentifier, name, commentId) {
    let $commentTextInput = $(`#commentText_${commentId}`)
    let comment = $commentTextInput.html();
    let tagIndex = tagIndexes[commentId];

    let host = $(location).prop('origin');
    let hrefUserProfile = `${host}/UserAdministration/DisplayUser?userId=${userIdentifier}`;

    let tagUserElement = $('<a></a>')
        .attr('href', hrefUserProfile)
        .attr('target', '_blank')
        .attr('rel', 'noopener noreferrer')
        .addClass('tagged-user')
        .text(`@${name}`);

    let contentBeforeTaggingHtml = comment.slice(0, tagIndex);
    let userInputLengthWithAtCharacter = searchWords[commentId] ? searchWords[commentId].length + 1 : 0;
    let contentAfterTaggingHtml = comment.slice(tagIndex + userInputLengthWithAtCharacter);
    let tagUserElementHtml = tagUserElement.prop('outerHTML');

    $commentTextInput.html(
        contentBeforeTaggingHtml +
        tagUserElementHtml +
        "&nbsp;" +
        contentAfterTaggingHtml
    );

    var startingTagIndexOffset = tagUserElementHtml.length - name.length - atCharacterLength - closingATagOffset; // tagUserHtml_full_el.length - name.length - 1 [@] - 4 [</a>]
    var taggedUserTagIndex = tagIndex + startingTagIndexOffset;
    $commentTextInput.data(taggedUserTagIndex.toString(), userIdentifier);

    let userOptions = getUserOptionsElement($commentTextInput);
    $(userOptions).hide();

    resetCommentMetadata(commentId);
    setCursorAfter(commentId, 1);
}

function handleIfClickIsInsideComment(target) {
    if ($(target).hasClass('option')) {
        let currentUserOptions = $(target).closest('.user-options');
        showActiveUserOptionsAutocomplete(currentUserOptions);
    }
    if ($(target).hasClass('comment-text')) {
        let currentUserOptions = getUserOptionsElement($(target));
        showActiveUserOptionsAutocomplete(currentUserOptions);
    }
}

function showActiveUserOptionsAutocomplete(currentUserOptions) {
    let commentId = currentUserOptions.attr('data-id');
    let tagIndex = tagIndexes[commentId];
    if (tagIndex != undefined && tagIndex != -1) {
        $(currentUserOptions).show();
    }
}

function getTaggedUsers(commentId) {
    let taggedUsers = getTaggedUsersFromHtml(commentId);
    let taggedUserIds = []
    for (let columnPosition in taggedUsers) {
        if (taggedUsers[columnPosition]) {
            taggedUserIds.push(taggedUsers[columnPosition]);
        }
    }
    return taggedUserIds;
}

function getTaggedUsersFromHtml(commentId) {
    let dataValues = $(`#commentText_${commentId}`).data();
    return Object.keys(dataValues).reduce(function (filtered, key) {
        if (isUserTagData(key)) filtered[key] = dataValues[key];
        return filtered;
    }, {});
}

function isUserTagData(value) {
    return !isNaN(value);
}

function log(text, value) {
    //console.log(text + ': ' + value);
}

var closingDivTagOffset = 6; //num of chars for: </div>
var closingATagOffset = 4; //num of chars for: </a>
var brTagOffset = 4; //num of chars for: <br>
var atCharacterLength = 1; //character length for: @
var indexOfset = 1;
var minSearchLength = 2;
var backSpace = 8;

var li = $('.option');
var liSelected = null;
var tagIndexes = {};
var searchWords = {};