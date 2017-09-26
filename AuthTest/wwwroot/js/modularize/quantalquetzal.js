/// <reference path='Crap.ts'/>
var ClassOne = foo.Crap;
// import ClassTwo = bar.Crap;
var bar = /** @class */ (function () {
    function bar() {
    }
    bar.prototype.foo = function () {
        alert("test");
    };
    return bar;
}());
//# sourceMappingURL=quantalquetzal.js.map