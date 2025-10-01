function enableEdgeCreationMode(selectedArrowType) {
    if (!cy) {
        initializeGraph({ nodes: [], edges: [] });
    }

    if (edgeCreationMode) {
        return;
    }

    edgeCreationMode = true;
    sourceNode = null;
    targetNode = null;
    tempEdge = null;
    tempSourceNode = null;
    tempTargetNode = null;

    const basePosition = getNewNodePosition(cy, 200, true);

    const tempSourceId = 'temp_source_' + Date.now();
    const tempTargetId = 'temp_target_' + Date.now();

    tempSourceNode = cy.add({
        group: 'nodes',
        data: { id: tempSourceId },
        position: { x: basePosition.x - 75, y: basePosition.y },
        classes: 'placeholder-node'
    });

    tempTargetNode = cy.add({
        group: 'nodes',
        data: { id: tempTargetId },
        position: { x: basePosition.x + 75, y: basePosition.y },
        classes: 'placeholder-node'
    });

    const newEdgeId = generateUniqueEdgeId();
    let edgeClasses = 'edge-label-background';
    if (selectedArrowType === 'bezier') {
        edgeClasses += ' temp-bezier';
    } else if (selectedArrowType === 'unbundled-bezier') {
        edgeClasses += ' unbundled-bezier';
    } else if (selectedArrowType === 'taxi') {
        edgeClasses += ' taxi';
    }

    tempEdge = cy.add({
        group: 'edges',
        data: {
            id: newEdgeId,
            source: tempSourceId,
            target: tempTargetId,
            state: 'NotStarted',
            thesaurusId: 0,
            title: '',
            type: 'Edge'
        },
        position: { x: 0, y: 0 },
        removed: false,
        selected: false,
        selectable: false,
        locked: false,
        grabbable: true,
        pannable: true,
        classes: edgeClasses
    });
    
    enforceZoomLevel();
    cy.center(tempEdge);

    tempSourceNode.style({
        'width': '10px',
        'height': '10px',
        'background-color': '#ff0000',
        'shape': 'ellipse',
        'opacity': 0.5,
        'border-width': '2px',
        'border-color': '#000'
    });
    tempSourceNode.grabbable(true);
    tempSourceNode.locked(false);

    tempTargetNode.style({
        'width': '10px',
        'height': '10px',
        'background-color': '#00ff00',
        'shape': 'ellipse',
        'opacity': 0.5,
        'border-width': '2px',
        'border-color': '#000'
    });
    tempTargetNode.grabbable(true);
    tempTargetNode.locked(false);

    updateJsonEditor();
    saveState();

    setupEdgeCreationHandlers();
}

function showEdgeMenu(edge) {
    const existingMenu = document.getElementById('edge-context-menu');
    if (existingMenu) {
        existingMenu.remove();
    }

    const menu = document.createElement('div');
    menu.id = 'edge-context-menu';
    menu.style.position = 'absolute';
    menu.style.background = 'white';
    menu.style.border = '1px solid #ccc';
    menu.style.borderRadius = '8px';
    menu.style.zIndex = '8';

    const menuWidth = 40;
    const menuHeight = 85;
    const padding = 10;

    const canvas = document.getElementById('cy');
    const canvasRect = canvas.getBoundingClientRect();

    // Calculate midpoint of edge for menu placement
    const sourceNode = cy.getElementById(edge.data('source'));
    const targetNode = cy.getElementById(edge.data('target'));
    const sourcePos = sourceNode.position();
    const targetPos = targetNode.position();
    const midX = (sourcePos.x + targetPos.x) / 2;
    const midY = (sourcePos.y + targetPos.y) / 2;

    const zoom = cy.zoom();
    const pan = cy.pan();
    const renderedX = (midX * zoom) + pan.x;
    const renderedY = (midY * zoom) + pan.y;

    let left = canvasRect.left + renderedX - menuWidth / 2 - 30;
    let top = canvasRect.top + renderedY - menuHeight - padding + 60;

    const canvasWidth = canvasRect.width;
    if (left < canvasRect.left) {
        left = canvasRect.left + 10;
    } else if (left + menuWidth > canvasRect.left + canvasWidth) {
        left = canvasRect.left + canvasWidth - menuWidth - 10;
    }

    if (top < canvasRect.top + 226) {
        top = canvasRect.top + renderedY + padding;
    }

    menu.style.left = `${left}px`;
    menu.style.top = `${top}px`;

    menu.innerHTML = `
        <button id="edge-properties" data-toggle="tooltip" title="Properties" class="node-context-menu-button">
            <img id="edgePropertyIcon" alt="" src="/css/img/icons/ClinicalPathway/Properties.svg" />
        </button>
        <button id="edge-shapes" data-toggle="tooltip" title="Shapes" class="node-context-menu-button">
            <img id="edgeShapesIcon" alt="" src="/css/img/icons/ClinicalPathway/Shapes.svg" />
        </button>
        <button id="delete-edge" data-toggle="tooltip" title="Delete edge" class="node-context-menu-button">
            <img id="deleteEdgeIcon" alt="" src="/css/img/icons/ClinicalPathway/Delete node.svg" />
        </button>
    `;

    document.body.appendChild(menu);

    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom',
        trigger: 'hover'
    });

    $('#edge-context-menu .node-context-menu-button').hover(
        function () {
            let imgElement = $(this).find('img');
            let currentSrc = imgElement.attr('src');
            if (!currentSrc.includes('hover.svg')) {
                let hoverSrc = currentSrc.replace('.svg', ' hover.svg');
                imgElement.attr('src', hoverSrc);
            }
        },
        function () {
            let imgElement = $(this).find('img');
            let defaultSrc = imgElement.attr('src').replace(' hover.svg', '.svg');
            imgElement.attr('src', defaultSrc);
        }
    );

    $('#edge-context-menu .node-context-menu-button').on('click', function () {
        $(this).tooltip('hide');
    });

    document.getElementById('edge-properties').addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        hideShapeDropdown();
        showEdgeInfoDropdown(edge);
    });

    document.getElementById('edge-shapes').addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        showEdgeShapeChangeModal(edge);
    });

    document.getElementById('delete-edge').addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        hideShapeDropdown();
        cy.remove(edge);
        cleanupTemporaryEdge(edge);
        updateJsonEditor();
        enforceZoomLevel();
        saveState();
        $('#node-info-dropdown').hide();
        menu.remove();
    });

    setTimeout(() => {
        document.addEventListener('click', function handler(e) {
            if (!e.target.closest('#edge-context-menu') && !e.target.closest('#edge-info-dropdown')) {
                menu.remove();
                document.removeEventListener('click', handler);
            }
        });
    }, 0);
}

function showEdgeInfoDropdown(edge) {
    $('#change-arrow-dropdown').hide();
    const shapesIcon = $('#edgeShapesIcon');
    const currentSrc = shapesIcon.attr('src');
    const defaultSrc = currentSrc.replace('%20hover', '');
    shapesIcon.attr('src', defaultSrc);

    const infoDropdown = $('#edge-info-dropdown');
    infoDropdown.show();

    const contextMenu = document.getElementById('edge-context-menu');
    const menuRect = contextMenu.getBoundingClientRect();

    const canvas = document.getElementById('cy');
    const canvasRect = canvas.getBoundingClientRect();

    // Calculate midpoint for dropdown placement
    const sourceNode = cy.getElementById(edge.data('source'));
    const targetNode = cy.getElementById(edge.data('target'));
    const sourcePos = sourceNode.position();
    const targetPos = targetNode.position();
    const midY = (sourcePos.y + targetPos.y) / 2;

    const zoom = cy.zoom();
    const pan = cy.pan();
    const renderedY = (midY * zoom) + pan.y + canvasRect.top;

    const left = menuRect.left + (menuRect.width / 2) - (infoDropdown.width() / 2) + 30 - getVisibleSidebarWidth('guidelineSidebar') - getSidebarWidthDifference();
    let top = menuRect.top - infoDropdown.outerHeight() - 5;

    const propertyIcon = document.getElementById('edgePropertyIcon');
    let originalSrc;
    if (propertyIcon) {
        originalSrc = propertyIcon.src;
        propertyIcon.src = originalSrc;
    }


    if (menuRect.top < renderedY) {
        top = menuRect.top - infoDropdown.outerHeight() - 5;
    } else {
        top = menuRect.bottom + 5;
    }

    infoDropdown.css({
        position: 'absolute',
        left: `${left - 140}px`,
        top: `${top - 60}px`,
        'z-index': 1000
    });

    $('#edge-title').val(edge.data('title') || '');
    $('#edge-thesaurusId').val(edge.data('thesaurusId') || 0);

    $(document).off('keydown.saveEdgeInfo').on('keydown.saveEdgeInfo', function (event) {
        if (event.key === 'Enter' && infoDropdown.is(':visible')) {
            event.preventDefault();
            event.stopPropagation();

            edge.data('title', $('#edge-title').val());
            edge.data('thesaurusId', parseInt($('#edge-thesaurusId').val()) || 0);

            updateJsonEditor();         
            enforceZoomLevel();
            saveState();

            infoDropdown.hide();
            currentMode = null;
        }
    });

    $('#save-edge-info').off('click').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        edge.data('title', $('#edge-title').val());
        edge.data('thesaurusId', parseInt($('#edge-thesaurusId').val()) || 0);

        updateJsonEditor();
        enforceZoomLevel();
        saveState();

        infoDropdown.hide();
        currentMode = null;
    });

    $('#cancel-edge-info').off('click').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        hideAllMenus();
        infoDropdown.hide();
        currentMode = null;
    });

    setTimeout(() => {
        $(document).on('click.edgeInfoDropdown', function handler(e) {
            if (!$(e.target).closest('#edge-info-dropdown').length && !$(e.target).closest('#edge-context-menu').length) {
                infoDropdown.hide();
                currentMode = null;
                $(document).off('click.edgeInfoDropdown');
            }
        });
    }, 0);
}

function showEdgeShapeChangeModal(edge) {
    $('#edge-info-dropdown').hide();
    const $propertyIcon = $('#edgePropertyIcon');
    const currentSrc = $propertyIcon.attr('src');
    const defaultSrc = currentSrc.replace('%20hover', '');
    $propertyIcon.attr('src', defaultSrc);

    const infoDropdown = $('#change-arrow-dropdown');
    infoDropdown.show();

    const contextMenu = document.getElementById('edge-context-menu');
    const menuRect = contextMenu.getBoundingClientRect();

    const canvas = document.getElementById('cy');
    const canvasRect = canvas.getBoundingClientRect();

    const sourceNode = cy.getElementById(edge.data('source'));
    const targetNode = cy.getElementById(edge.data('target'));
    const sourcePos = sourceNode.position();
    const targetPos = targetNode.position();
    const midY = (sourcePos.y + targetPos.y) / 2;

    const zoom = cy.zoom();
    const pan = cy.pan();
    const renderedY = (midY * zoom) + pan.y + canvasRect.top;

    const left = menuRect.left + (menuRect.width / 2) - (infoDropdown.width() / 2) + 30 - getVisibleSidebarWidth('guidelineSidebar') - getSidebarWidthDifference();
    let top = menuRect.top - infoDropdown.outerHeight() - 5;

    const propertyIcon = document.getElementById('edgeShapesIcon');
    let originalSrc;
    if (propertyIcon) {
        originalSrc = propertyIcon.src;
        propertyIcon.src = originalSrc;
    }

    if (menuRect.top < renderedY) {
        top = menuRect.top - infoDropdown.outerHeight() - 5;
    } else {
        top = menuRect.bottom + 5;
    }

    infoDropdown.css({
        position: 'absolute',
        left: `${left - 140}px`,
        top: `${top - 60}px`,
        'z-index': 1000
    });

    currentEdgeMode = 'change-arrow';

    setTimeout(() => {
        $(document).on('click.infoDropdown', function handler(e) {
            if (!$(e.target).closest('#change-shape-dropdown').length && !$(e.target).closest('#edge-context-menu').length) {
                infoDropdown.hide();
                currentEdgeMode = null;
                $(document).off('click.infoDropdown');
            }
        });
    }, 0);
}

function updateEdgeType(selectedArrowType) {
    const highlightedEdge = cy.$('edge.highlighted');
    if (highlightedEdge.length === 0) return;

    let newEdgeClasses = 'edge-label-background';
    const sourceId = highlightedEdge.data('source');
    const targetId = highlightedEdge.data('target');
    const reverseEdge = cy.edges(`[source = "${targetId}"][target = "${sourceId}"]`);

    if (selectedArrowType === 'bezier') {
        if (reverseEdge.length === 0) {
            const newEdgeId = generateUniqueEdgeId();
            cy.add({
                group: 'edges',
                data: {
                    id: newEdgeId,
                    source: targetId,
                    target: sourceId,
                    state: 'NotStarted',
                    thesaurusId: 0,
                    title: '',
                    type: 'Edge'
                },
                position: { x: 0, y: 0 },
                removed: false,
                selected: false,
                selectable: true,
                locked: false,
                grabbable: true,
                pannable: true,
                classes: 'edge-label-background'
            });
        }
    } else if (selectedArrowType === 'unbundled-bezier') {
        newEdgeClasses += ' unbundled-bezier';
        if (reverseEdge.length > 0) {
            cy.remove(reverseEdge);
        }
    } else if (selectedArrowType === 'taxi') {
        newEdgeClasses += ' taxi';
        if (reverseEdge.length > 0) {
            cy.remove(reverseEdge);
        }
    } else {
        if (reverseEdge.length > 0) {
            cy.remove(reverseEdge);
        }
    }

    highlightedEdge.classes(newEdgeClasses);
    enforceZoomLevel();
    updateJsonEditor();
}

function generateUniqueEdgeId() {
    let maxId = 0;
    cy.edges().forEach(e => {
        const edgeId = e.id();
        if (edgeId.startsWith('e')) {
            const num = parseInt(edgeId.replace('e', ''), 10);
            if (!isNaN(num) && num > maxId) {
                maxId = num;
            }
        }
    });
    return 'e' + (maxId + 1);
}

function cleanupTemporaryEdge(edgeToRemove) {
    if (edgeToRemove && tempEdge && edgeToRemove.id() === tempEdge.id()) {
        if (tempSourceNode && cy.getElementById(tempSourceNode.id()).length > 0) {
            cy.remove(tempSourceNode);
        }
        if (tempTargetNode && cy.getElementById(tempTargetNode.id()).length > 0) {
            cy.remove(tempTargetNode);
        }
        if (cy.getElementById(tempEdge.id()).length > 0) {
            cy.remove(tempEdge);
        }

        tempEdge = null;
        tempSourceNode = null;
        tempTargetNode = null;
        sourceNode = null;
        targetNode = null;
        edgeCreationMode = false;
    }
}

function setupEdgeCreationHandlers() {
    cy.off('mouseover', 'node.placeholder-node');
    cy.off('mouseout', 'node.placeholder-node');
    cy.off('drag', 'node.placeholder-node');
    cy.off('dragfree', 'node.placeholder-node');

    cy.on('mouseover', 'node.placeholder-node', function (evt) {
        const node = evt.target;
        node.style({
            'opacity': 0.8,
            'border-width': '3px',
            'border-color': '#1c94a3'
        });
    });

    cy.on('mouseout', 'node.placeholder-node', function (evt) {
        const node = evt.target;
        node.style({
            'opacity': 0.5,
            'border-width': '2px',
            'border-color': '#000'
        });
    });

    cy.on('drag', 'node.placeholder-node', function (evt) {
        const draggedNode = evt.target;
        if (!edgeCreationMode) return;

        let closestNode = null;
        let minDistance = Infinity;
        const draggedPos = draggedNode.position();

        cy.nodes().forEach(node => {
            if (node.hasClass('placeholder-node')) return;
            const nodePos = node.position();
            const distance = Math.sqrt(
                Math.pow(draggedPos.x - nodePos.x, 2) +
                Math.pow(draggedPos.y - nodePos.y, 2)
            );

            if (distance < minDistance && distance < 75) {
                minDistance = distance;
                closestNode = node;
            }
        });

        cy.nodes().style('border-width', '1px');
        if (closestNode) {
            closestNode.style('border-width', '3px');
            closestNode.style('border-color', '#ff0000');
        }
    });

    cy.on('dragfree', 'node.placeholder-node', function (evt) {
        const draggedNode = evt.target;
        if (!edgeCreationMode) return;

        let closestNode = null;
        let minDistance = Infinity;
        const draggedPos = draggedNode.position();

        cy.nodes().forEach(node => {
            if (node.hasClass('placeholder-node')) return;
            const nodePos = node.position();
            const distance = Math.sqrt(
                Math.pow(draggedPos.x - nodePos.x, 2) +
                Math.pow(draggedPos.y - nodePos.y, 2)
            );

            if (distance < minDistance && distance < 75) {
                minDistance = distance;
                closestNode = node;
            }
        });

        cy.nodes().style('border-width', '1px');

        if (closestNode) {
            const isSource = draggedNode.id() === tempSourceNode?.id();

            if (!tempEdge) {
                toastr.error('An error occurred while connecting the node. Please try again.');
                return;
            }

            const intendedSource = isSource ? closestNode : sourceNode;
            const intendedTarget = isSource ? (targetNode || cy.getElementById(tempEdge.data('target'))) : closestNode;

            if (intendedSource && intendedTarget) {
                const existingEdges = cy.edges().filter(edge => {
                    return edge.id() !== tempEdge.id() &&
                        edge.data('source') === intendedSource.id() &&
                        edge.data('target') === intendedTarget.id();
                });

                if (existingEdges.length > 0) {
                    toastr.error('A relationship between these nodes already exists. Duplicate relationships are not allowed.');
                    cy.remove(tempEdge);
                    if (tempSourceNode) cy.remove(tempSourceNode);
                    if (tempTargetNode) cy.remove(tempTargetNode);
                    edgeCreationMode = false;
                    sourceNode = null;
                    targetNode = null;
                    tempEdge = null;
                    tempSourceNode = null;
                    tempTargetNode = null;
                    cy.nodes().style({
                        'border-width': '1px',
                        'border-color': '#000'
                    });
                    cy.off('mouseover', 'node.placeholder-node');
                    cy.off('mouseout', 'node.placeholder-node');
                    cy.off('drag', 'node.placeholder-node');
                    cy.off('dragfree', 'node.placeholder-node');
                    updateJsonEditor();
                    saveState();
                    return;
                }
            }

            if (isSource) {
                sourceNode = closestNode;
                tempEdge.json({
                    data: {
                        id: tempEdge.id(),
                        source: sourceNode.id(),
                        target: tempEdge.data('target'),
                        state: 'NotStarted',
                        thesaurusId: 0,
                        title: '',
                        type: 'Edge'
                    }
                });
                if (tempSourceNode) cy.remove(tempSourceNode);
                tempSourceNode = null;
                updateJsonEditor();
                saveState();
            } else {
                targetNode = closestNode;
                tempEdge.json({
                    data: {
                        id: tempEdge.id(),
                        source: tempEdge.data('source'),
                        target: targetNode.id(),
                        state: 'NotStarted',
                        thesaurusId: 0,
                        title: '',
                        type: 'Edge'
                    }
                });
                if (tempTargetNode) cy.remove(tempTargetNode);
                tempTargetNode = null;
                updateJsonEditor();
                saveState();
            }     

            enforceZoomLevel();

            if (sourceNode && targetNode) {
                if (sourceNode.id() === targetNode.id()) {
                    toastr.error('Source and target nodes must be different. Please drag the arrowhead to a different node.');
                    targetNode = null;
                    tempEdge.json({
                        data: {
                            id: tempEdge.id(),
                            source: tempEdge.data('source'),
                            target: generateUniqueNodeId(),
                            state: 'NotStarted',
                            thesaurusId: 0,
                            title: '',
                            type: 'Edge'
                        }
                    });
                    tempTargetNode = cy.add({
                        group: 'nodes',
                        data: { id: tempEdge.data('target') },
                        position: { x: draggedPos.x + 75, y: draggedPos.y },
                        classes: 'placeholder-node'
                    });
                    tempTargetNode.style({
                        'width': '10px',
                        'height': '10px',
                        'background-color': '#00ff00',
                        'shape': 'ellipse',
                        'opacity': 0.5,
                        'border-width': '2px',
                        'border-color': '#000'
                    });
                    tempTargetNode.grabbable(true);
                    tempTargetNode.locked(false);                   
                    enforceZoomLevel();
                    updateJsonEditor();
                    saveState();
                    return;
                }

                let finalEdgeClasses = 'edge-label-background';
                if (selectedArrowType === 'unbundled-bezier') {
                    finalEdgeClasses += ' unbundled-bezier';
                } else if (selectedArrowType === 'taxi') {
                    finalEdgeClasses += ' taxi';
                }

                tempEdge.json({
                    data: {
                        id: tempEdge.id(),
                        source: sourceNode.id(),
                        target: targetNode.id(),
                        state: 'NotStarted',
                        thesaurusId: 0,
                        title: '',
                        type: 'Edge'
                    },
                    group: 'edges',
                    removed: false,
                    selected: false,
                    selectable: true,
                    locked: false,
                    grabbable: true,
                    pannable: true,
                    classes: finalEdgeClasses
                });

                if (selectedArrowType === 'bezier') {
                    const secondEdgeId = generateUniqueEdgeId();
                    cy.add({
                        group: 'edges',
                        data: {
                            id: secondEdgeId,
                            source: targetNode.id(),
                            target: sourceNode.id(),
                            state: 'NotStarted',
                            thesaurusId: 0,
                            title: '',
                            type: 'Edge'
                        },
                        position: { x: 0, y: 0 },
                        removed: false,
                        selected: false,
                        selectable: true,
                        locked: false,
                        grabbable: true,
                        pannable: true,
                        classes: finalEdgeClasses
                    });
                }

                tempSourceNode = null;
                tempTargetNode = null;
                tempEdge = null;
                edgeCreationMode = false;
                cy.off('mouseover', 'node.placeholder-node');
                cy.off('mouseout', 'node.placeholder-node');
                cy.off('drag', 'node.placeholder-node');
                cy.off('dragfree', 'node.placeholder-node');

                updateJsonEditor();              
                enforceZoomLevel();

                cy.nodes().style({
                    'border-width': '1px',
                    'border-color': '#000'
                });
            }
        }
        else {
            saveState();
        }
    });
}