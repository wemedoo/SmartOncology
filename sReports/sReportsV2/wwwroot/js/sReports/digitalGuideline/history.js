function undo() {
    if (historyIndex > 0) {
        historyIndex--;
        restoreState(pathwayHistory[historyIndex]);
        updateHistoryButtons();
    }
}

function redo() {
    if (historyIndex < pathwayHistory.length - 1) {
        historyIndex++;
        restoreState(pathwayHistory[historyIndex]);
        updateHistoryButtons();
    }
}

function updateHistoryButtons() {
    $('#undo-btn').prop('disabled', historyIndex <= 0);
    $('#redo-btn').prop('disabled', historyIndex >= pathwayHistory.length - 1);
    activateSelectIcon();
}

function saveState() {
    hideAllMenus();
    hideShapeDropdown();

    const graphJson = cy.json();

    const tempSourceNodeStyle = tempSourceNode ? tempSourceNode.style() : null;
    const tempTargetNodeStyle = tempTargetNode ? tempTargetNode.style() : null;
    const tempEdgeStyle = tempEdge ? tempEdge.style() : null;
    const tempSourceNodeClasses = tempSourceNode ? tempSourceNode.classes() : null;
    const tempTargetNodeClasses = tempTargetNode ? tempTargetNode.classes() : null;
    const tempEdgeClasses = tempEdge ? tempEdge.classes() : null;

    const nodePositions = {};
    cy.nodes().forEach(node => {
        nodePositions[node.id()] = { x: node.position().x, y: node.position().y };
    });

    const edgeData = tempEdge ? {
        id: tempEdge.id(),
        source: tempEdge.data('source'),
        target: tempEdge.data('target'),
        state: tempEdge.data('state') || 'NotStarted',
        thesaurusId: tempEdge.data('thesaurusId') || 0,
        title: tempEdge.data('title') || '',
        type: tempEdge.data('type') || 'Edge'
    } : null;

    const state = {
        graph: graphJson,
        edgeCreationMode: edgeCreationMode,
        tempEdgeId: tempEdge ? tempEdge.id() : null,
        tempSourceNodeId: tempSourceNode ? tempSourceNode.id() : null,
        tempTargetNodeId: tempTargetNode ? tempTargetNode.id() : null,
        sourceNodeId: sourceNode ? sourceNode.id() : null,
        targetNodeId: targetNode ? targetNode.id() : null,
        nodePositions: nodePositions,
        tempSourceNodeStyle: tempSourceNodeStyle,
        tempTargetNodeStyle: tempTargetNodeStyle,
        tempEdgeStyle: tempEdgeStyle,
        tempSourceNodeClasses: tempSourceNodeClasses,
        tempTargetNodeClasses: tempTargetNodeClasses,
        tempEdgeClasses: tempEdgeClasses,
        edgeData: edgeData,
        selectedArrowType: selectedArrowType
    };

    pathwayHistory.splice(historyIndex + 1, pathwayHistory.length - historyIndex - 1);
    pathwayHistory.push(state);
    historyIndex++;
    updateHistoryButtons();
}

function restoreState(state) {
    cy.off('mouseover', 'node.placeholder-node');
    cy.off('mouseout', 'node.placeholder-node');
    cy.off('drag', 'node.placeholder-node');
    cy.off('dragfree', 'node.placeholder-node');

    cy.elements().remove();
    cy.json(state.graph);

    edgeCreationMode = state.edgeCreationMode;
    tempEdge = state.tempEdgeId ? cy.getElementById(state.tempEdgeId) : null;
    tempSourceNode = state.tempSourceNodeId ? cy.getElementById(state.tempSourceNodeId) : null;
    tempTargetNode = state.tempTargetNodeId ? cy.getElementById(state.tempTargetNodeId) : null;
    sourceNode = state.sourceNodeId ? cy.getElementById(state.sourceNodeId) : null;
    targetNode = state.targetNodeId ? cy.getElementById(state.targetNodeId) : null;
    selectedArrowType = state.selectedArrowType;

    if (sourceNode && targetNode) {
        edgeCreationMode = false;
    }

    cy.nodes().forEach(node => {
        const pos = state.nodePositions[node.id()];
        if (pos) {
            node.position({ x: pos.x, y: pos.y });
        }
    });

    if (tempSourceNode && state.tempSourceNodeStyle) {
        tempSourceNode.style(state.tempSourceNodeStyle);
        if (state.tempSourceNodeClasses) {
            tempSourceNode.classes(state.tempSourceNodeClasses);
        }
        tempSourceNode.grabbable(true);
        tempSourceNode.locked(false);
    }

    if (tempTargetNode && state.tempTargetNodeStyle) {
        tempTargetNode.style(state.tempTargetNodeStyle);
        if (state.tempTargetNodeClasses) {
            tempTargetNode.classes(state.tempTargetNodeClasses);
        }
        tempTargetNode.grabbable(true);
        tempTargetNode.locked(false);
    }

    if (tempEdge && state.edgeData) {
        tempEdge.json({
            data: {
                id: state.edgeData.id,
                source: state.edgeData.source,
                target: state.edgeData.target,
                state: state.edgeData.state,
                thesaurusId: state.edgeData.thesaurusId,
                title: state.edgeData.title,
                type: state.edgeData.type
            },
            group: 'edges',
            removed: false,
            selected: false,
            selectable: state.edgeCreationMode ? false : true,
            locked: false,
            grabbable: true,
            pannable: true,
            classes: state.tempEdgeClasses || 'edge-label-background'
        });
        if (state.tempEdgeStyle) {
            tempEdge.style(state.tempEdgeStyle);
        }
    }

    if (edgeCreationMode && (tempSourceNode || tempTargetNode) && tempEdge) {
        setupEdgeCreationHandlers();
    }

    updateJsonEditor();
}