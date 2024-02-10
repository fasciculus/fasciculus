
import { ES } from "./es/es";
import { Names } from "./screeps/name";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    console.log(`next creep name: ${Names.nextCreepName("Y")}`);

    Screeps.cleanup();
}
