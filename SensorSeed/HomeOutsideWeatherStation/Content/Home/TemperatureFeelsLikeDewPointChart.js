$(document).ready(function () {


    // Get the data
    d3.json("./Home/TemperatureFeelsLikeDewPointChartData", function (error, json) {
        var data;
        data = json.Data;

        // Set the dimensions of the canvas / graph
        var margin = { top: 30, right: 20, bottom: 30, left: 50 },
                width = 1140 - margin.left - margin.right,
                height = 350 - margin.top - margin.bottom;

        // Parse the date / time
        var parseDate = d3.time.format.utc("%m/%d/%Y %I:%M:%S %p").parse;

        // Set the ranges
        var x = d3.time.scale().range([0, width]);
        var y = d3.scale.linear().range([height, 0]);

        // Define the axes
        var xAxis = d3.svg.axis()
            .scale(x)
            .orient("bottom")
            .ticks(10)
            .tickFormat(d3.time.format("%d"))
            .innerTickSize(-height)
            .outerTickSize(0)
            .tickPadding(10);

        var yAxis = d3.svg.axis()
            .scale(y)
            .orient("left")
            .ticks(3)
            .innerTickSize(-width)
            .outerTickSize(0)
            .tickPadding(10);

        // Define the line
        var TemperatureValueLine = d3.svg.line()
              .interpolate("basis")
              .x(function (d) { return x(d.Timestamp); })
              .y(function (d) { return y(d.Temperature); });
        var TemperatureFeelsLikeValueLine = d3.svg.line()
              .interpolate("basis")
              .x(function (d) { return x(d.Timestamp); })
              .y(function (d) { return y(d.TemperatureFeelsLike); });
        var DewPointValueLine = d3.svg.line()
              .interpolate("basis")
              .x(function (d) { return x(d.Timestamp); })
              .y(function (d) { return y(d.DewPoint); });

        // Adds the svg canvas
        var svg = d3.select("#TemperatureFeelsLikeDewPointChart")
            .append("svg")
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom)
            .append("g")
                .attr("transform",
                      "translate(" + margin.left + "," + margin.top + ")");


        data.forEach(function (d) {
            d.Timestamp = parseDate(d.Timestamp);
        });

        console.log(data);

        var startDate = new Date();
        // Scale the range of the data
        x.domain([new Date(startDate.setDate(startDate.getDate() - 9)).setHours(0,0,0,0), new Date().setHours(23, 59, 59, 999)]);
        y.domain([d3.min(data, function (d) { return d.DewPoint; }), d3.max(data, function (d) { return d.Temperature; })]);
        //y.domain([-20, 40]);



        // Add the X Axis
        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis);

        // Add the Y Axis
        svg.append("g")
            .attr("class", "y axis")
            .call(yAxis);


        svg.append("path")
            .attr("class", "TemperatureLine")
            .attr("d", TemperatureValueLine(data));
        /*svg.append("path")
            .attr("class", "TemperatureFeelsLikeLine")
            .attr("d", TemperatureFeelsLikeValueLine(data));*/
        svg.append("path")
            .attr("class", "DewPointLine")
            .attr("d", DewPointValueLine(data));

    });



});