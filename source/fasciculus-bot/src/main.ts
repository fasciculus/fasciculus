
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    const sources: Array<Source> = Source.safe;
    const slots: Array<Array<RoomPosition>> = sources.map(s => s.slots);

    for (let i = 0, n = sources.length; i < n; ++i)
    {
        console.log(`${sources[i].id}: ${slots[i]}`);
    }

    Screeps.cleanup();
}
