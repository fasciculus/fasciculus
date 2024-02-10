
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();
    Screeps.cleanup();

    const a1: Array<number | undefined> = [1, 2, undefined, 3];
    const a2: Array<number> = Array.defined(a1);

    console.log(`a1 = [${a1}]`);
    console.log(`a2 = [${a2}]`);
}