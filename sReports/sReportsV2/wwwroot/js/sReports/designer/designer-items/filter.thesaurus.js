$(document).on('click', '#applySearchButton', function (e) {
    currentPage = 1;
    reloadTable();
});

$(document).on('click', '.edit-thesaurus-entry', function (e) {
    e.preventDefault();
    e.stopPropagation();

    $('.thesaurus-full-info-container').show();
    $('.thesaurus-active-value-container').hide();
    $('.thesaurus-title').hide();
});

$(document).on('keypress', function (e) {
    if (e.which === enter) {
        e.stopPropagation();
        currentPage = 1;
        reloadTable();
    }
});

function backToThesaurus(e) {
    e.preventDefault();
    e.stopPropagation();

    $('.thesaurus-full-info-container').hide();
    $('.thesaurus-active-value-container').show();
    $('.thesaurus-title').show();
}

$(document).on('click', 'input[name="radioThesaurus"]:checked', function (e) {
    let o4mtid = $(this).val();
    loadThesaurusData(o4mtid);

    $("#activeThesaurus").attr('data-value', o4mtid);
    $('#thesaurusLabelPlaceholder').addClass('hide');
    $('#thesaurusIdDiv').removeClass('hide');
    $('button.edit-thesaurus-button.edit-thesaurus-entry').removeClass('hide');
    setThesaurusDetailsContainer(o4mtid);
})

$(document).on('click', '.thesaurus-preview-container input[name="radioThesaurus"]:checked', function (e) {
    let o4mtid = $(this).val();
    loadThesaurusData(o4mtid);

    $("#activeThesaurus").attr('data-value', o4mtid);
    $('#thesaurusLabelPlaceholder').addClass('hide');
    $('#thesaurusIdDiv').removeClass('hide');
    $('button.edit-thesaurus-button.edit-thesaurus-entry').removeClass('hide');
    setThesaurusDetailsContainer(o4mtid);
})

function setThesaurusDetailsContainer(o4mtid) {
    $('#thesaurusId').val(o4mtid);
    $('#thesaurusIdDiv').text(o4mtid);
    
    $('#thesaurusWarning').addClass('d-none');
}

function removeSelectedThesaurus() {
    $('#thesaurusId').val('');
    $('#thesaurusIdDiv').text('');
    $("#activeThesaurus").attr('data-value', 0);
    loadThesaurusData(-1);
    setThesaurusDetailsContainer('');
}

function reloadTable() {
    
    let inputVal = $('#thesaurusSearchInput').val();
    let activeThesaurus = $('#thesaurusId').val();

    if (inputVal) {
        let requestObject = {};
        requestObject.PreferredTerm = inputVal;
        requestObject.ActiveThesaurus = activeThesaurus;
        setTableProperties(requestObject, { doOrdering: false });

        callServer({
            method: 'get',
            url: `/ThesaurusEntry/ReloadSearchTable`,
            data: requestObject,
            success: function (data) {
                $('#tableContainer').html(data);
                $('#tableContainer').removeClass('w-50');
                $('#reviewContainer').hide();
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }

}

function loadThesaurusData(thesaurusId) {
    if (thesaurusId) {
        callServer({
            method: 'get',
            url: `/ThesaurusEntry/ThesaurusProperties?o4mtid=${thesaurusId}`,
            success: function (data) {
                $('#activeThesaurusInfo').html(data);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

$(document).on('blur', ".item-title", function (e) {
    let term = $("#designerFormModalMainContent").find('.item-title').val();
    $("#designerFormModalMainContent").find('#thesaurusSearchInput').val(term);
    $('#applySearchButton').click();
    $("#designerFormModalMainContent").find('.thesaurus-full-info-container').show();
    $("#designerFormModalMainContent").find('.thesaurus-active-value-container').hide();
    $("#designerFormModalMainContent").find('.thesaurus-title').hide();
});

function showThesaurusReview(o4mtId, event) {
    $('.thesaurus-table-container').hide();
    $('#searchThesaurusTitle').hide();

    loadThesaurusPreview(o4mtId);
}

function closeThesaurusPreview(e) {
    e.preventDefault();
    e.stopPropagation();

    currentPage = 1;
    $('.review-container').hide();
    $('.thesaurus-table-container').show();
    $('#searchThesaurusTitle').show();
    reloadTable();
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
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}


