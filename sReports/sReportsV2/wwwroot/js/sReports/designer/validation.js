function validateFormDefinition(formDefinition) {
    clearPreviousErrors();
    let message = '';
    let defaultMessage = 'Please define the following fields for the form: <br/>';
    if (!formDefinition.thesaurusid && !formDefinition.thesaurusId) {
        message = 'Please define the following fields for the form: <br/>'
        message += '-Thesaurus <br/>';
        addError(formDefinition.id);
    }

    if (!formDefinition.version) {
        message = message || defaultMessage;
        message += '-Version<br/>';
        addError(formDefinition.id);
    }

    message += validateChapters(formDefinition.Chapters);

    return message;
}

function validateChapters(chapters) {
    let message = '';
    if (chapters) {
        chapters.forEach(function (chapter, index) {
            if (!chapter.thesaurusid) {
                message = 'Please define the following fields for the chapter: <br/>';
                message += '-Thesaurus <br/>';
                addError(chapter.id);
            }
            message += validatePages(chapter.Pages);
        });
    }

    return message;
}

function validatePages(pages) {
    let message = '';
    if (pages) {
        pages.forEach(function (page, index) {
            if (!page.thesaurusid) {
                message = 'Please define the following fields for the page: <br/>';
                message += '-Thesaurus <br/>';
                addError(page.id);
            }
            message += validateFieldSets(page.ListOfFieldSets);
        });
    }

    return message;
}

function validateFieldSets(fieldSets) {
    let message = '';
    if (fieldSets) {
        fieldSets.forEach(function (listOfFS, index) {
            listOfFS.forEach(function (fieldSet, index) {
                if (!fieldSet.thesaurusid) {
                    message = 'Please define the following fields for the field set: <br/>';
                    message += '-Thesaurus <br/>';
                    addError(fieldSet.id);
                }

                if (fieldSet.listoffieldsets && Array.isArray(fieldSet.listoffieldsets) && fieldSet.listoffieldsets.length > 0) {
                    fieldSet.listoffieldsets.forEach(function (nestedfieldset) {
                        if (!nestedfieldset.thesaurusId && !nestedfieldset.ThesaurusId) {
                            message += 'Please define the following fields for the nested field set: <br/>';
                            message += '- Thesaurus <br/>';
                            addError(nestedfieldset.id);
                        }
                        message += validateFields(nestedfieldset.fields);
                    });

                } else {
                    message += validateFields(fieldSet.Fields);
                }
            });
        });
    }

    return message;
}

function validateFields(fields) {
    let message = '';
    if (fields) {
        fields.forEach(function (field, index) {
            if (!field.thesaurusid && !field.thesaurusId && !specificTypeFields.includes(field.type)) {
                message = 'Please define the following fields for the field: <br/>';
                message += '-Thesaurus <br/>';
                addError(field.id);
            }
            if (field.Values) {
                message += validateFieldValues(field.Values);
            }
        });
    }

    return message;
}

function validateFieldValues(values) {
    let message = '';
    if (values) {
        values.forEach(function (value, index) {
            if (!value.thesaurusid) {
                message = 'Please define the following fields for the field value: <br/>';
                message += '-Thesaurus <br/>';
                addError(value.id);
            }
        });
    }

    return message;
}

function addError(id) {
    if (id) {
        $("#nestable").find(`[data-id='${id}']`).children('.dd-handle').addClass('nestable-error');
    } else {
        $("#nestable").find(`[data-itemtype='form']`).children('.dd-nohandle').addClass('nestable-error');
    }
}

function clearPreviousErrors() {
    $('.nestable-error').each(function (index, element) {
        $(element).removeClass('nestable-error');
    });
}

function clearErrorFromElement(id) {
    if (id && id !=="formIdPlaceHolder") {
        $("#nestable").find(`[data-id='${id}']`).children('.dd-handle').removeClass('nestable-error');
    } else {
        $("#nestable").find(`[data-itemtype='form']`).children('.dd-nohandle').removeClass('nestable-error');
    }
}
