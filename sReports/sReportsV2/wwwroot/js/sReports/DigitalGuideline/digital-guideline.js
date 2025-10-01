let initialGraphState = "";
var cy;
let edgeCreationMode = false;
let sourceNode = null;
let targetNode = null;
let tempEdge = null;
let tempSourceNode = null;
let tempTargetNode = null;
let pathwayHistory = [];
let historyIndex = -1;
let currentMode = null;
let currentEdgeMode = null;
let selectedDirection = null;
let hoveredNode = null;
let lastAddedNodePosition = null;
let selectedArrowType = null;
let copiedNode = null;
let lastMousePosition = { x: 0, y: 0 }; 
let initialSettingsState = null;

$(document).ready(function () {
    if (elementData) {
        initializeGraph(elementData);
    }

    if (guidelineData) {
        showJsonEditor(guidelineData);
    }

    $('#select-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#shape-dropdown').hide();
        $('#arrow-dropdown').hide();
        hideAllMenus();
    });

    $('#add-node-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#arrow-dropdown').hide();
        hideAllMenus();

        $('#shape-dropdown').css({ left: '', top: '', position: 'absolute' }).toggle();
        currentMode = 'add-node';
    });

    $('.shape-option').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        const selectedShape = $(this).data('shape');

        if (currentMode === 'add-node') {
            addNodeWithShape(selectedShape);
        }
        else if (currentMode === 'change-shape') {
            changeNodeShape(selectedShape, cy.$(':selected'));
        }
        else {
            addNodeAndEdge(selectedShape, hoveredNode, selectedDirection);
        }

        hideAllMenus();
        resetInteractionState();
        activateSelectIcon();
    });

    $('#add-edge-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        hideAllMenus();
        $('#arrow-dropdown').toggle();
    });

    $('.arrow-option').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        selectedArrowType = $(this).data('arrow');

        if (currentEdgeMode != null) {
            updateEdgeType(selectedArrowType);
            saveState();
        }
        else {
            enableEdgeCreationMode(selectedArrowType);
        }
        $('#arrow-dropdown').hide();
        activateSelectIcon();
    });

    $(document).on('click', function (e) {
        const isSubmitButton = $(e.target).closest('.submit-data').length > 0;

        if (!$(e.target).closest('#visual-editor-menu, #node-context-menu, #node-info-dropdown, #edge-context-menu, #edge-info-dropdown, #change-shape-dropdown, #change-arrow-dropdown').length) {
            if (!isSubmitButton) {
                hideAllMenus();
                activateSelectIcon();
            }
        }
    });

    $(document).on('keydown', function (e) {
        if (e.key === 'Backspace' || e.key === 'Delete') {
            const activeEl = document.activeElement;
            const isInputActive = activeEl && (
                activeEl.tagName === 'INPUT' ||
                activeEl.tagName === 'TEXTAREA' ||
                activeEl.isContentEditable
            );

            if ($('#node-info-dropdown').is(':visible') || $('#edge-info-dropdown').is(':visible') || isInputActive) {
                return;
            }
            e.preventDefault();
            const deleteBtn = document.getElementById('delete-node');
            if (deleteBtn) {
                deleteBtn.click();
            }
            const deleteEdgeBtn = document.getElementById('delete-edge');
            if (deleteEdgeBtn) {
                deleteEdgeBtn.click();
            }
        }

        if (e.key === 'Escape') {
            hideAllMenus();
        }

        if (e.ctrlKey) {
            const key = e.key.toLowerCase();
            const activeElement = document.activeElement;

            const isTextInput =
                activeElement.tagName === 'INPUT' ||
                activeElement.tagName === 'TEXTAREA' ||
                activeElement.isContentEditable;

            const hasSelection = window.getSelection().toString().length > 0;

            const allowCustomHandling = !isTextInput && !hasSelection;

            if (key === 'c' && allowCustomHandling) {
                const selectedNodes = cy.$('node:selected');
                if (selectedNodes.length > 0) {
                    e.preventDefault();
                    copiedNode = selectedNodes[0];
                }
            } else if (key === 'x' && allowCustomHandling) {
                const selectedNodes = cy.$('node:selected');
                if (selectedNodes.length > 0) {
                    e.preventDefault();
                    copiedNode = selectedNodes[0];
                    cy.remove(copiedNode);
                    updateJsonEditor();
                    saveState();
                }
            } else if (key === 'v' && allowCustomHandling) {
                if (copiedNode) {
                    e.preventDefault();
                    duplicateNodeAtMousePosition(copiedNode, lastMousePosition);
                }
            }
        }
    });

    $('#zoom-in-btn, #zoom-out-btn, #reset-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        hideAllMenus();
    });

    $('#zoom-in-btn').on('click', () => {
        activateSelectIcon();
        cy.zoom(cy.zoom() * 1.2);
        cy.center();
    });

    $('#zoom-out-btn').on('click', () => {
        activateSelectIcon();
        cy.zoom(cy.zoom() / 1.2);
        cy.center();
    });

    $('#reset-btn').on('click', () => {
        activateSelectIcon();
        initGraphZoom();
    });

    $('#undo-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#direction-arrows').hide();
        undo();
    });

    $('#redo-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $('#direction-arrows').hide();
        redo();
    });

    $('.menu-left button').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        toggleButtonHoverState($(this));
    });

    $('.shape-option, .arrow-option').hover(
        function () {
            const img = $(this).find('img');
            const src = img.attr('src');
            if (!src.includes(' hover.svg')) {
                img.attr('src', src.replace('.svg', ' hover.svg'));
            }
        },
        function () {
            const img = $(this).find('img');
            img.attr('src', img.attr('src').replace(' hover.svg', '.svg'));
        }
    );

    $('.menu-right button').hover(
        function () {
            const img = $(this).find('img');
            if (img.length) {
                const src = img.attr('src');
                if (!src.includes(' hover.svg')) {
                    img.attr('src', src.replace('.svg', ' hover.svg'));
                }
            }
            $(this).css({ 'background-color': '#1c94a3', color: 'white' });
        },
        function () {
            const img = $(this).find('img');
            if (img.length) {
                img.attr('src', img.attr('src').replace(' hover.svg', '.svg'));
            }
            $(this).css({ 'background-color': '', color: '' });
        }
    );

    $('[data-toggle="tooltip"]').tooltip({ placement: 'bottom', trigger: 'hover' });
    $('[data-toggle="tooltip_2"]').tooltip({ placement: 'right', trigger: 'hover' });

    const closeButton = document.querySelector('#node-info-dropdown .close-modal');
    const modal = document.getElementById('node-info-modal');

    if (closeButton && modal) {
        closeButton.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            modal.style.display = 'none';
        });
    }

    $('#cy').on('wheel', function (e) {
        const deltaY = e.originalEvent.deltaY;
        const isTrackpadPinchZoom = Math.abs(deltaY) < 50 && e.ctrlKey;

        const isZoomGesture = e.ctrlKey || isTrackpadPinchZoom;

        if (isZoomGesture) {
            e.preventDefault();
            e.stopPropagation();

            const zoomFactor = 1.2;
            const cyContainer = cy.container();
            const rect = cyContainer.getBoundingClientRect();
            const mouseX = e.originalEvent.clientX - rect.left;
            const mouseY = e.originalEvent.clientY - rect.top;

            const zoomCenter = {
                x: mouseX,
                y: mouseY
            };

            const zoomAmount = deltaY < 0 ? zoomFactor : 1 / zoomFactor;

            cy.zoom({
                level: cy.zoom() * zoomAmount,
                renderedPosition: zoomCenter
            });
        }
    });
});

$(document).on('click', '.update-guideline', function (e) {
    e.preventDefault();
    e.stopPropagation();

    let guidelineData = editorCode.get();
    updateGuideline(guidelineData, e.target);
});

$(document).on('click', '.section-tab', function (e) {
    if ($(this).hasClass('active')) {
        return;
    }

    $(this).siblings().removeClass('active');
    $(this).addClass('active');
    let target = $(this).attr('data-target');
    $(`#${target}`).siblings().removeClass('active');
    $(`#${target}`).addClass('active');
})

$(document).on('click', '.publication-show-full-details', function (e) {
    $(this).siblings('.publication-full-details').toggle();

    $(this).toggleClass('active');
})

document.getElementById('toggleSidebar').addEventListener('click', function () {
    const sidebar = document.getElementById('guidelineSidebar');
    const cyContainer = document.getElementById('cy');
    const buttonImage = this.querySelector('img');
    const containerWidth = document.querySelector('.guideline-container').offsetWidth - 31;
    const sidebarWidthPercentage = 0.3;

    if (sidebar.classList.contains('json-collapsed')) {
        sidebar.classList.remove('json-collapsed');
        sidebar.removeAttribute('hidden');
        buttonImage.src = "/css/img/icons/ClinicalPathway/show less button.svg";
        cyContainer.style.width = `${containerWidth * (1 - sidebarWidthPercentage) - 10}px`;
        updateJsonEditor();
    } else {
        sidebar.classList.add('json-collapsed');
        sidebar.setAttribute('hidden', '');
        buttonImage.src = "/css/img/icons/ClinicalPathway/show more button.svg";
        cyContainer.style.width = `${containerWidth}px`;
    }

    setTimeout(() => {
        if (cy) {
            cy.resize();
            cy.center();
        }
    }, 300);
});

document.addEventListener('DOMContentLoaded', function () {
    adjustLayout();
    setInitialSettingsState();
});

$('.direction-arrow').on('mouseover click', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $('.direction-arrow').removeClass('hover');

    selectedDirection = $(this).data('direction');
    hideShapeDropdown();

    const arrow = $(this);
    arrow.addClass('hover');

    const arrowRect = this.getBoundingClientRect();
    let shapeDropdown = (selectedDirection === 'top' || selectedDirection === 'bottom')
        ? $('#shape-dropdown-up')
        : $('#shape-dropdown-right');

    shapeDropdown.show();

    const dropdownWidth = shapeDropdown.outerWidth();
    const dropdownHeight = shapeDropdown.outerHeight();

    const offset = 8;
    let left, top;

    switch (selectedDirection) {
        case 'top':
            left = arrowRect.left + (arrowRect.width / 2) - (dropdownWidth / 2);
            top = arrowRect.top - dropdownHeight - offset;
            break;
        case 'bottom':
            left = arrowRect.left + (arrowRect.width / 2) - (dropdownWidth / 2);
            top = arrowRect.bottom + offset;
            break;
        case 'left':
            left = arrowRect.left - dropdownWidth - offset;
            top = arrowRect.top + (arrowRect.height / 2) - (dropdownHeight / 2);
            break;
        case 'right':
            left = arrowRect.right + offset;
            top = arrowRect.top + (arrowRect.height / 2) - (dropdownHeight / 2);
            break;
    }

    left = Math.max(5, Math.min(left, window.innerWidth - dropdownWidth - 5));
    top = Math.max(5, Math.min(top, window.innerHeight - dropdownHeight - 5));

    shapeDropdown.css({
        position: 'fixed',
        left: `${left}px`,
        top: `${top}px`,
        'z-index': 1000
    });

    currentMode = 'add-node-from-arrow';
});

window.addEventListener('resize', function () {
    adjustLayout();
});

window.addEventListener("beforeunload", function (e) {
    const currentState = getSerializedGraphState();
    const currentSettingsState = {
        title: $('#pathway-title').val()?.trim() || '',
        thesaurusId: $('#thesaurusDisplay').text()?.trim() || '',
        versionMajor: $('#majorVersion').val() || '1',
        versionMinor: $('#minorVersion').val() || '0'
    };

    const hasSettingsChanges = JSON.stringify(initialSettingsState) !== JSON.stringify(currentSettingsState);

    if (!deepCompareIgnorePan(initialGraphState, currentState) || hasSettingsChanges) {
        e.preventDefault();
        e.returnValue = "You have unsaved changes. Are you sure you want to leave?";
        return e.returnValue;
    }
});

window.addEventListener('load', function () {
    adjustLayout();
});

function initializeGraph(elementData) {
    cy = cytoscape({
        container: document.getElementById('cy'),
        elements: elementData,
        layout: {
            name: 'preset'
        },
        style: guidelineStyle,
        zoom: 1,
        minZoom: 0.5,
        maxZoom: 2,
        userZoomingEnabled: false
    });

    updateHtmlLabels();
    initGraphZoom();

    cy.nodes().forEach(node => {
        updateHtmlLabelForNode(node);
    });

    cy.on('unselect', 'node', function (evt) {
        $('#showJsonDataButton').click();
    });

    cy.on('position', 'node', function () {
        $('#direction-arrows').hide();
        hideShapeDropdown();
        hideAllMenus();
    });

    cy.on('pan', function () {
        hideAllMenus();
    });

    cy.on('mousemove', (event) => {
        lastMousePosition = event.position;
    });

    cy.on('dragfreeon', 'node', function (evt) {
        let guidelineData = editorCode.get();
        let data = cy.json();
        guidelineData.guidelineElements = data.elements;
        if (this.hasClass('placeholder-node')) {
            return;
        }
        showJsonEditor(guidelineData);
        saveState();
    });

    let singleClickTimeout;
    let nodeClickCount = 0;
    let edgeClickCount = 0;

    cy.on('tap', 'node', function (evt) {
        evt.stopPropagation();
        cy.edges().removeClass('highlighted');
        $('#edge-context-menu').hide();
        $('#edge-info-dropdown').hide();
        $('#change-arrow-dropdown').hide();
        $('#change-shape-dropdown').hide();

        const node = evt.target;

        if (node.hasClass('placeholder-node')) {
            return;
        }
        nodeClickCount++;

        if (nodeClickCount === 1) {
            singleClickTimeout = setTimeout(() => {
                if (nodeClickCount === 1) {
                    $('#direction-arrows').hide();
                    $('#node-info-dropdown').hide();
                    hideShapeDropdown();
                    showNodeMenu(node);
                }
                nodeClickCount = 0;
            }, 200);
        }
        else if (nodeClickCount >= 2) {
            clearTimeout(singleClickTimeout);
            nodeClickCount = 0;
            enableInlineNodeEditing(node);
        }
    });

    cy.on('tap', 'edge', function (evt) {
        evt.stopPropagation();
        edgeClickCount++;

        hideAllMenus();

        if (edgeClickCount === 1) {
            singleClickTimeout = setTimeout(() => {
                if (edgeClickCount === 1) {
                    // Single click - show context menu
                    const edge = evt.target;
                    showEdgeMenu(edge);
                    cy.edges().removeClass('highlighted');
                    edge.addClass('highlighted');

                    showEdgeMenu(edge);
                }
                edgeClickCount = 0;
            }, 200);
        }
        else if (edgeClickCount >= 2) {
            clearTimeout(singleClickTimeout);
            edgeClickCount = 0;

            $('#node-info-dropdown').hide();
            hideAllMenus();

            // Double click - enable inline title editing
            const edge = evt.target;
            const edgeData = edge.data();
            const sourceNode = cy.getElementById(edgeData.source);
            const targetNode = cy.getElementById(edgeData.target);

            // Calculate midpoint for input placement
            const sourcePos = sourceNode.position();
            const targetPos = targetNode.position();
            const midX = (sourcePos.x + targetPos.x) / 2;
            const midY = (sourcePos.y + targetPos.y) / 2;

            const zoom = cy.zoom();
            const pan = cy.pan();
            const cyContainer = cy.container();
            const containerRect = cyContainer.getBoundingClientRect();

            const renderedX = (midX * zoom) + pan.x + containerRect.left;
            const renderedY = (midY * zoom) + pan.y + containerRect.top;

            // Temporarily hide edge title
            const originalTitle = edgeData.title || '';
            edge.data('title', '');

            // Create input for editing
            const input = document.createElement('input');
            input.type = 'text';
            input.value = originalTitle;
            input.className = 'edge-text';
            input.style.position = 'absolute';
            input.style.left = `${renderedX}px`;
            input.style.top = `${renderedY}px`;
            input.style.transform = 'translate(-50%, -50%)';
            input.style.width = `${100 * zoom}px`; // Adjust width as needed
            input.style.textAlign = 'center';
            input.style.zIndex = '9999';
            input.style.border = 'none';
            input.style.outline = 'none';
            input.style.boxShadow = 'none';
            input.style.backgroundColor = 'white';
            input.style.fontFamily = 'Nunito Sans, sans-serif';
            input.style.fontWeight = '600';
            input.style.fontSize = `${16 * zoom}px`;
            input.style.lineHeight = 'inherit';
            input.style.color = 'inherit';

            document.body.appendChild(input);
            input.focus();
            input.select();

            let isEnterOrEsc = false;

            // Handle key events
            input.addEventListener('keydown', function (e) {
                if (e.key === 'Enter' || e.key === 'Escape') {
                    e.preventDefault();
                    isEnterOrEsc = true;
                    const newTitle = input.value.trim();

                    if (e.key === 'Enter') {
                        edge.data('title', newTitle);
                    } else {
                        edge.data('title', originalTitle);
                    }

                    updateJsonEditor();
                    saveState();

                    try {
                        input.remove();
                    }
                    catch (err) { }
                }
            });

            input.addEventListener('blur', function () {
                if (!isEnterOrEsc) {
                    edge.data('title', input.value.trim());

                    updateJsonEditor();
                    saveState();
                }

                try {
                    input.remove();
                }
                catch (err) { }
            });
        }
    });

    cy.on('mouseover', 'node', function (evt) {
        if ($('#node-context-menu').is(':visible')) return;
        hideShapeDropdown();
        $('.direction-arrow').removeClass('hover');

        const node = evt.target;
        if (node.hasClass('placeholder-node')) return;

        hoveredNode = node;

        const arrowsContainer = document.getElementById('direction-arrows');
        arrowsContainer.style.display = 'block';

        const nodePosition = node.position();
        const nodeWidth = node.width();
        const nodeHeight = node.height();

        const zoom = cy.zoom();
        const pan = cy.pan();

        const renderedX = (nodePosition.x * zoom) + pan.x;
        const renderedY = (nodePosition.y * zoom) + pan.y;

        arrowsContainer.style.position = 'absolute';
        arrowsContainer.style.left = `${renderedX - 13}px`;
        arrowsContainer.style.top = `${renderedY + 2}px`;

        const renderedWidth = nodeWidth * zoom;
        const renderedHeight = nodeHeight * zoom;
        const baseArrowOffset = 30;
        const arrowOffset = baseArrowOffset * zoom;

        const topArrow = document.querySelector('.top-arrow');
        const bottomArrow = document.querySelector('.bottom-arrow');
        const leftArrow = document.querySelector('.left-arrow');
        const rightArrow = document.querySelector('.right-arrow');

        topArrow.style.position = 'absolute';
        bottomArrow.style.position = 'absolute';
        leftArrow.style.position = 'absolute';
        rightArrow.style.position = 'absolute';

        if (nodeWidth == nodeHeight) {
            topArrow.style.top = `-${renderedHeight / 2 + arrowOffset}px`;
            bottomArrow.style.top = `${renderedHeight / 2 + arrowOffset}px`;
            leftArrow.style.left = `-${renderedWidth / 2 + arrowOffset}px`;
            rightArrow.style.left = `${renderedWidth / 2 + arrowOffset}px`;
        }
        else {
            const adjustedOffset = arrowOffset - (18 * zoom);

            topArrow.style.top = `-${renderedHeight / 2 + adjustedOffset}px`;
            bottomArrow.style.top = `${renderedHeight / 2 + adjustedOffset}px`;
            leftArrow.style.left = `-${renderedWidth / 2 + adjustedOffset}px`;
            rightArrow.style.left = `${renderedWidth / 2 + adjustedOffset}px`;
        }
    });

    cy.on('mouseover', 'edge', function (evt) {
        const edge = evt.target;
        const title = edge.data('title');
        const container = cy.container();
        container.setAttribute('title', title);
        container.style.cursor = 'pointer';
    });

    cy.on('mouseout', 'edge', function (evt) {
        const container = cy.container();
        container.removeAttribute('title');
        container.style.cursor = 'default';
    });

    const existingNodes = cy.nodes();
    if (existingNodes.length > 0) {
        const rightmostNode = existingNodes.reduce((maxNode, currentNode) => {
            return currentNode.position().x > maxNode.position().x ? currentNode : maxNode;
        }, existingNodes[0]);
        lastAddedNodePosition = { ...rightmostNode.position() };
    }

    cy.center();
    enforceZoomLevel();
    saveState();

    initialGraphState = getSerializedGraphState();
}

function submitGuidline(e) {
    let jsonData = editorCode.get();

    if (jsonData.guidelineElements && jsonData.guidelineElements.nodes) {
        jsonData.guidelineElements.nodes = jsonData.guidelineElements?.nodes?.filter(
            node => node.data.id.startsWith('n_')
        );
    }

    if (jsonData.guidelineElements && jsonData.guidelineElements.edges) {
        jsonData.guidelineElements.edges = jsonData.guidelineElements?.edges?.filter(
            edge => !edge.data.source.includes('temp') && !edge.data.target.includes('temp')
        );
    }

    editorCode.set(jsonData);

    callServer({
        method: 'post',
        data: jsonData,
        url: '/DigitalGuideline/Create',
        contentType: 'application/json',
        success: function (data) {
            toastr.success('Success');
            updateGuidelineWithLastUpdate(data);
            updateGuideline(jsonData, null); 
            initialGraphState = getSerializedGraphState();
            setInitialSettingsState();
            if (!jsonData.id) {
                window.location.href = '/DigitalGuideline/Edit?id=' + data.id;
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function previewNode(data) {
    callServer({
        method: 'post',
        url: '/DigitalGuideline/PreviewNode',
        data: data,
        success: function (data) {
            $('#nodePreview').html(data);
            $('#showNodePreviewButton').click();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function updateGuideline(guidelineData, eventTarget) {
    try {
        if (!guidelineData || !guidelineData.guidelineElements) {
            console.error('Invalid guideline data or guidelineElements missing');
            return;
        }

        if (!cy) {
            initializeGraph(guidelineData);
        } else {
            cy.elements().remove();
            cy.add(guidelineData.guidelineElements);
            updateHtmlLabels();

            cy.nodes().forEach(node => {
                updateHtmlLabelForNode(node);
            });
            tempSourceNode = null;
            tempTargetNode = null;
            tempEdge = null;
            edgeCreationMode = false;

            enforceZoomLevel();
            updateJsonEditor();
        }

        if (!$(eventTarget).closest('#visual-editor-menu').length && !$(eventTarget).closest('#node-context-menu').length) {
            hideShapeDropdown();
            $('#shape-dropdown').hide();
            $('#arrow-dropdown').hide();
        }

        if (!$(eventTarget).closest('#node-info-dropdown').length && !$(eventTarget).closest('#visual-editor-menu').length && !$(eventTarget).closest('#node-context-menu').length) {
            $('#node-info-dropdown').hide();
        }
    } catch (error) {
        console.error('Error updating graph:', error);
    }
}

function updateGuidelineWithLastUpdate(data) {
    $('#lastUpdate').val(data.lastUpdate);
    let jsonData = editorCode.get();
    jsonData['lastUpdate'] = data.lastUpdate;
    editorCode.set(jsonData);
}

function activateSelectIcon() {
    $('.menu-left button img').each(function () {
        let defaultSrc = $(this).attr('src').replace(' hover', '');
        $(this).attr('src', defaultSrc);
    });

    $('#selectIcon').attr('src', '/css/img/icons/ClinicalPathway/select hover.svg');
}

function hideShapeDropdown() {
    $('#change-shape-dropdown, #shape-dropdown-up, #shape-dropdown-right').hide();
}

function hideAllMenus() {
    if (cy) {
        cy.edges().removeClass('highlighted');
    }
    $(
        '#shape-dropdown, #shape-dropdown-up, #shape-dropdown-right, #arrow-dropdown, #node-context-menu, #node-info-dropdown, #edge-context-menu, #edge-info-dropdown, #direction-arrows, #change-arrow-dropdown, #change-shape-dropdown'
    ).hide();
}

function toggleButtonHoverState($btn) {
    $('.menu-left button img').each(function () {
        let defaultSrc = $(this).attr('src').replace(' hover', '');
        $(this).attr('src', defaultSrc);
    });

    let imgElement = $btn.find('img');
    let currentSrc = imgElement.attr('src');

    if (!currentSrc.includes(' hover')) {
        let hoverSrc = currentSrc.replace(/(\.svg)$/, ' hover$1');
        imgElement.attr('src', hoverSrc);
    }
}