var configByType = {
    form: {
        excludeProperties: ['itemtype', 'activeversion'],
        formUrl: '/Form/GetFormGeneralInfoForm',
        title: 'Form information',
        parent: '',
        child: 'chapter',
        handleClass: '.dd-nohandle'
    },
    chapter: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetChapterInfoForm',
        title: 'Chapter information',
        parent: 'form',
        child: 'page',
        handleClass: '.dd-handle'
    },
    page: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetPageInfoForm',
        title: 'Page general info',
        parent: 'chapter',
        child: 'fieldset',
        handleClass: '.dd-handle'
    },
    fieldset: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetFieldSetInfoForm',
        title: 'Fieldset information',
        parent: 'page',
        child: 'field',
        handleClass: '.dd-handle'
    },
    field: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetFieldInfoForm',
        title: 'Field information',
        parent: 'fieldset',
        child: 'fieldvalue',
        handleClass: '.dd-handle'
    },
    fieldvalue: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetFieldValueInfoForm',
        title: 'Option Information',
        parent: 'field',
        child: '',
        handleClass: '.dd-handle'
    }
};

var selectableFields = ['checkbox', 'select', 'radio'];
var stringFields = ['text', 'date', 'datetime', 'calculative', 'number', 'regex', 'long-text', 'file', 'email', 'coded', 'paragraph', 'link', 'audio', 'rich-text-paragraph'];
var specificTypeFields = ['paragraph', 'link'];

addUnsavedDesignerChangesEventHandler();
$(document).ready(function () {
    configureImageMap();
});

$(document).on('mousedown', '.dd-item', function (e) {
    if (!$(e.target).hasClass('add-item-button')) {
        e.stopPropagation();
    } else {
        return;
    }

});

$(document).on('click', '.remove-button', function (e) {
    e.preventDefault();
    e.stopPropagation();
    let element = $(e.currentTarget).closest('.dd-item');
    if (canDelete(element)) {
        let targetId = $(element).attr('data-id');
        let parentId = $(element).attr('data-parentid');
        var maxItems = parseInt($(element).attr('data-maxitems'), 10);
        $('.remove-modal-button').attr('data-target', `${targetId}`);
        $('.remove-modal-button').attr('data-parentid', `${parentId}`);
        $('.remove-modal-button').attr('data-maxitems', `${maxItems}`);
        showDeleteModal(e, "", "deleteFormItem");
    }
});

$(document).on('click', '.remove-matrix-button', function (e) {
    e.preventDefault();
    e.stopPropagation();
    let element = $(e.currentTarget).closest('td').find('.fs-column-input');

    if (element.length == 0) {
        element = $(e.currentTarget).closest('.dd-item');
    }
    
    if (canDelete(element)) {
        let targetId = $(element).attr('data-id');
        let parentId = $(element).attr('data-parentid');
        var maxItems = parseInt($(element).attr('data-maxitems'), 10);
        let row = $(e.currentTarget).closest('.fs-row');
        let matrixId = $(element).attr('data-matrixid');

        $('.remove-modal-button').attr('data-target', `${targetId}`);
        $('.remove-modal-button').attr('data-parentid', `${parentId}`);
        $('.remove-modal-button').attr('data-maxitems', `${maxItems}`);
        $('.remove-modal-button').attr('data-row-id', row.index());
        $('.remove-modal-button').attr('data-matrixid', matrixId);
        showDeleteModal(e, "", "removeFieldsetRow");
    }
});

function canDelete(element) {
    let canDelete = true;
    let itemType = element.attr('data-itemtype');
    if (examineConditionsBeforeDelete(itemType)) {
        let itemId = element.attr('data-id');
        let params = [];
        let cannotDeleteHandler = getCannotActionHandler(itemType);
        if (cannotDeleteHandler(itemId, itemType, params, 'deleted')) {
            canDelete = false;
            toastr.error(params[0]);
        }
    }
    return canDelete;
}

function deleteFormItem(e) {
    let status = $(e.currentTarget).attr('data-remove-status');
    if (status === 'confirm') {
        let itemToRemove = $(e.currentTarget).attr('data-target');
        $(`[data-id='${itemToRemove}']`).remove();
        $(`[data-matrixid='${itemToRemove}']`).remove();
    }
    manageAddNewButton(e);
    $('#deleteModal').modal('hide');
    let nestable = $('#nestableFormPartial').data('nestable');
    nestable.managePlaceholderElements('nestableFormPartial');

    submitFullFormDefinition();
}

function manageAddNewButton(e) {
    var fieldSetId = $(e.currentTarget).attr('data-parentid');

    if (fieldSetId != 'undefined') {
        const mainListItem = document.querySelector(`.dd-item[data-itemtype="fieldset"][data-id="${fieldSetId}"]`);
        const fieldItems = mainListItem.querySelectorAll('li[data-itemtype="field"]');
        const maxItems = $(e.currentTarget).attr('data-maxitems');

        if (fieldItems.length < maxItems) {
            let addNewButton = mainListItem.querySelector(`li.add-item-button.add-chapter-button.dd-nodrag[data-parentid="${fieldSetId}"]`);

            if (!addNewButton) {
                const newButton = document.createElement('li');
                newButton.className = 'add-item-button add-chapter-button dd-nodrag';
                newButton.setAttribute('data-itemtype', 'field');
                newButton.setAttribute('data-parentid', fieldSetId);
                newButton.innerHTML = `
                <div>
                    <img src="/css/img/icons/add_new.svg">
                    Add New field
                </div>`;

                mainListItem.querySelector('ol.dd-list').appendChild(newButton);
            }
        }
    }
}

$(document).on('click', '.remove-modal-button', function (e) {
    let status = $(e.currentTarget).attr('data-remove-status');
    if (status === 'confirm') {
        let itemToRemove = $(e.currentTarget).attr('data-target');
        $(`[data-id=${itemToRemove}]`).remove();
    }

    $('#confirmRemovalModal').modal('hide');
});

$(document).on('click', '.edit-button', function (e) {
    e.stopPropagation();
    e.preventDefault();
    let element = $(e.currentTarget).closest('li.dd-item');
    let type = $(element).attr('data-itemtype');
    parentId = $(element).attr('data-parentid');

    showForm(type, element);
});

$(document).on('click', '.edit-matrix-button', function (e) {
    e.stopPropagation();
    e.preventDefault();

    let $button = $(e.currentTarget);
    let $input = $button.closest('td').find('.fs-column-input');
    let type = $input.attr('data-itemtype');
    parentId = $input.attr('data-parentid');

    showForm(type, $input);
});

var parentId;
$(document).on('click', '.add-item-button', function (e) {
    let type = $(e.currentTarget).attr('data-itemtype');
    parentId = $(this).attr('data-parentid');

    showForm(type, null);
    $('.add-item-button').removeClass('active');
    $(this).addClass('active');
});

function showForm(type, element) {
    let config = configByType[type];
    let requestObject = generateObjectFromDataProperties(element, config.excludeProperties);
    setAdditionShowFormRequestParams(type, requestObject);
    setIsReadOnlyViewModeInRequest(requestObject);
    callServer({
        method: 'post',
        data: requestObject,
        url: config.formUrl,
        contentType: 'application/json',
        success: function (data) {
            setAndShowDesignerModal(data, config.title);
            prepareConfigurationSections(requestObject.type);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function setAndShowDesignerModal(html, title) {
    $('#designerFormModalMainContent').html(html);
    $('body').addClass('no-scrollable');
    $('.designer-form-title-text').html(title);
    $('.designer-form-modal').addClass('show');
    $('.designer-form-modal-body').scrollTop(0);
    adjustTableWidth();
}

function setAdditionShowFormRequestParams(type, requestObject) {
    if (type == 'field') {
        var formId = $("li[data-itemtype='form']:first").attr('data-id');
        requestObject.formid = formId == 'formIdPlaceHolder' ? null : formId;
        requestObject.fieldsetid = parentId ? parentId : '';
    }
}

function prepareConfigurationSections(fieldType) {
    if (specificTypeFields.includes(fieldType))
        hideCommonElements();
    else
        if (fieldType == 'audio')
            hideSomeCommonElements();
        else
            showCommonElements();
}

$(document).on('click', '#submit-full-form-definition', function (e) {
    e.preventDefault();
    e.stopPropagation();

    submitFullFormDefinition(true);
});

function submitFullFormDefinition(fullFormSubmit = false, reloadPartial = false, isMatrixFieldSet = false) {
    let formDefinition;
    if (editorTree) {
        formDefinition = editorTree.get();
    } else {
        formDefinition = getNestableFullFormDefinition($("#nestable").find(`li[data-itemtype='form']`).first());
    }

    let formDefinitionValidationSummary = validateFormDefinition(formDefinition);
    if (formDefinitionValidationSummary) {
        if (formDefinitionValidationSummary.includes('Version') && !isMatrixFieldSet) {
            toastr.error(formDefinitionValidationSummary);
            return;
        }
        formDefinition.invalid = true;
    }
    else {
        formDefinition.invalid = false;
    }

    let createForm = isNewFormCreated(formDefinition);
    let action = formDefinition.id != "" ? 'Edit' : 'Create';

    if (!isMatrixFieldSet) {
        callServer({
            method: 'post',
            data: formDefinition,
            url: `/Form/${action}`,
            contentType: 'application/json',
            success: function (data) {
                updateFormWithLastUpdate(data);
                resetJsonEditorData();
                if (fullFormSubmit || createForm || isNewFormGenerated(data, createForm)) {
                    toastr.success('Success', '', {
                        timeOut: 100,
                        onHidden: function () {
                            window.location.href = `/Form/Edit?thesaurusId=${data.thesaurusId}&versionId=${data.versionId}`;
                        }
                    });
                } else {
                    reloadFormPartialIfNecessary(reloadPartial, data.lastUpdate, formDefinition);
                }
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }

    return createForm;
}

$(document).on("click", "#formAdministrativeButton", function () {
    callServer({
        method: 'GET',
        url: `/Form/GetFormAdministrativeData?formId=${getFormDefinitionId()}`,
        success: function (data) {
            $('.designer-form-modal-body').addClass('unset-height');
            setAndShowDesignerModal(data, 'Form Administrative Data');
            $("#administrative-container-form").addClass("workflow-show");
            $("#administrative-container-form").removeClass("workflow-hide");
            showAdministrativeArrowIfOverflow('administrative-container-form');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
});

function updateFormWithLastUpdate(data) {
    $('li[data-itemtype=form]').attr('data-lastupdate', data.lastUpdate);
}

function isNewFormGenerated(data, createForm) {
    return getFormDefinitionId() != data.id && !createForm;
}

function isNewFormCreated(formDefinition) {
    return !formDefinition.id;
}

function getFormDefinitionId() {
    return decodeToJsonOrText($('li[data-itemtype=form]').attr('data-id'));
}

function appendParameters(parameters) {
    var url = window.location.href;
    if (!url.indexOf('?') > -1) {
        url += `&${parameters}`;
    } else {
        url += `?${parameters}`
    }
    window.location.href = url;
}


function updateNestableData(id) {
    let formDefinition = getNestableFullFormDefinition($(`#${id}`).find(`li[data-itemtype='form']`).first());
    if (id === "nestable") {
        getFormPartial(formDefinition);
        submitFullFormDefinition();
    }
    else {
        getNestableTree(formDefinition);
    }
}

function getFormPartial(formDefinition) {
    setIsReadOnlyViewModeInRequest(formDefinition);
    callServer({
        method: 'post',
        data: formDefinition,
        url: '/Form/CreateDragAndDropFormPartial',
        contentType: 'application/json',
        success: function (data) {
            $('#formPreviewContainer').html(data);

            $('#nestableFormPartial').nestable({
                group: 1,
                maxDepth: 7
            }).on('change', function () {
                applyCollapsedState();
            });

            applyCollapsedState();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function saveCollapsedState() {
    let collapsedItems = [];
    $('li.dd-item.dd-collapsed.selectable-item').each(function () {
        collapsedItems.push($(this).data('id'));
    });
    localStorage.setItem('collapsedItems', JSON.stringify(collapsedItems));
}

function applyCollapsedState() {
    let collapsedItems = JSON.parse(localStorage.getItem('collapsedItems')) || [];
    $('li.dd-item.selectable-item').each(function () {
        if (collapsedItems.includes($(this).data('id'))) {
            $(this).addClass('dd-collapsed');
            $(this).find('button[data-action="collapse"]').hide();
            $(this).find('button[data-action="expand"]').show();
        } else {
            $(this).removeClass('dd-collapsed');
        }
    });
}

function getNestableTree(formDefinition, isMatrixFieldSet = false) {
    callServer({
        method: 'post',
        data: formDefinition,
        url: '/Form/CreateFormTreeNestable',
        contentType: 'application/json',
        success: function (data) {
            $('#formTreeNestable').html(data);
            if (isMatrixFieldSet)
                submitFullFormDefinition(false, true, isMatrixFieldSet);
            else
                submitFullFormDefinition();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getChapterWithData(chapterElement) {
    let chapter = generateObjectFromDataProperties(chapterElement, configByType['chapter'].excludeProperties);
    chapter.Pages = [];

    $(chapterElement).find(`li[data-itemtype='page']:not(.add-item-button):not(.dd-item-placeholder)`).each(function (index, pageElement) {
        chapter.Pages.push(getPageWithData(pageElement));
    })

    return chapter;
}

function getPageWithData(pageElement) {
    let page = generateObjectFromDataProperties(pageElement, configByType['page'].excludeProperties);
    page.ListOfFieldSets = [];
    let fieldsetMap = {}; 

    $(pageElement).find(`[data-itemtype='fieldset']:not(.add-item-button):not(.dd-item-placeholder)`).each(function (index, fieldsetElement) {
        let matrixId = $(fieldsetElement).attr("data-matrixid");
        let fieldset = getFieldSetWithData(fieldsetElement); 

        if (!matrixId) {
            page.ListOfFieldSets.push(fieldset);
        }
        else {
            let matrixFieldsetId = matrixId;
            if (!fieldsetMap[matrixFieldsetId]) {
                fieldsetMap[matrixFieldsetId] = { Fields: [], ListOfFieldSets: [] };
            }
            fieldsetMap[matrixFieldsetId].ListOfFieldSets.push(fieldset);
        }
    });

    Object.keys(fieldsetMap).forEach(fieldsetId => {
        if (fieldsetMap[fieldsetId].ListOfFieldSets.length > 0) {
            let flatFieldSets = page.ListOfFieldSets.flat();
            let parentFieldset = flatFieldSets.find(fs => fs.id === fieldsetId);
            if (parentFieldset) {
                parentFieldset.listoffieldsets.sort((a, b) => {
                    let aElement = $(`li[data-id='${a.id}']`);
                    let bElement = $(`li[data-id='${b.id}']`);
                    return aElement.index() - bElement.index();
                });
            }
        }
    });

    return page;
}

function getFieldSetWithData(fieldsetElement) {
    let fieldset = generateObjectFromDataProperties(fieldsetElement, configByType['fieldset'].excludeProperties);
    fieldset.Fields = [];
    if (!fieldset.listoffieldsets || fieldset.listoffieldsets.length == 0) {
        $(fieldsetElement).find(`li[data-itemtype='field']:not(.add-item-button):not(.dd-item-placeholder)`).each(function (index, fieldElement) {
            fieldset.Fields.push(getFieldWithData(fieldElement));
        })
    }
    return [fieldset];
}

function getFieldWithData(fieldElement) {
    let field = generateObjectFromDataProperties(fieldElement, configByType['field'].excludeProperties);
    if (field.type === "paragraph" && (!field.paragraph || field.paragraph.trim() === "")) {
        field.paragraph = "Paragraph";
    }
    else if (field.type === "link" && !field.link) {
        field.link = $(`#defaultLink`).val();
    }
    loadFieldValueIfSelectable(field, fieldElement);
    populateNullFlavors(field);
    return field;
}

function populateNullFlavors(field) {
    if (field.id == getOpenedElementId()) {
        field.nullflavors = [];
        $('.additional-checkbox').each(function () {
            if ($(this).is(':checked')) {
                field.nullflavors.push($(this).val());
            }
        });
    }
}

function loadFieldValueIfSelectable(field, fieldElement) {
    if (selectableFields.includes($(fieldElement).attr('data-type'))) {
        field.Values = [];
        $(fieldElement)
            .find(`[data-itemtype='fieldvalue']`)
            .not('.add-item-button')
            .not('.form-select-placeholder')
            .not('.dd-item-placeholder')
            .each(function (index, fieldValueElement) {
                let fieldValue = generateObjectFromDataProperties(fieldValueElement, configByType['fieldvalue'].excludeProperties);
                field.Values.push(fieldValue);
            })
    }
}


function generateObjectFromDataProperties(element, excludeProperties) {
    let result = {};
    if (element) {
        let dataProps = element.length ? element[0].dataset : element.dataset;
        Object.keys(dataProps).filter(x => !excludeProperties.includes(x)).forEach(x => {
            result[x] = decodeToJsonOrText(dataProps[x]);
        });
    }

    return result;
}

function getFieldInfo(element) {
    let fieldData = null;

    return fieldData;
}

/*COMMON CODE*/
function updateTreeItemTitle(element, title) {
    let type = $(element).attr('data-itemtype');
    let titleContainer = $(element).children(configByType[type].handleClass).first();
    if (type === 'field' && $(element).attr('data-type') === 'paragraph') {
        let txt = document.createElement("textarea");
        txt.innerHTML = title;
        $(titleContainer).html(txt.innerHTML);
    }
    else {
        $(titleContainer).html(title);
    }
    $(titleContainer).attr('title', title);
}

function getDataProperty(element, dataName) {
    let data = $(element).attr(`data-${dataName}`);
    let decodedData = data ? decodeURIComponent(removeEncodedPlusForWhitespace(data)) : null;
    return JSON.parse(decodedData) || {};
}

function getElement(type, title) {
    let result;
    let elementId = getOpenedElementId();

    if ($("#nestable").find(`li[data-id='${elementId}']`).length > 0) {
        result = $("#nestable").find(`li[data-id=${elementId}]`)[0];
    } else {
        result = createNewDragAndDropElement(type, elementId, title);
    }

    return result;
}

function createNewDragAndDropElement(type, elementId, title) {
    let newElement = document.createElement('li');
    $(newElement).attr('data-id', elementId);
    $(newElement).addClass('dd-item');
    $(newElement).attr('data-itemtype', type);

    let handle = document.createElement('div');
    $(handle).addClass('dd-handle');
    $(handle).html(title);
    $(newElement).append(handle);

    $(newElement).append(createDragAndDropRemoveButton());
    $(newElement).append(createDragAndDropEditButton());
    if (type != 'fieldvalue' && type != 'field') {
        $(newElement).append(createDragAndDropOrderedlist(type, elementId));
    }


    let olParent = $("#nestable").find(`.dd-item[data-id='${parentId}']`).find('ol').first();
    $(olParent).find('li:last').before(newElement);
    $('#nestable').nestable('reinit');

    return newElement;
}

function createDragAndDropEditButton() {
    let editButton = document.createElement('div');
    $(editButton).addClass('edit-button');
    let icon = document.createElement('img');
    $(icon).attr('src', '/css/img/icons/edit_pencil_03.svg');
    $(editButton).append(icon);

    return editButton;
}

function createDragAndDropRemoveButton() {
    let removeButton = document.createElement('div');
    $(removeButton).addClass('remove-button');
    let iconRemove = document.createElement('img');
    $(iconRemove).attr('src', '/css/img/icons/remove_simulator.svg');
    $(removeButton).append(iconRemove);

    return removeButton;
}

function createDragAndDropOrderedlist(type, elementId) {
    let orderedList = document.createElement('ol');
    $(orderedList).addClass('dd-list');
    if (type !== 'fieldvalue') {
        $(orderedList).append(createDragAndDropInsertItemButton(type, elementId));
    }

    return orderedList;
}

function createDragAndDropInsertItemButton(type, elementId) {
    let child = getDragAndDropChildNameForParent(type);
    let li = document.createElement('li');
    $(li).addClass('add-item-button add-page-button dd-nodrag');
    $(li).attr('data-itemtype', child);
    $(li).attr('data-parentid', elementId);
    let div = document.createElement('div');
    let img = document.createElement('img');
    $(img).attr('src', '/css/img/icons/add_new.svg');
    $(div).append(img).append(` Add new ${getPlaceholderText(child)}`);
    $(li).append(div);
    return li;
}

function getDragAndDropChildNameForParent(parentType) {
    return configByType[parentType].child;
}

function getPlaceholderText(elementName) {
    return elementName === 'fieldvalue' ? 'option' : elementName;
}
 
function decodeToJsonOrText(value) {
    let decoded = decode(value);
    let result = '';
    try {
        result = JSON.parse(decoded);
    }
    catch{
        result = decoded;
    }

    return result;
}

function decode(value) {
    let val = '';
    if (value || value == 0) {
        val = `${value}`;
    }
    return decodeURIComponent(removeEncodedPlusForWhitespace(val));
}



$(document).on('click', '#thesaurusShowModal', function (e) {
    let thesaurusId = $('#thesaurusId').val();
    if (thesaurusId) {
        callServer({
            method: 'get',
            url: `/ThesaurusEntry/ThesaurusProperties?o4mtid=${thesaurusId}`,
            success: function (data) {
                $('.active-thesaurus').html(data);
                $('#filterThesaurusModal').modal('show');

            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    } else {
        $('#filterThesaurusModal').modal('show');
    }

});

function reloadFormPreviewPartial() {
    let isNewFormCreated = submitFullFormDefinition(false, true);
    if (!isNewFormCreated) {
        getNestableFormElements();
    }
}

function reloadFormPartialIfNecessary(reloadPartial, lastUpdate, formDefinition) {
    if (reloadPartial) {
        formDefinition['lastupdate'] = decodeToJsonOrText(lastUpdate);
        saveCollapsedState();
        getFormPartial(formDefinition);
    }
}

function getNestableFormElements() {
    let formDefinition = getNestableFullFormDefinition($("#nestable").find(`li[data-itemtype='form']`).first());
    validateFormDefinition(formDefinition);
    callServer({
        method: 'post',
        url: '/Form/GetPredefinedFormElements',
        success: function (data) {
            $('#nestableFormElements').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

$(document).ready(function () {
    function initializeLayout() {
        getNestableFormElements();
        setCanvasSize();
    }

    initializeLayout();

    $(window).on('resize', initializeLayout);
});

function setCanvasSize() {
    $('#designerCanvas').attr("width", $('.designer-main-content:first').width());
    $('#designerCanvas').attr("height", $('.designer-main-content:first').height());
}

$(document).on('show.bs.dropdown', "#formStateDropdown", function (e) {
    toggleDropdownButton($(this));
}); 

$(document).on('hidden.bs.dropdown', "#formStateDropdown", function (e) {
    toggleDropdownButton($(this));
}); 

function toggleDropdownButton(dropdown) {
    let dropdownButton = $(dropdown).find("#dropdownMenuChangeState");
    if ($(dropdownButton).hasClass('active')) {
        $(dropdownButton).removeClass('active pressed');
    } else {
        $(dropdownButton).addClass('active pressed');
    }
}

$(document).on('click', '#formStateDropdown a', function () {
    var target = $(this);
    var targetValue = $(target).attr('data-value');
    $('li[data-itemtype=form]').attr('data-state', decodeURIComponent(targetValue));
   
    //Adds active class to selected item
    var previousSelected = $(target).parents('.dropdown-menu').find('.form-definition-state.active');
    $(previousSelected).removeClass('active');
    $(target).addClass('active');

    let data = {
        State: targetValue,
        Id: getFormDefinitionId(),
        LastUpdate: decodeToJsonOrText($('li[data-itemtype=form]').attr('data-lastupdate'))
    }
    callServer({
        method: 'PUT',
        data: data,
        url: `/Form/UpdateFormState`,
        success: function (data) {
            $('li[data-itemtype=form]').attr('data-lastupdate', encodeURIComponent(data.lastUpdate));
            toastr.success('You have successfully changed the state');
            resetJsonEditorData();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
            $(previousSelected).addClass('active');
            $(target).removeClass('active');
        }
        
    })
});

function getNestableFullFormDefinition(formElement) {
    let formDefinition = generateObjectFromDataProperties(formElement, configByType['form'].excludeProperties);
    formDefinition.id = formDefinition.id == 'formIdPlaceHolder' ? '' : formDefinition.id;
    formDefinition['Chapters'] = [];
    $(formElement).find(`li[data-itemtype='chapter']:not(.add-item-button):not(.dd-item-placeholder)`).each(function (index, chapterElement) {
        formDefinition.Chapters.push(getChapterWithData(chapterElement));
    });

    return formDefinition;
}
/*END COMMON CODE*/

/*form modal*/
$(document).on('click', '.cancel-modal-action', function (e) {
    closDesignerFormModal();
});

$(document).on('click', '.designer-form-modal-body', function (e) {
    e.stopPropagation();
});

$(document).on('click', '.close-designer-form-modal-button', function (e) {
    closDesignerFormModal();
});

function closDesignerFormModal(reloadFormPreview) {
    $('.designer-form-modal').removeClass('show');
    $('.designer-form-modal-body').removeClass('unset-height');
    $('body').removeClass('no-scrollable');

    if (reloadFormPreview) {
        reloadFormPreviewPartial();
    }
}

function showFormJson() {
    let formDefinition = getNestableFullFormDefinition($("#nestable").find(`li[data-itemtype='form']`).first());
    setIsReadOnlyViewModeInRequest(formDefinition);
    callServer({
        method: 'post',
        data: formDefinition,
        url: `/Form/GetFormJson`,
        contentType: 'application/json',
        success: function (data) {
            $('#formTreeContainer').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function collapseTree() {
    initTreeIfEmpty('formTreeNestable');
    $('#formTreeNestable').nestable('collapseAll');
}

function expandTree() {
    $('#formTreeNestable').nestable('expandAll');
}

function initTreeIfEmpty(nestableTreeId) {
    let nestable = $(`#${nestableTreeId}`).data('nestable');
    if (!nestable) {
        $(`#${nestableTreeId}`).nestable({
            group: 1,
            maxDepth: 7
        });
    }
}
/*End form modal*/

$(document).on('click', '.drag-icon-container', function (e) {
    if ($("#nestableFormElements").css('display') === "none") {
        $("#nestableFormElements").show();
        $(this).addClass('expanded');

    } else {
        $("#nestableFormElements").hide();
        $(this).removeClass('expanded');
    }

    showNestableContainer();

});

$(document).on('click', "#closeDragAndDrop", function (e) {
    $("#nestableFormElements").hide();
});

$(document).on('hover', '.icon-container', function (e) {
    if ($('.dd-dragel').length > 0) {
        e.stopPropagation();
        e.preventDefault();
    }
});

function createNewThesaurusIfNotSelected() {
    let thesaurusId = $("#designerFormModalMainContent").find('#thesaurusId').val();
    let preferredTerm = $("#designerFormModalMainContent").find('.item-title').val();
    let description = $("#designerFormModalMainContent").find('#description').val();

    if ((!thesaurusId || thesaurusId === "0" ) && preferredTerm) {

        callServer({
            method: 'post',
            data: {preferredTerm, description},
            url: `/ThesaurusEntry/CreateByPreferredTerm`,
            success: function (data) {
                setThesaurusDetailsContainer(data.id);
                loadThesaurusData(data.id);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            },
            async: false
        });
    }
}

function hideNestableContainer() {
    $('.comments-hidden').hide();
    $('#formPreviewContainer').addClass('comments-active');
}

function showNestableContainer() {
    $('.comments-hidden').show();
    $('#formPreviewContainer').removeClass('comments-active');
}

function getOpenedElementId() {
    return $('#elementId').val();
}

function generateUUIDWithoutHyphens() {
    const uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });

    return uuid.replace(/-/g, '');
}