/// This file contains methods that should be used for both form and forminstance

// START: IMAGE MAP

function configureImageMap() {
    $('map').imageMapResize();
    setTimeout(function () { triggerResize(); }, 100);
}

function triggerResize() {
    var el = document;
    var event = document.createEvent('HTMLEvents');
    event.initEvent('resize', true, false);
    el.dispatchEvent(event);
}

$(document).on('click', 'area', function (event) {
    executeEventFunctions(event, true);

    let fieldSetId = $(this).attr('data-fieldset');
    let $targetFieldSet = $(`#${fieldSetId}.form-fieldset`);
    let row = $targetFieldSet.closest('.row');
    $(row).find('.form-fieldset').hide();
    $targetFieldSet.show();
    scrollToElement($targetFieldSet.first(), 1500);
})

function scrollToElement(element, duration, additionalOffset) {
    let scrollToId = $(element).attr('id');
    if ($(`#${scrollToId}`).length > 0) {
        $([document.documentElement, document.body]).animate({
            scrollTop: $(`#${scrollToId}`).offset().top - getScrollOffset() - (additionalOffset ? additionalOffset : 0)
        }, duration);
    }
}

function getScrollOffset() {
    let offset = 70;
    if ($(window).width() <= 768) {
        offset = 50;
    }
    return offset;
}

// END: IMAGE MAP

// START: NOTES

function showHelpModal(data) {
    $('#helpModalTitle').html(data.Title);
    $('#helpModalBody').html(data.Content);
    $('#helpModal').modal('toggle');
}

function showHideDescription(event) {
    let des = $(event.currentTarget).closest(".form-element").find(".form-element-description");
    if ($(des).is(':visible')) {
        $(des).hide();
    } else {
        $(des).show();
    }
}

// END: NOTES

// START: HORIZONTAL SCROLL ARROWS

$(document).on('click', '.arrow-scroll-right-form', function (event) {
    executeEventFunctions(event, true);

    $('#idWorkflow').animate({
        scrollLeft: "+=500px"
    }, "slow");
});

$(document).on('click', '.arrow-scroll-left-form', function (event) {
    executeEventFunctions(event, true);

    $('#idWorkflow').animate({
        scrollLeft: "-=500px"
    }, "slow");
});

// END: HORIZONTAL SCROLL ARROWS