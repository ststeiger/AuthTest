# D3-Legend

**D3-Legend** is an open-source JavaScript library for rendering custom legends using the D3.js library.

Check out an [Example](https://arpitnarechania.github.io/d3-legend/) where you can test various configuration options.

# Installation

Download d3-legend using bower.

```
bower install d3-legend --save
```

To use this library then, simply include d3.js, legend.js and legend.css:

``` html
<script src="/path/to/d3.min.js"></script>
<script src="/path/to/dist/legend.css"></script>
<script src="/path/to/dist/legend.js"></script>
```

# Usage

To use this library, you must create a container element and instantiate a new
Legend:

```html
<div id="legend"></div>
```

Setting chart parameters
``` javascript

		Legend({
		    length : 600,
		    thickness: 50,
            min_color : '#ffffff',
            max_color : '#3498db',
            max_value : 500,
            min_value : 0,
            margin : {top:30,bottom:50,left:30,right:50},
            orientation : 'vertical',
            axis_position : 'right',
            axis_tick_orientation : 'right',
            min_value_at_start : false
		});

```

## Options

| Option                     | Description                                                               | Type     | Options
| -------------------------- | ------------------------------------------------------------------------- | -------- | ------------------------- |
| `width`                    | The width of the legend in pixels                                         | number   | `200`                     |
| `height`                   | The height of the legend in pixels                                        | number   | `300`                     |
| `margin.top`               | The top margin                                                            | number   | `10`                      |
| `margin.bottom`            | The bottom margin                                                         | number   | `10`                      |
| `margin.left`              | The left margin                                                           | number   | `10`                      |
| `margin.right`             | The right margin                                                          | number   | `10`                      |
| `max_color`                | The maximum of the color range                                            | string   | `'green'`                 |
| `min_color`                | The minimum of the color range                                            | string   | `'white'`                 |
| `max_value`                | The maximum value                                                         | number   | `100`                     |
| `min_value`                | The minimum value                                                         | number   | `0`                       |
| `orientation`              | The legend orientation                                                    | string   | `'vertical'`/`'horizontal'`   |
| `axis_position`            | The position of the axis                                                  | string   | `'left'`/`'right'`/`'bottom'`/`'top'`|
| `axis_tick_orientation`    | The position of the axis ticks                                            | string   | `'left'`/`'right'`/`'bottom'`/`'top'`|
| `min_value_at_start`       | Whether legend's minimum value is to be at the start or end               | bool     | `true`                       |

# Author

Arpit Narechania
arpitnarechania@gmail.com

# License

MIT license.