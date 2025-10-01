function formatConnectedFieldResult(node) {
    let color = isNotLeaf(node) ? 'color: lightgray;' : '';
    var $result = $(`<span style="padding-left: ${(20 * node.level)}px; ${color}"> ${node.text} </span>`);
    return $result;
};

function setCustomConnectedFields(element) {
    if (element) {
        $(element).attr('data-connectedFieldIds', encodeURIComponent(JSON.stringify(getConnectedFieldIds())));
        setCommonStringFields(element);
    }

}

function getConnectedFieldIds() {
    var selectedConnectedFieldIds = [];

    $("#connected option:selected").each(function () {
        selectedConnectedFieldIds.push($(this).val());
    });

    return selectedConnectedFieldIds;
}

function isNotLeaf(data) {
    return data.level !== '3';
}

function loadConnectedFieldOptions(dataSource, selectedValues) {
    $('#connected').initSelect2(
        getSelect2Object(
            {
                placeholder: 'Select an option',
                width: '100%',
                allowClear: true,
                modalId: 'designerFormModal',
                initialDataSource: dataSource,
                templateResult: formatConnectedFieldResult,
                minimumInputLength: 0
            }
        )
    );

    if (selectedValues && selectedValues.length > 0) {
        $('#connected').val(selectedValues);
        $('#connected').trigger('change');
    }

    $('#connected').on('select2:selecting', function (e) {
        const selectedId = e.params.args.data.id;
        let selectedOptionData = dataSource.find(d => d.id == selectedId);
        if (isNotLeaf(selectedOptionData)) {
            e.preventDefault();
        }
    });
}