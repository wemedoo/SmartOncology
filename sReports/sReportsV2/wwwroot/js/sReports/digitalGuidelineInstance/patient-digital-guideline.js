function addNewGuidelineInstance(e) {
    e.preventDefault();
    let episodeOfCareId = $('#patientGeneralInfoContainer').attr('data-eocid');
    callServer({
        method: 'get',
        url: `/DigitalGuidelineInstance/ListDigitalGuidelines?episodeOfCareId=${episodeOfCareId}`,
        success: function (data) {
            let modalMainContent = document.getElementById('guidelineInstanceMainContent');
            modalMainContent.innerHTML = data;
            $('body').addClass('no-scrollable');
            $('.guideline-instance-modal').addClass('show');
            $('.guideline-instance-modal').trigger('lowZIndex');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function insertGuidelineInstance(e) {
    let selectedFormElement = $('.single-form-list-element.active').first();
    if (selectedFormElement.length === 0) {
        toastr.error('Please select a digital guideline');
        return;
    }
    let digitalGuidelineId = $(selectedFormElement).attr('data-guidelineid');
    let title = $(selectedFormElement).attr('data-guidelinetitle');
    let episodeOfCareId = $('#patientGeneralInfoContainer').attr('data-eocid');
    callServer({
        type: "POST",
        data: { episodeOfCareId, digitalGuidelineId, title },
        url: `/DigitalGuidelineInstance/Create`,
        success: function (data) {
            reloadDigitalGuidelineTable(episodeOfCareId);
            closeGuidelineInstanceModal();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function searchDigitalGuideline(){
    var title = $('#searchDigitalGuideline').val();
    callServer({
        method: 'GET',
        data: {title},
        url: `/DigitalGuidelineInstance/FilterDigitalGuidelines`,
        success: function (data) {
            $('#formsContainer').html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function removeGuidelineInstance(){
    var guidelineInstanceId = document.getElementById("buttonSubmitDelete").getAttribute('data-id')

    callServer({
        type: "DELETE",
        url: `/DigitalGuidelineInstance/Delete?guidelineInstanceId=${guidelineInstanceId}`,
        success: function (data) {
            toastr.success(`Success`);
            $(`#${guidelineInstanceId}`).remove();
            $('#deleteModal').modal('hide');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
    //reloadDigitalGuidelineTable(episodeOfCareId);
}

function previewInstanceNode(event, data, elementId, guidelineId) {
    callServer({
        method: 'post',
        url: `/DigitalGuidelineInstance/PreviewInstanceNode?guidelineInstanceId=${elementId}&guidelineId=${guidelineId}`,
        data: data,
        success: function (data) {
            $(`#nodePreview-${elementId}`).html(data);
            $('#showInstanceNodeButton').click();
            showInstanceNodePreview(event, elementId);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function previewInstanceDecisionNode(event, data, elementId, guidelineId) {
    callServer({
        method: 'post',
        url: `/DigitalGuidelineInstance/PreviewInstanceDecisionNode?guidelineInstanceId=${elementId}&guidelineId=${guidelineId}`,
        data: data,
        success: function (data) {
            $(`#nodePreview-${elementId}`).html(data);
            $('#showInstanceNodeButton').click();
            showInstanceNodePreview(event, elementId);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function unselectInstanceNode(elementId) {
    $(`#nodePreview-${elementId}`).hide();
}

function showInstanceNodePreview(event, elementId) {
    event.preventDefault();
    $(event.target).siblings().removeClass('active');
    $(event.target).addClass('active');
    $(`#nodePreview-${elementId}`).show();
}

function loadGraph(id, digitalGuidelineId, refreshIfExisting) {
    if (!refreshIfExisting) {
        $(".guideline-container").remove();
        $(".guideline-header").remove();
    }
    toggleGuidelineInstances(id);
    callServer({
        method: 'GET',
        url: `/DigitalGuidelineInstance/LoadGraph?guidelineInstanceId=${id}&guidelineId=${digitalGuidelineId}`,
        success: function (data) {
            appendGraphToTable(id, data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function toggleGuidelineInstances(guidelineInstanceId) {
    var previousSelected = $('.single-guideline-instance-container.selected');
    $(previousSelected).removeClass('selected');

    var currentSelected = $(`#${guidelineInstanceId}`);
    setGuidelineInstanceArrows($(currentSelected).siblings().find('i.fas.fa-chevron-down'), $(currentSelected).siblings().find('i.fas.fa-chevron-up'));

    toggleCurrentSelected($(currentSelected));
    $(currentSelected).addClass('selected');
}

function toggleCurrentSelected(currentSelected) {
    if ($(currentSelected).find('i.fas.fa-chevron-up').hasClass("hide")) {
        setGuidelineInstanceArrows($(currentSelected).find('i.fas.fa-chevron-up'), $(currentSelected).find('i.fas.fa-chevron-down'));
    } else {
        setGuidelineInstanceArrows($(currentSelected).find('i.fas.fa-chevron-down'), $(currentSelected).find('i.fas.fa-chevron-up'));
    }
}

function setGuidelineInstanceArrows($itemsToShow, $itemsToHide) {
    $itemsToShow.removeClass("hide");
    $itemsToHide.addClass("hide");
}

$(document).on('keyup', '#searchDigitalGuideline', function (e) {
    searchDigitalGuideline();
})

function backToPatient(patientId, episodeOfCareId) {
    window.location.href = `/Patient/Edit?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}`;
}

function markAsCompleted(id) {
    var value = $("#valueId").val();
    var guidelineInstanceId = $("#nodeGuidelineInstanceId").val();
    var guidelineId = $("#nodeGuidelineId").val();
    callServer({
        method: 'GET',
        data: { value: value, nodeId: id, guidelineInstanceId: guidelineInstanceId, guidelineId: guidelineId },
        url: `/DigitalGuidelineInstance/MarksAsCompleted`,
        success: function (data) {
            appendGraphToTable(guidelineInstanceId, data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
    loadGraph(guidelineInstanceId, guidelineId, true);
}

function appendGraphToTable(guidelineInstanceId, data) {
    if (!document.getElementById(`digitalInstanceCy-${guidelineInstanceId}`).classList.contains("collapseGraph")) {
        $(`#digitalInstanceCy-${guidelineInstanceId}`).html(data);
        var x = document.getElementsByClassName("digitalInstanceCy");
        for (i = 0; i < x.length; i++) {
            if (x[i].id == `digitalInstanceCy-${guidelineInstanceId}`) {
                $(`#digitalInstanceCy-${guidelineInstanceId}`).addClass("collapseGraph");
            }
            else {
                $(`#${x[i].id}`).removeClass("collapseGraph");
            }
        }
    }
    else
        $(".digitalInstanceCy").removeClass("collapseGraph");
}

function addValueFromDocument(e) {
    e.preventDefault();
    let episodeOfCareId = $('#episodeOfCareId').val();
    callServer({
        method: 'get',
        url: `/DigitalGuidelineInstance/ListGuidelineDocuments?episodeOfCareId=${episodeOfCareId}`,
        success: function (data) {
            let modalMainContent = document.getElementById('guidelineDocumentMainContent');
            modalMainContent.innerHTML = data;
            $('body').addClass('no-scrollable');
            $('.guideline-document-modal').addClass('show');
            $('.guideline-document-modal').trigger('lowZIndex');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function searchGuidelineDocument() {
    var title = $('#searchDigitalGuidelineDocument').val();
    let episodeOfCareId = $('#patientGeneralInfoContainer').attr('data-eocid');

    callServer({
        method: 'GET',
        data: {episodeOfCareId, title},
        url: `/DigitalGuidelineInstance/FilterGuidelineDocuments`,
        success: function (data) {
            $('#formsDocumentContainer').html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    })
}

$(document).on('keyup', '#searchDigitalGuidelineDocument', function (e) {
    searchGuidelineDocument();
})

function insertValueFromDocument(e) {
    let selectedFormElement = $('.single-form-list-element.active').first();
    if (selectedFormElement.length === 0) {
        toastr.error('Please select a document');
        return;
    }
    let formInstanceId = $(selectedFormElement).data('forminstanceid');
    let thesaurusId = $("#nodeThesaurusId").val() ? $("#nodeThesaurusId").val() : -1;
    callServer({
        method: 'GET',
        url: `/DigitalGuidelineInstance/GetValueFromDocument?formInstanceId=${formInstanceId}&thesaurusId=${thesaurusId}`,
        success: function (data) {
            if (data == "")
                toastr.error(`Not found field with same thesaurus id in selected document!`)
            else
                document.getElementById('valueId').value = data;
            closeGuidelineInstanceModal();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function chooseCondition(event, nodeId, digitalGuidelineId, guidelineInstanceId) {
    event.preventDefault();
    callServer({
        method: 'get',
        url: `/DigitalGuidelineInstance/GetConditions?nodeId=${nodeId}&digitalGuidelineId=${digitalGuidelineId}&guidelineInstanceId=${guidelineInstanceId}`,
        data: {},
        success: function (data) {
            let modalMainContent = document.getElementById('guidelineConditionMainContent');
            modalMainContent.innerHTML = data;
            $('body').addClass('no-scrollable');
            $('.guideline-condition-modal').addClass('show');
            $('.guideline-condition-modal').trigger('lowZIndex');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function saveCondition() {
    let selectedFormElement = $('.single-form-list-element.active').first();
    if (selectedFormElement.length === 0) {
        toastr.error('Please select a condition');
        return;
    }
    let condition = $(selectedFormElement).data('condition');
    var nodeId = $('#nodeId').val();
    let guidelineInstanceId = $('#guidelineInstanceId').val();
    let digitalGuidelineId = $('#digitalGuidelineId').val();

    callServer({
        method: 'get',
        url: `/DigitalGuidelineInstance/SaveCondition?condition=${condition}&nodeId=${nodeId}&guidelineInstanceId=${guidelineInstanceId}&digitalGuidelineId=${digitalGuidelineId}`,
        data: {},
        success: function (data) {
            closeGuidelineInstanceModal();
            appendGraphToTable(guidelineInstanceId, data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
    loadGraph(guidelineInstanceId, digitalGuidelineId, true);
}


function reloadDigitalGuidelineTable(eocId) {
    callServer({
        type: "GET",
        url: `/DigitalGuidelineInstance/GuidelineInstanceTable?episodeOfCareId=${eocId}`,
        data: {},
        success: function (data) {
            $("#guidelineTableContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function closeGuidelineInstanceModal() {
    $('.digital-guideline-modal').removeClass('show');
    $('body').removeClass('no-scrollable');
    $('.digital-guideline-modal').trigger('defaultZIndex');
}

$(document).on('click', '.close-guideline-instance-modal-button', function (e) {
    closeGuidelineInstanceModal();
});

$(document).on('click', '.single-form-list-element', function (e) {
    $(this).siblings().removeClass('active');
    $(this).toggleClass('active');
});