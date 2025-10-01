function createFormInstance(id, language, projectId, projectName, showUserProject) {
    if (simplifiedApp) {
        window.location.href = `/crf/create?id=${id}&language=${language}`;
    } else {
        fetchFormInstance(getCreateUrl(projectId, showUserProject, projectName));
    }
}

function getCreateUrl(projectId, showUserProject, projectName) {
    let apiUrl;
    if (projectId !== "") {
        apiUrl = showUserProject !== ""
            ? `/FormInstance/CreateForUserProject?VersionId=${filter.versionId}&ThesaurusId=${filter.thesaurusId}&ProjectId=${filter.projectId}&ProjectName=${projectName}`
            : `/FormInstance/CreateForProject?VersionId=${filter.versionId}&ThesaurusId=${filter.thesaurusId}&ProjectId=${filter.projectId}&ProjectName=${projectName}`;
    } else {
        apiUrl = `/FormInstance/Create?VersionId=${filter.versionId}&ThesaurusId=${filter.thesaurusId}`;
    }
    return apiUrl;
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
    let requestObject = applyActionsBeforeServerReloadSimple(false, true);

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

    if (defaultFilter) {
        requestObject = getDefaultFilter();
        filter = requestObject;
        defaultFilter = null;
    } else {
        if (filter) {
            requestObject = filter;
        }
        requestObject.content = $('#content').val();
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
            convertToBlobAndDownload(data, true, formTitle);
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

function getFilterParametersObjectForDisplay(requestObject) {
    let filterObject = {};
    filterObject['content'] = requestObject['content'];

    return filterObject;
}

function mainFilter() {
    filterData();
}

function advanceFilter() {
    mainFilter();
}