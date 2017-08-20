$(document).ready(function () {

    var getUrlParameter = function getUrlParameter(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                if (sParameterName[1] === undefined) {
                    return new Date();
                }
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    };

    function formatDate(date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear(),
            hour = d.getHours(),
            minute = d.getMinutes()
        second = d.getSeconds();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;
        if (hour.length < 2) hour = '0' + hour;
        if (minute.length < 2) minute = '0' + minute;
        if (second.length < 2) second = '0' + second;

        return [month, day, year].join('-') + " " + [hour, minute, second].join(':');
    }

    var startDate = new Date(getUrlParameter("endDate"));
    if (startDate == "Invalid Date") {
        startDate = new Date();
    }


    // Get the data
    d3.json("./Home/HumidityPressureChartData" + "?endDate=" + formatDate(startDate), function (error, json) {
        var data;
        data = json.Data;

        // Set the dimensions of the canvas / graph
        var margin = { top: 30, right: 50, bottom: 30, left: 50 },
                width = 1140 - margin.left - margin.right,
                height = 350 - margin.top - margin.bottom;

        // Parse the date / time
        var parseDate = d3.time.format.utc("%m/%d/%Y %I:%M:%S %p").parse;

        // Set the ranges
        var x = d3.time.scale().range([0, width]);
        var y0 = d3.scale.linear().range([height, 0]);
        var y1 = d3.scale.linear().range([height, 0]);

        // Define the axes
        var xAxis = d3.svg.axis()
            .scale(x)
            .orient("bottom")
            .ticks(10)
            .tickFormat(d3.time.format("%d"))
            .innerTickSize(-height)
            .outerTickSize(0)
            .tickPadding(10);

        var yAxisLeft = d3.svg.axis() // humidity
            .scale(y0)
            .orient("left")
            .ticks(3)
            .innerTickSize(-width)
            .outerTickSize(0)
            .tickPadding(10);

        var yAxisRight = d3.svg.axis() // pressure
            .scale(y1)
            .orient("right")
            .ticks(3)
            .innerTickSize(-width)
            .outerTickSize(0)
            .tickPadding(10);

        // Adds the svg canvas
        var svg = d3.select("#HumidityPressureChart")
            .append("svg")
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom)
            .append("g")
                .attr("transform",
                      "translate(" + margin.left + "," + margin.top + ")");
        //console.log(data);


        // Scale the range of the data
        var xAxisLeftBounds = new Date(startDate);
        var xAxisRightBounds = new Date(startDate);
        xAxisLeftBounds = new Date(xAxisLeftBounds.setDate(xAxisLeftBounds.getDate() - 9)).setHours(0, 0, 0, 0);
        xAxisRightBounds = xAxisRightBounds.setHours(23, 59, 59, 999);

        x.domain([xAxisLeftBounds, xAxisRightBounds]);
        y0.domain([0, 100]);
        y1.domain([json.MinPressureY, json.MaxPressureY]);
        //y.domain([-20, 40]);



        // Add the X Axis
        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis);

        // Add the Y Axis
        svg.append("g")
            .attr("class", "y axis")
            .style("fill", "#87C404")
            .call(yAxisLeft);
        svg.append("g")
            .attr("class", "y axis")
            .attr("transform", "translate(" + width + " ,0)")
            .call(yAxisRight);


        data.forEach(function(seriesData) {
            seriesData.forEach(function (d) {
                d.Timestamp = parseDate(d.Timestamp);
            });
            // Define the line
            var HumidityValueLine = d3.svg.line()
                  .interpolate("basis")
                  .x(function (d) { return x(d.Timestamp); })
                  .y(function (d) { return y0(d.Humidity); });
            var PressureValueLine = d3.svg.line()
                  .interpolate("basis")
                  .x(function (d) { return x(d.Timestamp); })
                  .y(function (d) { return y1(d.Pressure); });

            svg.append("path")
                .attr("class", "HumidityLine")
                .attr("d", HumidityValueLine(seriesData));

            svg.append("path")
                .attr("class", "PressureLine")
                .attr("d", PressureValueLine(seriesData));


        });

        $.get("./Home/TenDaySunriseSunsetData" + "?endDate=" + formatDate(startDate),
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
                        previousSunset = item.Sunset;
                    });
            });


    });



});