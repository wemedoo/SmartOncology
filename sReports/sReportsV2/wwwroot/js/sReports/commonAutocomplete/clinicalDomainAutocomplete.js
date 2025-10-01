
function addNewClinicalDomain(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item).attr("data-value", value);
        $(item).text(translation);
        $(item).addClass('clinical');
        let i = document.createElement('i');
        $(i).addClass('fas fa-times clinical-remove selected-autocomplete-remove');
        $(item).append(i);
        $("#clinicals").append(item);
    }
    $('#clinicalOptions').hide();
}


function setClinicalDomain(clinicalDomainObjectCallback) {
    let result = [];
    $("#clinicals").find('.clinical').each(function (index, element) {
        result.push(clinicalDomainObjectCallback(element));
    });

    return result;
}

function getSimpleClinicalDomainObject() {
    return function (element) {
        return $(element).attr("data-value");
    }
}

function getOrganizationClinicalDomainObject() {
    return function (element) {
        return {
            clinicalDomainCD: $(element).attr("data-value"),
            organizationClinicalDomainId: $(element).attr("data-id")
        };

    }
}