const generateHtmlTemplate = (data) => {
    const title = data.title || '';
    const escapedTitle = $('<div>').text(title).html();

    switch (data.type) {
        case 'Event':
            return `<div class="shape event" shape="ellipse"><span class="shape-text" title="${escapedTitle}">${escapedTitle}</span></div>`;
        case 'Statement':
            return `<div class="shape statement"><span class="shape-text" title="${escapedTitle}">${escapedTitle}</span></div>`;
        case 'Decision':
            return `<div class="shape decision"><div class="shape-inner"><span class="shape-text" title="${escapedTitle}">${escapedTitle}</span></div></div>`;
        default:
            return '';
    }
};

const updateHtmlLabels = () => {
    cy.nodeHtmlLabel([
        {
            tpl: generateHtmlTemplate
        }
    ]);
};

const updateHtmlLabelForNode = (node) => {
    cy.nodeHtmlLabel([
        {
            query: `#${node.id()}`,
            tpl: generateHtmlTemplate
        }
    ]);
};

function showNodePreview(event) {
    event.preventDefault();
    if ($('#nodePreview').hasClass('active')) {
        return;
    }

    $(event.target).siblings().removeClass('active');
    $(event.target).addClass('active');
    $('#jsoneditorCode').hide();
    $('#nodePreview').show();
}

function addNodeWithShape(shape) {
    if (!cy) {
        initializeGraph({ nodes: [], edges: [] });
    }

    let nodeType;
    switch (shape) {
        case 'round-rectangle':
            nodeType = 'Statement';
            break;
        case 'round-diamond':
            nodeType = 'Decision';
            break;
        case 'ellipse':
            nodeType = 'Event';
            break;
        default:
            nodeType = 'Statement';
    }

    const position = getNewNodePosition(cy, 150);

    const newNode = {
        group: 'nodes',
        data: {
            id: generateUniqueNodeId(),
            state: 'NotStarted',
            thesaurusId: 0,
            title: '',
            type: nodeType
        },
        position,
        classes: 'foo bar baaarrr'
    };

    const addedNode = cy.add(newNode);
    lastAddedNodePosition = addedNode.position();

    updateHtmlLabelForNode(addedNode);
    enforceZoomLevel();
    cy.center(addedNode);

    updateJsonEditor();
    saveState();
}

function changeNodeShape(selectedShape, node) {
    if (!node.length) return;

    const shapeToTypeMap = {
        'ellipse': 'Event',
        'round-rectangle': 'Statement',
        'round-diamond': 'Decision'
    };

    const nodeType = shapeToTypeMap[selectedShape] || 'Statement';

    node.style('shape', selectedShape);
    node.data('type', nodeType);
    updateHtmlLabelForNode(node);
    updateJsonEditor();    
    enforceZoomLevel();
    saveState();
}

function addNodeAndEdge(selectedShape, hoveredNode, selectedDirection) {
    const shapeToTypeMap = {
        'ellipse': 'Event',
        'round-rectangle': 'Statement',
        'round-diamond': 'Decision'
    };
    const nodeType = shapeToTypeMap[selectedShape] || 'Statement';

    const originalPosition = hoveredNode.position();
    const nodeWidth = hoveredNode.width();
    const nodeHeight = hoveredNode.height();
    const offset = 100 / cy.zoom();

    const directionOffsets = {
        'top': { x: 0, y: -(nodeHeight + offset) },
        'bottom': { x: 0, y: nodeHeight + offset },
        'left': { x: -(nodeWidth + offset + 30), y: 0 },
        'right': { x: nodeWidth + offset + 30, y: 0 }
    };

    const offsetPos = directionOffsets[selectedDirection] || { x: 0, y: 0 };
    const newPosition = {
        x: originalPosition.x + offsetPos.x,
        y: originalPosition.y + offsetPos.y
    };

    const newNodeId = generateUniqueNodeId();
    const newNode = {
        group: 'nodes',
        data: { id: newNodeId, state: 'NotStarted', thesaurusId: 0, title: '', type: nodeType },
        position: newPosition,
        classes: 'foo bar baaarrr'
    };

    const addedNode = cy.add(newNode);
    updateHtmlLabelForNode(addedNode);

    const newEdge = {
        group: 'edges',
        data: {
            id: generateUniqueEdgeId(),
            source: hoveredNode.id(),
            target: newNodeId,
            state: 'NotStarted',
            thesaurusId: 0,
            title: '',
            type: 'Edge'
        }
    };

    cy.add(newEdge);   
    cy.center(addedNode);
    enforceZoomLevel();
    updateJsonEditor();
    saveState();
}

function enableInlineNodeEditing(node) {
    hideShapeDropdown();
    $('#direction-arrows').hide();
    $('#node-context-menu').hide();

    if (node.hasClass('placeholder-node')) return;

    const nodeData = node.data();
    const nodePosition = node.position();
    const zoom = cy.zoom();
    const pan = cy.pan();
    const nodeWidth = node.width();

    const originalTitle = nodeData.title;
    node.data('title', '');
    updateHtmlLabelForNode(node);

    // Create inline editing input with proper positioning
    const input = document.createElement('input');
    input.type = 'text';
    input.value = originalTitle || '';
    input.className = 'shape-text';
    input.style.position = 'absolute';

    const cyContainer = cy.container();
    const containerRect = cyContainer.getBoundingClientRect();
    const renderedX = (nodePosition.x * zoom) + pan.x + containerRect.left;
    const renderedY = (nodePosition.y * zoom) + pan.y + containerRect.top;

    input.style.left = `${renderedX}px`;

    // Different positioning for Decision nodes (move lower)
    let verticalOffset = 2 / zoom;
    input.style.top = `${renderedY + verticalOffset}px`;

    input.style.transform = 'translate(-50%, 0)';
    input.style.width = `${nodeWidth * zoom}px`;
    input.style.textAlign = 'center';
    input.style.zIndex = '9999';
    input.style.border = 'none';
    input.style.outline = 'none';
    input.style.boxShadow = 'none';
    input.style.backgroundColor = 'transparent';
    input.style.fontFamily = 'Nunito Sans, sans-serif';
    input.style.fontWeight = '600';
    input.style.fontSize = `${16 * zoom}px`;
    input.style.lineHeight = 'inherit';
    input.style.color = 'inherit';
    input.style.overflow = 'hidden';
    input.style.textOverflow = 'ellipsis';
    input.style.whiteSpace = 'nowrap';

    document.body.appendChild(input);
    input.focus();
    input.select();

    let isEnterOrEsc = false;

    input.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' || e.key === 'Escape') {
            e.preventDefault();
            isEnterOrEsc = true;
            const newTitle = input.value.trim();

            if (e.key === 'Enter') {
                node.data('title', newTitle);
            } else {
                node.data('title', originalTitle);
            }

            updateHtmlLabelForNode(node);

            let guidelineData = editorCode.get();
            let data = cy.json();
            guidelineData.guidelineElements = data.elements;
            showJsonEditor(guidelineData);
            saveState();

            try {
                input.remove();
            }
            catch (err) { }
        }
    });

    input.addEventListener('blur', function () {
        if (!isEnterOrEsc) {
            node.data('title', input.value.trim());

            updateHtmlLabelForNode(node);

            let guidelineData = editorCode.get();
            let data = cy.json();
            guidelineData.guidelineElements = data.elements;
            showJsonEditor(guidelineData);
            saveState();
        }
        if (input.parentNode) {
            input.parentNode.removeChild(input);
        }
    });
}

function showNodeMenu(node) {
    const existingMenu = document.getElementById('node-context-menu');
    if (existingMenu) {
        existingMenu.remove();
    }

    const menu = document.createElement('div');
    menu.id = 'node-context-menu';
    menu.style.position = 'absolute';
    menu.style.background = 'white';
    menu.style.border = '1px solid #ccc';
    menu.style.borderRadius = '8px';
    menu.style.zIndex = '8';

    const menuWidth = 40;
    const menuHeight = 170;
    const padding = 10;

    const canvas = document.getElementById('cy');
    const canvasRect = canvas.getBoundingClientRect();

    const nodePosition = node.position();
    const nodeX = nodePosition.x;
    const nodeY = nodePosition.y;

    const zoom = cy.zoom();
    const pan = cy.pan();
    const renderedX = (nodeX * zoom) + pan.x;
    const renderedY = (nodeY * zoom) + pan.y;

    let left = canvasRect.left + renderedX - menuWidth / 2 - 65;
    let top = canvasRect.top + renderedY - menuHeight - padding + 113;
    const canvasWidth = canvasRect.width;

    if (left < canvasRect.left) {
        left = canvasRect.left + 10;
    } else if (left + menuWidth > canvasRect.left + canvasWidth) {
        left = canvasRect.left + canvasWidth - menuWidth - 10;
    }

    if (top < canvasRect.top + 263) {
        top = canvasRect.top + renderedY + padding + 42;
    }

    menu.style.left = `${left}px`;
    menu.style.top = `${top}px`;

    menu.innerHTML = `
        <button id="properties" data-toggle="tooltip" title="Properties" class="node-context-menu-button">
            <img id="propertyIcon" alt="" src="/css/img/icons/ClinicalPathway/Properties.svg" />
        </button>
        <button id="shapes" data-toggle="tooltip" title="Shapes" class="node-context-menu-button">
            <img id="shapesIcon" alt="" src="/css/img/icons/ClinicalPathway/Shapes.svg" />
        </button>
        <button id="duplicate-node" data-toggle="tooltip" title="Duplicate node" class="node-context-menu-button">
            <img id="duplicateNodeIcon" alt="" src="/css/img/icons/ClinicalPathway/Duplicate node.svg" />
        </button>
        <button id="delete-node" data-toggle="tooltip" title="Delete node" class="node-context-menu-button">
            <img id="deleteNodeIcon" alt="" src="/css/img/icons/ClinicalPathway/Delete node.svg" />
        </button>
    `;

    document.body.appendChild(menu);

    $('[data-toggle="tooltip"]').tooltip({
        placement: 'bottom',
        trigger: 'hover'
    });

    $('#node-context-menu .node-context-menu-button').hover(
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

    $('#node-context-menu .node-context-menu-button').on('click', function () {
        $(this).tooltip('hide');
    });

    document.getElementById('properties').addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        hideShapeDropdown();
        showNodeInfoDropdown(node);
    });

    document.getElementById('shapes').addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        showShapeChangeModal(node);
    });

    document.getElementById('duplicate-node').addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        $('#node-info-dropdown').hide();
        hideShapeDropdown();
        duplicateNode(node);

        menu.remove();
    });

    document.getElementById('delete-node').addEventListener('click', (e) => {
        e.preventDefault();
        e.stopPropagation();
        hideShapeDropdown();
        cy.remove(node);
        updateJsonEditor();     
        enforceZoomLevel();
        saveState();
        $('#node-info-dropdown').hide();
        menu.remove();
    });

    setTimeout(() => {
        document.addEventListener('click', function handler(e) {
            if (!e.target.closest('#node-context-menu') && !e.target.closest('#node-info-dropdown')) {
                menu.remove();
                document.removeEventListener('click', handler);
            }
        });
    }, 0);
}

function showNodeInfoDropdown(node) {
    $('#change-shape-dropdown').hide();
    const shapesIcon = $('#shapesIcon');
    const currentSrc = shapesIcon.attr('src');
    const defaultSrc = currentSrc.replace('%20hover', '');
    shapesIcon.attr('src', defaultSrc);

    const infoDropdown = $('#node-info-dropdown');
    infoDropdown.show();

    const contextMenu = document.getElementById('node-context-menu');
    const menuRect = contextMenu.getBoundingClientRect();

    const canvas = document.getElementById('cy');
    const canvasRect = canvas.getBoundingClientRect();

    const nodePosition = node.position();
    const zoom = cy.zoom();
    const pan = cy.pan();
    const renderedY = (nodePosition.y * zoom) + pan.y + canvasRect.top;

    const left = menuRect.left + (menuRect.width / 2) - (infoDropdown.width() / 2) - 110 - getVisibleSidebarWidth('guidelineSidebar') - getSidebarWidthDifference();
    let top = -60;
    const dropdownHeight = infoDropdown.outerHeight();
    const offset = 5;

    const propertyIcon = document.getElementById('propertyIcon');
    let originalSrc;
    if (propertyIcon) {
        originalSrc = propertyIcon.src;
        propertyIcon.src = originalSrc;
    }

    if (menuRect.top < renderedY) {
        top += menuRect.top - dropdownHeight - offset;
    } else {
        top += menuRect.bottom + offset;
    }

    infoDropdown.css({
        position: 'absolute',
        left: `${left}px`,
        top: `${top}px`,
        'z-index': 1000
    });

    // Prefill values
    $('#node-title').val(node.data('title') || '');
    $('#node-type').val(node.data('type') || 'Statement');
    $('#node-thesaurusId').val(node.data('thesaurusId') || 0);


    $(document).off('keydown.saveNodeInfo').on('keydown.saveNodeInfo', function (event) {
        if (event.key === 'Enter' && infoDropdown.is(':visible')) {
            event.preventDefault();
            event.stopPropagation();

            node.data('title', $('#node-title').val());
            node.data('type', $('#node-type').val());
            node.data('thesaurusId', parseInt($('#node-thesaurusId').val()) || 0);

            updateJsonEditor();           
            enforceZoomLevel();
            saveState();

            infoDropdown.hide();
            currentMode = null;
        }
    });

    $('#save-node-info').off('click').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        node.data('title', $('#node-title').val());
        node.data('type', $('#node-type').val());
        node.data('thesaurusId', parseInt($('#node-thesaurusId').val()) || 0);

        updateJsonEditor();      
        enforceZoomLevel();
        saveState();

        infoDropdown.hide();
        currentMode = null;
    });

    $('#cancel-node-info').off('click').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        hideAllMenus();
        infoDropdown.hide();
        currentMode = null;
    });

    setTimeout(() => {
        $(document).on('click.nodeInfoDropdown', function handler(e) {
            if (!$(e.target).closest('#node-info-dropdown').length && !$(e.target).closest('#node-context-menu').length) {
                infoDropdown.hide();
                currentMode = null;
                $(document).off('click.nodeInfoDropdown');
            }
        });
    }, 0);
}

function showShapeChangeModal(node) {
    $('#node-info-dropdown').hide();
    const $propertyIcon = $('#propertyIcon');
    const currentSrc = $propertyIcon.attr('src');
    const defaultSrc = currentSrc.replace('%20hover', '');
    $propertyIcon.attr('src', defaultSrc);

    const shapeDropdown = $('#change-shape-dropdown');
    shapeDropdown.show();

    const contextMenu = document.getElementById('node-context-menu');
    const menuRect = contextMenu.getBoundingClientRect();

    const canvas = document.getElementById('cy');
    const canvasRect = canvas.getBoundingClientRect();

    const nodePosition = node.position();
    const zoom = cy.zoom();
    const pan = cy.pan();
    const renderedY = (nodePosition.y * zoom) + pan.y + canvasRect.top;

    const left = menuRect.left + (menuRect.width / 2) - (shapeDropdown.width() / 2) - 110 - getVisibleSidebarWidth('guidelineSidebar') - getSidebarWidthDifference();
    let top = -60;

    const dropdownHeight = shapeDropdown.outerHeight();
    const offset = 5;

    const shapesIcon = document.getElementById('shapesIcon');
    let originalSrc;
    if (shapesIcon) {
        originalSrc = shapesIcon.src;
        shapesIcon.src = originalSrc;
    }

    if (menuRect.top < renderedY) {
        top += menuRect.top - dropdownHeight - offset;
    } else {
        top += menuRect.bottom + offset;
    }

    shapeDropdown.css({
        position: 'absolute',
        left: `${left}px`,
        top: `${top}px`,
        'z-index': 1000
    });

    currentMode = 'change-shape';

    setTimeout(() => {
        $(document).on('click.shapeDropdown', function handler(e) {
            if (!$(e.target).closest('#change-shape-dropdown').length && !$(e.target).closest('#node-context-menu').length) {
                shapeDropdown.hide();
                currentMode = null;
                $(document).off('click.shapeDropdown');
            }
        });
    }, 0);
}

function duplicateNode(node) {
    const newNodeId = generateUniqueNodeId();

    const position = node.position();
    const newPosition = {
        x: position.x + 50,
        y: position.y + 50
    };

    const newNode = {
        group: 'nodes',
        data: {
            id: newNodeId,
            state: node.data('state'),
            thesaurusId: node.data('thesaurusId'),
            title: node.data('title') + ' (Copy)',
            type: node.data('type'),
            hyperlink: node.data('hyperlink')
        },
        position: newPosition,
        classes: node.classes()
    };

    const addedNode = cy.add(newNode);
    updateHtmlLabelForNode(addedNode);

    const newNodeElement = cy.getElementById(newNodeId);
    newNodeElement.style({
        'border-color': node.style('border-color'),
        'border-width': node.style('border-width')
    });

    updateJsonEditor();   
    enforceZoomLevel();
    saveState();
}

function duplicateNodeAtMousePosition(node, position) {
    const newNodeId = generateUniqueNodeId();

    const newNode = {
        group: 'nodes',
        data: {
            id: newNodeId,
            state: node.data('state'),
            thesaurusId: node.data('thesaurusId'),
            title: node.data('title') + ' (Copy)',
            type: node.data('type'),
            hyperlink: node.data('hyperlink')
        },
        position: {
            x: position.x,
            y: position.y
        },
        classes: node.classes()
    };

    const addedNode = cy.add(newNode);
    updateHtmlLabelForNode(addedNode);

    const newNodeElement = cy.getElementById(newNodeId);
    newNodeElement.style({
        'border-color': node.style('border-color'),
        'border-width': node.style('border-width')
    });

    updateJsonEditor();
    enforceZoomLevel();
    saveState();
}

function generateUniqueNodeId() {
    let maxId = 0;
    cy.nodes().forEach(n => {
        const nodeId = n.id();
        if (nodeId.startsWith('n_')) {
            const num = parseInt(nodeId.replace('n_', ''), 10);
            if (!isNaN(num) && num > maxId) {
                maxId = num;
            }
        }
    });
    return 'n_' + (maxId + 1);
}

function resetInteractionState() {
    currentMode = null;
    selectedDirection = null;
    hoveredNode = null;
}

function getNewNodePosition(cy, offsetX, isPlaceholder = false) {
    const existingNodes = cy.nodes();
    const existingEdges = cy.edges();

    const edgeConnectedNodes = existingEdges.map(edge => {
        const source = cy.getElementById(edge.data('source'));
        const target = cy.getElementById(edge.data('target'));
        return [source, target];
    }).reduce((acc, nodes) => acc.concat(nodes), []);

    const allRelevantNodes = cy.collection([...existingNodes, ...edgeConnectedNodes]);

    const allRelevantNodesArray = allRelevantNodes.toArray();
    const uniqueNodes = allRelevantNodesArray.filter((value, index, self) =>
        self.findIndex(n => n.id() === value.id()) === index
    );

    const rightmostNode = uniqueNodes.reduce((maxNode, currentNode) => {
        return currentNode.position().x > maxNode.position().x ? currentNode : maxNode;
    }, uniqueNodes[0]);

    let position = {};

    if (rightmostNode) {
        if (rightmostNode.hasClass('placeholder-node') && !isPlaceholder) {
            offsetX = 100;
        }
        position.x = rightmostNode.position().x + offsetX;
        position.y = rightmostNode.position().y;
    } else {
        const viewport = cy.extent();
        position.x = (viewport.x1 + viewport.x2) / 2;
        position.y = (viewport.y1 + viewport.y2) / 2;
    }

    return position;
}