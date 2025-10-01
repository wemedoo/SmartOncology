function showAliasModal(e, title, aliasId) {
    e.stopPropagation();
    callServer({
        type: 'GET',
        url: `/Code/ShowAliasModal?aliasId=${aliasId}`,
        success: function (data) {
            $('#addAliasModal').html(data);
            document.getElementById("aliasCodeId").innerHTML = $('#codeValue').val();
            document.getElementById("aliasCodeDisplay").innerText = $('#codeValueDisplay').val();
            document.getElementById('title').innerHTML = title;
            $('#addAliasModal').modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, true);
        }
    });
}

function addNewAlias(e) {
    updateDisabledOptions(false);
    $('#newAliasForm').validate();
    if ($('#newAliasForm').valid()) {
        var request = {};
        var aliasId = $("#aliasId").val();
        request['AliasId'] = aliasId;
        request['InboundAliasId'] = $("#inboundAliasId").val();
        request['OutboundAliasId'] = $("#outboundAliasId").val();;
        request['System'] = $("#newAliasSystem").val();
        request['Inbound'] = $("#newAliasInbound").val();
        request['Outbound'] = $("#newAliasOutbound").val();
        request['CodeId'] = $("#codeValue").val();
        request['ActiveFrom'] = calculateDateWithOffset("#validFromDefault");
        request['ActiveTo'] = calculateDateWithOffset("#validToDefault");

        let action = aliasId != 0 ? 'EditAlias' : 'CreateAlias';
        callServer({
            type: "POST",
            url: `/Code/${action}`,
            data: request,
            success: function (data) {
                toastr.options = {
                    timeOut: 100
                }
                $('#aliasModal').modal('hide');
                showAliases(e);
                toastr.success("Success");
                $('#addAliasModal').modal('hide');
                saveInitialCodeFormData("#idCodes");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function editAlias(e, title, aliasId) {
    if (!$(e.target).hasClass('dropdown-button') && !$(e.target).hasClass('fa-bars') && !$(e.target).hasClass('dropdown-item') && !$(e.target).hasClass('dots')) {
        showAliasModal(e, title, aliasId);
    }
}

function removeAlias(event, id, inboundId, outboundId) {
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/Code/DeleteAlias?inboundAliasId=${inboundId}&outboundAliasId=${outboundId}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
            reloadTable();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}