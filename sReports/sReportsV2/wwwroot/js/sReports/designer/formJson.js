var editorTree;
var editorCode;
var formDefinitionBefore;

function resetJsonEditorData() {
    formDefinitionBefore = undefined;
}

function showJsonDesignerEditor(json, isReadOnly) {
    var ajv = new Ajv({
        allErrors: true,
        verbose: true,
        schemaId: 'auto'
    });
    $('#jsoneditor').html('');
    $('.form-item').removeClass('active');

    let editorModes = isReadOnly ? {
        treeMode: 'view',
        contentMode: 'preview'
    } : {
        treeMode: 'tree',
        contentMode: 'code'
    };

    editorTree = new JSONEditor(document.getElementById('jsoneditorTree'), {
        ajv: ajv,
        mode: editorModes.treeMode,
        onChangeText: function (jsonString) {
            try {
                editorCode.updateText(jsonString);
                $('.jsoneditor-format').click();
            }
            catch (exception) {
                logError(exception);
            }
        },
        onError: function (error) {
            logError(error);
        }
    });

    // create editor 2
    editorCode = new JSONEditor(document.getElementById('jsoneditorCode'), {
        ajv: ajv,
        mode: editorModes.contentMode,
        onChangeText: function (jsonString) {
            try {
                editorTree.updateText(jsonString);
            }
            catch (exception) {
                logError(exception);
            }

        },
        onError: function (error) {
            logError(error);
        }
    });

    editorTree.set(json);
    editorCode.set(json);
    $('#jsoneditorContainer').show();
    formDefinitionBefore = JSON.stringify(editorTree.get());
}

function getFormTree(formDefinition) {
    setIsReadOnlyViewModeInRequest(formDefinition);
    callServer({
        method: 'post',
        data: formDefinition,
        url: `/Form/GetFormTree`,
        contentType: 'application/json',
        success: function (data) {
            $('#formTreeContainer').html(data);
            getNestableFormElements();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function goBackToTree() {
    if (!$('.jsoneditor-undo').attr("disabled")) {
        toastr.info("You have some unsaved changes. Press submit to save.");
    }
    getFormTree(editorTree.get());
}