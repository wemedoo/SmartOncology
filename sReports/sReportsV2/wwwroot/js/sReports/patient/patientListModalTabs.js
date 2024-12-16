
// ----- Tabs -----

$(document).on('click', '.scroll-list-tab', function (e) {

    if (leavingMainTab()) {
        reloadPersonnelTable();
        $('#personnel-add-btn-group').removeClass('d-none');
        $('#personnel-save-btn-group').addClass('d-none');
        switchTab(this);
    }
    else {
        $('#personnel-add-btn-group').addClass('d-none');
        $('#personnel-save-btn-group').addClass('d-none');
        switchTab(this);
    }
});

function leavingMainTab() {
    return $('#first-patientlist-modal-tab').hasClass('selected');
}

function switchTab(tabElement) {
    $('.scroll-list-tab').removeClass('selected');
    $(tabElement).addClass('selected');

    $('.scroll-list-tab-container').hide();

    let activeContainerId = $(tabElement).attr("data-id");

    $(`#${activeContainerId}`).show();
}
