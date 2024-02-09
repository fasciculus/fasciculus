"use strict";
Objects.setFunction(Array, "defined", function (values) {
    var result = new Array();
    for (const value of values) {
        if (value !== undefined) {
            result.push(value);
        }
    }
    return result;
});
