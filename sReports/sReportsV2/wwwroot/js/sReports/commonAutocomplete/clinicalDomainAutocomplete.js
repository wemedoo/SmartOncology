$(document).on('keyup', '#clinicalDomain', function (e) {
    if (e.which !== downArrow && e.which !== upArrow && e.which !== enter) {
        let searchValue = $("#clinicalDomain").val();
        if (searchValue.length > 2) {
            callServer({
                method: 'get',
                data: { term: searchValue },
                url: `/Form/ReloadClinicalDomain`,
                success: function (data) {
                    $('#clinicalOptions').html(data);
                    $('#clinicalOptions').show();
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }

});

$(document).on('keydown', '#search', function (e) {
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

$(document).on("click", '.designer-form-modal-body', function (e) {
    if (!$(e.currentTarget).hasClass('dropdown-search') || $(e.currentTarget).closest('dropdown-search').length == 0) {
        $("#clinicalOptions").hide();
    }
});

$(document).on("click", '.clinical-remove', function (e) {
    $(this).closest(".clinical").remove();
});

$(document).on("click", '.sidebar-shrink', function (e) {
    let target = e.target;
    let isClicalDomainInput = $(target).attr('id') === 'clinicalDomain';
    if (!$(target).hasClass('option') && !isClicalDomainInput) {
        $("#clinicalOptions").hide();
    }
});

function optionClicked(e, value, translation) {
    let exist = false;
    $("#clinicals").find('div').each(function (index, element) {
        if ($(element).attr("data-value") == value) {
            exist = true;
        }
    });

    addNewClinicalDomain(exist, value, decodeLocalizedString(translation));
    $("#clinicalDomain").val('');
}

function addNewClinicalDomain(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item).attr("data-value", value);
        $(item).text(translation);
        $(item).addClass('clinical');
        let i = document.createElement('i');
        $(i).addClass('fas fa-times clinical-remove');
        $(item).append(i);
        $("#clinicals").append(item);
    }
    $('#clinicalOptions').hide();
}


function setClinicalDomain(clinicalDomainObjectCallback) {
    let result = [];
    $("#clinicals").find('.clinical').each(function (index, element) {
        result.push(clinicalDomainObjectCallback(element));
    });

    return result;
}

function getSimpleClinicalDomainObject() {
    return function (element) {
        return $(element).attr("data-value");
    }
}

var li = $('.option');
var liSelected = null;