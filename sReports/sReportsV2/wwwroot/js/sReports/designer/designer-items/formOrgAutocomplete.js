$(document).on('keyup', '#searchOrganizationByName', function (e) {
    if (e.which !== downArrow && e.which !== upArrow && e.which !== enter) {
        let searchValue = $("#searchOrganizationByName").val();
        if (searchValue.length > 2) {
            callServer({
                method: 'get',
                data: { name: searchValue },
                url: `/Organization/GetOrganizationValues`,
                success: function (data) {
                    $('#organizationOptions').html(data);
                    $('#organizationOptions').show();
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }

});

$(document).on('keydown', '#searchOrganization', function (e) {
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
        $("#organizationOptions").hide();
    }
});

$(document).on("click", '.sidebar-shrink', function (e) {
    let target = e.target;
    let isClicalDomainInput = $(target).attr('id') === 'clinicalDomain';
    if (!$(target).hasClass('option') && !isClicalDomainInput) {
        $("#organizationOptions").hide();
    }
});

function orgOptionClicked(e, value, translation) {
    let exist = false;
    $("#selectedOrganizations").find('div').each(function (index, element) {
        if ($(element).attr("data-org-id") == value) {
            exist = true;
        }
    });

    addNewFormOrg(exist, value, decodeLocalizedString(translation));
    $("#searchOrganizationByName").val('');
}

function addNewFormOrg(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item)
            .attr("data-org-id", value)
            .text(translation)
            .addClass('filter-element');
        let img = document.createElement('img');
        $(img)
            .attr("src", "/css/img/icons/Administration_remove.svg")
            .addClass('ml-2 remove-form-org');
        $(item).append(img);
        $("#selectedOrganizations").append(item);
    }
    $('#organizationOptions').hide();
}

function getOrganizationIds() {
    var organizationIds = [];
    $('#selectedOrganizations .filter-element').each(function () {
        organizationIds.push($(this).attr("data-org-id"));
    });

    return organizationIds;
}

$(document).on('click', '.remove-form-org', function (e) {
    $(this).closest('.filter-element').remove();
});

var li = $('.option');
var liSelected = null;