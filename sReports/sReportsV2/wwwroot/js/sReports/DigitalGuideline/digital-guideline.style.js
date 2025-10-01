const guidelineStyle = [
    {
        selector: 'node',
        style: {
            'background-opacity': 0,
            'border-width': 0,
            'label': '',
            'width': 1,
            'height': 1,
            'opacity': 0
        }
    },
    {
        selector: 'node[type="Event"]',
        style: {
            'shape': 'ellipse',
            'width': 135,
            'height': 48,
            'border-radius': 24
        }
    },
    {
        selector: 'node[type="Statement"]',
        style: {
            'shape': 'round-rectangle',
            'width': 135,
            'height': 48,
        }
    },
    {
        selector: 'node[type="Decision"]',
        style: {
            'shape': 'round-diamond',
            'width': 89.5,
            'height': 89.5,
            'padding': '20px'
        }
    },
    {
        selector: '.placeholder-node',
        style: {
            'width': '1px',
            'height': '1px',
            'background-color': 'transparent',
            'border-width': '0px',
            'shape': 'ellipse',
            'label': '',
            'cursor': 'pointer',
            'opacity': 0
        }
    },
    {
        selector: 'edge',
        style: {
            'width': 1,
            'line-color': 'black',
            'target-arrow-shape': 'triangle',
            'target-arrow-color': 'black',
            'label': 'data(title)',
            'font-family': 'Nunito Sans, sans-serif',
            'font-weight': 600,
            'font-size': 16,
            'text-background-color': 'white',
            'text-background-opacity': 1,
            'text-max-width': '100px',
            'text-wrap': 'ellipsis',
            'text-overflow-wrap': 'ellipsis',
            'text-justification': 'center',
            'text-halign': 'center',
            'text-valign': 'center',
            'curve-style': 'bezier',
            'control-point-distance': 30,
            'control-point-weight': 0.5
        }
    },
    {
        selector: 'edge.highlighted',
        style: {
            'width': 3,
            'line-color': 'black',
            'target-arrow-color': 'black',
            'shadow-blur': 10,
            'shadow-color': '#888',
            'shadow-opacity': 0.7,
            'shadow-offset-x': 0,
            'shadow-offset-y': 0,
            'text-max-width': '100px',
            'text-wrap': 'ellipsis',
            'text-overflow-wrap': 'ellipsis',
            'text-justification': 'center',
            'text-halign': 'center',
            'text-valign': 'center'
        }
    },
    {
        selector: 'edge.temp-bezier',
        style: {
            'width': 1,
            'line-color': 'black',
            'target-arrow-shape': 'triangle',
            'target-arrow-color': 'black',
            'source-arrow-shape': 'triangle',
            'source-arrow-color': 'black',
            'label': 'data(title)',
            'font-family': 'Nunito Sans, sans-serif',
            'font-weight': 600,
            'font-size': 16,
            'text-background-color': 'white',
            'text-background-opacity': 1,
            'text-max-width': '100px',
            'text-wrap': 'ellipsis',
            'text-overflow-wrap': 'ellipsis',
            'text-justification': 'center',
            'text-halign': 'center',
            'text-valign': 'center',
            'curve-style': 'bezier',
            'control-point-distance': 30,
            'control-point-weight': 0.5
        }
    },
    {
        selector: 'edge.unbundled-bezier',
        style: {
            'width': 1,
            'line-color': 'black',
            'target-arrow-shape': 'triangle',
            'target-arrow-color': 'black',
            'label': 'data(title)',
            'font-family': 'Nunito Sans, sans-serif',
            'font-weight': 600,
            'font-size': 16,
            'text-background-color': 'white',
            'text-background-opacity': 1,
            'text-max-width': '100px',
            'text-wrap': 'ellipsis',
            'text-overflow-wrap': 'ellipsis',
            'text-justification': 'center',
            'text-halign': 'center',
            'text-valign': 'center',
            'curve-style': 'unbundled-bezier',
            'control-point-distances': [40, -40],
            'control-point-weights': [0.25, 0.75]
        }
    },
    {
        selector: 'edge.unbundled-bezier.highlighted',
        style: {
            'width': 3,
            'line-color': 'black',
            'target-arrow-color': 'black',
            'shadow-blur': 10,
            'shadow-color': '#888',
            'shadow-opacity': 0.7,
            'shadow-offset-x': 0,
            'shadow-offset-y': 0,
            'text-max-width': '100px',
            'text-wrap': 'ellipsis',
            'text-overflow-wrap': 'ellipsis',
            'text-justification': 'center',
            'text-halign': 'center',
            'text-valign': 'center'
        }
    },
    {
        selector: 'edge.taxi',
        style: {
            'width': 1,
            'line-color': 'black',
            'target-arrow-shape': 'triangle',
            'target-arrow-color': 'black',
            'label': 'data(title)',
            'font-family': 'Nunito Sans, sans-serif',
            'font-weight': 600,
            'font-size': 16,
            'text-background-color': 'white',
            'text-background-opacity': 1,
            'text-max-width': '100px',
            'text-wrap': 'ellipsis',
            'text-overflow-wrap': 'ellipsis',
            'text-justification': 'center',
            'text-halign': 'center',
            'text-valign': 'center',
            'curve-style': 'taxi',
            'taxi-direction': 'auto',
            'taxi-turn': 50,
            'taxi-turn-min-distance': 10
        }
    },
    {
        selector: 'edge.taxi.highlighted',
        style: {
            'width': 3,
            'line-color': 'black',
            'target-arrow-color': 'black',
            'shadow-blur': 10,
            'shadow-color': '#888',
            'shadow-opacity': 0.7,
            'shadow-offset-x': 0,
            'shadow-offset-y': 0,
            'text-max-width': '100px',
            'text-wrap': 'ellipsis',
            'text-overflow-wrap': 'ellipsis',
            'text-justification': 'center',
            'text-halign': 'center',
            'text-valign': 'center'
        }
    }
];