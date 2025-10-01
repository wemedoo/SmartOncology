function initTinyMCE(isFormInstance = false) {
    tinymce.remove();

    tinymce.init({
        apiKey: 'vx3ljr86fkwhagchor89xd6fitmgvd5vziwkkvx6jt1uzuhs',
        selector: '#paragraph, .richtext-editor',
        height: 'calc(90vh - 437px)',
        menubar: false,
        plugins: 'advlist lists autolink link image charmap print preview anchor',
        toolbar: 'undo redo | formatselect | bold italic underline | bullist numlist',
        advlist_bullet_styles: 'default,circle,square',
        advlist_number_styles: 'default,lower-alpha,lower-roman,upper-alpha,upper-roman',
        setup: function (editor) {
            editor.on('change keyup input', function () {
                const content = editor.getContent();
                const id = editor.id;
                const textarea = document.getElementById(id);
                if (textarea) {
                    textarea.value = content;
                }
            });

            editor.on('init', function () {
                const id = editor.id;
                const textarea = document.getElementById(id);
                if (textarea) {
                    if (textarea.hasAttribute('readonly') || textarea.hasAttribute('disabled')) {
                        editor.setMode('readonly');
                    }
                }

            });
        }
    });

    if (isFormInstance && !initialFormData) {
        setTimeout(() => {
            tinymce.triggerSave();
            saveInitialFormData("#fid");
        }, 1000);
    }
}

function setTinyMCEReadOnly($missingValueInput) {
    var fieldInstanceName = $missingValueInput.attr("name");
    let $textarea = $("textarea[name='" + fieldInstanceName + "']");

    if ($textarea.length > 0) {
        let textareaId = $textarea.attr("id");
        let editor = tinymce.get(textareaId);

        if (editor) {
            editor.setContent("");
            editor.setMode('readonly');
        }

        $textarea.prop("readonly", true).addClass("mce-readonly");
    }
}

function reenableTinyMCEEditor(fieldInstanceName) {
    let $textarea = $("textarea[name='" + fieldInstanceName + "']");

    if ($textarea.length > 0) {
        let textareaId = $textarea.attr("id");
        let editor = tinymce.get(textareaId);

        if (editor) {
            editor.undoManager.transact(function () {
                editor.setContent("");
            });
            editor.setMode('design');
            $textarea.val("");
        }

        $textarea.prop("readonly", false).removeAttr("readonly").removeClass("mce-readonly");
    }
}