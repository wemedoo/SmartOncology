$(document).ready(function () {
    isPathwayModule = true;
    $('#settings-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        hideAll();
        $('#settings-btn').hide();
        $('#settingsMenu').show();

        $(document).off('keydown.saveOnEnter keydown.cancelOnEscape').on('keydown.saveOnEnter', function (event) {
            if (event.key === 'Enter' && $('#pathway-title').is(':focus')) {
                event.preventDefault();
                event.stopPropagation();
                saveSettings();
                $('#pathway-title').blur();
                $('.settings-version').show();
                $('.thesaurus-input').show();
            }
        }).on('keydown.cancelOnEscape', function (event) {
            if (event.key === 'Escape' && $('#pathway-title').is(':focus')) {
                event.preventDefault();
                event.stopPropagation();
                cancelSettings();
                $('#pathway-title').blur();
                $('.settings-version').show();
                $('.thesaurus-input').show();
            }
        });
    });

    $('#inner-settings-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#settingsMenu').hide();
        $('#settings-btn').show();
        $(document).off('keydown.saveOnEnter keydown.cancelOnEscape');
    });

    $('#pathway-title').on('focus', function (e) {
        e.preventDefault();
        $('.settings-version').hide();
        $('.thesaurus-input').hide();
    });

    $('#pathway-title').on('blur', function (e) {
        e.preventDefault();
        saveSettings();
        $('.settings-version').show();
        $('.thesaurus-input').show();
    });


    $('.settings-dots').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        hideAllMenus();
        const isDropdownVisible = $('#settingsDropdownMenu').is(':visible');
        $('#settingsDropdownMenu').toggle();
        $(this).attr('src', isDropdownVisible ? '/css/img/icons/3dots.png' : '/css/img/icons/dots-active.png');
    });

    $('.dropdown-option').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        const action = $(this).data('action');
        handleDropdownAction(action);
        $('#settingsDropdownMenu').hide();
        $('.settings-dots').attr('src', '/css/img/icons/3dots.png');
    });

    $(document).on('click', function (e) {
        if (!$(e.target).closest('.settings-dots, #settingsDropdownMenu').length) {
            $('#settingsDropdownMenu').hide();
            $('.settings-dots').attr('src', '/css/img/icons/3dots.png');
        }
    });

    $('#settingsDropdownMenu').on('mouseleave', function () {
        hideAllMenus();
    });

    $('#settingsDropdownMenu').on('mouseenter', function () {
        hideAllMenus();
    });
});

$(document).on('click', '#applySearchButton', function (e) {
    setVersionTablePageSize();
    reloadTable();
});

$(document).on('click', '.search-thesaurus-table input[name="radioThesaurus"]:checked', function (e) {
    $(this).addClass('hide');
    $(this).siblings('.select-button').removeClass('hide');
    $('#thesaurusSearchInput').attr('data-thesaurusid', $(this).val() || 0);
})

$(document).on('click', '.preview-thesaurus-button input[name="radioThesaurus"]:checked', function (e) {
    $(this).addClass('hide');
    $(this).siblings('.select-button').removeClass('hide');
    $('#thesaurusSearchInput').attr('data-thesaurusid', $(this).val() || 0);
})

function hideAll() {
    $('.settings-menu').hide();
    $('#settings-btn').show();
    $('.settings-dots').attr('src', '/css/img/icons/3dots.png');
}

function saveSettings(changeVersion = false) {
    const title = $('#pathway-title').val().trim();
    const thesaurusIdRaw = $('#thesaurusDisplay').text().trim();
    const versionMajor = $('#majorVersion').val();
    const versionMinor = $('#minorVersion').val();
    const thesaurusId = parseInt(thesaurusIdRaw, 10);

    guidelineData.title = title;
    guidelineData.thesaurusId = thesaurusId;
    guidelineData.version = {
        id: guidelineData.version?.id || '',
        major: versionMajor,
        minor: versionMinor
    };
    if (cy) {
        guidelineData.guidelineElements = cy.json().elements;
    }

    if (changeVersion) {
        delete guidelineData.id;
        if (guidelineData.version?.id) {
            delete guidelineData.version.id;
        }
    }

    editorCode.set(guidelineData);
    showJsonEditor(guidelineData);
}

function cancelSettings() {
    $('#pathway-title').val(guidelineData.title || '');
}

function handleDropdownAction(action) {
    switch (action) {
        case 'edit-thesaurus':
            $('#thesaurusFilterModal').modal('show');
            isModalTable = false;
            var preferredTerm = $('#pathway-title').val();
            if (preferredTerm != "" && preferredTerm != null)
                document.getElementById("thesaurusSearchInput").value = preferredTerm;
            $('#applySearchButton').click();
            break;
        case 'edit-version':
            $('#versionModal').modal('show');
            isModalTable = true;
            setVersionTablePageSize();
            reloadModalTable();
            break;
        case 'export':
            $('#exportModal').modal('show');
            break;
        case 'commands':
            $('#commandsModal').modal('show');
            break;
        default:
            logError('Unknown action:', action);
    }
}

function thesaurusFilterModal() {
    $('#applySearchButton').click();
}

function reloadTable() {
    let inputVal = encodeURIComponent($('#thesaurusSearchInput').val());

    if (inputVal) {
        let requestObject = {};
        requestObject.Page = currentPage;
        requestObject.PageSize = getPageSize();
        requestObject.PreferredTerm = encodeURIComponent(inputVal);
        let activeThesaurus = $('#thesaurusSearchInput').attr('data-thesaurusid');
        callServer({
            method: 'get',
            url: `/ThesaurusEntry/ReloadSearchTable?preferredTerm=${encodeURIComponent(inputVal)}&activeThesaurus=${activeThesaurus}`,
            data: requestObject,
            success: function (data) {
                $('#thesaurusTableContainer').html(data);
                $('#reviewContainer').hide();
                $('.codeset-thesaurus-group').show();
            },
            error: function () {

            }
        });
    }
}

function getFilterParametersObject() {
    let result = {};
    if ($('#id').val()) {
        addPropertyToObject(result, 'id', $('#id').val());
    }

    return result;
}

function saveThesaurus(e) {
    e.preventDefault();
    e.stopPropagation();
    let activeThesaurus = $('#thesaurusSearchInput').attr('data-thesaurusid');

    $('#thesaurusDisplay').text(activeThesaurus);
    saveSettings();
    $('#thesaurusFilterModal').modal('hide');
}

function showThesaurusReview(o4mtId, event, preferredTerm) {
    var thesaurusSearchElement = document.getElementById("thesaurusSearchInput");
    thesaurusSearchElement.value = preferredTerm;
    $('.codeset-thesaurus-group').hide();
    $('.review-container').show();

    loadThesaurusPreview(o4mtId);
}

function loadThesaurusPreview(thesaurusId) {
    if (thesaurusId) {
        callServer({
            method: 'get',
            url: `/ThesaurusEntry/ThesaurusPreview?o4mtid=${thesaurusId}&activeThesaurus=${$('#thesaurusSearchInput').attr('data-thesaurusid')}`,
            success: function (data) {
                $('#reviewContainer').html(data);
                $('#reviewContainer').show();
            },
            error: function () {

            }
        });
    }
}

function closeThesaurusPreview(e) {
    e.preventDefault();
    e.stopPropagation();
    currentPage = 1;
    $('.review-container').hide();
    $('.codeset-thesaurus-group').show();
    reloadTable();
}

function saveVersion(e, originalMajor, originalMinor) {
    e.preventDefault();
    e.stopPropagation();

    const major = parseInt($('#majorVersion').val());
    const minor = parseInt($('#minorVersion').val());
    const currentText = $('#versionDisplay').text();
    const versionLabel = currentText.split(':')[0] + ': ';
    const hasChanged = major !== originalMajor || minor !== originalMinor;

    $('#versionDisplay').text(`${versionLabel}${major}.${minor}`);

    saveSettings(hasChanged);
    $('#versionModal').modal('hide');
}

function exportPathway(event, defaultTitle) {
    event.preventDefault();
    event.stopPropagation();

    const title = $('#exportName').val() || defaultTitle;
    const exportType = $('#fileType').val();
    const data = editorCode.get();

    if (exportType === 'png') {
        exportGraphAsPng(title, generateHtmlTemplate, () => {
            $('#exportModal').modal('hide');
            $('#exportModalForm')[0].reset();
        });
    }
    else if (exportType === 'PDF') {
        exportGraphAsPdf(title, generateHtmlTemplate, () => {
            $('#exportModal').modal('hide');
            $('#exportModalForm')[0].reset();
        });
    } else {
        exportToJson(title, data);
        $('#exportModal').modal('hide');
        $('#exportModalForm')[0].reset();
    }
}

function exportToJson(title, data) {
    convertToBlobAndDownload(data, true, title);
}

function exportGraphAsPng(title, generateHtmlTemplate, callback) {
    const container = document.getElementById('cy-export');
    container.innerHTML = '';

    const exportCy = cytoscape({
        container: container,
        elements: cy.json().elements,
        style: cy.style().json(),
        layout: {
            name: 'preset'
        },
        headless: false
    });

    exportCy.nodeHtmlLabel([
        {
            tpl: generateHtmlTemplate
        }
    ]);

    setTimeout(() => {
        html2canvas(container).then(canvas => {
            canvas.toBlob(blob => {
                getDownloadedFile(blob, `${title}.png`);
                exportCy.destroy();
                if (callback) callback();
            }, 'image/png', 1);
        });
    }, 1000);
}

function exportGraphAsPdf(title, generateHtmlTemplate, callback) {
    const container = document.getElementById('cy-export');
    container.innerHTML = '';

    const exportCy = cytoscape({
        container: container,
        elements: cy.json().elements,
        style: cy.style().json(),
        layout: { name: 'preset' },
        headless: false
    });

    exportCy.nodeHtmlLabel([
        { tpl: generateHtmlTemplate }
    ]);

    setTimeout(() => {
        html2canvas(container).then(canvas => {
            const imgData = canvas.toDataURL('image/png');
            const { jsPDF } = window.jspdf;
            const pdf = new jsPDF({
                orientation: 'landscape',
                unit: 'px',
                format: [canvas.width, canvas.height]
            });

            pdf.addImage(imgData, 'PNG', 0, 0, canvas.width, canvas.height);
            pdf.save(`${title}.pdf`);
            exportCy.destroy();

            if (callback) callback();
        });
    }, 1000);
}

function reloadModalTable(columnName, isAscending) {
    let requestObject = {};
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();
    requestObject.IsAscending = isAscending;
    requestObject.ColumnName = columnName;
    const guidelineId = $('#guidelineId').val();
    requestObject.ThesaurusId = $('#thesaurusSearchInput').attr('data-thesaurusid');

    callServer({
        method: 'get',
        url: `/DigitalGuideline/ReloadVersionHistoryTable?guidelineId=${guidelineId}`,
        data: requestObject,
        success: function (data) {
            $('#versionHistoryTableContainer').html(data);
            if (columnName)
                setTableContent(data);
        },
        error: function () {

        }
    });
}

function setVersionTablePageSize() {
    currentPage = 1;
    if (document.getElementById("pageSizeSelector") != null)
        document.getElementById("pageSizeSelector").id = "pageSizeThesaurusSelector";
}

function sortVersionHistoryTable(column) {
    if (switchcount == 0) {
        if (columnName == column)
            isAscending = checkIfAsc(isAscending);
        else
            isAscending = true;
        switchcount++;
    }
    else {
        if (columnName != column)
            isAscending = true;
        else
            isAscending = checkIfAsc(isAscending);
        switchcount--;
    }
    columnName = column;
    reloadModalTable(columnName, isAscending);
}

function removeEntry(event, id, lastUpdate) {
    event.stopPropagation();
    event.preventDefault();
    callServer({
        type: "DELETE",
        url: `/DigitalGuideline/Delete?id=${id}&&LastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function downloadPathwayVersion(event, title, data) {
    event.preventDefault();
    event.stopPropagation();

    exportToJson(title, data);
}

function setInitialSettingsState() {
    initialSettingsState = {
        title: $('#pathway-title').val()?.trim() || '',
        thesaurusId: $('#thesaurusDisplay').text()?.trim() || '',
        versionMajor: $('#majorVersion').val() || '1',
        versionMinor: $('#minorVersion').val() || '0'
    };
}