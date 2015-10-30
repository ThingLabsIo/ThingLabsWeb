function setupChart(chartData)
{
    var ctx = $("#myChart").get(0).getContext("2d");

    var myLineChart = new Chart(ctx).Line(chartData );
}

function getWeatherRecords(deviceId) {
    $.ajax({
        method: 'get',
        url: "/api/message" + deviceId,
        contentType: "application/json; charset=utf-8",
        headers: {
            'Authorization': 'Bearer ' + sessionStorage.getItem("accessToken")
        },
        success: function (data) {
            var labels = new Array();
            var celsiusDataset = {
                label: "Celsius",
                fillColor: "rgba(220,220,220,0.2)",
                strokeColor: "rgba(220,220,220,1)",
                pointColor: "rgba(220,220,220,1)",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(220,220,220,1)",
                data: new Array()
            }
            var relativeHumidityDataset = {
                label: "Relative Humidity",
                fillColor: "rgba(220,220,220,0.2)",
                strokeColor: "rgba(220,220,220,1)",
                pointColor: "rgba(220,220,220,1)",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(220,220,220,1)",
                data: new Array()
            }

            for (var i = 0; i < data.length; i++) {
                var newLiContent = "";
                for (var methods in data[i]) {
                    newLiContent += methods + ": " + data[i][methods] + " ";
                }
                $("#messagesList").prepend("<li>" + newLiContent + "</li>");

                labels.push(new Date(data[i].timestamp).toLocaleTimeString());
                celsiusDataset.data.push(data[i].celsius)
                relativeHumidityDataset.data.push(data[i].relativeHumidity)
            }

            var chartData = {
                labels: labels,
                datasets: [celsiusDataset]
            };

            setupChart(chartData);
        }
    });
}

$('#messagesList li:first').css('margin-top', 0 - $('#messagesList li:first').outerHeight());
$('#messagesList').css('overflow', 'hidden');

