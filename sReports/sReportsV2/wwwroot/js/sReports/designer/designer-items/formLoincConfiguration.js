$(document).on('click', '#loincConfigurationBtn', function (e) {
    $('#submit-general-form-info').click();
});

function formatResult(node) {
    var $result = $('<span style="padding-left:' + (20 * node.level) + 'px;">' + node.text + '</span>');
    return $result;
};

function loincConfiguration(documentProperties) {
    callServer({
        type: 'GET',
        url: `/Form/GetLoincDataSource`,
        success: function (data) {
            $(".loinc-conf-input").each(function () {
                let propertyName = $(this).attr("id");
                $(this).initSelect2(
                    getSelect2Object(
                        {
                            placeholder: 'Select an option',
                            width: '100%',
                            allowClear: true,
                            modalId: 'designerFormModal',
                            initialDataSource: data[propertyName],
                            templateResult: formatResult,
                            minimumInputLength: 0
                        }
                    )
                );
                let selectedValueId = documentProperties[propertyName];
                if (selectedValueId) {
                    $(this).val(selectedValueId).trigger("change");
                }
            });
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function setDocumentLoincProperties(element) {
    let documentProperties = getDataProperty(element, 'documentloincproperties');
    documentProperties['SubjectMatterDomain'] = $('#SubjectMatterDomain').val();
    documentProperties['Role'] = $('#Role').val();
    documentProperties['Setting'] = $('#Setting').val();
    documentProperties['TypeOfService'] = $('#TypeOfService').val();
    documentProperties['Kind'] = $('#Kind').val();

    $(element).attr('data-documentloincproperties', encodeURIComponent(JSON.stringify(documentProperties)));
}