var chart1;
$(document).ready(function () {
    const targetNode = document.getElementById('DateRange');

    new MutationObserver(() => {
        var selectedRange = targetNode.innerHTML;

        if (selectedRange !== '') {

            var dateRange = selectedRange.split(' - ');
            chart1.destroy();
            drawRentalsChart(dateRange[0], dateRange[1]);
        }
        

    }).observe(targetNode, { childList: true, subtree: true, attributes: true });
        });



    


drawRentalsChart();


function drawRentalsChart(startDate = null , endDate= null) {
    var element = document.getElementById('RentalsPerDay');

    var height = parseInt(KTUtil.css(element, 'height'));
    var labelColor = KTUtil.getCssVariableValue('--kt-gray-500');
    var borderColor = KTUtil.getCssVariableValue('--kt-gray-200');
    var baseColor = KTUtil.getCssVariableValue('--kt-primary');
    var lightColor = KTUtil.getCssVariableValue('--kt-primary-light');

    if (!element) {
        return;
    }

    $.get({          /// or use : $.ajax({

     

        url: `/Dashboard/GetRentalsPerDay?startdate=${startDate}&endDate=${endDate}`,
        success: function (data) {

            var options = {
                series: [{
                    name: 'Books',
                    data: data.map(chartItem => chartItem.value)    // Note : map is looks like  select in c#
                }],
                chart: {
                    fontFamily: 'inherit',
                    type: 'area',
                    height: height,
                    toolbar: {
                        show: true
                    }
                },
                plotOptions: {

                },
                legend: {
                    show: false
                },
                dataLabels: {
                    enabled: false
                },
                fill: {
                    type: 'solid',
                    opacity: 1
                },
                stroke: {
                    curve: 'smooth',
                    show: true,
                    width: 3,
                    colors: [baseColor]
                },
                xaxis: {
                    categories:  data.map(chartItem => chartItem.label),    
                    axisBorder: {
                        show: false,
                    },
                    axisTicks: {
                        show: false
                    },
                    labels: {
                        style: {
                            colors: labelColor,
                            fontSize: '12px'
                        }
                    },
                    crosshairs: {
                        position: 'front',
                        stroke: {
                            color: baseColor,
                            width: 1,
                            dashArray: 3
                        }
                    },
                    tooltip: {
                        enabled: true,
                        formatter: undefined,
                        offsetY: 0,
                        style: {
                            fontSize: '12px'
                        }
                    }
                },
                yaxis: {
                    min: 0,
                    tickAmount: Math.max(...data.map(chartItem => chartItem.value)), // here ... this create new array of chartItem.value (int type)
                    labels: {
                        style: {
                            colors: labelColor,
                            fontSize: '12px'
                        }
                    }
                },
                states: {
                    normal: {
                        filter: {
                            type: 'none',
                            value: 0
                        }
                    },
                    hover: {
                        filter: {
                            type: 'none',
                            value: 0
                        }
                    },
                    active: {
                        allowMultipleDataPointsSelection: false,
                        filter: {
                            type: 'none',
                            value: 0
                        }
                    }
                },
                tooltip: {
                    style: {
                        fontSize: '12px'
                    }
                },
                colors: [lightColor],
                grid: {
                    borderColor: borderColor,
                    strokeDashArray: 4,
                    yaxis: {
                        lines: {
                            show: true
                        }
                    }
                },
                markers: {
                    strokeColor: baseColor,
                    strokeWidth: 3
                }
            };

             chart1 = new ApexCharts(element, options);
            chart1.render();
        }
        });


    



}

var chart3;

function drawSubscriberChart() {
    var ctx = document.getElementById('SubscribersPerCity');

    if (!ctx) {
        console.log('Element not found');
        return;
    }

    // Destroy the existing chart if it exists
    if (chart3) {
        chart3.destroy();
    }

    $.ajax({
        url: '/Dashboard/GetSubscribersPerCity',
        method: 'GET',
        success: function (Data) {
            // Define colors
            var primaryColor = KTUtil.getCssVariableValue('--kt-primary');
            var dangerColor = KTUtil.getCssVariableValue('--kt-danger');
            var successColor = KTUtil.getCssVariableValue('--kt-success');
            var warningColor = KTUtil.getCssVariableValue('--kt-warning');
            var infoColor = KTUtil.getCssVariableValue('--kt-info');
            var fontFamily = KTUtil.getCssVariableValue('--bs-font-sans-serif');

            const labels = Data.map(d => d.label);
            const data = {
                labels: labels,
                datasets: [
                    {
                        label: 'Subscribers',
                        data: Data.map(d => d.value),
                        backgroundColor: [
                            infoColor,
                            primaryColor,

                            warningColor,
                            '#5F91B6',
                            successColor,
                            '#D3F6FC',
                            '#C8B0D2',
                            "#FFEBCD",
                            dangerColor,
                            "#00CED1",
                            "#FFF5EE",
                            "#A0522D",
                            "#00FF00",
                            "#98FB98",
                            "#FFEFD5",
                            "#A0522D",
                            "#FFFAF0",
                            "#A0522D",
                            "#FFFAF0",
                            "#E0FFFF",
                            "#40E0D0",
                        ],
                        borderRadius: 10
                    }
                ]
            };

            const config = {
                type: 'pie',
                data: data,
                options: {
                    responsive: true,
                    plugins: {
                        title: {
                            display: true,
                            text: 'Subscribers Per City',
                            font: {
                                size: 16,
                                family: fontFamily
                            }
                        },
                        legend: {
                            display: true,
                            labels: {
                                font: {
                                    family: fontFamily,
                                    size: 12
                                }
                            }
                        }
                    },
                    animation: {
                        animateRotate: true, // Rotate animation
                        animateScale: true, // Disable scaling effect
                        duration: 3000,
                        easing: 'easeOutBounce'
                    }
                }
            };

            // Create new chart
            chart3 = new Chart(ctx, config);
            //chart3 = new Chart(ctx, {
            //    type: 'pie',
            //    data: data,
            //    options: {
            //        maintainAspectRatio: false, // prevent effect on  the  chart below
            //        responsive: true,
            //    },
            //});

        },
        error: function () {
            console.log('Error fetching data');
        }
    });
}

// Attach event listener to the button
document.getElementById('ResetChart').addEventListener('click', function () {
    drawSubscriberChart(); // Redraw the chart on button click
});

// Initial render
drawSubscriberChart();








var chart2;

function drawRentedBooksStatusChart(selectedYear) {
    var element = document.getElementById('rentedBooksPerMonth');
    var height = parseInt(KTUtil.css(element, 'height'));
    var labelColor = KTUtil.getCssVariableValue('--kt-gray-500');
    var borderColor = KTUtil.getCssVariableValue('--kt-gray-200');
    var baseColor = KTUtil.getCssVariableValue('--kt-danger');
    var secondaryColor = KTUtil.getCssVariableValue('--kt-success');
    var otherColor = KTUtil.getCssVariableValue('--kt-warning');

    if (!element) {
        return;
    }

    if (chart2) {
        chart2.destroy();
    }

    $.get({
        url: `/Dashboard/GetStatusRentedBooksperMonth?year=${selectedYear}`,
        success: function (data) {
            const categories = data.map(item => item.label);
            const delayedBooks = data.map(item => item.numOfDelayedBooks);
            const undelayedBooks = data.map(item => item.numOfUnDelayedBooks);
            const extendedBooks = data.map(item => item.numOfExtendedBooks);

            var options = {
                series: [
                    { name: 'UnDelayed Books', data: undelayedBooks },
                    { name: 'Delayed Books', data: delayedBooks },
                    { name: 'Extended Books', data: extendedBooks }
                ],
                chart: {
                    type: 'bar',
                    height: height,
                    toolbar: { show: false },
                    animations: {
                        enabled: true,
                        easing: 'easeOutBounce', // Bouncy animation
                        speed: 2000,
                        animateGradually: {
                            enabled: true,
                            delay: 300 // Delay between bars
                        },
                        dynamicAnimation: {
                            enabled: true,
                            speed: 800
                        }
                    }
                },
                plotOptions: {
                    bar: {
                        horizontal: false,
                        columnWidth: '30%',
                        endingShape: 'rounded',
                        distributed: false, 
                    }
                },
                fill: {
                    type: 'gradient',
                    gradient: {
                        shade: 'dark',
                        type: 'vertical',
                        gradientToColors: [secondaryColor, baseColor, otherColor],
                        stops: [0, 100]
                    }
                },
                xaxis: {
                    categories: categories,
                    labels: {
                        style: {
                            colors: labelColor,
                            fontSize: '12px'
                        }
                    }
                },
                yaxis: {
                    labels: {
                        style: {
                            colors: labelColor,
                            fontSize: '12px'
                        }
                    }
                },
                dataLabels: {
                    enabled: false // Disable numbers on the columns
                },
                colors: [secondaryColor, baseColor, otherColor],
                grid: {
                    borderColor: borderColor,
                    strokeDashArray: 4,
                    animation: {
                        enabled: true, // Animated gridlines
                        speed: 1000
                    }
                },
                tooltip: {
                    enabled: true,
                    theme: 'dark',
                    style: {
                        fontSize: '12px',
                        fontFamily: 'inherit'
                    },
                    marker: {
                        show: true,
                        animation: {
                            duration: 400, // Smooth marker animation
                            easing: 'easeOutElastic'
                        }
                    }
                }
            };

            chart2 = new ApexCharts(element, options);
            chart2.render();
        }
    });
}

document.getElementById('yearSelector').addEventListener('change', function () {
    const selectedYear = this.value;
    drawRentedBooksStatusChart(selectedYear);
});

drawRentedBooksStatusChart(document.getElementById('yearSelector').value);
