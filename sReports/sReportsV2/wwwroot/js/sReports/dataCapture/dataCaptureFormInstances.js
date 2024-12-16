var params;

function createFormInstance(id, language, projectId, projectName, showUserProject) {
    if (simplifiedApp) {
        window.location.href = `/crf/create?id=${id}&language=${language}`;
    } else {
        if (projectId !== "") {
            const apiUrl = showUserProject !== ""
                ? `/FormInstance/CreateForUserProject?VersionId=${filter.versionId}&ThesaurusId=${filter.thesaurusId}&ProjectId=${filter.projectId}&ProjectName=${projectName}`
                : `/FormInstance/CreateForProject?VersionId=${filter.versionId}&ThesaurusId=${filter.thesaurusId}&ProjectId=${filter.projectId}&ProjectName=${projectName}`;

            fetchFormInstance(apiUrl);
        } else {
            const apiUrl = `/FormInstance/Create?VersionId=${filter.versionId}&ThesaurusId=${filter.thesaurusId}`;
            fetchFormInstance(apiUrl);
        }
    }
}

function fetchFormInstance(url) {
    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            var existingDiv = document.getElementById('temporalFormInstanceDiv');
            existingDiv.innerHTML = html;
            submitForm();
        })
        .catch(error => {
            console.error('Error loading content:', error);
        });
}

function createPdfFormInstance(event, formId) {
    event.stopPropagation();
    event.preventDefault();
    
    $(window).unbind('beforeunload');
    window.location.href = `/Pdf/GetPdfForFormId?formId=${formId}`;

    $(window).on('beforeunload', function (event) {
        $("#loaderOverlay").show(100);
    });
}

function uploadPDF(event) {
    event.stopPropagation();
    event.preventDefault();

    var fd = new FormData(),
        myFile = document.getElementById("file").files[0];

    fd.append('file', myFile);

    callServer({
        url: `/Pdf/Upload`,
        data: fd,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (data) {
            $("#uploadModal").modal('toggle');
            toastr.success(`Success`);
            reloadTable();
            removeFile();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#uploadModal").modal('toggle');
            handleResponseError(xhr);
        }
    });
    return false;
}

function editFormDefinition(id) {
    window.location.href = `/Form/Edit?formId=${id}`;
}

function downloadTxt(event) {
    event.stopPropagation();
    event.preventDefault();
    var chkArray = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        chkArray.push(element);
    });

    var numOfSelectedDocuments = chkArray.length;
    if (numOfSelectedDocuments === 0) {
        toastr.warning("Please select at least one document for export.");
        return;
    }
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');

        exportToTxt(formId, formTitle, i === numOfSelectedDocuments - 1);
    }
}

function exportToTxt(id, formTitle, lastFile = true) {
    getDocument('/FormInstance/ExportToTxt', formTitle, '.txt', { formInstanceId: id }, { lastFile });
}

function editEntity(event, id, projectId) {
    event.preventDefault();
    if (simplifiedApp) {
        let language = $('.dropdown-menu').find('.language.active')[0];
        url = `${simplifiedApp}?FormInstanceId=${id}&language=${$(language).data('value')}`;
    } else {
        if (projectId != "") {
            if ($("#showUserProjects").val() == "true") {
                url = `/FormInstance/EditForUserProject?VersionId=${filter.versionId}&FormInstanceId=${id}&ProjectId=${projectId}`;
            }
            else {
                url = `/FormInstance/EditForProject?VersionId=${filter.versionId}&FormInstanceId=${id}&ProjectId=${projectId}`;
            }
        }
        else {
            url = `/FormInstance/Edit?VersionId=${filter.versionId}&FormInstanceId=${id}`;
        }
    }
    window.location.href = url;
}

function viewEntity(event, id) {
    event.preventDefault();
    let url = `/FormInstance/View?VersionId=${filter.versionId}&FormInstanceId=${id}`;
    window.location.href = url;
}

function removeFormInstance(event, id, lastUpdate) {
    event.preventDefault();
    event.stopPropagation();
    callServer({
        type: "DELETE",
        url: `/FormInstance/Delete?formInstanceId=${id}&lastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success('Removed');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}


function reloadTable(initLoad) {
    let requestObject = getFilterParametersObject();
    checkUrlPageParams();
    setFilterTagsFromObj(requestObject);
    setTableProperties(requestObject);
    callServer({
        type: 'GET',
        url: `/FormInstance/ReloadByFormThesaurusTable?showUserProjects=${$('#showUserProjects').val()}`,
        data: requestObject,
        traditional: true, // Explanation: https://stackoverflow.com/questions/31152130/is-it-good-to-use-jquery-ajax-with-traditional-true/31152304#31152304
        success: function (data) {
            setTableContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getFilterParametersObject() {
    let requestObject = {};

    if (filter) {
        requestObject = filter;
        defaultFilter = null;
    } else {
        requestObject.Content = $('#content').val();
        //requestObject.UserIds = setUser();
        //requestObject.PatientIds = setPatient();
        requestObject.VersionId = $('#VersioId').val();
        requestObject.ThesaurusId = $('#thesaurusId').val();
    }

    return requestObject;
}

document.getElementById("file").onchange = function () {
    document.getElementById("uploadFile").value = this.value.replace("C:\\fakepath\\", "");
};

function downloadJsons(event) {
    event.preventDefault();
    event.stopPropagation();
    var chkArray = [];
    $("input:checkbox[name=checkboxDownload]:checked").each(function (index, element) {
        chkArray.push(element);
    });

    var numOfSelectedDocuments = chkArray.length;
    if (numOfSelectedDocuments === 0) {
        toastr.warning("Please select at least one document for export.");
        return;
    }
    for (var i = 0; i < numOfSelectedDocuments; i++) {
        var formId = $(chkArray[i]).val();
        var formTitle = $(chkArray[i]).data('title');

        getJson(formId, formTitle, i === numOfSelectedDocuments - 1);
    }
}

function getJson(formId, formTitle, lastFile = true) {
    callServer({
        url: `/Patholink/Export?formInstanceId=${formId}`,
        beforeSend: function (request) {
            request.setRequestHeader("LastFile", lastFile);
        },
        success: function (data) {
            var jsonse = JSON.stringify(data, null, 2);
            var blob = new Blob([jsonse], { type: "application/json" });
            getDownloadedFile(blob, formTitle);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function redirectToDistributionParams(thesaurusId) {
    window.location.href = `/FormDistribution/GetByThesaurusId?thesaurusId=${thesaurusId}`;
}

$(document).on('change', '#selectAllCheckboxes', function () {
    var c = this.checked;
    $(':checkbox').prop('checked', c);
});

function singleDocumentFilter() {
    $('#content').val($('#ContentTemp').val());

    if (filter) {
        filter['Content'] = $('#ContentTemp').val();
        //filter['UserIds'] = setUser();
        //filter['PatientIds'] = setPatient();
    }

    filterData();
    //clearSingleDocumentFilters();
}

function clearSingleDocumentFilters() {
    $('#ContentTemp').val(' ');
}

function advanceFilter() {

    $('#ContentTemp').val($('#content').val());

    singleDocumentFilter();
    //clearFilters();
}

function getFilterParametersObjectForDisplay(requestObject) {
    let filterObject = {};
    filterObject['Content'] = requestObject['Content'];

    return filterObject;
}