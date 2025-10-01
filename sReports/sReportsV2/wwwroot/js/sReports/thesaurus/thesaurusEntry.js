var closestRow;
var thesaurusPageNum = 0;

addUnsavedChangesEventHandler("#thesaurusEntryForm");

$(document).ready(function () {
    let isEdit = $('#Id').val();
    if (isEdit) {
        loadThesaurusTree();
    }

    if (sidebarSkosHasContent()) {
        $('#thHierarchy').trigger('click');
    }
    saveInitialFormData("#thesaurusEntryForm");
    setValidationCodeFunctions();
});

function reloadTable() {
    let requestObject = applyActionsBeforeServerReload(['Id', 'StateCD', 'PreferredTerm', 'page', 'pageSize'], true);

    callServer({
        type: 'GET',
        url: '/ThesaurusEntry/ReloadTable',
        data: requestObject,
        success: function (data) {
            setTableContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).on('click', '#startMergeBtn', function (e) {
    e.preventDefault();
    callServer({
        type: 'GET',
        url: '/ThesaurusEntry/MergeThesaurusOccurences',
        success: function (data) {
            toastr.success(data, '', {
                timeOut : "5000",
                extendedTimeOut : "4000",
                closeButton : true
            });
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
});

function loadDocumentProperties(id) {
    if (id != "") {
        callServer({
            type: 'GET',
            url: `/Form/GetDocumentProperties?id=${id}`,
            success: function (data) {
                $('#documentPropertiesData').html(data);
                $("#collapseDocumentProperties").addClass("show");
                let args = getThesaurusSelectors("documentProperties");
                let codeElement = $(args.codeElement);
                resetDocumentArrow(codeElement);
                checkThesaurusHeader(codeElement, $(args.element));
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    else
    {
        $('#documentPropertiesData').html("");
        $("#collapseDocumentProperties").removeClass("show");
        resetDocumentArrow($("#documentArrow"));
    }
}

function createThesaurusEntry() {
    window.location.href = "/ThesaurusEntry/Create";
}

function editThesaurusEntry(id) {
    window.location.href = `/ThesaurusEntry/Edit?thesaurusEntryId=${id}`;
}

function editEntity(event, id) {
    editThesaurusEntry(id);
    event.preventDefault();
}

function viewEntity(event, id) {
    window.location.href = `/ThesaurusEntry/View?thesaurusEntryId=${id}`;
    event.preventDefault();
}

$(document).on("keypress", ".tag-input", function (e) {
    if (e.which === enter) {
        let isAppended = appendTagToContainer($(this).val().trim(), $(e.currentTarget).data("tag"), $(e.currentTarget).data("language"));
        if (isAppended) {
            $(this).val('');
        }
        return false;
    }
});

$(document).on('click', "*[data-action='remove-tag']", function (e) {
    let id = $(e.currentTarget).data("id");
    let tagType = $(e.currentTarget).data("tag");
    let language = $(e.currentTarget).data("language");
    $(`#tag-${id}-${tagType}-${language}`).remove();
});

$(document).on('click', '.plus-button-synonym', function (e) {
    let tagType = $(e.currentTarget).data("tag");
    let input = $(e.currentTarget).siblings(`input[data-tag=${tagType}]`)[0];
    let inputValue = $(input).val().trim();
    let isAppendend = appendTagToContainer(inputValue, tagType, $(e.currentTarget).data("language"));
    if (isAppendend) {
        $(input).val('');
    }
    return false;
});

function saveAndCreateNew(event) {
    saveInitialFormData("#thesaurusEntryForm");
    event.preventDefault();
    event.stopPropagation();

    submitThesaurusEntryForm(function () {
        createThesaurusEntry();
    });
}

function submitThesaurusEntryForm(callback) {
    var form = $('#thesaurusEntryForm');
    $(form).validate();
    if ($(form).valid()) {
        let data = {};
        let thesaurusId = $("#Id").val();
        let action = thesaurusId != 0 ? 'Edit' : 'Create';
        data['id'] = thesaurusId;
        data['stateCD'] = $('#thesaurusState').val();
        data['translations'] = getFormTranslations($(form).serializeArray());
        data['parentId'] = $('#parentId').val();
        data['umlsDefinitions'] = $('#UmlsDefinitions').html();
        data['umlsName'] = $('#UmlsName').val(); 
        data['umlsCode'] = $('#UmlsCode').val();
        data['codes'] = getCodes();
        data['skosData'] = getSkosData();

        callServer({
            type: "POST",
            url: `/ThesaurusEntry/${action}`,
            data: data,
            success: function (data) {
                toastr.options = {
                    timeOut: 100
                }
                toastr.success("Success");

                if (callback) {
                    callback();
                    return;
                }
                saveInitialFormData("#thesaurusEntryForm");
                editThesaurusEntry(data.id);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    return false;
}

function getCodes() {
    let result = [];
    $('#codeTable').find('tr').each(function (index, element) {
        let codeId = $(element).find('[data-property="codeId"]')[0];
        let codeSystem = $(element).find('[data-property="codeSystem"]')[0];
        let codeVersion = $(element).find('[data-property="codeVersion"]')[0];
        let codeCode = $(element).find('[data-property="codeCode"]')[0];
        let codeValue = $(element).find('[data-property="codeValue"]')[0];
        let codeVersionPublishDate = $(element).find('[data-property="codeVersionPublishDate"]')[0];

        if ($(codeSystem).data('value') && $(codeCode).data('value') && $(codeValue).data('value')) {
            result.push({
                Id: $(codeId).data("value"),
                CodeSystemId: $(codeSystem).data('value'),
                Version: $(codeVersion).data('value'),
                Code: $(codeCode).data('value'),
                Value: $(codeValue).data('value'),
                CodeSystemAbbr: $(codeSystem).data('system'),
                VersionPublishDate: toDateStringIfValue($(codeVersionPublishDate).data('value'))
            });
        }
    });
    return result;
}

function getFormTranslations(data) {
    let result = [];
    languages.forEach(language => {
        let object = {};
        object['language'] = language.value;
        object[`definition`] = data.find(x => x.name === `definition-${language.value}`).value;
        object[`preferredTerm`] = encodeURIComponent(data.find(x => x.name === `preferredTerm-${language.value}`).value);
        object[`synonyms`] = getTagValue('synonym', language.value);
        object[`abbreviations`] = getTagValue('abbreviation', language.value);
        result.push(object);
    });
    return result;
}

function getTagValue(tagType, language) {
    let result = [];
    $(`*[data-info=tag-value-${tagType}-${language}]`).each(function (index, value) {
        result.push($(this).html());
    });
    return result;
}

function appendTagToContainer(value, tagType, language) {
    if (!value || existsTagValue(value, tagType, language)) {
        return false;
    }
    $(`#${tagType}-values-${language}`).append(createSingleTag(value, tagType, language));
    return true;
}

function createSingleTag(value, tagType, language) {
    var id = value.replace(/\W/g, "-").toLowerCase();
    var removeIcon = getNewRemoveIcon(id, tagType, language);
   
    var element = getNewSingleTagContainer(id, tagType, language);
    $(element).append(getNewSingleTagValue(tagType, language, value));
    $(element).append(removeIcon);

    return element;
}

function getNewRemoveIcon(id, tagType, language) {
    var removeIcon = document.createElement('img');
    $(removeIcon).addClass('ml-2');
    $(removeIcon).addClass('tag-value');
    $(removeIcon).addClass('tag-value-synonym');
    $(removeIcon).attr("src", "/css/img/icons/Administration_remove.svg");
    $(removeIcon).attr("data-id", id);
    $(removeIcon).attr('data-action', `remove-tag`);
    $(removeIcon).attr('data-tag', tagType);
    $(removeIcon).attr('data-language', language);
    return removeIcon;
}

function getNewSingleTagValue(tagType, language,value) {
    var text = document.createElement('span');
    $(text).addClass('single-tag-value');
    $(text).attr('data-info', `tag-value-${tagType}-${language}`);
    $(text).html(value);

    return text;
}

function getNewSingleTagContainer(id, tagType, language) {
    var element = document.createElement('div');
    $(element).attr("id", `tag-${id}-${tagType}-${language}`);
    $(element).addClass('filter-element');
    $(element).addClass('synonyms-element');

    return element;
}

function existsCodeValue(element, selector) {
    return $(element + `:contains(${selector})`).length > 0;
}

function existsTagValue(value, tagType, language) {
    let id = value.replace(/\W/g, "-").toLowerCase();
    return $(`#${tagType}-values-${language}`).find(`#tag-${id}-${tagType}-${language}`).length > 0;
}

function getNewTranslationDOM(language, value) {
    let languageElement = document.createElement('span');
    $(languageElement).addClass('language');
    $(languageElement).html(language);

    let valueElement = document.createElement('span');
    $(valueElement).addClass('value');
    $(valueElement).html(value);

    let translationElement = document.createElement('div');
    $(translationElement).addClass('single-translation');
    $(translationElement).append(languageElement);
    $(translationElement).append(valueElement);

    return translationElement;
}

function selectParent(e, language) {
    if ($(e.srcElement).hasClass('active')) {
        $(e.srcElement).removeClass('active');
        $('#treeStructure').html('');
        $('#parentId').val('');
    } else {
        $(e.srcElement).siblings().removeClass('active');
        $(e.srcElement).addClass('active');
        loadParent($(e.srcElement).data("id"), language);
        $('#parentId').val($(e.srcElement).data("id"));

    }
}

function backToList() {
    unsavedChangesCheck("#thesaurusEntryForm",
        function () {
            window.location.href = '/ThesaurusEntry/GetAll';
        },
        function () {
            window.location.href = '/ThesaurusEntry/GetAll';
        }
    )
}

function loadParent(parentId, language) {
    callServer({
        type: "GET",
        url: `/ThesaurusEntry/GetTreeById?thesaurusEntryId=${parentId}`,
        success: function (data) {
            let thesaurusEntry = getThesaurusEntryFromForm(language);
            $('#treeStructure').html(getTreeStructureDOM(data, createTreeElementWithContent(thesaurusEntry, true)));
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getThesaurusEntryFromForm(language) {
    let result = null;
    if ($('#Id').val()) {
        result = {
            O40MTId: $('#O40MTID').val(),
            Id: $('#Id').val(),
            Definition: $(`#definition-${language}`).val() ? $(`#definition-${language}`).val() : $(`#definition-en`).val()
        };
    }
    return result;
}

function getTreeStructureDOM(data, child) {
    let element = createTreeElementWithContent(data, false, child);
    let structure = element;
    if (data.parent) {
        structure = getTreeStructureDOM(data.parent, element);
    }
    return structure;
}

function createTreeElementWithContent(data, current, child = null) {
    let divText = document.createElement('div');
    let definitionText = data?.definition || '';
    let textContent = data ? `${data.O40MTId}[${definitionText}]` : 'Current Entry';

    $(divText).html(textContent);
    $(divText).addClass('tree-item-value');
    if (current) {
        $(divText).addClass('current');
    }

    let element = document.createElement('div');

    $(element).addClass('tree-node');
    if (data) {
        $(element).attr('id', data.id);
    }
    $(element).append(divText);

    if (child) {
        $(element).append(child);
    }

    return element;
}

function removeThesaurusEntry(event, id) {
    event.preventDefault();
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/ThesaurusEntry/Delete?thesaurusEntryId=${id}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getFilterParametersObject() {
    let requestObject = {};
    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Id', $('#O40MtIdTemp').val());
        addPropertyToObject(requestObject, 'PreferredTerm', $('#PreferredTermTemp').val());
        addPropertyToObject(requestObject, 'Synonym', $('#synonym').val());
        addPropertyToObject(requestObject, 'Abbreviation', $('#abbreviation').val());
        addPropertyToObject(requestObject, 'UmlsCode', $('#umlsCode').val());
        addPropertyToObject(requestObject, 'UmlsName', $('#umlsName').val());
        addPropertyToObject(requestObject, 'StateCD', $('#StateTemp').val());

    }

    return requestObject;
}

function getFilterParametersObjectForDisplay(filterObject) {
    getFilterParameterObjectForDisplay(filterObject, 'StateCD');
    return filterObject;
}

$(document).on('change', '#selectedLanguage', function () {
    $(`#myTabContent #${$(this).val()}`)
        .addClass('show active')
        .siblings()
        .removeClass('show active');
});

function showUmlsModal(event) {
    event.stopPropagation();
    resetUmlsModal();
    $('#umlsModal').modal('show');
}

function resetUmlsModal() {
    if ($("#umlsTerm").val()) {
        $("#umlsTerm").val('');
        searchByTerm(true);
    }
}

function showCodeModal(event) {
    event.stopPropagation();

    resetCodeForm();
    closestRow = document.createElement('div');
    showCodeModalTitle("addCodeTitle");
    $('#codeModal').modal('show');
}

function showCodeModalTitle(titleId) {
    $(".code-form-title").hide();
    $(`#${titleId}`).show();
}

function showAdministrativeModal(event) {
    event.stopPropagation();
    $('#administrativeModal').modal('show');
}

function resetCodeForm() {
    resetCodeValues();
    resetValidation($('#newCodeForm'));
}

function resetCodeValues() {
    $('#codeSystem').val('');
    $('#codeVersion').val('');
    $('#codeVersionPublishDate').val('');
    $('#codeCode').val('');
    $('#codeValue').val('');
    $('#codeId').val('');
}

function addNewCode(e) {
    e.preventDefault();
    e.stopPropagation();

    if ($('#newCodeForm').valid()) {
        let codingObject = getNewCode();
        let thesaurusEntryId = $("#Id").val();

        if (thesaurusEntryId) {
            callServer({
                type: "post",
                data: codingObject,
                url: `/ThesaurusEntry/CreateCode?thesaurusEntryId=${thesaurusEntryId}`,
                success: function (data, textStatus, jqXHR) {
                    if (codingObject["id"]) {
                        closestRow.replaceWith(data);
                    } else {
                        $('#codeTable tbody').append(data);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr);
                }
            });
        } else {
            var codeRow = addNewCodeRowToTable(
                codingObject.codeSystemId,
                codingObject.codeSystem,
                codingObject.codeSystem,
                codingObject.version,
                codingObject.code,
                codingObject.value,
                codingObject.versionPublishDate
            );
            $('#codeTable tbody').append(codeRow);
            submitThesaurusEntryForm();
        }
        $('#codeModal').modal('hide');
    }
}

function getNewCode() {
    return {
        "id" : $('#codeId').val(),
        "codeSystemId": $('#codeSystem').val(),
        "codeSystem": getSelectedOptionLabel('codeSystem'),
        "version" : $('#codeVersion').val(),
        "code" : $('#codeCode').val(),
        "value": $('#codeValue').val(),
        "versionPublishDate": toDateStringIfValue($('#codeVersionPublishDate').val())
    }
}

function addNewCodeRowToTable(codeSystemId, codeSystem, codeSystemDisplay, codeVersion, codeCode, codeValue, codeVersionPublishDate) {
    let system = document.createElement('td');
    $(system).attr("data-property", 'codeSystem');
    $(system).attr("data-value", codeSystemId);
    $(system).attr("data-system", codeSystem);
    $(system).html(codeSystemDisplay);
    $(system).addClass("custom-td-first");

    let version = document.createElement('td');
    $(version).attr("data-property", 'codeVersion');
    $(version).attr("data-value", codeVersion);
    $(version).html(codeVersion);
    $(version).addClass("custom-td");

    let code = document.createElement('td');
    $(code).attr("data-property", 'codeCode');
    $(code).attr("data-value", codeCode);
    $(code).html(codeCode);
    $(code).addClass("custom-td");

    let value = document.createElement('td');
    $(value).attr("data-property", 'codeValue');
    $(value).attr("data-value", codeValue);
    $(value).html(codeValue);
    $(value).addClass("custom-td");

    let versionPublishDate = document.createElement('td');
    $(versionPublishDate).attr("data-property", 'codeVersionPublishDate');

    if (codeVersionPublishDate) {
        $(versionPublishDate).attr("data-value", codeVersionPublishDate);
        let formatted_date = $(versionPublishDate).attr('data-value');
        $(versionPublishDate).html(formatted_date);
    } else {
        $(versionPublishDate).attr("data-value", "");
        $(versionPublishDate).html("");
    }

    $(versionPublishDate).addClass("custom-td");

    let del = document.createElement('a');
    $(del).addClass("dropdown-item delete-code");
    $(del).attr("href", '#');
    $(del).append(appendDeleteIcon()).append(deleteItem);

    let edit = document.createElement('a');
    $(edit).addClass("dropdown-item edit-code");
    $(edit).attr("href", '#');
    $(edit).append(appendEditIcon()).append(editItem);

    let dropDownMenu = document.createElement('div');
    $(dropDownMenu).addClass("dropdown-menu");
    $(dropDownMenu).append(edit).append(del);

    let icon = document.createElement('img');
    $(icon).addClass("dots-active");
    $(icon).attr("src", "/css/img/icons/dots-active.png");

    let a = document.createElement('a');
    $(a).addClass("dropdown-button");
    $(a).attr("href", "#");
    $(a).attr("role", "button");
    $(a).attr("data-toggle", "dropdown");
    $(a).attr("aria-haspopup", "true");
    $(a).attr("aria-expanded", "false");
    $(a).append(icon);

    let div = document.createElement('div');
    $(div).addClass("dropdown show");
    $(div).append(dropDownMenu).append(a);

    let lastTD = document.createElement('td');
    lastTD.style.padding = "unset";
    $(lastTD).addClass("custom-td-last");
    $(lastTD).append(div);

    let coding = document.createElement('tr');
    $(coding).addClass("tr edit-raw");

    $(coding).append(system).append(version).append(code).append(value).append(versionPublishDate).append(lastTD);

    return coding;
}

$(document).on('click', '.dropdown-item.edit-code', function (e) {
    e.stopPropagation();
    e.preventDefault();
    editCodeModal($(this).closest('td'));
});

$(document).on('click', '.tr.edit-raw', function (e) {
    e.stopPropagation();
    e.preventDefault();
    if (!$(e.target).hasClass('dropdown-button') && !$(e.target).hasClass('fa-bars') && !$(e.target).hasClass('dropdown-item')) {
        editCodeModal(e.target);
    }
});

function editCodeModal(elementTd) {
    resetCodeForm();
    setCodeFormValues(elementTd);

    closestRow = $(elementTd).closest('tr');

    showCodeModalTitle("editCodeTitle");
    $('#codeModal').modal('show');
}

function setCodeFormValues(elementTd) {
    $(`#${$(elementTd).data('property')}`).val($(elementTd).data('value'));
    $(elementTd).siblings().each(function () {
        $(`#${$(this).data('property')}`).val($(this).data('value'));
    });
}

$(document).on('click', '.dropdown-item.delete-code', function (e) {
    var codeRow = $(this).closest("tr")
    var codeId = $(codeRow).data("id");
    if (codeId) {
        callServer({
            type: "delete",
            url: `/ThesaurusEntry/DeleteCode?codeId=${codeId}`,
            success: function (data, textStatus, jqXHR) {
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
    $(codeRow).remove();
});

function setValidationCodeFunctions() {
    $.validator.addMethod('codeValid', function (val, element, options) {
        let isValid = true;

        let formObject = {
            codeSystem: $("#codeSystem").val(),
            codeVersion: $("#codeVersion").val(),
            codeCode: $("#codeCode").val(),
            codeValue: $("#codeValue").val()
        }

        let codeId = $("#codeId").val();

        $(`#codeTable > tbody > tr`).each(function () {
            if ($(this).data('id') != codeId) {
                let rowCellEqual = [];
                let rowIsEqual = false;
                $(this).find("td[data-property]").each(function () {
                    let propertyName = $(this).data('property');
                    let propertyValue = $(this).data('value');

                    if (formObject[propertyName]) {
                        rowCellEqual.push(formObject[propertyName] == propertyValue);
                    }
                })
                rowIsEqual = !(rowCellEqual.length < 4 || rowCellEqual.some(x => !x));
                if (rowIsEqual) {
                    isValid = false;
                }
            }
        });

        return isValid;
    },
        "Code already exist!"
    );
    $('[name="codeSystem"]').each(function () {
        $(this).rules('add', {
            codeValid: true
        });
    });
}

function advanceFilter() {
    $('#O40MtIdTemp').val($('#id').val());
    $('#PreferredTermTemp').val($('#preferredTerm').val());
    $('#StateTemp').val($('#stateCD').val());

    filterData();
}

function mainFilter() {
    $('#id').val($('#O40MtIdTemp').val());
    $('#preferredTerm').val($('#PreferredTermTemp').val());
    $('#stateCD').val($('#StateTemp').val());

    filterData();
}

$(document).on('click', '.thesaurus-collapse', function (e) {
    let args = getThesaurusSelectors($(this).attr("id"));
    args.function($(args.codeElement), $(args.element));
});

function getThesaurusSelectors(elementId) {
    return {
        "codesTable": {
            function: checkThesaurusContainer,
            codeElement: '#codesTable',
            element: '#codesArrow'
        },
        "collapseO4MTSpecificField": {
            function: checkThesaurusContainer,
            codeElement: '#collapseO4MTSpecificField',
            element: '#specificFieldsArrow'
        },
        "skosProperties": {
            function: checkThesaurusContainer,
            codeElement: '#skosProperties',
            element: '#skosArrow'
        },
        "foundIn": {
            function: checkThesaurusHeader,
            codeElement: '#foundArrow',
            element: '#foundIn'
        },
        "documentProperties": {
            function: checkThesaurusHeader,
            codeElement: '#documentArrow',
            element: '#documentProperties'
        },
        "thHierarchy": {
            function: checkThesaurusHeader,
            codeElement: '#hierarchyArrow',
            element: '#thHierarchy'
        }
    }[elementId];
}

$(document).on('click', '#administrativeButton', function (e) {
    var containerWidth = document.getElementById("containerFluid").offsetWidth - 47;
    let administrativeDataWidth = $(document.body)[0].scrollHeight > $(window).height() ? containerWidth - 30 - 30 + "px" : containerWidth - 30 - 30 - 10 + "px";
    $("#collapseAdministrativeData").css("width", administrativeDataWidth);

    var $arrowElement = $("#administrativeArrow");
    if ($arrowElement.hasClass("administrative-arrow")) {
        $arrowElement.removeClass("administrative-arrow");
        $arrowElement.addClass("administrative-arrow-up");
        $("#collapseAdministrativeData").removeClass("d-none");
        showAdministrativeArrowIfOverflow('collapseAdministrativeData');
    } else {
        $arrowElement.removeClass("administrative-arrow-up");
        $arrowElement.addClass("administrative-arrow");
        $("#collapseAdministrativeData").addClass("d-none");
    }
});

$(document).on('click', '.arrow-scroll-right', function (e) {
    e.preventDefault();
    $('#arrowRight').animate({
        scrollLeft: "+=500px"
    }, "slow");
});

$(document).on('click', '.arrow-scroll-left', function (e) {
    e.preventDefault();
    $('#arrowRight').animate({
        scrollLeft: "-=500px"
    }, "slow");
});

$(document).on('click', '#codeVersionPublishDate', function () {
    $("#codeCalendar").click();
});

function resetDocumentArrow($codeElement) {
    $codeElement.removeClass("arrow-tree");
    $codeElement.addClass("arrow-tree-inactive");
}

function checkArrowClass($arrowElement) {
    if ($arrowElement.hasClass("administrative-state-arrow-down")) {
        $arrowElement.removeClass("administrative-state-arrow-down");
        $arrowElement.addClass("administrative-state-arrow-up");
    }
    else {
        $arrowElement.removeClass("administrative-state-arrow-up");
        $arrowElement.addClass("administrative-state-arrow-down");
    }
}

function checkActiveClass($codeElement) {
    if ($codeElement.hasClass("umls-content")) {
        $codeElement.removeClass("umls-content");
        $codeElement.addClass("umls-content-active");
    }
    else {
        $codeElement.removeClass("umls-content-active");
        $codeElement.addClass("umls-content");
    }
}

function checkThesaurusContainer($codeElement, $element) {
    checkActiveClass($codeElement);
    checkArrowClass($element);
}

function checkThesaurusHeader($codeElement, $element) {
    if ($codeElement.hasClass("arrow-tree-inactive")) {
        $codeElement.removeClass("arrow-tree-inactive");
        $codeElement.addClass("arrow-tree");
        $element.addClass("umls-border");
    }
    else {
        $codeElement.removeClass("arrow-tree");
        $codeElement.addClass("arrow-tree-inactive");
        $element.removeClass("umls-border");
    }
}

function setCodeValues(requestObject) {
    requestObject.StateCD = $('#stateCD').find(':selected').attr('id');
}