function exportCSV(event) {
    event.stopPropagation();
    event.preventDefault();
    myFile = document.getElementById("file").files[0];
    var title = myFile.name;
    getDocument(`/CSVExport/ExportCSV`, title, '.csv');
}

function exportFromUMLS(event) {
    event.stopPropagation();
    event.preventDefault();
    var term = $("#term").val();
    getDocument(`/CSVExport/ExportFromUMLS`, term, '.csv', {term});
}

function upload() {
    var fd = new FormData(),
        myFile = document.getElementById("file").files[0];

    fd.append('file', myFile);

    callServer({
        url: `/CSVExport/SetFirstCSV`,
        data: fd,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
    return false;
}

function uploadSecond() {
    var fd = new FormData(),
        myFile = document.getElementById("file2").files[0];

    fd.append('file', myFile);

    callServer({
        url: `/CSVExport/SetSecondCSV`,
        data: fd,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
    return false;
}

document.getElementById("file").onchange = function () {
    document.getElementById("uploadFile").value = this.value.replace("C:\\fakepath\\", "");
    upload();
};

document.getElementById("file2").onchange = function () {
    document.getElementById("uploadFile2").value = this.value.replace("C:\\fakepath\\", "");
    uploadSecond();
};