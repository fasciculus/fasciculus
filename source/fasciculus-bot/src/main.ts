
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    console.log(Source.safe.map(s => ` ${s.id}: ${s.freeWork}`));

    Screeps.cleanup();
}
