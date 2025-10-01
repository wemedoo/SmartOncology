let patientLanguages;

function setPatientLanguages() {
    patientLanguages = getCommunications();
}

function getCommunications() {
    let result = [];

    $('input[name=radioPreferredLanguage]').each(function (index, element) {
        result.push({
            preferred: $(element).is(":checked"),
            languageCD: $(element).val(),
            id: $(element).attr("data-id")
        })
    })

    return result;
}

function validateCommunication(communications) {
    if (communications.length == 0 || communications.some(c => c.preferred)) {
        return true;
    } else {
        toastr.error("Please select one language as preferred!");
        return false;
    }
}

$(document).on('click', '.plus-button', function (e) {
    if (validateLanguage() && $('#language').val()) {
        let language = createLanguageElement();
        let input = createRadioInput();
        let removeButton = createRemoveLanguageButton();
        let preferredText = createPreferredText();

        let patientLanguageDiv = document.createElement('div');
        $(patientLanguageDiv).addClass("preferred-language-group preferred-language-text");

        if ($('input:radio[name=radioPreferredLanguage]').length == 0) {
            $(input).attr('checked', true);
            $(language).append(preferredText);
            $(patientLanguageDiv).addClass("selected-pref-lang");
        }

        let patientLanguageInputElement = createRadioField();
        $(patientLanguageInputElement).append(input);
        $(patientLanguageDiv).append(patientLanguageInputElement).append(language).append(removeButton)

        $("#communicationTable").append(patientLanguageDiv);
    }
});

function createLanguageElement() {
    let language = document.createElement('span');
    $(language).html(getSelectedOptionLabel('language'));

    return language;
}

function createRadioInput() {
    let input = document.createElement('input');
    $(input).addClass("form-radio-field");
    $(input).attr("value", $('#language').val());
    $(input).attr("name", 'radioPreferredLanguage');
    $(input).attr("type", 'radio');
    $(input).attr("data-id", '0');
    $(input).attr("data-no-color-change", "true");

    return input;
}

function createPreferredText() {
    let preferredText = document.createElement('span');
    $(preferredText).addClass("preferred-text-class");
    preferredText.innerHTML = " (Preferred)";

    return preferredText;
}

function createRadioField() {
    let preferred = document.createElement('span');
    $(preferred).addClass("radio-space");
    $(preferred).css("margin-right", "13px");

    return preferred;
}

function validateLanguage() {
    var isValid = true;
    let newSelectedLanguage = $("#language").val();

    if ($(`[name="radioPreferredLanguage"][value="${newSelectedLanguage}"]`).length > 0) {
        isValid = false;
        toastr.error(`This language already added`);
    }

    return isValid;
}

function createRemoveLanguageButton() {
    let span = document.createElement('span');
    $(span).addClass('remove-language-button right-remove-button');

    let i = document.createElement('i');
    $(i).addClass('fas fa-times');

    $(span).append(i);
    return span;
}

$(document).on('click', '.remove-language-button', function (e) {
    $(this).closest('div').remove();
});

$(document).on('click', '[name="radioPreferredLanguage"]', function () {
    let previousPreferredLanguage = $('.preferred-language-group.selected-pref-lang');
    $(previousPreferredLanguage).removeClass('selected-pref-lang');
    $(previousPreferredLanguage).find(".preferred-text-class").remove();

    let newPreferredLanguage = $(this).closest('.preferred-language-group');
    $(newPreferredLanguage).addClass('selected-pref-lang');
    $(newPreferredLanguage).append(createPreferredText());
});

function resetCommunication(patientId) {
    $('#language').val('');
    let patientLanguagesAfterSave = getCommunications();
    if (!arraysAreEqual(patientLanguages, patientLanguagesAfterSave)) {
        callServer({
            url: '/Patient/GetPatientLanguages',
            data: { patientId },
            success: function (data) {
                $('#communicationTable').html(data);
                setPatientLanguages();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}