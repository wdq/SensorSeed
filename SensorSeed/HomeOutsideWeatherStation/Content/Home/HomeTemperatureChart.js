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
        .y(function (d) { return y(d.Temperature); });

    var svg = d3.select("#TemperatureGraph").append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
      .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");


    var table = $("#TemperatureTable").tableToJSON();
    table.sort(function (a, b) {
        return formatDate.parse(a.Timestamp) - formatDate.parse(b.Timestamp);
    })
    $("#TemperatureTable").css("display", "none");
    x.domain(d3.extent(table, function (d) { return formatDate.parse(d.Timestamp); }));
    y.domain([-20, 40]);

   /* svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis); */

    svg.append("g")
        .attr("class", "y axis")
        .call(yAxis);

    svg.append("path")
        .datum(table)
        .attr("class", "TemperatureLine")
        .attr("d", line);



    var focus = svg.append("g")
        .attr("class", "focus")
        .style("display", "none");

    focus.append("circle")
        .attr("r", 4.5);

    focus.append("text")
        .attr("x", 9)
        .attr("dy", ".35em");

    svg.append("rect")
        .attr("class", "overlay")
        .attr("width", width)
        .attr("height", height)
        .on("mouseover", function () { focus.style("display", null); })
        .on("mouseout", function () { focus.style("display", "none"); })
        .on("mousemove", mousemove);

    var bisectDate = d3.bisector(function (d) { return d.Timestamp; }).left;

    function mousemove() {
        var x0 = x.invert(d3.mouse(this)[0]);
        var i = 0;
        //var i = bisectDate(table, x0, d3.max(table));
        //console.log(new Date(table[500].Timestamp).toString());
        //console.log(new Date(x0).toString());
        for (var count = 0; count < table.length; count++) {
            if (new Date(table[count].Timestamp).toString() == new Date(x0).toString()) {
                i = count;
                console.log(i);
            }
        }
        var d0 = table[i - 1];
        var d1 = table[i];
        //console.log(x0);
        //console.log(i);
        //console.log(d0);
        //console.log(d1);
        //console.log("");
        // var d = x0 - d0.Timestamp > d1.Timestamp - x0 ? d1 : d0;
        var d = d1;
            focus.attr("transform", "translate(" + x(formatDate.parse(d.Timestamp)) + "," + y(d.Temperature) + ")");
            focus.select("text").text(d.Temperature);
    }


});