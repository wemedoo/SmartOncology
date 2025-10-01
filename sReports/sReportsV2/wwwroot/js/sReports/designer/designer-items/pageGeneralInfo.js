$(document).on('click', '#submit-page-info', function (e) {
    if ($('#pageGeneralInfo').valid()) {
        createNewThesaurusIfNotSelected();

        let title = $('#title').val();
        let elementId = getOpenedElementId();
        let element = getElement('page', title);

        if (element) {
            $(element).attr('data-id', decode(elementId));
            $(element).attr('data-title', decode($('#title').val()));
            $(element).attr('data-thesaurusid', decode($('#thesaurusId').val()));
            $(element).attr('data-description', decode($('#description').val()));
            $(element).attr('data-isvisible', decode($('#isVisible').is(":checked")));
            setPageLayout(element);
            setImageMap(element);
            updateTreeItemTitle(element, title);
            closDesignerFormModal(true);
            clearErrorFromElement(elementId);
        }
    }
    else {
        toastr.error("Page informations are not valid");
    }
})

function setPageLayout(element) {
    let layoutType = $('#layoutType').val();
    let layouMaXItems = $('#layoutMaxItems').val();
    let layout = null;
    if (layoutType || layouMaXItems) {
        layout = getDataProperty(element, 'layoutstyle') || {};
        layout['layoutType'] = layoutType;
        layout['maxItems'] = layouMaXItems;
    }
    $(element).attr('data-layoutstyle', encodeURIComponent(JSON.stringify(layout)));
}

function setImageMap(element) {
    let imgUrl = $('#imagemapUrl').val();
    let mapId = getImageMapId();
    let imageMapAreas = getImageMapAreas();

    if (imgUrl && mapId) {
        let imageMapData = {
            url: imgUrl,
            mapId: mapId,
            map: imageMapAreas
        };
        $(element).attr('data-imagemap', encodeURIComponent(JSON.stringify(imageMapData)));
    } else {
        $(element).attr('data-imagemap', null);
    }
}

function getImageMapId() {
    let map = $('.imagemaps-wrapper map').first();
    let result = $(map).attr('id').replace('designer-', '');
    return result;
}

function getImageMapAreas() {
    let map = $('.imagemaps-wrapper map').first();
    return $(map).html().trim();
}

function readImgMapImageURL(input) {
    if (input?.files?.[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imageWithMap').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]); // convert to base64 string
        let fileData = { id: $(input).attr('data-id'), content: input.files[0] };
        sendFileData([fileData], setResourceName, handleImageMapImgUpload, getBinaryDomain('imageMap'));

        $('.file-upload-button').hide();
        $('#imageWithMap').show();
        $('.imagemaps-control').show();
    }
}

function handleImageMapImgUpload() {
    let mapId = $('#imageWithMap').attr('usemap');
    triggerResize();
    $(mapId).imageMapResize();
}

$(document).on('change', '#imgMapImgFile', function () {
    readImgMapImageURL(this);
});

function loadFieldSets(oldTemplate, pageId) {
    if (pageId) {
        let template = $(oldTemplate).clone();
        let selectFieldElementFs = $(template).find('.area-fieldset');
        $(selectFieldElementFs).find('option').each(function (index, element) {
            $(element).remove();
        })

        $(`#nestable li.dd-item[data-id=${pageId}] li.dd-item[data-itemtype=fieldset]`).each(function (index, element) {
            if (decode($(element).attr('data-label'))) {
                $(selectFieldElementFs).append(`<option value=${$(element).attr('data-id')}>${decode($(element).attr('data-label'))}</option>`);
            }
        });         
        return template;
    }

}

$(document).on("click", ".rotate-left-button img", function (e) {
    e.stopPropagation();
    let rotateValue = 5;
    let rect = $(this).parent().parent();
    let rotate = +$(rect).attr('data-rotate');
    if (rotate) {
        rotate += rotateValue;
        $(rect).attr('data-rotate', rotate);
        $(rect).css("transform", `rotate(${rotate}deg)`);
    } else {
        $(rect).attr('data-rotate', rotateValue);
        $(rect).css("transform", `rotate(5deg)`);
    }
});

$(document).on('click', '.file-upload-button', function (e) {
    $('#imgMapImgFile').click();
})

$(document).on('click', '.remove-file-icon', function (e) {
    let imageName = $('#imagemapUrl').val();
    removeRects();
    $('.imagemaps-control').hide();
    $('#imageWithMap').hide();
    $('#imageWithMap').attr('src', '');
    $('#imagemapUrl').val('');
    $('.file-upload-button').show();
    emptyImagemapTable();
    removeAreas();
    $('.imagemaps-wrapper').attr('data-count', 0);
    deleteExistingBinaryFromServer(imageName, 'imageMap');
});

function removeRects() {
    $('.image-map-rect').each(function (index, element) {
        $(element).remove();
    })
}

function emptyImagemapTable() {
    $('.imagemaps-output').find('tr').each(function (index, element) {
        $(element).remove();
    })
}

function removeAreas(){
    $('.imagemaps area').each(function (index, element) {
        $(element).remove();
    });
}


$(document).on('mouseover', '.page-custom-dd-handle', function (e) {

    $(e.target).closest('li').children('button').addClass('white');
});

$(document).on('mouseout', '.page-custom-dd-handle', function (e) {

    $(e.target).closest('li').children('button').removeClass('white');
});

