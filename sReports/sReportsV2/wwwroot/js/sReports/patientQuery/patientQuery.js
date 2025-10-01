function viewEntity(event, id) {
    event.preventDefault();
    window.location.href = `/FormInstance/View?FormInstanceId=${id}`;
}

function reloadTable() {
    let requestObject = applyActionsBeforeServerReload(['Page', 'PageSize']);

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }
    updateSearchObject(requestObject.DiagnoseId);
    callServer({
        type: 'GET',
        url: '/PatientQuery/GetPatientSemanticQuery',
        data: requestObject,
        success: function (data) {
            setTableContent(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function getFilterParametersObject() {
    let result = {};
    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    }
    else {
        addPropertyToObject(result, 'DiagnoseId', $("#diagnoseId").val());
    }

    return result;
}

function getFilterParametersObjectForDisplay(filterObject) {
    if (filterObject.hasOwnProperty('DiagnoseId')) {
        let display = getSelectedSelect2Label("diagnoseId");
        if (display) {
            addPropertyToObject(filterObject, 'DiagnoseId', display);
        }
    }

    return filterObject;
}

function mainFilter() {
    filterData();
}

function advanceFilter() {
    mainFilter();
}

function updateSearchObject(thesaurusId) {
    let searchObject = {
        id: thesaurusId,
        text: getSelectedSelect2Label("diagnoseId")
    };
    if (searchObject.text) {
        localStorage.setItem("semanticSearchObject", JSON.stringify(searchObject));
    }
}

function getSearchObjectFromStorage() {
    let searchObject = JSON.parse(localStorage.getItem("semanticSearchObject"));
    localStorage.setItem("semanticSearchObject", {});
    return searchObject;
}

function initialFilterLoad(placeholder) {
     if (hasParamInUrl('DiagnoseId')) {
         let initDataSource = [];
         let data = getSearchObjectFromStorage();
         if (!jQuery.isEmptyObject(data)) {
             initDataSource.push(data);
         }
         $('#diagnoseId').select2(
             getSelect2Object(
                 {
                     placeholder: placeholder,
                     url: `/PatientQuery/GetAutocompleteData`,
                     initialDataSource: initDataSource,
                     templateResult: formatSkosResult
                 }
             )
         );

         if (initDataSource.length > 0) {
             $('#diagnoseId').val(initDataSource[0].id.toString()).trigger('change');
             reloadTable();
         }
     } else {
         $('#diagnoseId').initSelect2(
             getSelect2Object(
                 {
                     placeholder: placeholder,
                     url: `/PatientQuery/GetAutocompleteData`,
                     templateResult: formatSkosResult
                 }
             )
         );
     }
}

function formatSkosResult(skosData) {
    if (!skosData.id) {
        return skosData.text;
    }
    let synonymSuffix = skosData.issynonym ? '(synonym)' : '';
    let className = skosData.issynonym ? 'alt-label-option' : 'pref-label-option';
    var $episode = $(`<span class="${className}">${skosData.text} ${synonymSuffix}</span>`);
    return $episode;
}

function loadThesaurusGraph(graphData) {
    if (graphData.nodes.length == 0) return;
    localStorage.setItem("semanticGraphData", JSON.stringify(graphData.nodes.map(n => n.id)));
    var container = document.getElementById("thesaurusNetwork");
    var options = {};

    let triggeredNode = graphData.nodes.find(n => n.id == graphData.startingMatchingThesaurusId);
    let [affectedEdges, affectedNodes] = getAffectedElements(triggeredNode, graphData);
    
    const nodesFilter = (node) => {
        let nodeMatch = affectedNodes.find(n => n.id == node.id);
        if (nodeMatch) {
            nodeMatch.shown = true;
            return true;
        } else {
            return false;
        }
    };

    const edgesFilter = (edge) => {
        let edgeMatch = affectedEdges.find(tE => tE.from == edge.from && tE.to == edge.to);
        if (edgeMatch) {
            edgeMatch.shown = true;
            return true;
        } else {
            return false;
        }
    };

    const nodesView = new vis.DataView(new vis.DataSet(graphData.nodes), { filter: nodesFilter });
    const edgesView = new vis.DataView(new vis.DataSet(graphData.edges), { filter: edgesFilter });

    var network = new vis.Network(container, { nodes: nodesView, edges: edgesView }, options);

    network.on("click", function (params) {
        if (params.nodes.length > 0) {
            const nodeId = params.nodes[0];
            triggeredNode = graphData.nodes.find(n => n.id == nodeId);
            [affectedEdges, affectedNodes] = getAffectedElements(triggeredNode, graphData);
            nodesView.refresh();
            edgesView.refresh();
        }
    });
}

function getAffectedElements(triggeredNode, graphData) {
    let edgeFilter, nodeFilter, affectedEdges, affectedNodes;
    if (triggeredNode.expanded) {
        edgeFilter = e => e.from != triggeredNode.id && e.shown;
        affectedEdges = graphData.edges.filter(edgeFilter);
        triggeredNode.expanded = false;
        nodeFilter = node => affectedEdges.some(tE => tE.from == node.id || tE.to == node.id);
    } else {
        edgeFilter = e => e.from == triggeredNode.id || e.shown;
        affectedEdges = graphData.edges.filter(edgeFilter);
        triggeredNode.expanded = true;
        nodeFilter = node => affectedEdges.some(tE => tE.from == node.id || tE.to == node.id) || node.shown;
    }
    affectedNodes = graphData.nodes.filter(nodeFilter);
    
    resetShownStatus(graphData);
    return [affectedEdges, affectedNodes];
}

function resetShownStatus(graphData) {
    for (let shownEdge of graphData.edges.filter(e => e.shown)) {
        shownEdge.shown = false;
    }
    for (let shownNode of graphData.nodes.filter(n => n.shown)) {
        shownNode.shown = false;
    }
}