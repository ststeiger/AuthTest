var foo;
(function (foo) {
    // Crap.ts 
    var Crap // implements shape.IShape
     = /** @class */ (function () {
        function Crap() {
        }
        Crap.prototype.draw = function () {
            console.log("Cirlce is drawn (external module)");
        };
        return Crap;
    }());
    foo.Crap = Crap;
})(foo || (foo = {}));
//# sourceMappingURL=Crap.js.map