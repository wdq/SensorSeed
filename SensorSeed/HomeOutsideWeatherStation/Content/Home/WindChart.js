$(document).ready(function () {


    // Get the data
    d3.json("./Home/WindChartData", function (error, json) {
        var data;
        data = json.Data;

        // Set the dimensions of the canvas / graph
        var margin = { top: 30, right: 50, bottom: 30, left: 50 },
                width = 1140 - margin.left - margin.right,
                height = 350 - margin.top - margin.bottom;

        var marginDir = { top: 30, right: 50, bottom: 30, left: 50 },
        widthDir = 1140 - marginDir.left - marginDir.right,
        heightDir = 350 - marginDir.top - marginDir.bottom;

        // Parse the date / time
        var parseDate = d3.time.format.utc("%m/%d/%Y %I:%M:%S %p").parse;

        // Set the ranges
        var x = d3.time.scale().range([0, width]);
        var y = d3.scale.linear().range([height, 0]);

        var xDir = d3.time.scale().range([0, widthDir]);
        var yDir = d3.scale.linear().range([heightDir, 0]);

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
            .ticks(5)
            .innerTickSize(-width)
            .outerTickSize(0)
            .tickPadding(10);

        var xAxisDir = d3.svg.axis()
            .scale(xDir)
            .orient("bottom")
            .ticks(10)
            .tickFormat(d3.time.format("%d"))
            .innerTickSize(-heightDir)
            .outerTickSize(0)
            .tickPadding(10);

        var yAxisDir = d3.svg.axis()
            .scale(yDir)
            .orient("left")
            .ticks(5)
            .innerTickSize(-widthDir)
            .outerTickSize(0)
            .tickPadding(10);


        // Adds the svg canvas
        var svg = d3.select("#WindChart")
            .append("svg")
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom)
            .append("g")
                .attr("transform",
                      "translate(" + margin.left + "," + margin.top + ")");

        var svgDir = d3.select("#WindDirectionChart")
            .append("svg")
                .attr("width", widthDir + marginDir.left + marginDir.right)
                .attr("height", heightDir + marginDir.top + marginDir.bottom)
            .append("g")
                .attr("transform",
                      "translate(" + marginDir.left + "," + marginDir.top + ")");

        data.forEach(function (d) {
            d.Timestamp = parseDate(d.Timestamp);
            if (d.GustSpeed > 500) { // there is some glitch in the code on the weather station that generates large gust speeds, this filters that out.
                d.GustSpeed = 0;
            }
        });

        //console.log(data);

        var startDate = new Date();
        var startDateDir = new Date();

        // Scale the range of the data
        x.domain([new Date(startDate.setDate(startDate.getDate() - 9)).setHours(0, 0, 0, 0), new Date().setHours(23, 59, 59, 999)]);
        y.domain([0, d3.max(data, function (d) { if (d.GustSpeed > d.WindSpeed) {
            return d.GustSpeed;
        } else {
            return d.WindSpeed;
        }
        })]);

        xDir.domain([new Date(startDateDir.setDate(startDateDir.getDate() - 9)).setHours(0, 0, 0, 0), new Date().setHours(23, 59, 59, 999)]);
        yDir.domain([0, 370]);




        // Add the X Axis
        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis);

        svgDir.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + heightDir + ")")
            .call(xAxisDir);

        // Add the Y Axis
        svg.append("g")
            .attr("class", "y axis")
            .call(yAxis);

        svgDir.append("g")
            .attr("class", "y axis")
            .call(yAxisDir);

        var g = svg.append("svg:g");
        var gDir = svgDir.append("svg:g");

        g.selectAll("scatter-dots")
          .data(data)
          .enter().append("svg:circle")
              .attr("cx", function (d, i) { return x(d.Timestamp); })
              .attr("cy", function (d) { return y(d.WindSpeed); })
              .attr("r", 2)
              .style("fill", "#002F80");

        g.selectAll("scatter-dots")
          .data(data)
          .enter().append("svg:circle")
              .attr("cx", function (d, i) { return x(d.Timestamp); })
              .attr("cy", function (d) { return y(d.GustSpeed); })
              .attr("r", 2)
              .style("fill", "#8BD5EE");

        gDir.selectAll("scatter-dots")
          .data(data)
          .enter().append("svg:circle")
              .attr("cx", function (d, i) { return xDir(d.Timestamp); })
              .attr("cy", function (d) { return yDir(d.WindDirection); })
              .attr("r", 2)
              .style("fill", "#D6212A");

        $.get("./Home/TenDaySunriseSunsetData",
            function (data) {
                var previousSunset = data.Data[0].Date;
                $.each(data.Data,
                    function (i, item) {
                        //console.log(new Date(previousSunset));
                        //console.log(new Date(item.Sunrise));

                        var start = new Date(previousSunset);
                        var end = new Date(item.Sunrise);
                        var timeDiff = Math.abs(start.getTime() - end.getTime());
                        //console.log(timeDiff / 1000);

                        // todo: there is a lot of weirdness in the next bit
                        // The width is coming from the relationship between the number of seconds in 10 days (864000) and the width of the chart (1040). 
                        // The starting position has a lot of time zone and daylight savings time things going on. The 25200 comes from 6 hours (difference between central time and UTC) + 1 hour (daylight savings time thing), and 1040 is the chart width
                        // I'll need to get all of this to work year round.

                        //console.log("");
                        svg.append("rect")
                            .attr("x", x(parseDate(previousSunset)) + (25200 / 1040))
                            .attr("y", 0)
                            .attr("width", ((timeDiff / 1000)) * (1040 / 864000))
                            .attr("height", 290)
                            .style("opacity", 0.06)
                            .style("fill", "#000000");

                        svgDir.append("rect")
                            .attr("x", xDir(parseDate(previousSunset)) + (25200 / 1040))
                            .attr("y", 0)
                            .attr("width", ((timeDiff / 1000)) * (1040 / 864000))
                            .attr("height", 290)
                            .style("opacity", 0.06)
                            .style("fill", "#000000");

                        previousSunset = item.Sunset;
                    });
            });

    });



});