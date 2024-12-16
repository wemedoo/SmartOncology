function createOrganizationEntry(){
    window.location.href = "/Organization/Create";
}

function editEntity(event,id) {
    window.location.href = `/Organization/Edit?organizationId=${id}`;
    event.preventDefault();
}

function viewEntity(event, id) {
    window.location.href = `/Organization/View?organizationId=${id}`;
    event.preventDefault();
}

function removeOrganizationEntry(event, id, rowVersion) {
    event.preventDefault();
    event.stopPropagation();
    let data = {
        id: id,
        rowVersion: rowVersion
    };
    callServer({
        type: "DELETE",
        url: `/Organization/Delete`,
        data: data,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function reloadTable() {
    hideAdvancedFilterModal();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setFilterTagsFromObj(requestObject);
    setAdvancedFilterBtnStyle(requestObject, ['Name', 'ClinicalDomainCD', 'Type', 'Page', 'PageSize']);
    checkUrlPageParams();
    setTableProperties(requestObject);
    requestObject.ClinicalDomainCD = $('#organizationClinicalDomain').find(':selected').attr('id');

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    callServer({
        type: 'GET',
        url: '/Organization/ReloadTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getFilterParametersObject() {
    let result = {};
    var name = $("#organizationName").val();
    var type = $("#organizationType").val();
    var clinicalDomainCD = $("#organizationClinicalDomain").val();
    var alias = $("#alias").val();
    var identifierType = $("#identifierType").val();
    var identifierValue = $("#identifierValue").val();
    var state = $("#state").val();
    var countryCD = $("#countryCD").val();
    var postalCode = $("#postalCode").val();
    var street = $("#street").val();
    var city = $("#city").val();
    var parentId = $("#parent").val();
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'Name', name);
        addPropertyToObject(result, 'City', city);
        addPropertyToObject(result, 'Type', type);
        addPropertyToObject(result, 'Alias', alias);
        addPropertyToObject(result, 'IdentifierType', identifierType);
        addPropertyToObject(result, 'IdentifierValue', identifierValue);
        addPropertyToObject(result, 'State', state);
        addPropertyToObject(result, 'CountryCD', countryCD);
        addPropertyToObject(result, 'PostalCode', postalCode);
        addPropertyToObject(result, 'Street', street);
        addPropertyToObject(result, 'ClinicalDomainCD', clinicalDomainCD);
        addPropertyToObject(result, 'Parent', parentId);
    }

    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    getFilterParameterObjectForDisplay(filterObject, 'IdentifierType');
    getFilterParameterObjectForDisplay(filterObject, 'Type');

    if (filterObject.hasOwnProperty('Parent')) {
        let parentDisplay = getSelectedSelect2Label("parent");
        if (parentDisplay) {
            addPropertyToObject(filterObject, 'Parent', parentDisplay);
        }
    }

    if (filterObject.hasOwnProperty('CountryCD')) {
        let countryNameByHidden = $('#countryName').val();
        if (countryNameByHidden) {
            addPropertyToObject(filterObject, 'CountryCD', countryNameByHidden);
        }
        let countryNameBySelect2 = getSelectedSelect2Label("countryCD");
        if (countryNameBySelect2) {
            addPropertyToObject(filterObject, 'CountryCD', countryNameBySelect2);
        }
    }
   
    return filterObject;
}

function mainFilter() {
    $('#name').val($('#organizationName').val());
    $('#type').val($('#organizationType').val());
    $('#clinicalDomain').val($('#organizationClinicalDomain').val());

    filterData();
    //clearFilters();
}

function advanceFilter() {
    $('#organizationName').val($('#name').val());
    $('#organizationType').val($('#type').val());
    $('#organizationClinicalDomain').val($('#clinicalDomain').val());

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#name').val('');
    $('#city').val('');
    $('#checkBoxGroup').val('');
    $('#clinicalDomain').val('');
    $('#alias').val('');
    $('#identifierType').val('');
    $('#identifierValue').val('');
    $('#state').val('');
    $('#country').val('');
    $('#postalCode').val('');
    $('#street').val('');
    $('#parentId').val('');
}