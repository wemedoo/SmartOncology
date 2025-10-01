function recalculateSidebarWidth() {
    const sidebar = document.getElementById('guidelineSidebar');
    const cyContainer = document.getElementById('cy');
    const containerWidth = document.querySelector('.guideline-container').offsetWidth - 31;
    const sidebarWidthPercentage = 0.3;

    if (sidebar && cyContainer && !sidebar.classList.contains('json-collapsed')) {
        cyContainer.style.width = `${containerWidth * (1 - sidebarWidthPercentage) - 10}px`;
    } else if (cyContainer) {
        cyContainer.style.width = `${containerWidth}px`;
    }

    setTimeout(() => {
        if (typeof cy !== 'undefined' && cy) {
            cy.resize();
            cy.center();
        }
    }, 300);
}

function adjustLayout() {
    const cyContainer = document.getElementById('cy');
    const sidebar = document.getElementById('guidelineSidebar');
    const containerWidth = document.querySelector('.guideline-container').offsetWidth - 31;
    const sidebarWidthPercentage = 0.3;

    if (!cyContainer || !sidebar) return;

    if (sidebar.hasAttribute('hidden') || sidebar.classList.contains('json-collapsed')) {
        cyContainer.style.width = `${containerWidth}px`;
    } else {
        cyContainer.style.width = `${containerWidth * (1 - sidebarWidthPercentage) - 10}px`;
    }

    setTimeout(() => {
        if (cy) {
            cy.resize();
            enforceZoomLevel();
        }
    }, 300);
}

function getSidebarWidthDifference() {
    const sidebar = document.getElementById("sidebar");
    const baseWidth = 56;

    if (sidebar && sidebar.classList.contains("expanded")) {
        return sidebar.offsetWidth - baseWidth;
    }

    return 0;
}

function getVisibleSidebarWidth(sidebarId) {
    const sidebar = document.getElementById(sidebarId);
    if (sidebar && sidebar.offsetParent !== null) {
        return sidebar.offsetWidth;
    }
    return 0;
}

function enforceZoomLevel() {
    if (!cy) return;

    hideAllMenus();
    hideShapeDropdown();

    const nodes = cy.nodes();
    if (nodes.length === 0) return;
}

function initGraphZoom() {
    cy.layout({ name: 'preset' }).run();

    const bb = cy.elements().boundingBox();
    const container = cy.container();
    const containerWidth = container.clientWidth;
    const containerHeight = container.clientHeight;
    const graphWidth = bb.w;
    const graphHeight = bb.h;
    const padding = 20;

    if (graphWidth + padding <= containerWidth && graphHeight + padding <= containerHeight) {
        cy.zoom(1);
        cy.center();
    } else {
        cy.fit(cy.elements(), padding);
    }
}