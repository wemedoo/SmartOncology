var codeSetId = null;
var activeThesaurus = 0;
var thesaurusId;
var preferredTerm;
var isChangedCodeSetId = true;
var isInvalidThesaurus = false;
var isSelectThesaurus = false;
var isApplicableInDesignerChanged = false;

function thesaurusFilterModal(e, preferredTerm) {
    $('.designer-form-modal').addClass('show');
    if (preferredTerm != "" && preferredTerm != null)
        document.getElementById("thesaurusSearchInput").value = preferredTerm;
    else if ($('#codeValueDisplay').val() != null)
        document.getElementById("thesaurusSearchInput").value = $('#codeValueDisplay').val();
    activeThesaurus = $('#thesaurusIdCode').val() != "" && $('#thesaurusIdCode').val() != undefined ? $('#thesaurusIdCode').val() : 0;
    $('#applySearchButton').click();
}

function reloadThesaurusTable() {
    let inputVal = encodeURIComponent($('#thesaurusSearchInput').val());

    if (inputVal) {
        let requestObject = {};
        requestObject.Page = currentPage;
        requestObject.PageSize = getPageSize();
        requestObject.PreferredTerm = encodeURIComponent(inputVal);
        callServer({
            method: 'get',
            url: `/ThesaurusEntry/ReloadSearchTable?preferredTerm=${encodeURIComponent(inputVal)}&activeThesaurus=${activeThesaurus}`,
            data: requestObject,
            success: function (data) {
                $('#thesaurusTableContainer').html(data);
                $('#reviewContainer').hide();
                $('.codeset-thesaurus-group').show();
            },
            error: function () {

            }
        });
    }
}

function showThesaurusReview(o4mtId, event, preferredTerm) {
    var thesaurusSearchElement = document.getElementById("thesaurusSearchInput");
    thesaurusSearchElement.value = preferredTerm;
    $('.codeset-thesaurus-group').hide();
    $('.review-container').show();

    loadThesaurusPreview(o4mtId);
}

function loadThesaurusPreview(thesaurusId) {
    if (thesaurusId) {
        callServer({
            method: 'get',
            url: `/ThesaurusEntry/ThesaurusPreview?o4mtid=${thesaurusId}&activeThesaurus=${$('#activeThesaurus').attr('data-value')}`,
            success: function (data) {
                $('#reviewContainer').html(data);
                $('#reviewContainer').show();
            },
            error: function () {

            }
        });
    }
}

function closeThesaurusPreview(e) {
    e.preventDefault();
    e.stopPropagation();
    currentPage = 1;
    $('.review-container').hide();
    $('.codeset-thesaurus-group').show();
    reloadThesaurusTable();
}

$(document).on('blur', ".item-title", function (e) {
    let term = $("#designerFormModalMainContent").find('.item-title').val();
    $("#designerFormModalMainContent").find('#thesaurusSearchInput').val(term);
    $('#applySearchButton').click();
    $("#designerFormModalMainContent").find('.thesaurus-full-info-container').show();
});

$(document).on('click', '#applySearchButton', function (e) {
    currentPage = 1;
    isThesaurusFilterTable = true;
    if (document.getElementById("pageSizeSelector") != null)
        document.getElementById("pageSizeSelector").id = "pageSizeThesaurusSelector";
    reloadThesaurusTable();
});

$(document).on('click', '.search-thesaurus-table input[name="radioThesaurus"]:checked', function (e) {
    isInvalidThesaurus = false;
    isInvalidCodeThesaurus = false;
    isSelectThesaurus = true;
    let o4mtid = $(this).val();
    thesaurusId = o4mtid;
    preferredTerm = $(this).data('preferredterm');
    activeThesaurus = thesaurusId;
    $('#thesaurusIdCode').val(o4mtid);

    $(this).addClass('hide');
    $(this).siblings('.select-button').removeClass('hide');
    $("#activeThesaurus").attr('data-value', o4mtid);
    document.getElementById("thesaurusSearchInput").setAttribute("value", "");
    if (document.getElementById("codeThesaurusLink") != null)
        document.getElementById("codeThesaurusLink").src = "/css/img/icons/thesaurus_green.svg";
    $("#thesaurusFilterModalForm").validate().element("#thesaurusSearchInput");
    submitCodeSetEntry(e, false);
})

$(document).on('click', '.preview-thesaurus-button input[name="radioThesaurus"]:checked', function (e) {
    isInvalidThesaurus = false;
    isInvalidCodeThesaurus = false;
    isSelectThesaurus = true;
    let o4mtid = $(this).val();
    thesaurusId = o4mtid;
    preferredTerm = $(this).data('preferredterm');
    $('#thesaurusIdCode').val(o4mtid);
    activeThesaurus = thesaurusId;
    $(this).addClass('hide');
    $("#activeThesaurus").attr('data-value', o4mtid);
    document.getElementById("thesaurusSearchInput").setAttribute("value", "");
    if (document.getElementById("codeThesaurusLink") != null)
        document.getElementById("codeThesaurusLink").src = "/css/img/icons/thesaurus_green.svg";
    $("#thesaurusFilterModalForm").validate().element("#thesaurusSearchInput");
    submitCodeSetEntry(e, false);
})

function submitCodeSetForm(event, isPopulate, preferredTerm) {
    event.preventDefault();
    var request = {};
    if ($('#codeSetNumberForCode').val() != null) {
        if (isPopulate)
            populateCodeSetName(thesaurusId, preferredTerm);
        else {
            if (!codeSetExists()) {
                if (($("#newCodeSetNumberForCode").val() != $("#codeSetNumberForCode").val() || $('#codeThesaurusId').val() != thesaurusId || isValidFromOrToChanged || isApplicableInDesignerChanged)
                    && thesaurusId != undefined) {
                    isChangedCodeSetId = false;
                    createCodeSetFromCode(request, thesaurusId);
                    $('#codeThesaurusId').val(thesaurusId);
                    saveInitialFormData("#codeSetsForm");
                }
            }
        }
    }
    else {
        $('#thesaurusFilterModalForm').validate();
        if ($('#thesaurusFilterModalForm').valid()) {
            request['ThesaurusEntryId'] = thesaurusId;
            if (codeSetId != null)
                request['CodeSetId'] = codeSetId;

            request['ActiveFrom'] = calculateDateWithOffset($("#activeFromDate").val());
            request['ActiveTo'] = calculateDateWithOffset($("#activeToDate").val());
            request['ApplicableInDesigner'] = $('#applicableInDesigner-yes').is(':checked'); 

            callServer({
                type: "POST",
                url: "/CodeSet/Create",
                data: request,
                success: function (data) {
                    toastr.options = {
                        timeOut: 100
                    }
                    toastr.options.onHidden = function () { window.location.href = `/CodeSet/GetAll`; }
                    toastr.success("Success");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }
}

$(document).on('click', '.close-designer-form-modal-button', function (e) {
    closeThesaurusFilterFormModal();
});

function closeThesaurusFilterFormModal(reloadFormPreview) {
    $('.designer-form-modal').removeClass('show');
    $('body').removeClass('no-scrollable');
    resetCodeSetModal();

    if (reloadFormPreview) {
        reloadFormPreviewPartial();
    }
}

function resetCodeSetModal() {
    resetValidation($("#thesaurusFilterModalForm"));
    resetThesaurusContent();
    $('.review-container').hide();
    $('.codeset-thesaurus-group').show();
    $('#codeSetIdInput').val('');
    $('#thesaurusSearchInput').val('');
    $('#activeFromDate').val('');
    $('#activeToDate').val('');
}

function editEntry(e, preferredTerm, id, selectedThesaurusId) {
    var thesaurusSearchElement = document.getElementById("thesaurusSearchInput");
    thesaurusSearchElement.value = preferredTerm;
    codeSetId = id;
    if (thesaurusId !== undefined)
        activeThesaurus = thesaurusId;
    else if (thesaurusId == null) {
        thesaurusId = selectedThesaurusId;
        activeThesaurus = selectedThesaurusId;
    }
    else
        activeThesaurus = selectedThesaurusId;
    $('#applySearchButton').click();
    $('#thesaurusFilterModal').addClass('show');
}

function openCodeValues(e, codeSetId, codeSetDisplay) {
    codeSetDisplay = encodeURIComponent(codeSetDisplay);
    const targetClasses = ['dropdown-button', 'fa-bars', 'dropdown-item', 'dots', 'table-more'];
    const isDropdownRelated = targetClasses.some(cls => $(e.target).hasClass(cls));

    if (!isDropdownRelated || $(e.target).hasClass('editCodeSet')) {
        window.location.href = `/Code/GetAll?CodeSetId=${codeSetId}&CodeSetDisplay=${codeSetDisplay}`;
    }
}

function viewCodeValues(e, codeSetId, codeSetDisplay) {
    codeSetDisplay = encodeURIComponent(codeSetDisplay);
    window.location.href = `/Code/ViewCodes?CodeSetId=${codeSetId}&CodeSetDisplay=${encodeURIComponent(codeSetDisplay)}`;
}

$(document).on('click', '.search-button', function (e) {
    editEntry(e, $('#thesaurusSearchInputCode').val(), $('#codeSetNumberForCode').val(), $('#codeThesaurusId').val())
});

function populateCodeSetName(thesaurusId, preferredTerm) {
    $('.designer-form-modal').removeClass('show');
    $('body').removeClass('no-scrollable');
    document.getElementById("thesaurusSearchInputCode").value = preferredTerm;
    $("#thesaurusSearchInputCode").attr('data-value', thesaurusId);
    if ($('#codeSetsForm').valid()) {
        console.log('The form is valid');
    }
}

$(document).on('keyup', '#thesaurusSearchInputCode', function (e) {
    if (e.which !== enter)
        isInvalidThesaurus = true;
});

function invalidThesaurus() {
    return isInvalidThesaurus;
}

function createCodeSetFromCode(request, thesaurusId) {
    $('#codeSetsForm').validate();
    if ($('#codeSetsForm').valid()) {
        if ($("#thesaurusSearchInputCode").data("value") != null)
            thesaurusId = $("#thesaurusSearchInputCode").data("value");

        request['ThesaurusEntryId'] = thesaurusId;
        request['NewCodeSetId'] = $('#newCodeSetNumberForCode').val();
        request['CodeSetId'] = $('#codeSetNumberForCode').val();
        request['ActiveFrom'] = calculateDateWithOffset($("#newCodeSetActiveFromForCode").val());
        request['ActiveTo'] = calculateDateWithOffset($("#newCodeSetActiveToForCode").val());
        request['ApplicableInDesigner'] = $('#applicableInDesigner-yes').is(':checked'); 

        if (!isInvalidThesaurus) {
            callServer({
                type: "POST",
                url: "/Code/CreateCodeSet",
                data: request,
                success: function (data) {
                    toastr.options = {
                        timeOut: 100
                    }
                    $("#thesaurusSearchInputCode").attr('data-value', thesaurusId);
                    pushState();
                    toastr.success("Success");
                    document.getElementById('codeSetNumberForCode').value = $('#newCodeSetNumberForCode').val();
                    document.getElementById("thesaurusSearchInputCode").setAttribute("value", $("#thesaurusSearchInputCode").val());
                    isChangedCodeSetId = true;
                    isApplicableInDesignerChanged = false;
                    reloadNominatorTable();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr);
                }
            });
        }
    }
}

function submitCodeSetEntry(event, validateCodeSetId) {
    if (document.getElementById("codeValueImg") != null)
        populateCodeValueName(thesaurusId, preferredTerm);
    else {
        var element = document.getElementById("codeSetIdInput");
        if (element != null) {
            if (validateCodeSetId) {
                $('#thesaurusFilterModalForm').validate();
                if ($('#thesaurusFilterModalForm').valid()) {
                    codeSetId = $('#codeSetIdInput').val();
                    submitCodeSetForm(event, true, preferredTerm);
                }
            }
        }
        else
            submitCodeSetForm(event, true, preferredTerm);
    }
}

$(document).ready(function () {
    destroyValidator();
});

function isInvalidThesaurusEntry() {
    return thesaurusId === null && !isSelectThesaurus && document.getElementById("thesaurusSearchInputCode") === null;
}

function codeSetExistsThesaurusModal() {
    let result = false;
    callServer({
        type: 'GET',
        url: `/CodeSet/ExistCodeSetId?codeSetId=${$('#codeSetIdInput').val()}`,
        async: false,
        success: function (data) {
            result = data;
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });

    return result;
}

function resetThesaurusContent() {
    $('.search-thesaurus-table tbody').empty();
    $('.pager-container').remove();
    $("#activeThesaurus").attr('data-value', 0);
    thesaurusId = null;
    activeThesaurus = null;
    isSelectThesaurus = false;
}

$(document).on("change click",'input[type=radio][name="applicableInDesigner"]', function () {
    isApplicableInDesignerChanged = true;
    if ($("#codeValueDisplay").data("value") != undefined)
        thesaurusId = $("#codeValueDisplay").data("value");
    else
        thesaurusId = $('#codeThesaurusId').val();
});