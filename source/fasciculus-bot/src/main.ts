
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    console.log(Creep.my.map(c => ` ${c.name}: ${c.workParts}`));

    Screeps.cleanup();
}
