function sidebarSkosHasContent() {
    return $('#collapsethHierarchy .concept-item').length > 0;
}

function getSkosData() {
    var skos = {};
    skos['thesaurusId'] = $("#Id").val();
    skos["broaderThesaurusIds"] = getSelectedConcepts('#selectedBroaderConcepts');
    skos["narrowerThesaurusIds"] = getSelectedConcepts('#selectedNarrowerConcepts');
    skos["conceptSchemes"] = getSelectedConcepts('#selectedConceptSchemes');

    return skos;
}

function addNewExtension(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item).attr("data-value", value);
        $(item).text(translation);
        $(item).addClass('selected-concept display-selected-concept');
        let i = document.createElement('i');
        $(i).addClass('fas fa-times clinical-remove selected-autocomplete-remove');
        $(item).append(i);
        $("#selectedConceptSchemes").append(item);
    }
    $('#extensionOptions').hide();
}

function addNewBroaderConcept(exist, value, translation) {
    addNewConcept(exist, value, translation, false);
}

function addNewNarrowerConcept(exist, value, translation) {
    addNewConcept(exist, value, translation, true);
}

function addNewConcept(exist, value, translation, isNarrowConcept) {
    if (!exist) {
        let item = document.createElement('div');
        $(item).attr("data-value", value);
        $(item).text(translation);
        $(item).addClass('selected-concept display-selected-concept click-selected-concept');
        let i = document.createElement('i');
        $(i).addClass('fas fa-times clinical-remove selected-autocomplete-remove');
        $(item).append(i);
        $(isNarrowConcept ? "#selectedNarrowerConcepts" : "#selectedBroaderConcepts").append(item);
    }
    $(isNarrowConcept ? '#narrowerConceptOptions' : '#broaderConceptOptions').hide();
}

$(document).on('click', '.click-selected-concept', function (e) {
    e.preventDefault();
    window.location.href = `/ThesaurusEntry/Edit?thesaurusEntryId=${$(this).attr('data-value')}`;
});

$(document).on('click', '.concept-arrow-tree', function (e) {
    e.preventDefault();
    e.stopPropagation();
    let targetDiv = $(this).closest('.concept-item').siblings('div.collapse');
    if ($(this).hasClass("collapsed-arrow")) {
        $(targetDiv).removeClass('show');
        $(this).removeClass("collapsed-arrow");
    } else {
        $(targetDiv).addClass('show');
        $(this).addClass("collapsed-arrow");
    }
});

function getSelectedConcepts(containerId) {
    let result = [];
    $(containerId).find('.selected-concept').each(function (index, element) {
        result.push($(this).attr('data-value'));
    });

    return result;
}

function exportEntity(event, thesaurusEntryId) {
    event.preventDefault();
    callServer({
        url: `/ThesaurusEntry/ExportSkos?thesaurusEntryId=${thesaurusEntryId}`,
        success: function (data, status, xhr) {
            convertToBlobAndDownload(data, false, '', '', xhr.getResponseHeader('Original-File-Name'));
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}