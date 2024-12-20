﻿$(document).ready(function () {
    reloadSimilarThesauruses();
    $('body').css("background-color", "#f9f9f9");
});

function reloadSimilarThesauruses() {
    callServer({
        type: 'GET',
        url: `/ThesaurusEntry/GetThesaurusInfo?id=${$('#id').val()}`,
        success: function (data) {
            $('#cuurentThesaurus').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function loadTargetThesaurusTree(O4MTId, element) {
    thesaurusPageNum = 0;
    $('#buttons').show();
    $('#targetAppereance').show();
    $('#targetThesaurusUsage').html($(element).html() + '- Appearence');
    $('#targetO4MTId').val(O4MTId);

    callServer({
        type: 'GET',
        url: `/Form/GetDocumentsByThesaurusId?o4MtId=${O4MTId}&thesaurusPageNum=${thesaurusPageNum}`,
        success: function (data) {
            $(`#foundIn-${O4MTId}`).html(data);
            loadTreeStructure();
            loadMoreReviewButton(O4MTId);
            removeLoadMoreReview(O4MTId);
            setSameHeights();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });

    callServer({
        type: 'GET',
        url: `/Form/GetDocumentsByThesaurusId?o4MtId=${$('#O4MTId').val()}&thesaurusPageNum=${thesaurusPageNum}`,
        success: function (data) {
            $(`#foundIn-${$('#O4MTId').val()}`).html(data);
            loadTreeStructure();
            loadMoreReviewButton($('#O4MTId').val());
            removeLoadMoreReview(O4MTId);
            setSameHeights();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function setSameHeights() {
    let maxHeight = 0;

    $(".thesaurus-description").each(function (index, element) {
        $(element).css("height", "unset");
        if ($(element).height() > maxHeight) {
            maxHeight = $(element).height();
        }

    });
    $(".thesaurus-description").each(function (index, ele) {
        $(ele).height(maxHeight);
    });

    maxHeight = 0;
    $(".thesaurus-o4").each(function (index, element) {
        $(element).css("height","unset");

        if ($(element).height() > maxHeight) {
            maxHeight = $(element).height();
        }

    });
    $(".thesaurus-o4").each(function (index, ele) {
        $(ele).height(maxHeight);
    });

    maxHeight = 0;
    $(".thesaurus-term").each(function (index, element) {
        $(element).css("height", "unset");

        if ($(element).height() > maxHeight) {
            maxHeight = $(element).height();
        }

    });
    $(".thesaurus-term").each(function (index, ele) {
        $(ele).height(maxHeight);
    });


    maxHeight = 0;
    $(".thesaurus-syno").each(function (index, element) {
        $(element).css("height", "unset");

        if ($(element).height() > maxHeight) {
            maxHeight = $(element).height();
        }

    });
    $(".thesaurus-syno").each(function (index, ele) {
        $(ele).height(maxHeight);
    });


    maxHeight = 0;
    $(".thesaurus-abre").each(function (index, element) {
        $(element).css("height", "unset");

        if ($(element).height() > maxHeight) {
            maxHeight = $(element).height();
        }

    });
    $(".thesaurus-abre").each(function (index, ele) {
        $(ele).height(maxHeight);
    });
}

function openMergeModal(event) {
    $('#mergeModal').modal('show');
}

function getMergeParams() {
    let mergeParams = [];
    $('[name="mergeValue"]:checked').each(function (index, element) {
        if ($(element).val()) {
            mergeParams.push(`valuesForMerge=${$(element).val()}`);
        }
    });

    return mergeParams;
}

function mergeThesauruses(event) {
    event.preventDefault();
    let mergeParams = getMergeParams();
    let currentThesaurus = $('#O4MTId').val();
    let targetThesaurus = $('#targetO4MTId').val();

    if (targetThesaurus) {
        callServer({
            type: 'GET',
            url: `/ThesaurusEntry/MergeThesauruses?currentId=${currentThesaurus}&targetId=${targetThesaurus}&${mergeParams.join('&')}`,
            success: function (data) {
                $('#mergeModal').modal('hide');
                reloadThesauruses(`Success! Thesaurus(id=${currentThesaurus}) is merged into Thesaurus(id=${targetThesaurus}.`);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    else {
        toastr.warning(`Please select target thesaurus!`);
    }

}

function getSelectedValuesForMerge() {
    var chkArray = [];

    $(".chk-th-merge:checked").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function reloadTable(isFilter) {
    $('#buttons').hide();
    $('#targetAppereance').hide();

    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    checkUrlPageParams();
    requestObject.Id = $('#id').val();

    setTableProperties(requestObject, { doOrdering: false });
 
    if ($('#preferredTerm').val()) {
        requestObject.PreferredTerm = $('#preferredTerm').val();
    }

    callServer({
        type: 'GET',
        url: '/ThesaurusEntry/ReloadReviewTree',
        data: requestObject,
        success: function (data) {
            $("#reviewList").html(data);
            if ($("#targetO4MTId").val()) {
                $(`#${$("#targetO4MTId").val()}`).addClass('active-thesaurus');
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getFilterParametersObject() {
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {

        if ($('#id').val()) {
            result['Id'] = $('#id').val();
        }
    }

    return result;
}

function takeBoth(event) {
    let currentThesaurus = $('#O4MTId').val();
    let targetThesaurus = $('#targetO4MTId').val();
    callServer({
        type: 'GET',
        url: `/ThesaurusEntry/TakeBoth?currentId=${currentThesaurus}`,
        success: function (data) {
            reloadThesauruses(`Success! Both thesauruses (id=${currentThesaurus} and id=${targetThesaurus}) are taken.`);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '.card', function () {
    let draftThesaurus = $("#currentThesaurusInfo");
    $(draftThesaurus).css("display", "block");
    $(this).find('.draft-thesaurus:first').append(draftThesaurus);

});

$(document).on('click', '.target', function () {
    let currentId = $(this).closest('.card-header').attr('id');
    $('.card-header').each(function (ind, ele) {
        if (currentId != $(ele).attr('id')) {
            if ($(ele).find('i:first').hasClass('fa-angle-up')) {
                $(ele).find('i:first').removeClass('fa-angle-up');
                $(ele).find('i:first').addClass('fa-angle-down');
            }
            if ($(ele).hasClass('active-thesaurus')) {
                $(ele).removeClass('active-thesaurus');
            }
        }
    });

    if ($(this).find('i:first').hasClass('fa-angle-down')) {
        $(this).find('i:first').removeClass('fa-angle-down');
        $(this).find('i:first').addClass('fa-angle-up');
    } else {
        $(this).find('i:first').removeClass('fa-angle-up');
        $(this).find('i:first').addClass('fa-angle-down');
    }

    if ($(this).closest('.card-header').hasClass('active-thesaurus')) {
        $(this).closest('.card-header').removeClass('active-thesaurus');
    } else {
        $(this).closest('.card-header').addClass('active-thesaurus');
    }
});

$(document).on('keypress', '.filter-input', function (e) {
    if (e.which === enter) {
        reloadTable(true);
    }
});

function loadTreeStructure(id) {
    setFieldValueThesaurusAppearance('.fv');
    setFieldOrFieldSetThesaurusAppearance('.f');
    setFieldOrFieldSetThesaurusAppearance('.fs');
    setPageOrChapterThesaurusAppearance('.p', false);
    setPageOrChapterThesaurusAppearance('.c', false);

    $('span').each(function (ind, elem) {
        if (!$(elem).closest('div').hasClass('main') && $(elem).closest('div').hasClass('tree-item')) {
            $(elem).css('background', 'white');
        }
    });

    calculateLineHeight('.form-tree', '.c:visible', 0);
    calculateLineHeight('.c:visible', '.p:visible', 0);
    calculateLineHeight('.p:visible', '.fs:visible', 0);
    calculateLineHeight('.fs:visible', '.f:visible', -19);
    calculateLineHeight('.f:visible', '.fv:visible', -19);
    setTreeLine();
    loadMoreReviewButton(id);
    setThesaurusAppearances(id);
}

function loadMore(id, e) {
    e.stopPropagation();
    e.preventDefault();
    hideLoadMoreReview(id);
    thesaurusPageNum = $('#foundInContainer-' + id).find(".tree-item-thesaurus").length;

    callServer({
        type: 'GET',
        url: `/Form/GetDocumentsByThesaurusId?o4MtId=${id}&thesaurusPageNum=${thesaurusPageNum}`,
        success: function (data) {
            $(`#foundInContainer-` + id).append(data);
            document.getElementById("loadMoreThesaurus-" + id).remove();
            loadTreeStructure(id);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function loadMoreReviewButton(id) {
    if ($('#foundInContainer-' + id).find(".tree-item-thesaurus").length < thesaurusPageNum + 15)
        hideLoadMoreReview(id);
    else
        document.getElementById("loadMoreThesaurus-" + id).style.display = "block";
}

function hideLoadMoreReview(id) {
    if (document.getElementById("loadMoreThesaurus-" + id) != null)
        document.getElementById("loadMoreThesaurus-" + id).style.display = "none";
}

function removeLoadMoreReview(O4MTId) {
    if ($('#foundInContainer-' + O4MTId).find(".tree-item-thesaurus").length < thesaurusPageNum + 15 && document.getElementById("loadMoreThesaurus-" + O4MTId) != null)
        document.getElementById("loadMoreThesaurus-" + O4MTId).remove();
}

function setThesaurusAppearances(id) {
    if ($('#foundInContainer-' + id).find(".tree-item-thesaurus").length != 0)
        document.getElementById("thesaurusAppearances-" + id).innerHTML = $('#foundInContainer-' + id).find(".tree-item-thesaurus").length;
}

function reloadThesauruses(successMsg) {
    toastr.options = {
        timeOut: 2000
    }
    toastr.options.onHidden = function () {
        window.location.href = '/ThesaurusEntry/GetAll';
    }
    toastr.success(successMsg);
}

$(document).on('mouseenter', '.thesaurus-merge-btn', function () {
    $(this).closest('.row').find('.triangle-right').addClass('triangle-right-hover');
    $(this).closest('.row').find('.triangle-right').removeClass('triangle-right');

});

$(document).on('mouseleave', '.thesaurus-merge-btn', function () {
    $(this).closest('.row').find('.triangle-right-hover').addClass('triangle-right');
    $(this).closest('.row').find('.triangle-right').removeClass('triangle-right-hover');
});