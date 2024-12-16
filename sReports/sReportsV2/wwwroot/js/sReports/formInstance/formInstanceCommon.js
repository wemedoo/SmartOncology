function readyFunctionInEditableMode() {
    setDateTimeValidatorMethods();
    initValidationForRegexFieldInstances();
    saveInitialFormData("#fid");
    configureImageMap();
    scrollActivePageTabIfNecessarry();
}

function readyFunctionInReadOnlyMode() {
    configureImageMap();
    scrollActivePageTabIfNecessarry();
}

function initValidationForRegexFieldInstances() {
    $.validator.addMethod(
        "regex",
        function (value, element) {
            let regexp = $(element).data('regex');
            let elementValue = $(element).val();
            if (regexp) {
                var re = new RegExp(regexp);
                return this.optional(element) || re.test(elementValue) || isSpecialValueSelected($(element));
            }
            else {
                return true;
            }

        },
        "Please check your input."
    );
    $('[data-fieldtype="regex"]').each(function () {
        var regexDescription = $(this).data('regexdescription');
        $(this).rules('add', {
            regex: true,
            messages: { // optional custom messages
                regex: regexDescription
            }
        });
    });
}

function resetValue(event) {
    event.preventDefault();
    let $resetLink = $(event.currentTarget);
    let fieldInstanceName = $resetLink.data("field-name");

    let $specialValueElement = getSpecialValueElement(fieldInstanceName);
    unsetSpecialValueIfSelected($specialValueElement);
    resetErrorLabel(fieldInstanceName);
    setInputFieldToDefault($resetLink, fieldInstanceName, true);
}

function setInputFieldToDefault($element, fieldInstanceName, revertToDefaultEditableState) {
    var fieldContainer = $element.closest(".form-element");

    $(fieldContainer).find(`[name=${fieldInstanceName}]`).not("[spec-value]").each(function () {
        setInputToDefault(
            $(this),
            {
                revertToDefaultEditableState: revertToDefaultEditableState,
                setValue: true
            }
        );
    });
}

function resetErrorLabel(fieldInstanceName) {
    let $errorLabel = $(`#field-id-${fieldInstanceName}-error`);
    $errorLabel.text('').css('display', 'none');
}

function setInputToDefault($el, params) {
    if (!isInputIncludedInSubmit($el)) {
        return;
    }

    let fieldType = $el.attr('data-fieldtype');
    params.fieldType = fieldType;
    let handlers = {
        'audio': setInputValueForBinary,
        'file': setInputValueForBinary,
        'radio': setInputValueForCheckboxOrRadio,
        'checkbox': setInputValueForCheckboxOrRadio,
        'select': setInputValueForSelect,
        'coded': setInputValueForSelect,
        'date': setInputValueForDate,
        'datetime': setInputValueForDateTime,
        'numeric': setInputValueForNumber,
        'text': setInputValueForText,
        'long-text': setInputValueForSimpleText,
        'regex': setInputValueForSimpleText,
        'email': setInputValueForSimpleText,
        'calculative': setInputValueForSimpleText,
    };

    return handlers[fieldType] ? handlers[fieldType]($el, params) : undefined;
}

function setInputValueForText($el, params) {
    removeValidationMessages($el);

    let minLength = $el.attr("data-minlength");
    if (minLength) {
        $el.attr("minlength", minLength);
    }
    let maxLength = $el.attr("data-maxlength");
    if (maxLength) {
        $el.attr("maxlength", maxLength);
    }

    setInputValueForSimpleText($el, params);
}

function setInputValueForSimpleText($el, params) {
    removeValidationMessages($el);

    if (!isInputCalculative($el)) {
        $el.attr("readonly", !params.revertToDefaultEditableState);
    }

    setFieldInstanceValue($el, params);
}

function setInputValueForBinary($el, params) {
    removeValidationMessages($el);

    let $fileFieldContainer = $el.closest(".repetitive-field");
    allowFileUploadIfValueIsReset($fileFieldContainer, params.revertToDefaultEditableState, $el);

    if (params.setValue) {
        handleBinaryChange($fileFieldContainer, params.fieldType, params.customValue ? params.customValue[0] : '');
    }
}

function setInputValueForNumber($el, params) {
    removeValidationMessages($el);

    $el.attr("min", $el.attr("data-min"));
    $el.attr("max", $el.attr("data-max"));

    $el.attr("readonly", !params.revertToDefaultEditableState);

    setFieldInstanceValue($el, params);
}

function setInputValueForCalculative($el, params) {
    removeValidationMessages($el);

    setFieldInstanceValue($el, params);
}

function setInputValueForDate($el, params) {
    removeValidationMessages($el);

    let maxLength = $el.attr("data-maxlength");
    if (maxLength) {
        $el.attr("maxlength", maxLength);
    }

    if (params.revertToDefaultEditableState) {
        $el.siblings(".field-date-btn").removeClass("pe-none");
    } else {
        $el.siblings(".field-date-btn").addClass("pe-none");
    }
    $el.attr("disabled", !params.revertToDefaultEditableState);

    if (params.setValue) {
        transformCustomValue(params, formatUtcDateToClientFormat);
        setFieldInstanceValue($el, params);
    }
}

function setInputValueForTime($el, params) {
    removeValidationMessages($el);

    if (params.revertToDefaultEditableState) {
        $el.siblings(".field-time-btn").removeClass("pe-none");
    } else {
        $el.siblings(".field-time-btn").addClass("pe-none");
    }
    $el.attr("disabled", !params.revertToDefaultEditableState);
    if (params.setValue) {
        if (params.customValue) {
            $el.val(params.customValue);
        } else {
            $el.val('');
        }
    }
}

function setInputValueForDateTime($datetimeinput, params) {
    setFieldInstanceValue($datetimeinput, params, false);

    let customDateTime = params.customValue ? params.customValue[0] : undefined;
    params.customValue = customDateTime ? [extractUtcDatePart(customDateTime)] : undefined;
    setInputValueForDate($datetimeinput.siblings('.field-date-input'), params);
    params.customValue = customDateTime ? [extractUtcTimePart(customDateTime)] : undefined;
    setInputValueForTime($datetimeinput.closest('.datetime-picker-container').find('.field-time-input'), params);
}

function setInputValueForCheckboxOrRadio($el, params) {
    removeValidationMessages($el);

    $el.attr("disabled", !params.revertToDefaultEditableState);
    if (params.setValue) {
        if (wasPreviousCheckboxOrRadioOptionChecked($el.val(), params)) {
            $el.prop("checked", true);
            $el.trigger("change");
        } else if ($el.is(":checked")) {
            $el.prop("checked", false);
            $el.trigger("change");
        }
    }
}

function wasPreviousCheckboxOrRadioOptionChecked(checkboxOrRadioOpttionValue, params) {
    return params?.customValue?.includes(checkboxOrRadioOptionValue) ?? false;
}

function setInputValueForSelect($el, params) {
    removeValidationMessages($el);

    $el.attr("disabled", !params.revertToDefaultEditableState);

    setFieldInstanceValue($el, params);
}

function setFieldInstanceValue($el, params, triggerEvent = true) {
    if (params.setValue) {
        if (params.customValue) {
            $el.val(params.customValue[0]);
        } else {
            $el.val('');
        }
        if (triggerEvent) {
            $el.trigger("change");
        }
    }
}

function transformCustomValue(params, transformHandler) {
    if (params.customValue) {
        params.customValue[0] = transformHandler(params.customValue[0]);
    }
}

function isInputDate($el) {
    return $el.hasClass("field-date-input");
}

function isInputCheckboxOrRadio($el) {
    var inputType = $el.attr("type");
    return inputType == "checkbox" || inputType == "radio";
}

function isInputFile($el) {
    return $el.hasClass("file-hid");
}

function isInputAudio($el) {
    return $el.hasClass("audio-hid");
}

function isInputCalculative($el) {
    return $el.hasClass("field-calculative");
}

function getActiveOptionDataThesaurusId(element) {
    var selectEl = $(element).siblings("select");
    var selectedOption = selectEl.find(":selected");
    var selectedOptionThesaurusId = selectedOption.attr('data-thesaurusid');

    return selectedOptionThesaurusId ? selectedOptionThesaurusId : '';
}

$(document).on("change", '.select-input-field', function (event) {

    var imgSrc = $(this).val() ? "/css/img/icons/thesaurus_green.svg" : "/css/img/icons/thesaurus_grey.svg";
    var $image = $(this).siblings("img");
    $image.attr("src", imgSrc);
});

function getFormInstanceFieldsSelector() {
    return '.field-group, .form-radio, .form-checkbox';
}

$(document).on('click', '.chapter-li', function (event) {
    executeEventFunctions(event);

    chapterLinkIsClicked($(this));
});

function chapterLinkIsClicked($chapterTab) {
    let clickedChapterContainerId = $chapterTab.attr('id').replace('li', 'acc');
    $('.form-accordion').hide();

    $(`#${clickedChapterContainerId}`).show();
    collapseChapter($(`#${clickedChapterContainerId}`).children('.enc-form-instance-header:first'));
}

function updateChapterTab($chapterTab, isActiveChapter) {
    if (isActiveChapter) {
        $chapterTab.addClass('active');
        $chapterTab.find('.active-chapter-action').removeClass('d-none');
        $chapterTab.find('.inactive-chapter-action').addClass('d-none');
    } else {
        $chapterTab.removeClass('active');
        $chapterTab.find('.active-chapter-action').addClass('d-none');
        $chapterTab.find('.inactive-chapter-action').removeClass('d-none');
    }
}

function updatePageTab($pageTab, isActivePage) {
    if (isActivePage) {
        $pageTab.addClass('active');
    } else {
        $pageTab.removeClass('active');
    }
}

function collapseChapter(clickedChapterHeader) {
    var previousActivePage = getPreviousActivePage();
    $(previousActivePage).hide();

    let clickedChapterContentId = $(clickedChapterHeader).data('target');
    let $chaptersContainer = $(clickedChapterHeader).closest(".chapters-container");
    let $clickedChapterContent = $chaptersContainer.find(clickedChapterContentId);

    $chaptersContainer.find(`.chapter.collapse:not(${clickedChapterContentId})`).collapse('hide');
    $chaptersContainer.find(`.chapter.collapse .page`).hide();
    $clickedChapterContent.collapse('show');
    $clickedChapterContent.find('.page:eq(0)').show(function () {
        configureImageMap();
    });

    if (clickedChapterContentId == '#administrativeChapter') {
        showAdministrativeArrowIfOverflow('administrative-container-form-instance');
    }

    scrollPageTabs(true, true, true);
    let $activatedChapterTab = $($(clickedChapterHeader).attr('data-link-id'));
    updateChapterTab($('.chapter-li.active'), false);
    updateChapterTab($activatedChapterTab, true);

    updatePageTab(getPreviousActivePageTab(), false);
    updatePageTab($clickedChapterContent.find('.pages-link:first'), true);

    var allChapters = document.querySelectorAll('.enc-form-instance-header');
    allChapters.forEach(function (chapter) {
        if (chapter != clickedChapterHeader) {
            $(chapter).find('.chapter-icon').attr('src', '/css/img/icons/u_plus.svg');
            $(chapter).removeClass("chapter-non-collapse");
        } else {
            let $chapterIcon = $(clickedChapterHeader).find('.chapter-icon');
            if ($chapterIcon.attr('src') === '/css/img/icons/u_plus.svg') {
                $chapterIcon.attr('src', '/css/img/icons/u_minus.svg');
                $(clickedChapterHeader).addClass('chapter-non-collapse');
            } else {
                $chapterIcon.attr('src', '/css/img/icons/u_plus.svg');
                $(clickedChapterHeader).removeClass('chapter-non-collapse');
            }
        }

    });
}

$(document).on("click", ".pages-link", function (event) {
    executeEventFunctions(event, true);
    pageIsClicked($(this));
});

function pageIsClicked($pageTab) {
    $('.pages-link').removeClass('active');
    $pageTab.addClass('active');

    let pageId = $pageTab.attr("id").replace("page-link-", "");
    var pageToShow = $(".chapters-container").find("#" + pageId);
    showPage(pageToShow);
}

function showPage(pageToShow) {
    let currentShownPage = $(".page:visible");
    let currentVerticalScroll = window.scrollY;
    $(currentShownPage).hide(0);
    $(pageToShow).show(150, function () {
        scrollAfterPageChange(currentVerticalScroll);
    });
    
    setTimeout(function () { triggerResize(); }, 100);
}

function scrollAfterPageChange(currentVerticalScroll) {
    let displayedHeight = document.documentElement.clientHeight;
    let totalHeight = document.documentElement.scrollHeight;
    let possibleScroll = totalHeight - displayedHeight;
    let newVerticalScroll = possibleScroll <= currentVerticalScroll ? possibleScroll : currentVerticalScroll;
    $('html, body').animate({
        scrollTop: newVerticalScroll
    }, 100);
}

function goToReferrableFormInstance(id, versionId) {
    window.open(`/FormInstance/View?VersionId=${versionId}&FormInstanceId=${id}`, '_blank');
}

function handleBackInForm() {
    handleBackInFormAction();
}

$(document).on('click', '.form-des', function (event) {
    executeEventFunctions(event);

    showDescription(this, '.main-content', '.form-description:first');
});

function showDescription(element, elementDesc, description) {
    $(element).closest(elementDesc).find(description).toggle();
}

$(document).on('click', '.chapter-des', function (event) {
    executeEventFunctions(event);

    showDescription(this, '.form-accordion', '.chapter-description:first');
});

$(document).on('click', '.page-des', function (event) {
    executeEventFunctions(event);

    showDescription(this, '.page', '.page-description:first');
});

$(document).on('click', '.field-set-des', function (event) {
    executeEventFunctions(event);

    showDescription(this, '.field-set', '.fieldset-description:first');
});

$(document).on('click', '.x-des', function (event) {
    executeEventFunctions(event);

    $(this).closest('.desc').hide();
    $(this).closest(".des-container").find("div:first").find('.fa-angle-up').addClass('fa-angle-down');
    $(this).closest(".des-container").find("div:first").find('.fa-angle-up').removeClass('fa-angle-up');
});

function scrollActivePageTabIfNecessarry() {
    scrollPageTabs(true, true);
}

$(document).on('click', '.arrow-scroll-right-page', function (event) {
    executeEventFunctions(event, true);

    scrollPageTabs(true);
});

$(document).on('click', '.arrow-scroll-left-page', function (event) {
    executeEventFunctions(event, true);

    scrollPageTabs(false);
});

function scrollPageTabs(scrollToTheRight, scrollToSpecificPosition = false, resetScrollPosition = false) {
    let $pageTabs = getPreviousActiveChapterPageTabs();
    if (resetScrollPosition) {
        $pageTabs.attr("data-current-left-scroll", '0')
    }
    let scrollToTheLeftCurrent = +$pageTabs.attr("data-current-left-scroll");
    if (scrollToSpecificPosition) {
        $pageTabs.animate(
            {
                scrollLeft: scrollToTheLeftCurrent
            },
            "fast",
            "swing",
            function () {
                $(this).removeClass('invisible');
                $(this).siblings(".scroll-page-action").removeClass("d-none");
            }
        );
    } else {
        let scrollInPixels = 500;
        let scrollPrefix = scrollToTheRight ? '+' : '-';
        $pageTabs.animate({
            scrollLeft: `${scrollPrefix}=${scrollInPixels}px`
        }, "slow");
        updateCurrentScrollPosInCache($pageTabs, scrollToTheRight, scrollToTheLeftCurrent, scrollInPixels);
    }
}

function updateCurrentScrollPosInCache($pageTabs, scrollToTheRight, scrollToTheLeftCurrent, scrollInPixels) {
    let scrollToTheLeftUpdated = scrollToTheLeftCurrent + (scrollToTheRight ? 1 : -1) * scrollInPixels;
    if (scrollToTheLeftUpdated < 0) {
        scrollToTheLeftUpdated = 0;
    }
    $pageTabs.attr("data-current-left-scroll", scrollToTheLeftUpdated);
}

function toggleFileNameContainer($field, binaryFieldType, resourceName = '') {
    let $fileNameDiv = $field.find(".file-name-div");
    let $fileNameDisplayDiv = $fileNameDiv.find(".file-name-text");
    setFileDisplayNameComponent($fileNameDisplayDiv, binaryFieldType, resourceName);
    if (resourceName) {
        $fileNameDiv.show();
    } else {
        $fileNameDiv.hide();
    }
}

function setFileDisplayNameComponent($fileNameDisplayDiv, binaryFieldType, dataGuidName) {
    let fileName = getDisplayFileName(dataGuidName, binaryFieldType == 'file')
    $fileNameDisplayDiv
        .attr('data-guid-name', dataGuidName)
        .attr('title', fileName)
        .text(fileName);
}

function allowFileUploadIfValueIsReset($fileFieldContainer, revertToDefaultEditableState, $el) {
    var fieldName = $($el).attr('name');
    var inputFile = $('input[data-fileid="' + fieldName + '"]');
    inputFile.removeAttr('disabled');
    if (revertToDefaultEditableState) {
        $fileFieldContainer.removeClass("pe-none");
    } else {
        $fileFieldContainer.addClass("pe-none");
    }
}

$(document).on("change", ".file", function (event) {
    executeEventFunctions(event);

    var $fileNameField = $(this).siblings(".file-hid");
    unsetSpecialValueIfSelected(getSpecialValueElement($fileNameField.attr('name')));
    removeFieldErrorIfValid($fileNameField, $fileNameField.attr("id"));
    let binaryFieldType = 'file';
    deleteExistingBinaryFromServer($fileNameField.val(), binaryFieldType);
    uploadFileBinaryToServer($(this), binaryFieldType);
});

function uploadFileBinaryToServer($fileInput, binaryFieldType) {
    let file = $fileInput.prop('files')[0];
    if (file) {
        let filesData = [{
            id: $fileInput.attr('data-id'),
            content: file
        }];
        sendFileData(filesData,
            setResourceName,
            function (resourceName) {
                let $fieldContainer = $fileInput.closest(".repetitive-field");
                toggleFileNameContainer($fieldContainer, binaryFieldType, resourceName);
            },
            getBinaryDomain(binaryFieldType)
        );
        $fileInput.val('');
    }
}

$(document).on("change", ".file-hid", function (event) {
    executeEventFunctions(event);

    var $fileNameField = $(this);
    removeFieldErrorIfValid($fileNameField, $fileNameField.attr("id"));
});

function removeBinary(event, binaryFieldType) {
    let $fieldContainer = $(event.currentTarget).closest(".repetitive-field");
    deleteExistingBinaryFromServer(getBinaryNameInput($fieldContainer, binaryFieldType).val(), binaryFieldType);
    handleBinaryChange($fieldContainer, binaryFieldType);
}

function handleBinaryChange($field, binaryFieldType, resourceName = '') {
    toggleFileNameContainer($field, binaryFieldType, resourceName);
    let $binaryNameInput = getBinaryNameInput($field, binaryFieldType);
    $binaryNameInput.val(resourceName);
}

function getBinaryNameInput($fieldContainer, binaryFieldType) {
    return $fieldContainer.find(`.${binaryFieldType}-hid`);
}

$(document).on("click", ".file-choose", function (event) {
    executeEventFunctions(event);

    $(this).closest(".repetitive-field").find(".file").click();
});

function removeFieldErrorIfValidForTimeInput($correspondantDateInput) {
    var $timeField = $correspondantDateInput.closest(".datetime-picker-container").find(".field-time-input");
    removeFieldErrorIfValid($timeField, $timeField.attr("id"));
}

function removeFieldErrorIfValid($field, customFieldName = '') {
    if ($field.hasClass("error")) {
        $field.removeClass("error");
        var fieldName = customFieldName ? customFieldName : $field.attr("name");
        var $fieldErrorMessage = $(`#${fieldName}-error`);
        $fieldErrorMessage.remove();
    }
    $field.closest(".repetitive-field").removeClass('repetitive-error');
}

$(document).on('blur', '#fid input', function (e) {
    executeEventFunctions(e);

    validateInput(this, e);

    return false;
});

function validateInput(input, e) {
    e.preventDefault();
    e.stopPropagation();

    if (skipInputValidation($(input))) {
        return;
    }
    $(input).closest(".repetitive-field").removeClass('repetitive-error');
    if ($(input).hasClass("error")) {
        $(input).closest(".repetitive-field").addClass('repetitive-error');
    }
}

function skipInputValidation($input) {
    return $input.hasClass("ne-radio")
        || $input.hasClass("date-time-local")
        || $input.hasClass("field-time-input")
        || $input.attr("type") == "hidden"
        || $input.attr("type") == "file";
}

$('.collapse').on('shown.bs.collapse', function (e) {
    e.preventDefault();
    scrollToElement(this, 1000, 50);
});

function removeValidationMessages(element) {
    if (element.hasClass("error")) {
        element.removeClass("error");
        var repetitionId = $(element).attr('data-fieldinstancerepetitionid');
        var labelToRemove = document.getElementById(`${repetitionId}-error`);
        if (labelToRemove)
            labelToRemove.remove();
    }
}

$(document).on('click', '.hidden-fields-actions', function (event) {
    executeEventFunctions(event);
    toggleShowHiddenFieldsImage($(this));
    let restore = $(this).hasClass('hide-hidden-fields-action');

    if (restore) {
        $('.show-hidden-fields')
            .addClass('d-none')
            .removeClass('show-hidden-fields');
    } else {
        $('[data-dependables="False"]')
            .removeClass('d-none')
            .addClass('show-hidden-fields');
    }
});

function toggleShowHiddenFieldsImage($actionButton) {
    $('.hidden-fields-actions').removeClass('d-none');
    $actionButton.addClass('d-none');
}

function checkScrollButtons() {
    const cards = document.querySelectorAll('.card');
    cards.forEach(function (card) {
        const pageSelector = card.querySelector('.page-selector'); 
        const leftArrow = card.querySelector('.arrow-scroll-left-page'); 
        const rightArrow = card.querySelector('.arrow-scroll-right-page'); 

        if (pageSelector) {
            if (pageSelector.scrollWidth > pageSelector.clientWidth) {
                leftArrow.style.display = 'inline';
                rightArrow.style.display = 'inline';
                card.style.padding = '0px 40px 0px 40px';
            } else {
                leftArrow.style.display = 'none';
                rightArrow.style.display = 'none';
                card.style.padding = '0px 8px 0px 8px';
            }
        }
    });
}

//dropdown for matrix table
$(document).on('click', '.dropdown-matrix', function (event) {
    event.preventDefault();
    let $target = $(event.currentTarget);
    clickDropdownButton($target);
});

$(window).on('scroll', function () {
    hideOpenedDropdowns();
});

$('.chapters-container').on('scroll', function () {
    hideOpenedDropdowns();
});

function clickDropdownButton($target) {
    let $dropdown = $target.closest('.dropdown');
    let $tr = $target.closest('tr');
    if ($dropdown.hasClass('show')) {
        dropdownIsHidding($target);
        hideOpenedDropdown($dropdown, $tr);
    } else {
        hideOpenedDropdowns();
        dropdownIsShowing($target);
        showDropdown($dropdown, $tr);
    }
}

function showDropdown($dropdown, $tr) {
    let $dropdownMenu = $dropdown.find('.dropdown-menu');
    $dropdown.addClass('show');
    $dropdownMenu.addClass('show');
    $tr.addClass('grey-background');
    relocateDropdown($dropdown, $dropdownMenu);
}

function relocateDropdown($dropdown, $dropdownMenu) {
    let $td = $dropdown.closest('td');
    let offsets = getDropdownOffsets($td, $dropdownMenu);
    const { top, left } = getPosition($td);

    $dropdownMenu
        .css(
            "cssText",
            `left: ${left - offsets.leftOffset}px !important; 
             top: ${top + offsets.topOffset}px; 
             position: fixed
             `
        );

    if ($td) {
        $td.css("z-index", "10");
    }
}

function getDropdownOffsets($td, $dropdownMenu) {
    let dropdownMenuWidth = getWidth($dropdownMenu);
    let tdWidth = getWidth($td);
    let elementsWidthDiff = dropdownMenuWidth - tdWidth;
    let staticLeftOffset = 10;
    let leftOffset = elementsWidthDiff + staticLeftOffset;
    let topOffset = 15;

    return {
        leftOffset,
        topOffset
    }
}

$(document).on('click', function (e) {
    clickOutsideDropdown($(e.target));
});

function clickOutsideDropdown($target) {
    if ($('.fieldsets').length > 0 && !($target.hasClass('dots') && $target.parent().is('.dropdown-matrix'))) {
        hideOpenedDropdowns();
    }
}

function hideOpenedDropdowns() {
    $('.fieldsets .dropdown.show').each(function () {
        let $dropdown = $(this);
        let $tr = $dropdown.closest('tr');
        dropdownIsHidding($(this).find('.dropdown-matrix'));
        hideOpenedDropdown($dropdown, $tr);
    });
}

function hideOpenedDropdown($dropdown, $tr) {
    $dropdown.removeClass('show');
    $dropdownMenu = $dropdown.children('.dropdown-menu');
    $dropdownMenu.removeClass('show');
    $tr.removeClass('grey-background');
    $dropdownMenu
        .css(
            "cssText",
            `left: ; 
                top: ; 
                position: 
                `
        );

    let $td = $dropdown.closest('td');
    if ($td) {
        $td.css("z-index", "");
    }
}