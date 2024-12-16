function initializeCodedFieldsSelect2(selector, codesetId) {
    var placeholder = "-";

    $(selector).each(function () {
        $(this).initSelect2(
            getSelect2Object(
                {
                    placeholder: placeholder,
                    width: '100%',
                    url: `/CodeSet/GetCodesForAutoCompleteByCodeset?codesetId=${codesetId}`,
                    customAjaxData: function (params) {
                        return {
                            Term: params.term, // search term
                            Page: params.page || 1,
                        };
                    }
                }
            )
        );
    });
}

function setCustomCodedFields(element) {

    if (element) {
        $(element).attr('data-codeset', encodeURIComponent($('#codeset').val()));
        $(element).attr('data-codesetid', encodeURIComponent($('#codeset').val()));
        setCommonStringFields(element);
    }

}

function getCodedFieldsCodesets(codesetSelected) {
    initializeCodedCodesetSelect2();

    if (codesetSelected != '0') {
        try {
            $("#codeset").prop("disabled", true);
            initializeCodedCodesetSelectedValue(codesetSelected);
        }
        catch (e) {
            console.log(e);
            $("#codeset").prop("disabled", false);
        }
    }
}

function initializeCodedCodesetSelect2() {
    $('#codeset').initSelect2(
        getSelect2Object(
            {
                minimumInputLength: 0, // Set to 0 to enable search on opening
                minimumResultsForSearch: 5,
                placeholder: '-',
                width: '100%',
                url: `/CodeSet/GetAutoCompleteNames`,
                urlDelay: 250,
                customAjaxData: function (params) {
                    return {
                        Term: params.term,
                        Page: params.page || 1,
                        onlyApplicableInDesigner: true
                    }
                }
            }
        )
    );
}

function initializeCodedCodesetSelectedValue(codesetSelected) {
    callServer({
        type: 'GET',
        url: `/CodeSet/GetCodedCodesetDisplay?codeSetId=${codesetSelected}`,
        success: function (data) {
            if (data?.codeSetDisplay) {
                addSelectedOptionSelect2('#codeset', codesetSelected, data.codeSetDisplay);
            }
            $("#codeset").prop("disabled", false);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
            $("#codeset").prop("disabled", false);
        }
    });
}

$('#codeset').on('select2:open', function (e) {
    $('.select2-container--open').addClass('select2-on-top');
});