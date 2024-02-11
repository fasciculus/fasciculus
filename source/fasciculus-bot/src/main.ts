
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    console.log(Room.safe.map(r => ` ${r.name}: ${r.energy}/${r.energyCapacity}`));

    Screeps.cleanup();
}
