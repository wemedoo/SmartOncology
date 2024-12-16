$.ajaxSetup({
    statusCode: {
        401: function (response) {
            var returnUrl = encodeURI(window.location.pathname + window.location.search);
            var loginUrl = '/User/Login?ReturnUrl=' + returnUrl;

            window.location.href = loginUrl;
        },
        403: function (response) {
            let errorMessage = getErrorMessage(response);
            toastr.error(errorMessage ? errorMessage : response.statusText);
        }
    }
});

function callServer(options) {
    if (options.data && typeof options.data === 'object') {
        prepareDataForServer(options.data);

        if (options.contentType == 'application/json') {
            options.data = JSON.stringify(options.data);
        }
    }

    $.ajax(options);
}

function prepareDataForServer(data) {
    for (let key in data) {
        if (typeof data[key] === 'string') {
            data[key] = data[key].trim();
        } else if (data[key] == 'object') {
            prepareDataForServer(data[key]);
        }
    }
}

$(document).on("change", "select[data-codesetid]", function (event) {
    let element = event.target;
    reloadCodeSetChildren(element.id);
});

function showFormDefinitionData(event, formId) {
    $('.table-active').removeClass('table-active');
    $(event.srcElement).closest('tr').addClass('table-active');
}

function setActiveLanguage(event, value) {
    event.preventDefault();
    let params = changeLanParam(value);
    if (simplifiedApp) {
        $(event.srcElement).parent().siblings().removeClass('active');
        $(event.srcElement).parent().addClass('active');
        window.location.href = `${location.protocol}//${location.host}${location.pathname}${params}`;
    }
    else
    {
        let formData = { newLanguage: value };
        callServer({
            type: "PUT",
            url: `/User/UpdateLanguage`,
            data: formData,
            success: function (data) {
                $(event.srcElement).parent().siblings().removeClass('active');
                $(event.srcElement).parent().addClass('active');
                window.location.href = `${location.protocol}//${location.host}${location.pathname}${getRemovedPageInfo() ? '?' + getRemovedPageInfo() : ''}`;
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }

}

function changeLanParam(newValue) {
    let params = location.search.split('&');
    let newParams = [];
    params.filter(x => !x.includes('language') && !x.includes('Language')).forEach(x => {
        newParams.push(x);
    })
    newParams.push(`language=${newValue}`);

    return newParams.join('&');
}

function updatePageSize(value) {
    if (!simplifiedApp) {
        let formData = { PageSize: value };
        callServer({
            type: "PUT",
            url: `/User/UpdatePageSizeSettings`,
            data: formData,
            success: function (data) {
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }

}

function setActiveOrganization(event, value, targetUrl) {
    event.preventDefault();
    callServer({
        type: "PUT",
        url: `/UserAdministration/UpdateOrganization?organizationId=${value}`,
        success: function (data) {
            $(event.srcElement).parent().siblings().removeClass('active');
            $(event.srcElement).parent().addClass('active');
            if (targetUrl) {
                window.location.href = `${targetUrl}`;
            } else {
                window.location.href = `${location.protocol}//${location.host}${location.pathname}${getRemovedPageInfo() ? '?' + getRemovedPageInfo() : ''}`;
            }
            
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getRemovedPageInfo(){
    var url = window.location.search;
    let splitted = url.replace("?", '').split('&');
    return splitted.filter(x => !x.includes('page=') && !x.includes('pageSize=')).join('&');
}

function showHideLoaderOnAjaxRequest() {
    var oldXHR = window.XMLHttpRequest;

    function newXHR() {
        var realXHR = new oldXHR();
        realXHR.addEventListener("readystatechange", function () {
            if (realXHR.readyState === 1) {
                $("#loaderOverlay").show();
                //console.log('server connection established');
            }
            if (realXHR.readyState === 2) {
                //console.log('request received');
            }
            if (realXHR.readyState === 3) {
                //console.log('processing request');
            }
            if (realXHR.readyState === 4) {
                var multiFileDownLoad = getResponseBooleanHeader(realXHR, "MultiFile", false);
                if (realXHR.responseType == "blob" || multiFileDownLoad) {
                    hideLoaderForMultiFileResponse(realXHR);
                } else {
                    hideLoader();
                }
                //console.log('request finished and response is ready');
            }
        }, false);
        return realXHR;
    }
    window.XMLHttpRequest = newXHR;
}

function hideLoader() {
    $("#loaderOverlay").hide(100);
    $('#loaderOverlay').hide();
}

function hideLoaderForMultiFileResponse(xhr) {
    var lastFile = getResponseBooleanHeader(xhr, "LastFile", true);
    if (lastFile) {
        hideLoader();
    }
}

function getResponseBooleanHeader(xhr, name, predifinedValue) {
    var headerParam = xhr.getAllResponseHeaders().indexOf(name) >= 0 ? xhr.getResponseHeader(name) : null;
    return headerParam == null ? predifinedValue : JSON.parse(headerParam);
}

function checkUrlPageParams() {

    var url = new URL(window.location.href);
    var page = url.searchParams.get("page");
    var pageSize = url.searchParams.get("pageSize");

    if (page && pageSize) {
        currentPage = page;
        $('#pageSizeSelector').val(pageSize);
    }
    else {
        currentPage = 1;
    }
}

function checkSecondaryPage() {

    var url = new URL(window.location.href);
    var secondaryPage = url.searchParams.get("secondaryPage");

    if (secondaryPage) {
        formsCurrentPage = secondaryPage;
    } else {
        formsCurrentPage = 1;
    }
}



$(document).ready(function () {
    showHideLoaderOnAjaxRequest();
    $('#closeSidebar, #showSidebar').on('click', function () {
        $('body').toggleClass('sidebar-on');
    });

    $('#menuShrinkBtn').on('click', function () {
        $('body').toggleClass('sidebar-shrink');
    });


    $('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
        
        if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
        }
        var $subMenu = $(this).next(".dropdown-menu");
        $subMenu.toggleClass('show');


        $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
            $('.dropdown-submenu .show').removeClass("show");
        });

        return false;
    });

   

    $('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
        $('.dropdown-item-custom').each(function (index, element) {
            switchAngles(element);
        });
       
        return false;
    });

    $('.dropdown-menu').on('change', function (e) {
        $('.dropdown-item-custom').each(function (index, element) {
            switchAngles(element);
        });

        return false;
    });

    $(document).on('show.bs.dropdown', function (event) {
        dropdownIsShowing($(event.relatedTarget));
    });

    $(document).on('hide.bs.dropdown', function (event) {
        dropdownIsHidding($(event.relatedTarget));
    });

    $(document).on('focusin', 'input, select, textbox, textarea', function () {
        if ($(this).data('no-color-change')) return;

        if ($(this).prev().length > 0) {
            setLabelColor($(this).prev(), '#4dbbc8');
        } else {
            setLabelColor($(this).parent().parent().prev(), '#4dbbc8');
        }
    });

    $(document).on('focusout', 'input, select, textbox, textarea', function () {
        if ($(this).data('no-color-change')) return;

        if ($(this).prev().length > 0) {
            setLabelColor($(this).prev(), '#000000');
        } else {
            setLabelColor($(this).parent().parent().prev(), '#000000');
        }  
    });

    $(document).on('click', '.close-modal' , function (e) {
        $(this).closest('.modal').modal('hide');

        return false;
    });

    $('#sessionBreakModal').on('hidden.bs.modal', function () {
        restartIdleInterval();
        clearInterval(secondsInteral);
    });


    function setLabelColor(element, color) {
        $(element).css('color', color);
    }

    window.addEventListener('popstate', function (event) {
        this.window.location.reload(true);
    }, false);

    setCommonValidatorMethods();
});

function dropdownIsShowing($dropdownButton) {
    if ($dropdownButton.hasClass("dropdown-button") || $dropdownButton.hasClass("dropdown-matrix")) {
        $('.dots').each(function (index, element) {
            if ($(element).hasClass('active')) {
                $(element).removeClass('active');
            }
            if ($dropdownButton.hasClass("dropdown-matrix")) {
                $(element).attr('src', '/css/img/icons/dots_black.svg');
            }
            else {
                $(element).attr('src', '/css/img/icons/3dots.png');
            }
        });
        let button = $dropdownButton; // Get the text of the element
        let dots = $(button).children('img:first');
        $(dots).attr('src', '/css/img/icons/dots-active.png');
        if (!$dropdownButton.hasClass("dropdown-matrix")) {
            $(dots).addClass('active');
        }

        if ($dropdownButton.children('div:first').hasClass('btns')) {
            $('#btns').children('img:first').attr('src', '/css/img/icons/dropdown_open.svg');
        }
    }
}

function dropdownIsHidding($dropdownButton) {
    let $dropdown = $dropdownButton.closest('.dropdown').children('.dropdown-menu');
    if ($dropdown.hasClass('show')) {
        let dots = $dropdownButton.children('img:first');
        if ($(dots).hasClass('dots')) {
            if ($dropdownButton.hasClass("dropdown-matrix")) {
                $(dots).attr('src', '/css/img/icons/dots_black.svg');
            }
            else {
                $(dots).attr('src', '/css/img/icons/3dots.png');
            }
            $(dots).removeClass('active');
        }

        $('#btns').children('img:first').attr('src', '/css/img/icons/dropdown.svg');
    }

    $dropdown.find('.dropdown-item-custom').each(function (index, element) {
        $(element).find('i:first').removeClass('fa-angle-down');
        $(element).find('i:first').addClass('fa-angle-right');
    });
}

function setCommonValidatorMethods() {
    setDateTimeValidatorMethods();
}

function isRadioOrCheckbox(element) {
    return element.is(":radio") || element.is(":checkbox");
}

function getElementWhereErrorShouldBeAdded(element) {
    var parentFieldSelector = element.attr("data-parent-field");
    if (!parentFieldSelector) {
        parentFieldSelector = ".advanced-filter-item";
    }
    return element.closest(parentFieldSelector);
}

function modifyIfSecondError(targetContainerForErrors, error) {
    var elementHasAlreadyOneError = targetContainerForErrors.find("label.error").length == 1;
    if (elementHasAlreadyOneError) {
        error.css("bottom", "-37px");
    }
}

function handleErrorPlacementForRadioOrCheckbox(error, element) {
    error.appendTo(element.parent().parent());
}

function handleErrorPlacementForOther(error, element) {
    error.appendTo(element.parent());
}

$(window).on('beforeunload', function (event) {
    $("#loaderOverlay").show(100);
    setTimeout(function () {
        hideLoader();
    }, 1500);
});

function logout(e) {
    e.preventDefault();
    e.stopPropagation();

    window.location.href = '/User/Logout';

}

function switchAngles(element) {
    if ($(element).children('ul:first').hasClass('show')) {
        $(element).find('i:first').removeClass('fa-angle-right');
        $(element).find('i:first').addClass('fa-angle-down');
    }
    else {
        $(element).find('i:first').removeClass('fa-angle-down');
        $(element).find('i:first').addClass('fa-angle-right');
    }
}

function sendFileData(fileData, setFieldCallback, filesUploadedCallBack, domain, url = '/Blob/Create') {
    var uploaded = 0;
    for (const file of fileData) {
        var fd = new FormData();
        fd.append('file', file.content);
        fd.append('domain', domain);

        callServer({
            url: url,
            data: fd,
            processData: false,
            contentType: false,
            type: 'POST',
            success: function (data) {
                uploaded++;
                if (setFieldCallback) {
                    console.log('setting field: ' + file.id);
                    setFieldCallback(file.id, data);
                }

                if (uploaded === fileData.length) {
                    filesUploadedCallBack(data);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function setResourceName(id, url) {
    $(`#${id}`).val(url);
}

$(".menu-btn-md").on("click", function () {
    if ($('.navbar-collapse').hasClass('show')) {
        $('body').css('overflow', 'auto');
    } else {
        $('body').css('overflow', 'hidden');
    }
    $(".navbar-collapse").collapse('toggle');
});

function setFilterFromUrl() {
    let url = new URL(window.location.href);
    let entries = url.searchParams.entries();
    let params = paramsToObject(entries);

    if (defaultFilter) {
        defaultFilter = params;
    }

    for (let param in params) {
        $(`#${param}`).val(`${params[param]}`);
        $(`#${param}Temp`).val(`${params[param]}`);
        $(`#${firstLetterToLower(param)}`).val(`${params[param]}`);
        $(`#${firstLetterToLower(param)}Temp`).val(`${params[param]}`);
        $(`#${firstLetterToLower(param)}`).prop('checked', params[param] == 'true');
        setValueForDateTime(param, params[param]);
    }
}

function paramsToObject(entriesData) {
    let result = {};
    for (let tupple of entriesData) { // each 'entry' is a [key, value] tupple
        const [key, value] = tupple;
        result[key] = value;
    }
    return result;
}

function reviewThesaurus(event,id)
{
    event.stopPropagation();
    event.preventDefault();
    window.location.href = `/ThesaurusEntry/GetReviewTree?id=${id}&page=${1}&pageSize=10`;
}

$(document).on('click', '.remove-filter, .remove-multitable-filter', function (e) {

    $(this).closest('.filter-element').remove();
    
    let filterInputId = $(this).attr('name');
    let filterInputIdFirstCharLower = firstLetterToLower(filterInputId);

    if (filterInputId == 'PatientListName') {
        removePatientListFilter();
    } else {
        resetInputForRemovedFilter($(`#${filterInputId}`));
        resetInputForRemovedFilter($(`#${filterInputIdFirstCharLower}`));
    }

    advanceFilter();
});

function resetInputForRemovedFilter($input) {
    if ($input.length > 0) {
        if ($input.hasClass('date-time-local')) {
            $input.closest('.datetime-picker-container').find('.filter-input').val('');
        } else {
            $input.val("");
            $input.prop("checked", false);
            emptyValueForSelect2($input);
        }
    }
}

function firstLetterToLower(string) {
    return string.charAt(0).toLowerCase() + string.slice(1);
}

function goToThesaurus(thesaurusId) {
    if (thesaurusId) {
        window.open(`/ThesaurusEntry/EditByO4MtId?id=${thesaurusId}`, '_blank');

        //window.location.href = `/ThesaurusEntry/EditByO4MtId?id=${thesaurusId}`;
    }
}


var startIdleTime = Date.now();
var secondsInteral;
var totalSeconds;


document.addEventListener('mousemove', resetIdleTime, false);
document.addEventListener('keypress', resetIdleTime, false);

function resetIdleTime() {
    startIdleTime = Date.now();
}


function checkIfIdle() {
    var elapsedTime = Date.now() - startIdleTime;
    if (elapsedTime >= 7200000) {
        //after 2 hours inactivity modal shows
        showSessionBreakModal();
        clearInterval(idleInterval);
    }
}

var idleInterval = setInterval(checkIfIdle, 1000);
function restartIdleInterval() {
    idleInterval = setInterval(checkIfIdle, 1000);
}

function showSessionBreakModal() {
    $('#expiredSeconds').width(0);
    $('#remainingSeconds').css("width","100%");
    totalSeconds = 60;
    $("#seconds").text(totalSeconds);
    secondsInteral = setInterval(setTime, 1000);
    $('#sessionBreakModal').modal('show');

}


function setTime() {
    if (totalSeconds > 0) {
        --totalSeconds;
        $("#seconds").text(totalSeconds);
        let secondsLineWidth = $('#secondsLine').width();
        let oneSecondWidth = secondsLineWidth / 60;
        $('#remainingSeconds').width(oneSecondWidth * totalSeconds);
        $('#expiredSeconds').width(oneSecondWidth * (60 - totalSeconds));
    }
    else {
        $('#sessionBreakLogout').click();
    }
}

function continueSession(){
    $('#sessionBreakModal').modal('hide');
    window.location.reload();
}

function decodeLocalizedString(value) {
    return $('<div/>').html(value).text();
}

function objectHasNoProperties(object, excludeList) {
    return !Object.keys(object).filter(x => !excludeList.includes(x)).length;
}

function handleResponseError(xhr, ignoreStatusCode = false) {
    if (xhr.status == 0) {
        var errorMessage = 'Unknown error (SO client)! Please contact your administrator for more details';
        toastr.error(errorMessage);
        throw new Error(errorMessage);
    } else if (xhr.status != 403) {
        let responseErrorMessage = getErrorMessage(xhr);
        if (ignoreStatusCode) {
            toastr.error(`${responseErrorMessage}`)
        } else {
            toastr.error(`${xhr.status} ${responseErrorMessage}`);
        }
    }
}

function getErrorMessage(xhr) {
    return xhr.responseJSON?.errorMessage ?? '';
}

$(document).on('show.bs.modal', '.modal, .custom-modal', function (e) {
    setLowerZIndexForSidebar();
});

$(document).on('hidden.bs.modal', '.modal, .custom-modal', function (e) {
    setDefaultZIndexForSidebar();
});

$(document).on('lowZIndex', '.custom-modal, .digital-guideline-modal, .modal-window', function (e) {
    setLowerZIndexForSidebar();
});

$(document).on('defaultZIndex', '.custom-modal, .digital-guideline-modal, .modal-window', function (e) {
    setDefaultZIndexForSidebar();
});

function setLowerZIndexForSidebar() {
    $('.sidebar').css("z-index", "1010");
    $('nav.sticky-top').css("z-index", "1040");
}

function setDefaultZIndexForSidebar() {
    $('.sidebar').css("z-index", "1052");
    $('nav.sticky-top').css("z-index", "1140");
}

function showAdministrativeArrowIfOverflow(administrativeContainerId) {
    var $administrativeChangeContainer = $(`#${administrativeContainerId}`);

    if ($administrativeChangeContainer.length > 0) {
        var childrenWidthTotal = 0;
        $administrativeChangeContainer.find('.workflow-item').each(function () {
            childrenWidthTotal += Math.round($(this).outerWidth());
        });

        var $arrows = $administrativeChangeContainer.find(".arrow-scroll");
        var overflow = childrenWidthTotal > $administrativeChangeContainer.outerWidth();
        if (overflow) {
            $arrows.removeClass('d-none');
        } else {
            $arrows.addClass('d-none');
        }
    }
}

function downloadBinary(event, excludeGUIDPartFromName, domain) {
    let dataGuidName = $(event.currentTarget).attr('data-guid-name');
    if (dataGuidName) {
        downloadResource(event, dataGuidName, getDisplayFileName(dataGuidName, excludeGUIDPartFromName), domain);
    }
}

function getDisplayFileName(dataGuidName, excludeGUIDPartFromName) {
    let displayName = dataGuidName;
    if (excludeGUIDPartFromName) {
        let simpleName = dataGuidName.split('_')[1];
        displayName = simpleName ? simpleName : displayName;
    }
    return displayName;
}

function downloadResource(event, dataGuidName, fileName, domain) {

    if (dataGuidName) {
        let request = {};
        request['resourceId'] = dataGuidName;
        request['domain'] = getBinaryDomain(domain);

        getDocument('/Blob/Download', fileName, '', request);
    }
}

function getResponseErrorWhenDownload(url, request) {
    let ajaxObject = {
        type: 'GET',
        url: url,
        data: request,
        success: function (data) {
            console.error(`Success request ${request} in 2nd try`);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    };
    if (request) {
        ajaxObject['data'] = request;
    }
    callServer(ajaxObject);
}

function getDocument(url, title, extension, requestData, beforeSend) {
    let ajaxObject = {
        url: url,
        xhr: function () {
            var xhr = new XMLHttpRequest();
            xhr.responseType = 'blob';
            return xhr;
        },
        success: function (data) {
            getDownloadedFile(data, title, extension);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            getResponseErrorWhenDownload(url);
        }
    };

    if (requestData) {
        ajaxObject['data'] = requestData;
    }

    if (beforeSend) {
        ajaxObject['beforeSend'] = function (request) {
            request.setRequestHeader("LastFile", beforeSend.lastFile);
        };
    }

    callServer(ajaxObject);
}

function getDownloadedFile(blob, fileName, extension = '') {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = fileName + (extension ? extension : '');
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
}

function deleteExistingBinaryFromServer(currentFileName, domain) {
    if (currentFileName) {
        callServer({
            type: "DELETE",
            url: `/Blob/Delete?resourceId=${currentFileName}&domain=${getBinaryDomain(domain)}`,
            success: function (data) {
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function pressButtonOnEnterKey(event, buttonId) {
    if (event.keyCode === enter) {  // when pressing Enter key
        event.preventDefault();
        $(`#${buttonId}`).click();
    }
}

function hasParamInUrl(name) {
    const urlParams = new URLSearchParams(location.search);
    return urlParams.has(name);
}

function getParamFromUrl(name) {
    const urlParams = new URLSearchParams(location.search);
    return urlParams.get(name);
}

function inputsAreInvalid(inputformType = "search") {
    var numOfInvalidInputs = 0;
    $("input[data-date-input]").each(function () {
        var isInputValid = validateDateInput($(this));
        if (!isInputValid) {
            ++numOfInvalidInputs;
        }
    });
    $(".time-helper").each(function () {
        var isInputValid = validateTimeInput($(this));
        if (!isInputValid) {
            ++numOfInvalidInputs;
        }
    });
    var areInputsInvalid = numOfInvalidInputs > 0;
    if (areInputsInvalid) {
        toastr.error(`Some of ${inputformType} inputs are not valid!`);
    }
    return areInputsInvalid;
}

function setIsReadOnlyViewModeInRequest(requestObject) {
    requestObject['isReadOnlyViewMode'] = readOnly;
}

function reloadCodeSetChildren(elementId) {
    try {
        var selectElement = document.getElementById(elementId);
        var selectedOption = selectElement.options[selectElement.selectedIndex];
        if (selectedOption) {
            var parentId = selectedOption.getAttribute("value");
            const codeSetId = selectElement.getAttribute("data-codesetid");

            let executeReloadCodeSetChildren = parentId != "" && codeSetId;
            if (executeReloadCodeSetChildren) {
                const childCodeSetId = dictionary.size > 0 && dictionary.has(codeSetId) ? dictionary.get(codeSetId) : 0;
                getCodeValues(parentId, childCodeSetId, codeSetId);
            }
        }
        
    } catch (e) {

    }
}

function getCodeValues(parentId, childCodeSetId, codeSetId) {
    callServer({
        type: 'GET',
        url: `/Code/GetChildCodes?ParentId=${parentId}&CodeSetId=${childCodeSetId}`,
        success: function (data) {
            if (data.length > 0) {
                updateCodeSetSelect(data, data[0].codeSetId);
                addToDictionary(codeSetId, data[0].codeSetId);
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function addToDictionary(parentId, childId) {
    const elementsWithDataCodesetId = document.querySelectorAll('[data-codesetid]');
    let existsDataCodesetId = false;
    elementsWithDataCodesetId.forEach((element) => {
        const codesetIdValue = element.getAttribute('data-codesetid');
        if (codesetIdValue == childId) {
            existsDataCodesetId = true;
        }
    });
    if (existsDataCodesetId) {
        if (!dictionary.has(parentId)) {
            dictionary.set(parentId, [childId]);
        } else {
            const children = dictionary.get(parentId);
            if (!children.includes(childId)) {
                children.push(childId);
            }
        }
    }
}

function updateCodeSetSelect(newData, codesetId) {
    var selectOptions = '';

    newData.forEach(function (code) {
        var languageIndicatorElement = document.querySelector('.language-indicator');
        var activelLanguage = languageIndicatorElement.textContent.trim();

        var desiredTranslation = code.thesaurus.translations.find(function (translation) {
            return translation.language === activelLanguage;
        });

        if (!desiredTranslation) {
            desiredTranslation = code.thesaurus.translations.find(function (translation) {
                return translation.language === 'en';
            });
        }
        if (desiredTranslation != undefined)
            selectOptions += '<option value="' + code.id + '">' + desiredTranslation.preferredTerm + '</option>';
    });

    var currentSelectedValue = $(`[data-codesetid="${codesetId}"]`).val();
    $(`[data-codesetid="${codesetId}"]`).html('<option value=""></option>' + selectOptions);
    if (currentSelectedValue != null || currentSelectedValue != "")
        $(`[data-codesetid="${codesetId}"]`).val(currentSelectedValue);
}

function destroyValidator() {
    var $form = $('form:has([data-date-input])');
    if ($form.length != 0)
        $form.validate().destroy();
}

const enter = 13;
const downArrow = 40;
const upArrow = 38;
const dictionary = new Map();

function getBinaryDomain(domainName) {
    return binaryDomains[domainName];
}

const binaryDomains = {
    'audio': 'audios',
    'file': 'files',
    'imageMap': 'imageMaps',
    'organizationLogo': 'organizationLogos',
};