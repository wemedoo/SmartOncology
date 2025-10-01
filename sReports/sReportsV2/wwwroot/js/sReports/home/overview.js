let myChart = null;

function respondCanvas(chartData) {
    var c = $('#documentsPerThesaurus');
    var ctx = c.get(0).getContext('2d');
    var container = c.parent().parent();
    var $container = $(container);
    c.attr('width', $container.width());
    c.attr('height', $container.height() - 60);
   
    if (myChart) {
        myChart.destroy();
        myChart = null;
    }
    myChart = new Chart(ctx, chartData);
}

function getDocumentChartData() {
    destroyChartIfExist('documentsPerThesaurus');

    callServer({
        url: '/FormInstance/GetDocumentsPerDomain',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            const canvas = document.getElementById('documentsPerThesaurus');
            const ctx = canvas.getContext('2d');
            const gradient = ctx.createLinearGradient(0, 0, 0, 450);
            gradient.addColorStop(0.5, '#34b5bf');
            gradient.addColorStop(1, '#1c94a3');

            chartData = {
                type: 'bar',
                data: {
                    labels: response.map(x => x.label),
                    datasets: [
                        {
                            label: '# Documents',
                            backgroundColor: gradient,
                            data: response.map(x => x.count)
                        }
                    ]
                },
                options: {
                    scales: {
                        x: {
                            grid: {
                                display: true,
                                color: 'transparent',
                                drawBorder: true,
                                zeroLineColor: 'lightgray'
                            },
                            ticks: {
                                beginAtZero: true,
                                color: 'black',
                                font: { size: 13 },
                                callback: function (value, index, ticks) {
                                    var characterLimit = 20;
                                    let label = this.getLabelForValue(value);
                                    if (label && label.length >= characterLimit) {
                                        return label.substring(0, characterLimit - 1).trim() + '...';
                                    }
                                    return label;
                                }
                            },
                            display: true
                        },
                        y: {
                            grid: {
                                display: true,
                                color: '#e0e0e0',
                                drawBorder: true,
                                zeroLineColor: 'lightgray'
                            },
                            ticks: {
                                beginAtZero: true,
                                color: 'lightgrey',
                                font: { size: 15 },
                                padding: 25
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            enabled: false,
                            external: function (context) {
                                var tooltipEl = document.getElementById('chartjs-tooltip');
                                if (!tooltipEl) {
                                    tooltipEl = document.createElement('div');
                                    tooltipEl.id = 'chartjs-tooltip';
                                    tooltipEl.innerHTML = '<table></table>';
                                    document.body.appendChild(tooltipEl);
                                }

                                let tooltipModel = context.tooltip;
                                if (tooltipModel.opacity === 0) {
                                    tooltipEl.style.opacity = 0;
                                    return;
                                }

                                tooltipEl.classList.remove('above', 'below', 'no-transform');
                                if (tooltipModel.yAlign) {
                                    tooltipEl.classList.add(tooltipModel.yAlign);
                                } else {
                                    tooltipEl.classList.add('no-transform');
                                }

                                if (tooltipModel.body && tooltipModel.dataPoints.length > 0) {
                                    let dataPoint = tooltipModel.dataPoints[0];
                                    let chartItem = response[dataPoint.dataIndex];
                                    let label = `${chartItem.label} (${chartItem.domain})`;
                                    let count = `${dataPoint.dataset.label}: ${chartItem.count}`;

                                    var innerHtml = `<div><div class="tooltip-label-value">value</div><div class="tooltip-value">${count}</div> <div class="tooltip-label-value">document</div><div class="tooltip-value">${label}</div></div>`;
                                    var tableRoot = tooltipEl.querySelector('table');
                                    tableRoot.innerHTML = innerHtml;
                                }

                                var position = context.chart.canvas.getBoundingClientRect();
                                tooltipEl.style.opacity = 1;
                                tooltipEl.style.position = 'absolute';
                                tooltipEl.style.left = position.left + window.pageXOffset + tooltipModel.caretX - 40 + 'px';
                                tooltipEl.style.top = position.top + window.pageYOffset + tooltipModel.caretY + 'px';
                                tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                                tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
                                tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                                tooltipEl.style.pointerEvents = 'none';
                                tooltipEl.style.width = '254px';
                                tooltipEl.style.backgroundColor = 'white';
                                tooltipEl.style.borderRadius = '8px';
                                tooltipEl.style.boxShadow = '0 2px 14px 0 rgba(199, 199, 199, 0.5)';
                                tooltipEl.style.padding = '20px';
                            }
                        }
                    },
                    legend: {
                        display: true,
                        labels: {
                            color: 'rgb(0, 0, 0)'
                        }
                    }
                }
            };
            respondCanvas(chartData);
        },
        error: function (xhr, textStatus, thrownError) {
            console.error('AJAX error:', xhr);
        }
    });
};

function getTotalChartData() {
    callServer({
        url: '/ThesaurusEntry/GetEntriesCount',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            $('#totalThesaurusEntries').html(response.total);
            $('#totalUmls').html(response.totalUmls);
            $('.items').show();

        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    })
}

function getOrganizationUsersCountData() {
    callServer({
        url: '/Organization/GetUsersByOrganizationCount',
        method: 'GET',
        success: function (response) {
            $('#organizationUsersCount').html(response);

        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr);
        }
    });
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

$(document).ready(function () {
    const debouncedGetDocumentChartData = debounce(getDocumentChartData, 500);
    $(window).resize(getDocumentChartData);

    getDocumentChartData();
    getTotalChartData();
    getOrganizationUsersCountData();
});