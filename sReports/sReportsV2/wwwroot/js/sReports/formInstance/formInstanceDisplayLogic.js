function setFormInstanceContent(data) {
    $("#formInstanceContentContainer").html(data);
    var lastUpdateValue = $("input[name='LastUpdate']").val();
    $('#buttonSubmitDelete').attr('data-dynamiclastupdate', lastUpdateValue);
}

function getPreviousActivePage() {
    var previousActivePage = $('.page').filter(function () {
        return $(this).css('display') === 'block';
    });

    return $(previousActivePage);
}

function getPreviousActivePageId() {
    return getPreviousActivePage().attr("id");
}

function getPreviousActivePageTab() {
    return $('.pages-link.active');
}

function getPreviousActiveChapterId() {
    return $('.chapter-li.active').data('id');
}

function getPreviousActiveChapterPageTabs() {
    let activeChapterId = getPreviousActiveChapterId();
    let $pageTabs = $(`#pagesTabs-chapter-${activeChapterId}`);
    return $pageTabs;
}

function getSwitchFormInstanceViewModeRequest(viewMode, isReadOnly) {
    var request = {};

    request["viewMode"] = viewMode;
    request["formInstanceId"] = getFormInstanceId();
    request["isReadOnlyViewMode"] = isReadOnly;
    request['activeChapterId'] = getPreviousActiveChapterId();
    request['activePageId'] = getPreviousActivePageId();
    request['ActivePageLeftScroll'] = getPreviousActiveChapterPageTabs().attr("data-current-left-scroll");
    request['hiddenFieldsShown'] = getIsHiddenFieldsShown();;

    return request;
}

function downloadSynopticPdf(formTitle) {
    request = {};
    request['formInstanceId'] = getFormInstanceId();

    getDocument('/Pdf/GetSynopticPdf', formTitle, '.pdf', {formInstanceId: getFormInstanceId()});
}

function switchFormInstanceViewMode(viewMode, isPatientModule, toggleBtn, isReadOnly) {
    if (toggleBtn) {
        $(".form-instance-view-mode").toggle();
    }

    let controller = isPatientModule ? 'DiagnosticReport' : 'FormInstance';
    let url = `/${controller}/GetFormInstanceContent`;
    getFormInstanceContent(viewMode, isReadOnly, url);
}

function getFormInstanceContent(viewMode, isReadOnly, url) {
    callServer({
        type: "GET",
        url: url,
        data: getSwitchFormInstanceViewModeRequest(viewMode, isReadOnly),
        success: function (data) {
            setFormInstanceContent(data);
            showAdministrativeArrowIfOverflow('administrative-container-form-instance');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getLastUpdate() {
    return $("input[name=LastUpdate]").val();;
}

function getFormInstanceId() {
    return $("input[name=formInstanceId]").val();
}

function getIsHiddenFieldsShown() {
    return $('#showHiddenFieldsAction').hasClass('d-none');
}

function reloadAfterFormInstanceChange() {
    switchFormInstanceViewMode('RegularView', isPatientModule(), false, false);
}

function changeImageSrc(source, restore) {
    var imagePrefix = "/css/img/icons/";
    var imageElement;

    switch (source) {
        case "synoptic_view":
            imageElement = document.getElementById("synopticViewImage");
            break;
        case "audit_trail":
            imageElement = document.getElementById("auditTrailImage");
            break;
        case "form_instance_view":
            imageElement = document.getElementById("formInstanceView");
            break;
        case "download_synoptic":
            imageElement = document.getElementById("downloadSynoptic");
            break;
        case "show_hidden_fields":
            imageElement = document.getElementById("showHiddenFieldsImage");
            break;
        case "hide_hidden_fields":
            imageElement = document.getElementById("hideHiddenFieldsImage");
            break;
        default:
            console.error("Unknown image source:", source);
            return;
    }

    if (restore)
        imageElement.src = imagePrefix + source + ".svg";
    else
        imageElement.src = imagePrefix + source + "_dark.svg";
}