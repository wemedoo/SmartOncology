$(document).ready(function () {
    $('#schemaNameMobile').initSelect2(
        getSelect2Object(
            {
                placeholder: 'Type schema\'s name',
                url: `/SmartOncology/GetAutocompleteSchemaData`
            }
        )
    );
    $('#schemaNameMobile').on('change', function (e) {
        viewSchema(e, 'Mobile');
    });
    $('#schemaNameMobile').on('select2:opening select2:closing', function (e) {
        autocompleteEventHandler(e);
    });

    $('#patientNameMobile').initSelect2(
        getSelect2Object(
            {
                placeholder: 'Type patient\'s name',
                url: `/SmartOncology/GetAutocompletePatientData`
            }
        )
    );

    $('#patientNameMobile').on('change', function (e) {
        viewPatientData(e, 'Mobile');
        resetSchema();
    });
    $('#patientNameMobile').on('select2:opening select2:closing', function (e) {
        autocompleteEventHandler(e);
    });

    $('#patientName').initSelect2(
        getSelect2Object(
            {
                placeholder: 'Type patient\'s name',
                url: `/SmartOncology/GetAutocompletePatientData`
            }
        )
    );

    $('#patientName').on('change', function (e) {
        viewPatientData(e);
        resetSchema();
    });
    $('#patientName').on('select2:opening select2:closing', function (e) {
        autocompleteEventHandler(e);
    });

    $('#schemaName').initSelect2(
        getSelect2Object(
            {
                placeholder: 'Type schema\'s name',
                url: `/SmartOncology/GetAutocompleteSchemaData`
            }
        )
    );
    $('#schemaName').on('change', function (e) {
        viewSchema(e);
    });
    $('#schemaName').on('select2:opening select2:closing', function (e) {
        autocompleteEventHandler(e);
    });
});

function resetSchema() {
    $("#schemaData").html('');
}

