﻿$(document).on('click', '#submit-fieldvalue-info', function (e) {
    let label = $('#label').val();
    let elementId = getOpenedElementId();
    populateValueIfEmpty();

    if ($('#fieldValueGeneralInfoForm').valid()) {
        let element = getElement('fieldvalue', label);
        createNewThesaurusIfNotSelected();

        if (element) {
            $(element).attr('data-id', encodeURIComponent(elementId));
            $(element).attr('data-label', encodeURIComponent($('#label').val()));
            $(element).attr('data-thesaurusid', encodeURIComponent($('#thesaurusId').val()));
            $(element).attr('data-value', encodeURIComponent($('#value').val()));
            $(element).attr('data-numericvalue', encodeURIComponent($('#numericValue').val()));

            updateTreeItemTitle(element, label);
            closDesignerFormModal(true);
            clearErrorFromElement($(element).attr('data-id'));
        }
    }
    else {
        toastr.error("Field value informations are not valid");
    }
});

$(document).on('blur', '#label', function (e) {
    populateValueIfEmpty();
});

function generateValueFromLabel(label) {
    return label.replace(/\s+/g, '_').toLowerCase();
}

function populateValueIfEmpty() {
    let label = $('#label').val();
    if (!$('#value').val()) {
        $("#value").val(generateValueFromLabel(label));
    }
}