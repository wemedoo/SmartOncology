function addNewFormOrg(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item)
            .attr("data-value", value)
            .text(translation)
            .addClass('filter-element');
        let img = document.createElement('img');
        $(img)
            .attr("src", "/css/img/icons/Administration_remove.svg")
            .addClass('ml-2 remove-form-org selected-autocomplete-remove');
        $(item).append(img);
        $("#selectedOrganizations").append(item);
    }
    $('#organizationOptions').hide();
}

function getOrganizationIds() {
    var organizationIds = [];
    $('#selectedOrganizations .filter-element').each(function () {
        organizationIds.push($(this).attr("data-value"));
    });

    return organizationIds;
}