
import "fasciculus-es5";

var input: Array<number | undefined> = [1, 2, undefined, 3];
var output: Array<number> = Array.defined(input);

console.log(`input = ${input}`);
console.log(`output = ${output}`);
