var editorCode;

function showJsonEditor(json) {
    var ajv = new Ajv({
        allErrors: true,
        verbose: true,
        schemaId: 'auto'
    });

    if (!editorCode) {
        editorCode = new JSONEditor(document.getElementById('jsoneditorCode'), {
            ajv: ajv,
            mode: 'code',
            onError: function (error) {
                logError(error);
            }
        });
    }
    editorCode.set(json);
    $('#jsoneditorContainer').show();
}

function updateJsonEditor() {
    let guidelineData = editorCode.get();
    let data = cy.json();
    guidelineData.guidelineElements = data.elements;
    showJsonEditor(guidelineData);    
    enforceZoomLevel();
    cy.style().update();
}

function deepCompareIgnorePan(a, b) {
    const obj1 = typeof a === 'string' ? JSON.parse(a) : a;
    const obj2 = typeof b === 'string' ? JSON.parse(b) : b;

    const cleaned1 = removePan(obj1);
    const cleaned2 = removePan(obj2);

    return JSON.stringify(cleaned1) === JSON.stringify(cleaned2);
}

function removePan(obj) {
    if (Array.isArray(obj)) {
        return obj.map(removePan);
    } else if (obj && typeof obj === 'object') {
        const newObj = {};
        for (const key in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, key) && key !== 'pan') {
                newObj[key] = removePan(obj[key]);
            }
        }
        return newObj;
    }
    return obj;
}

function getSerializedGraphState() {
    if (!cy) {
        initializeGraph(guidelineData);
    }
    return JSON.stringify(cy.json());
}