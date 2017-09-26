"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var circle = require("./Circle");
var triangle = require("./Triangle");
function drawAllShapes(shapeToDraw) {
    shapeToDraw.draw();
}
drawAllShapes(new circle.Circle());
drawAllShapes(new triangle.Triangle());
//# sourceMappingURL=TestShape.js.map