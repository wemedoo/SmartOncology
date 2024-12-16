$(document).ready(function () {
    validateCodeSetForm();
});

function validateCodeSetForm() {
    jQuery.validator.addMethod("invalidCodeSetThesaurusInput", function (value, element) {
        return !invalidThesaurus();
    }, "Please search and select thesaurus!");

    jQuery.validator.addMethod("validateCodeSetActiveFromTo", function (value, element) {
        return compareActiveDate("newCodeSetActiveFromForCode", "newCodeSetActiveToForCode");
    }, "Active From shouldn't be greater than Active To!");

    jQuery.validator.addMethod("dateInputValidation", function (value, element) {
        return validateDateInput($(element));
    }, `Please put your date in [${getDateFormatDisplay()}] format.`);

    $("#codeSetsForm").validate({
        onkeyup: false,
        rules: {
            thesaurusSearchInputCode: {
                required: true,
                invalidCodeSetThesaurusInput: true
            },
            newCodeSetActiveToForCode: {
                validateCodeSetActiveFromTo: true,
                dateInputValidation: true
            },
            newCodeSetActiveFromForCode: {
                dateInputValidation: true
            },

        },
        messages: {
            thesaurusSearchInputCode: {
                remote: "Please search and select thesaurus!"
            }
        }
    });
}