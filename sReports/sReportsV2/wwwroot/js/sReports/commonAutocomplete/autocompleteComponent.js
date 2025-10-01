$(document).on('keyup', '.autocomplete-component', function (e) {
    if (isInputCharacter(e.which)) {
        let autocompleteInput = $(this);
        let autocompleteInputId = $(autocompleteInput).attr('id');
        if (autocompleteInputId == 'patientSearch') {
            searchForPatient();
        } else {
            let searchValue = $(autocompleteInput).val();
            if (autocompleteInputId == 'searchExtensionByName') {
                callServer({
                    method: 'post',
                    data: { name: searchValue, allConceptSchemes: conceptSchemes },
                    url: getAutocompleteEndpoint(autocompleteInputId),
                    success: function (data) {
                        showSearchOptions(autocompleteInput, data);
                    },
                    error: function (xhr, textStatus, thrownError) {
                        handleResponseError(xhr);
                    }
                });
            } else if (searchValue.length > 2) {
                callServer({
                    method: 'get',
                    data: { name: searchValue },
                    url: getAutocompleteEndpoint(autocompleteInputId),
                    success: function (data) {
                        showSearchOptions(autocompleteInput, data);
                    },
                    error: function (xhr, textStatus, thrownError) {
                        handleResponseError(xhr);
                    }
                });
            }
        }
    }
});

function showSearchOptions(autocompleteInput, data) {
    let optionsContainer = $(autocompleteInput).closest('.autocomplete-wrapper').siblings('.autocomplete-options');
    $(optionsContainer).html(data);
    $(optionsContainer).show();
}

function getAutocompleteEndpoint(inputId) {
    switch (inputId) {
        case 'clinicalDomain':
            return '/Form/ReloadClinicalDomain';
        case 'searchNarrowerConceptByName':
            return '/ThesaurusEntry/GetNarrowerConcepts';
        case 'searchBroaderConceptByName':
            return '/ThesaurusEntry/GetBroaderConcepts';
        case 'searchExtensionByName':
            return '/ThesaurusEntry/GetExtensions';
        case 'searchOrganizationByName':
            return '/Organization/GetOrganizationValues';
        default:
            return '';
    }
}

$(document).on('keydown', '.autocomplete-wrapper', function (e) {
    let next;
    if (e.which === downArrow) {
        if (liSelected) {
            $(liSelected).removeClass('selected');
            next = $(liSelected).next();
            if (next.length > 0) {
                liSelected = $(next).addClass('selected');
            } else {
                liSelected = $('.option').eq(0).addClass('selected');
            }
        } else {
            liSelected = $('.option').eq(0).addClass('selected');
        }
    } else if (e.which === upArrow) {
        if (liSelected) {
            $(liSelected).removeClass('selected');
            next = $(liSelected).prev();
            if (next.length > 0) {
                liSelected = $(next).addClass('selected');
            } else {
                liSelected = $('.option').last().addClass('selected');
            }
        } else {
            liSelected = $('.option').last().addClass('selected');
        }
    }
    else if (e.which === enter) {
        e.preventDefault();
        $(liSelected).click();
    }

    e.stopImmediatePropagation();
});

$(document).on("click", '.main-content', function (e) {
    if (!$(e.currentTarget).hasClass('dropdown-search') || $(e.currentTarget).closest('dropdown-search').length == 0) {
        $(".autocomplete-options").hide();
    }
});

$(document).on("click", '.sidebar-shrink', function (e) {
    let target = e.target;
    let isAutocompleteComponent = $(target).hasClass('autocomplete-component');
    if (!$(target).hasClass('option') && !isAutocompleteComponent) {
        $(".autocomplete-options").hide();
    }
});

$(document).on("click", '.selected-autocomplete-remove', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    $(this).parent().remove();
});

function optionClicked(e, value, translation, componentName) {
    let componentParams = getOptionClickedParams(componentName);
    
    let exist = false;
    $(`#${componentParams.optionsContainerId}`).find('div').each(function (index, element) {
        if ($(element).attr("data-value") == value) {
            exist = true;
        }
    });

    componentParams.functionName(exist, value, decodeLocalizedString(translation));
    $(`#${componentParams.inputId}`).val('');
}

function getOptionClickedParams(componentName) {
    switch (componentName) {
        case 'clinicalDomain':
            componentParams = {
                optionsContainerId: 'clinicals',
                inputId: 'clinicalDomain',
                functionName: addNewClinicalDomain
            };
            break;
        case 'organization':
            componentParams = {
                optionsContainerId: 'selectedOrganizations',
                inputId: 'searchOrganizationByName',
                functionName: addNewFormOrg
            };
            break;
        case 'narrowerConcept':
            componentParams = {
                optionsContainerId: 'selectedNarrowerConcepts',
                inputId: 'searchNarrowerConceptByName',
                functionName: addNewNarrowerConcept
            };
            break;
        case 'broaderConcept':
            componentParams = {
                optionsContainerId: 'selectedBroaderConcepts',
                inputId: 'searchBroaderConceptByName',
                functionName: addNewBroaderConcept
            };
            break;
        case 'extension':
            componentParams = {
                optionsContainerId: 'selectedConceptSchemes',
                inputId: 'searchExtensionByName',
                functionName: addNewExtension
            };
            break;
        default:
            componentParams = null;
            break;
    }
    return componentParams;
}

var li = $('.option');
var liSelected = null;