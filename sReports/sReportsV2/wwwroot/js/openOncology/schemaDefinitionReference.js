$(document).ready(function () {
    setCommonValidatorMethods();
    $('#publicationDate').initDatePicker();
});

function updateReference(form, event) {
    event.preventDefault();
    event.stopPropagation();
    if (!isSchemaNameSpecified()) return;

    $(form).validate({
        ignore: []
    });

    if ($(form).valid()) {
        let request = getReferenceObjectRequest();
        let literatureReference = request.LiteratureReference;
        if (isNaN(literatureReference.PubMedID)) {
            toastr.warning("Please enter number value for PubMed Id");
            return;
        }
        let isNewObject = literatureReference.Id == 0;
        callServer({
            type: "POST",
            url: "/SmartOncology/UpdateSchemaReference",
            data: request,
            success: function (data) {
                toastr.success("Reference is updated");
                literatureReference['Id'] = data.id;
                literatureReference['ChemotherapySchemaId'] = data.parentId;
                literatureReference['PublicationDate'] = $("#publicationDate").val();
                updateSchemaDataOnUI(literatureReference['ChemotherapySchemaId'], data.rowVersion);
                updateReferenceOnUI(literatureReference, isNewObject);
                closeModal('.modal-reference');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function referenceKeyPressed(form, event) {
    if (event.which == enter) {
        updateReference(form, event);
    }
}

function getReferenceObjectRequest() {
    let request = {};

    request['LiteratureReference'] = getReferenceObject();
    request['RowVersion'] = getRowVersion();

    return request;
}

function getReferenceObject() {
    let reference = {};

    let schemaId = getSchemaId();
    reference['Id'] = $('#referenceId').val();
    reference['PubMedLink'] = $('#pubMedLink').val();
    reference['PubMedID'] = $('#pubMedId').val();
    reference['ShortReferenceNotation'] = $('#shortReferenceNotation').val();
    reference['DOI'] = $('#referenceDoi').val();
    reference['PublicationDate'] = toDateStringIfValue($("#publicationDate").val());
    reference['ChemotherapySchemaId'] = schemaId;

    return reference;
}

$(document).on('click', '.modal-reference .close-modal', function () {
    closeModal('.modal-reference');
});

function updateReferenceOnUI(reference, isNewObject) {
    let referenceContent =
        `<div class="item-body-header">
            <p class="text-strong reference-item">PMID: ${reference.PubMedID}</p>
            <span class="icon edit-reference" data-id="${reference.Id}">
                <img src="/css/open-oncology/images/icons/edit-icon.svg" alt="Edit">
            </span>
        </div>
        <p class="schema-text reference-item">Pub Med Link: <a class="link reference-item" href="${reference.PubMedLink}" target="_blank">${reference.PubMedLink}</a></p>
        <p class="schema-text reference-item">Reference: ${reference.ShortReferenceNotation}</p>
        <p class="schema-text reference-item">Doi: ${reference.DOI}</p>
        <p class="schema-text reference-item">Publish date: ${reference.PublicationDate}</p>
        `;
    if (isNewObject) {
        $(".reference-body").append(
            `<div class="item-body">${referenceContent}</div>`
        );
    } else {
        var elementToUpdate = $(`.edit-reference[data-id="${reference.Id}"]`).closest(".item-body");
        $(elementToUpdate).html(referenceContent);
    }
}