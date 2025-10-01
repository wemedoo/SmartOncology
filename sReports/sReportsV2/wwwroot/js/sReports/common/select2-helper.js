$.fn.initSelect2 = function (select2Arguments) {
    return this.select2(select2Arguments);
}

function initCodeSelect2Component($select2Element, select2Object) {
    $select2Element.initSelect2(select2Object);
}

function getSelect2Object(select2Arguments) {
    let minimumInputLength = select2Arguments.minimumInputLength != undefined ? select2Arguments.minimumInputLength : 1;
    let placeholder = select2Arguments.placeholder ? select2Arguments.placeholder : '';
    let allowClear = select2Arguments.allowClear != undefined ? select2Arguments.allowClear : true;
    let initialDataSource = select2Arguments.initialDataSource ? select2Arguments.initialDataSource : [];

    let select2Object = {
        minimumInputLength: minimumInputLength,
        placeholder: placeholder,
        allowClear: allowClear,
        data: initialDataSource
    };

    if (select2Arguments.modalId) {
        select2Object.dropdownParent = $(`#${select2Arguments.modalId}`)
    }

    setSelect2PropertiesIfNecessarry(select2Object, select2Arguments);

    if (select2Arguments.url) {
        let ajaxObject = {
            url: select2Arguments.url,
            dataType: 'json'
        };

        if (select2Arguments.urlDelay) {
            ajaxObject.delay = select2Arguments.urlDelay;
        }

        if (select2Arguments.customAjaxData) {
            ajaxObject.data = select2Arguments.customAjaxData;
        } else {
            ajaxObject.data = function (params) {
                return {
                    Term: params.term,
                    Page: params.page
                }
            }
        }

        select2Object.ajax = ajaxObject;
    }

    return select2Object;
}

function setSelect2PropertiesIfNecessarry(select2Object, select2Arguments) {
    for (let property of getSelect2Properties()) {
        if (select2Arguments[property]) {
            select2Object[property] = select2Arguments[property];
        }
    }
}

function getSelect2Properties() {
    return ['width', 'minimumResultsForSearch', 'templateResult', 'templateSelection'];
}

function initCodeSelect2(hasSelectedCode, codeId, codeName, codeDisplayName, codeSetId, modalId = '', formId = '') {
    var $codeElement = $(`#${codeName}`);
    if (hasSelectedCode) {
        callServer({
            type: 'GET',
            url: `/Patient/GetCode?codeId=${codeId}&readOnlyMode=${readOnlyViewModeViewBag}`,
            success: function (data) {
                var initDataSource = [];
                if (!jQuery.isEmptyObject(data)) {
                    initDataSource.push(data);
                }
                initCodeSelect2Component($codeElement, getCodeSelect2Object(codeDisplayName, codeSetId, modalId, initDataSource));
                $codeElement.val(codeId).trigger("change");
                if (formId != "")
                    saveInitialFormData(formId);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    } else {
        initCodeSelect2Component($codeElement, getCodeSelect2Object(codeDisplayName, codeSetId, modalId));
    }
}

function getCodeSelect2Object(placeholderName, codeSetId, modalId, initialData = []) {
    return getSelect2Object(
        {
            placeholder: `Type ${placeholderName}'s name`,
            url: `/Patient/GetAutoCompleteCodeData?codeSetId=${codeSetId}`,
            initialDataSource: initialData,
            modalId: modalId
        }
    );
}

function emptyValueForSelect2(element) {
    if (isSelect2Component(element)) {
        $(element).val("").trigger("change");
    }
}

function isSelect2Component(element) {
    return $(element).data('select2Id');
}

function getSelectedSelect2Label(inputId) {
    return $(`#select2-${inputId}-container`).attr('title');
}

function addSelectedOptionSelect2(selectorId, valueToSelect, textToSelect) {
    // Set the value, creating a new option if necessary
    if ($(selectorId).find("option[value='" + valueToSelect + "']").length) {
        $(selectorId).val(valueToSelect).trigger('change');
    } else {
        var newOption = new Option(textToSelect, valueToSelect, true, true);
        $(selectorId).append(newOption).trigger('change');
    }
}

// Removing Validation Errors on select2 selection
$(document).on("select2:select", 'select', function (e) {
    $(e.target).removeClass('error');
    $(e.target).addClass('valid');
    $(e.target).siblings('.error').hide();
});