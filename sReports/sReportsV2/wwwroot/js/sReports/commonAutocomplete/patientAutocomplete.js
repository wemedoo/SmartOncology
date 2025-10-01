function searchForPatient() {
    page = 1;
    reloadPatients(false);
}

function loadMorePatients() {
    page++;
    reloadPatients(true);
}

function reloadPatients(loadMore) {
    let requestObject = {};
    requestObject.Name = $('#patientSearch').val().toLowerCase();
    requestObject.Page = page;
    requestObject.PageSize = 20;

    if (requestObject.Name.length > 2) {
        callServer({
            method: 'get',
            url: `/Patient/ReloadPatients`,
            data: requestObject,
            success: function (data) {
                if (loadMore) {
                    $('#patientOptions').append(data);
                    document.getElementById("patientSearch").focus();
                }
                else
                    $('#patientOptions').html(data);
                $('#loadPatients').remove();
                $('#patientOptions').show();
                if (data.trim()) {
                    if ($('#patientOptions').find(".option").length >= requestObject.PageSize * page) {
                        $('#patientOptions').append(appendLoadMore());
                    }
                }
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr);
            }
        });
    }
}

function appendLoadMore() {
    let divElement = document.createElement('div');
    $(divElement).addClass("load-more-button-container");
    $(divElement).addClass("load-more-umls");
    divElement.id = "loadPatients";
    let loadMoreElement = document.createElement('div');
    $(loadMoreElement).addClass("load-more-button");
    $(loadMoreElement).addClass("load-concepts");
    loadMoreElement.onclick = function () { loadMorePatients() };
    var LoadMoreText = loadMore;
    var element = $(loadMoreElement).append(LoadMoreText)
    $(divElement).append(element);

    return divElement;
}

var page = 1;