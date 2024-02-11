
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    Creep.ofType("W").map(c => c.name)

    console.log(Creep.ofType("W").map(c => c.name));

    Screeps.cleanup();
}
