
$(document).on('keyup', '#patient', function (e) {
    if (e.which !== downArrow && e.which !== upArrow && e.which !== enter) {
        let searchValue = $("#patient").val();
        if (searchValue.length > 2) {
            callServer({
                method: 'get',
                data: { searchValue },
                url: `/Patient/ReloadPatients`,
                success: function (data) {
                    $('#patientOptions').html(data);
                    $('#patientOptions').show();
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }

});

$(document).on('keydown', '#patient-search', function (e) {
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
    }
    else if (e.which === enter) {
        $(liSelected).click();
    }

    e.stopImmediatePropagation();
});

$(document).on("click", '.main-content', function (e) {
    if (!$(e.currentTarget).hasClass('dropdown-search') || $(e.currentTarget).closest('dropdown-search').length == 0) {
        $("#patientOptions").hide();
    }
});

$(document).on("click", '.patient-remove', function (e) {
    $(this).closest(".selected-patient").remove();
});

$(document).on("click", '.sidebar-shrink', function (e) {
    let target = e.target;
    let isPatientInput = $(target).attr('id') === 'patient';
    if (!$(target).hasClass('option') && !isPatientInput) {
        $("#patientOptions").hide();
    }
});

function patientOptionClicked(e, value, translation) {
    let exist = false;
    $("#selected-patients").find('div').each(function (index, element) {
        if ($(element).attr("data-value") == value) {
            exist = true;
        }
    });

    addNewPatient(exist, value, decodeLocalizedString(translation));
    $("#patient").val('');
}

function addNewPatient(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item).attr("data-value", value);
        $(item).text(translation);
        $(item).addClass('selected-patient');
        let i = document.createElement('i');
        $(i).addClass('fas fa-times patient-remove');
        $(item).append(i);
        $("#selected-patients").append(item);
    }
    $('#patientOptions').hide();
}

function setPatient() {
    let result = [];
    $("#selected-patients").find('.selected-patient').each(function (index, element) {
        result.push($(element).attr("data-value"));
    });

    return result;
}

$(document).on('keyup', '#patientSearch', function (e) {
    if (e.which !== downArrow && e.which !== upArrow && e.which !== enter) {
        page = 1;
        reloadPatients(false);
    }
});

function loadMorePatients() {
    page++;
    reloadPatients(true);
}

function reloadPatients(loadMore) {
    let requestObject = {};
    requestObject.SearchValue = $('#patientSearch').val().toLowerCase();
    requestObject.Page = page;
    requestObject.PageSize = 20;
    requestObject.ComplexSearch = true;

    if (requestObject.SearchValue.length > 2) {
        callServer({
            method: 'get',
            url: `/Patient/ReloadPatients`,
            data: requestObject,
            success: function (data) {
                if (loadMore) {
                    $('#patientOptions').append(data);
                    document.getElementById("patientSearch").focus();
                }
                else
                    $('#patientOptions').html(data);
                $('#loadPatients').remove();
                $('#patientOptions').show();
                if (data.trim()) {
                    if ($('#patientOptions').find(".option").length >= requestObject.PageSize * page) {
                        $('#patientOptions').append(appendLoadMore());
                    }
                }
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function appendLoadMore() {
    let divElement = document.createElement('div');
    $(divElement).addClass("load-more-button-container");
    $(divElement).addClass("load-more-umls");
    divElement.id = "loadPatients";
    let loadMoreElement = document.createElement('div');
    $(loadMoreElement).addClass("load-more-button");
    $(loadMoreElement).addClass("load-concepts");
    loadMoreElement.onclick = function () { loadMorePatients() };
    var LoadMoreText = loadMore;
    var element = $(loadMoreElement).append(LoadMoreText)
    $(divElement).append(element);

    return divElement;
}

var li = $('.option');
var liSelected = null;
var page = 1;