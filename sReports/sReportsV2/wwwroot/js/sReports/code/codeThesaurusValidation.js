$(document).ready(function () {
    validateThesaurusModal();
});

$("#activeToDate, #activeFromDate").on('change', function () {
    $("#thesaurusFilterModalForm").validate().element("#activeToDate");
    $("#thesaurusFilterModalForm").validate().element("#activeFromDate");
});

$("#codeSetIdInput").on('change', function () {
    $("#thesaurusFilterModalForm").validate().element("#codeSetIdInput");
});

function validateThesaurusModal() {
    jQuery.validator.addMethod("duplicateCodeSetId", function (value, element) {
        return !codeSetExistsThesaurusModal();
    }, "Code set with this id is already defined!");

    jQuery.validator.addMethod("invalidThesaurusEntry", function (value, element) {
        return !isInvalidThesaurusEntry();
    }, "Please search and select thesaurus!");

    jQuery.validator.addMethod("validateCodeSetActiveDate", function (value, element) {
        return compareActiveDate("activeFromDate", "activeToDate");
    }, "Active From shouldn't be greater than Active To!");

    jQuery.validator.addMethod("dateInputValidation", function (value, element) {
        return validateDateInput($(element));
    }, `Please put your date in [${getDateFormatDisplay()}] format.`);

    $("#thesaurusFilterModalForm").validate({
        onkeyup: false,
        onfocusout: false,
        rules: {
            codeSetIdInput: {
                required: true,
                duplicateCodeSetId: true
            },
            thesaurusSearchInput: {
                required: true,
                invalidThesaurusEntry: true
            },
            activeToDate: {
                validateCodeSetActiveDate: true,
                dateInputValidation: true
            },
            activeFromDate: {
                dateInputValidation: true
            },
        },
    });
}