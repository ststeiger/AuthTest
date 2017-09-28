function Legend(options) {

    var thickness = options.thickness,
    length = options.length,
    startColor = options.min_color,
    endColor = options.max_color,
    margin = options.margin,
    maxValue = options.max_value,
    minValue = options.min_value,
    orientation = options.orientation,
    axisPosition = options.axis_position;
    axisTickOrientation = options.axis_tick_orientation;
    minValueAtStart = options.min_value_at_start;

    if(orientation == "horizontal"){
        width = length;
        height = thickness;

        if(axisPosition == "top"){
            if(axisTickOrientation != "top" && axisTickOrientation != "bottom" ){
                throw new Error("In horizontal legend mode, axisTickOrientation must one of 'top' or 'bottom");
            }
            axisTranslation = "translate(" + (margin.left) + "," + (margin.top) + ")";
        }else if(axisPosition == "bottom"){
            if(axisTickOrientation != "top" && axisTickOrientation != "bottom" ){
                throw new Error("In horizontal legend mode, axisTickOrientation must one of 'top' or 'bottom");
            }
            axisTranslation = "translate(" + (margin.left) + "," + (margin.top + height) + ")";
        }else{
            throw new Error("In horizontal legend mode, axisPosition must one of 'top' or 'bottom");
        }

        axisLength = width;

        if(minValueAtStart){
            legendRange = [0,axisLength];
            x1 = "100%";
            y1 = "100%";
            x2 = "0%";
            y2 = "100%";
        }else{
            legendRange = [axisLength,0];
            x1 = "0%";
            y1 = "100%";
            x2 = "100%";
            y2 = "100%";
        }
    }

    else if(orientation == "vertical"){
        width = thickness;
        height = length;

        if(axisPosition == "right"){
            if(axisTickOrientation != "left" && axisTickOrientation != "right" ){
                throw new Error("In vertical legend mode, axisTickOrientation must one of 'left' or 'right");
            }
            axisTranslation = "translate(" + (width + margin.left) + "," + (margin.top) + ")";
        }else if(axisPosition == "left"){
            if(axisTickOrientation != "left" && axisTickOrientation != "right" ){
                throw new Error("In vertical legend mode, axisTickOrientation must one of 'left' or 'right");
            }
            axisTranslation = "translate(" + (margin.left) + "," + (margin.top) + ")";
        }else{
            throw new Error("In vertical legend mode, axisPosition must be one out of 'left' or 'right");
        }

        axisLength = height;

        if(minValueAtStart){
            x1 = "100%";
            y1 = "100%";
            x2 = "100%";
            y2 = "0%";
            legendRange = [0,axisLength];
        }else{
            x1 = "100%";
            y1 = "0%";
            x2 = "100%";
            y2 = "100%";
            legendRange = [axisLength,0];
        }
    }

    var key = d3.select("#legend")
    .append("svg")
    .attr("width", width + margin.left + margin.right)
    .attr("height", height + margin.top + margin.bottom)

    var legend = key
    .append("defs")
    .append("svg:linearGradient")
    .attr("id", "gradient")
    .attr("x1", x1)
    .attr("y1", y1)
    .attr("x2", x2)
    .attr("y2", y2)
    .attr("spreadMethod", "pad");

    legend
    .append("stop")
    .attr("offset", "0%")
    .attr("stop-color", endColor)
    .attr("stop-opacity", 1);

    legend
    .append("stop")
    .attr("offset", "100%")
    .attr("stop-color", startColor)
    .attr("stop-opacity", 1);

    key.append("rect")
    .attr("width", width)
    .attr("height", height)
    .style("fill", "url(#gradient)")
    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    var legendScale = d3.scale.linear()
    .range(legendRange)
    .domain([minValue, maxValue]);

    var legendAxis = d3.svg.axis()
    .scale(legendScale)
    .orient(axisTickOrientation);

    key.append("g")
    .attr("class", "x axis")
    .attr("transform", axisTranslation)
    .call(legendAxis)
}