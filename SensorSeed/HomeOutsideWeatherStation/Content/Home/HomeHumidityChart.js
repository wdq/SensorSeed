$(document).ready(function () {

    var margin = { top: 30, right: 20, bottom: 30, left: 50 },
        width = 960 - margin.left - margin.right,
        height = 150 - margin.top - margin.bottom;

    var formatDate = d3.time.format("%x %I:%M:%S %p");

    var x = d3.time.scale()
        .range([0, width]);

    var y = d3.scale.linear()
        .range([height, 0]);

    var xAxis = d3.svg.axis()
        .scale(x)
        .orient("bottom")
        .ticks(10);

    var yAxis = d3.svg.axis()
        .scale(y)
        .orient("left")
        .ticks(3);

    var line = d3.svg.line()
        .x(function (d) { return x(formatDate.parse(d.Timestamp)); })
        .y(function (d) { return y(d.Humidity); });

    var svg = d3.select("#HumidityGraph").append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
      .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    var table = $("#HumidityTable").tableToJSON();
    $("#HumidityTable").css("display", "none");
    x.domain(d3.extent(table, function (d) { return formatDate.parse(d.Timestamp); }));
    y.domain([0, 100]);

   /* svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis); */

    svg.append("g")
        .attr("class", "y axis")
        .call(yAxis);

    svg.append("path")
        .datum(table)
        .attr("class", "HumidityLine")
        .attr("d", line);

    function type(d) {
        d.Timestamp = formatDate.parse(d.Timestamp);
        d.Humidity = d.Humidity;
        console.log(d);
        return d;
    }

});