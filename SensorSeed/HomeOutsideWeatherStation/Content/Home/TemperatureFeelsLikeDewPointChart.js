﻿$(document).ready(function () {


    // Set the dimensions of the canvas / graph
    var margin = { top: 30, right: 20, bottom: 30, left: 50 },
            width = 960 - margin.left - margin.right,
            height = 150 - margin.top - margin.bottom;

    // Parse the date / time
    var parseDate = d3.time.format.utc("%m/%d/%Y %I:%M:%S %p").parse;

    // Set the ranges
    var x = d3.time.scale().range([0, width]);
    var y = d3.scale.linear().range([height, 0]);

    // Define the axes
    var xAxis = d3.svg.axis().scale(x)
        .orient("bottom").ticks(10).tickFormat(d3.time.format("%d"));

    var yAxis = d3.svg.axis().scale(y)
        .orient("left").ticks(3);
     // Define the line
      var valueline = d3.svg.line()
          .x(function (d) { return x(d.Timestamp); })
          .y(function (d) { return y(d.Temperature); });
          

    // Adds the svg canvas
    var svg = d3.select("#TemperatureFeelsLikeDewPointChart")
        .append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
        .append("g")
            .attr("transform",
                  "translate(" + margin.left + "," + margin.top + ")");

    // Get the data
    d3.json("./TemperatureFeelsLikeDewPointChartData", function (error, json) {
        var data;
        data = json.Data;
        data.forEach(function (d) {
            d.Timestamp = parseDate(d.Timestamp);
        });

        console.log(data);

        // Scale the range of the data
        x.domain(d3.extent(data, function (d) { return d.Timestamp; }));
        y.domain([0, d3.max(data, function (d) { return d.Temperature; })]);
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

        /*svg.selectAll(".dot")
            .data(data)
          .enter().append("circle")
            .attr("class", "dot")
            .attr("r", 3.5)
            .attr("cx", function (d) { return x(d.Timestamp); })
            .attr("cy", function (d) { return y(d.Temperature); }); */
        svg.append("path")
            .attr("class", "TemperatureLine")
            .attr("d", valueline(data));

    });



});