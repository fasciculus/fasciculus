
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    console.log(`free slots = ${Source.safeFreeSlots}, free work ${Source.safeFreeWork}`);

    Screeps.cleanup();
}
