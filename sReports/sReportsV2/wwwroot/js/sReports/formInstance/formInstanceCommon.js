function readyFunctionInEditableMode() {
    setDateTimeValidatorMethods();
    initValidationForRegexFieldInstances();
    readyFunctionInReadOnlyMode();
}

function readyFunctionInReadOnlyMode() {
    saveInitialFormData("#fid");
    configureImageMap();
    scrollActivePageTabIfNecessarry();
    initTooltips();
    restorePageScrollPosition();
    markSemanticQueryElements();
    initializeInputDropdown();
}

function restorePageScrollPosition() {
    let activeChapterId = $('.chapter-li.active').attr('data-id');
    let savedScrollPosition = localStorage.getItem(`pageScrollPosition_${activeChapterId}`) || '0';
    let $pageTabs = $(`#pagesTabs-chapter-${activeChapterId}`);
    let isFirstPageActive = $pageTabs.find('.pages-link').first().hasClass('active');

    if ($pageTabs.length && savedScrollPosition !== '0' && !isFirstPageActive) {
        $pageTabs.attr("data-current-left-scroll", savedScrollPosition);
        $pageTabs.animate(
            {
                scrollLeft: savedScrollPosition
            },
            "fast",
            "swing",
            function () {
                $(this).removeClass('invisible');
                $(this).siblings(".scroll-page-action").removeClass("d-none");
            }
        );
    }
}

function initTooltips() {
    $('[data-toggle="tooltip"]').tooltip('dispose');
    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom',
        trigger: 'hover focus',
        container: 'body',
        boundary: 'window',
    });

    $('[data-toggle="tooltip"]').on('mouseenter', function () {
        $(this).tooltip('update');
    });
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
            messages: { 
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
    inactivateAllMatrixFields(document.querySelectorAll(".fieldset-matrix-td"));
    updateMissingValueDisplay(fieldInstanceName);

    if (typeof reenableTinyMCEEditor !== 'undefined') {
        reenableTinyMCEEditor(fieldInstanceName);
    }
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
        'number': setInputValueForNumber,
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

    changeChapterAction(event, this, true);
});

function saveIfThereAreChangesAndCollapse(event, chapterContainerId, isChapterLinkClicked) {
    saveIfThereAreChangesAndContinue(event, function () {
        let $chapterContainer = $(`#${chapterContainerId}`);
        if (isChapterLinkClicked) {
            $('.form-accordion').hide();
            $chapterContainer.show();
        }

        collapseChapter($chapterContainer);
    });
}

function changeChapterAction(event, chapterElement, isChapterLinkClicked) {
    let clickedChapterContainerId;
    if (isChapterLinkClicked) {
        clickedChapterContainerId = $(chapterElement).attr('id').replace('li', 'acc');
    } else {
        clickedChapterContainerId = $(chapterElement).parent().attr("id");
    }

    saveIfThereAreChangesAndCollapse(event, clickedChapterContainerId, isChapterLinkClicked);
}

function chapterLinkIsClicked(event, $chapterContainerElement) {
    let clickedChapterContainerId = $chapterContainerElement.attr('id').replace('li', 'acc');

    saveIfThereAreChangesAndCollapse(event, clickedChapterContainerId, true);
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

function updateChapterHeaders($clickedChapterHeader) {
    $('.enc-form-instance-header').each(function () {
        let $chapter = $(this);
        if ($chapter.attr('data-link-id') != $clickedChapterHeader.attr('data-link-id')) {
            $chapter.find('.chapter-icon').attr('src', '/css/img/icons/u_plus.svg');
            $chapter.removeClass("chapter-active");
        } else {
            let $chapterIcon = $chapter.find('.chapter-icon');
            if ($chapterIcon.attr('src') === '/css/img/icons/u_plus.svg') {
                $chapterIcon.attr('src', '/css/img/icons/u_minus.svg');
                $chapter.addClass('chapter-active');
            } else {
                $chapterIcon.attr('src', '/css/img/icons/u_plus.svg');
                $chapter.removeClass('chapter-active');
            }
        }

    });
}

function collapseChapter($chapterContainer) {
    let $clickedChapterHeader = $chapterContainer.children('.enc-form-instance-header:first')
    let clickedChapterContentId = $clickedChapterHeader.data('target');
    let $chaptersContainer = $clickedChapterHeader.closest(".chapters-container");
    let $clickedChapterContent = $chaptersContainer.find(clickedChapterContentId);

    handlePageChange($clickedChapterContent.find('.pages-link:first').attr("id"));

    $chaptersContainer.find(`.chapter.collapse:not(${clickedChapterContentId})`).collapse('hide');
    $clickedChapterContent.collapse('show');

    if (clickedChapterContentId == '#administrativeChapter') {
        showAdministrativeArrowIfOverflow('administrative-container-form-instance');
    }

    scrollPageTabs(true, true, true);
    let $activatedChapterTab = $($clickedChapterHeader.attr('data-link-id'));
    updateChapterTab($('.chapter-li.active'), false);
    updateChapterTab($activatedChapterTab, true);
    updateChapterHeaders($clickedChapterHeader);
}

$(document).on("click", ".pages-link", function (event) {
    executeEventFunctions(event, true);
    pageIsClicked(event, $(this));
});

function addAtTheEnd(original, extra) {
    return function (...args) {
        original.apply(this, args); // run the original
        extra.apply(this, args);    // then run the extra
    };
}

function saveIfThereAreChangesAndContinue(event, callback) {
    callback = addAtTheEnd(callback, function () {
        saveInitialFormData("#fid");
    });
    if (compareForms("#fid")) {
        executeCallback(callback);
    } else {
        clickedSubmit(event, callback);
    }
}

function pageIsClicked(event, $pageTab) {
    let pageTabId = $pageTab.attr('id');
    saveIfThereAreChangesAndContinue(event, function () {
        handlePageChange(pageTabId);
    });
}

function handlePageChange(pageTabId) {
    if (pageTabId) {
        let $clickedPageTab = $(`#${pageTabId}`);

        showPage(getPageFromPageTab($clickedPageTab));

        $('.pages-link').removeClass('active');
        $clickedPageTab.addClass('active');

        let $activeChapterContainer = $clickedPageTab.closest('.pages-links');
        let $chapterPages = $activeChapterContainer.find(".pages-link");
        let currentIndex = $chapterPages.index($clickedPageTab);
        updateArrowState(currentIndex, $chapterPages.length);
    } else {
        $(".navigation-arrow")
            .addClass("arrow-disabled");
    }
}

function showPage(pageToShow) {
    let currentShownPage = getPreviousActivePage();
    let currentVerticalScroll = window.scrollY;
    $(currentShownPage).hide(0);
    $(pageToShow).show(150, function () {
        scrollAfterPageChange(currentVerticalScroll);
        configureImageMap();
    });
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

$(document).on("click", ".navigation-arrow", function (event) {
    executeEventFunctions(event);
    if ($(this).hasClass("arrow-disabled")) return;
    let isRightDirection = $(this).attr('id') == "rightArrow";
    saveIfThereAreChangesAndContinue(event, function () {
        scrollToAdjacentPage(isRightDirection ? 1 : - 1);
        scrollPages(event, isRightDirection);
    });
});

function scrollToAdjacentPage(direction) {
    let $activeChapterContainer = $(".form-accordion:visible");
    let $pages = $activeChapterContainer.find(".pages-link");

    let $active = $pages.filter(".active");
    let index = $pages.index($active);
    let nextIndex = index + direction;

    if (nextIndex >= 0 && nextIndex < $pages.length) {
        let $nextPage = $pages.eq(nextIndex);
        $nextPage.trigger("click");
        updateArrowState(nextIndex, $pages.length);
    }
}

function updateArrowState(currentIndex, totalPages) {
    if (currentIndex <= 0) {
        $("#leftArrow")
            .addClass("arrow-disabled");
    } else {
        $("#leftArrow")
            .removeClass("arrow-disabled");
    }

    if (currentIndex >= totalPages - 1) {
        $("#rightArrow")
            .addClass("arrow-disabled");
    } else {
        $("#rightArrow")
            .removeClass("arrow-disabled");
    }
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
    scrollPages(event, true);
});

$(document).on('click', '.arrow-scroll-left-page', function (event) {
    scrollPages(event, false);
});

function scrollPages(event, scrollRight) {
    executeEventFunctions(event, true);
    scrollPageTabs(scrollRight);
}

function scrollPageTabs(scrollToTheRight, scrollToSpecificPosition = false, resetScrollPosition = false) {
    let $pageTabs = getPreviousActiveChapterPageTabs();
    if (resetScrollPosition) {
        $pageTabs.attr("data-current-left-scroll", '0');
        localStorage.setItem('activePageLeftScroll', '0');
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
        let $pageLinks = $pageTabs.find('.pages-link');
        let activeIndex = $pageLinks.index($pageLinks.filter('.active'));
        let $targetTab = null;

        if (scrollToTheRight) {
            $targetTab = $pageLinks.eq(activeIndex - 1).length ? $pageLinks.eq(activeIndex - 1) : $pageLinks.last();
        } else {
            $targetTab = $pageLinks.eq(activeIndex + 1).length ? $pageLinks.eq(activeIndex + 1) : $pageLinks.first();
        }

        let scrollInPixels = $targetTab.length ? $targetTab.outerWidth() - 10 : 140;
        let scrollPrefix = scrollToTheRight ? '+' : '-';
        $pageTabs.animate({
            scrollLeft: `${scrollPrefix}=${scrollInPixels}px`
        }, "slow");
        updateCurrentScrollPosInCache($pageTabs, scrollToTheRight, scrollToTheLeftCurrent, scrollInPixels);
    }
}

function savePageScrollPosition($pageTabs) {
    let scrollPosition = $pageTabs.attr("data-current-left-scroll") || '0';
    let activeChapterId = $('.chapter-li.active').attr('data-id');
    localStorage.setItem(`pageScrollPosition_${activeChapterId}`, scrollPosition);
}

function updateCurrentScrollPosInCache($pageTabs, scrollToTheRight, scrollToTheLeftCurrent, scrollInPixels) {
    let scrollToTheLeftUpdated = scrollToTheLeftCurrent + (scrollToTheRight ? 1 : -1) * scrollInPixels;
    if (scrollToTheLeftUpdated < 0) {
        scrollToTheLeftUpdated = 0;
    }
    $pageTabs.attr("data-current-left-scroll", scrollToTheLeftUpdated);
    savePageScrollPosition($pageTabs);
}

function toggleFileNameContainer($field, binaryFieldType, resourceName = '') {
    let $fileNameDiv = $field.find(".file-name-div");
    let $fileNameDisplayDiv = $fileNameDiv.find(".file-name-text");
    let $downloadFile = $field.find(".download-predefined");
    setFileTitleComponent($fileNameDisplayDiv, binaryFieldType, resourceName);
    setFileDisplayNameComponent($downloadFile, binaryFieldType, resourceName);

    if (resourceName) {
        $fileNameDiv.show();
    } else {
        $fileNameDiv.hide();
    }
}

function setFileTitleComponent($fileNameDisplayDiv, binaryFieldType, dataGuidName) {
    let fileName = getDisplayFileName(dataGuidName, binaryFieldType == 'file')
    $fileNameDisplayDiv
        .attr('data-guid-name', dataGuidName)
        .text(fileName);
}

function setFileDisplayNameComponent($fileNameDisplayDiv, binaryFieldType, dataGuidName) {
    $fileNameDisplayDiv
        .attr('data-guid-name', dataGuidName);
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

function updateMissingValueDisplay(fieldInstanceName) {
    let $missingValueDiv = $(".missing-value-span[name='" + fieldInstanceName + "']");
    let $secondDiv = $missingValueDiv.next("span");
    let $inputElement = $missingValueDiv.next(".form-element").find("input");

    if ($missingValueDiv.length > 0) {
        $missingValueDiv.addClass("hide-missing-value").removeClass("show-missing-value");
        $secondDiv.removeClass("hide-missing-value");
        if ($inputElement.length > 0) {
            $inputElement.removeClass("hide-missing-value");
        }
    }
}

function markSemanticQueryElements() {
    let data = JSON.parse(localStorage.getItem("semanticGraphData"));
    $('[data-thesaurusid]').each(function () {
        let thesaurusid = +$(this).attr('data-thesaurusid');
        if (data?.includes(thesaurusid)) {
            $(this).closest('fieldset.form-element')
                .addClass('semantic-data-field')
                .prepend($(`<p class="semantic-data-field-info">Semantic Data Field</p>`));
        }
    });
}