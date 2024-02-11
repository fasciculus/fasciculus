
import { ES } from "./es/es";
import { Screeps } from "./screeps/screeps";

ES.setup();

export const loop = function ()
{
    Screeps.setup();

    const sources: Array<Source> = Source.safe;
    const freeSlots: Array<number> = sources.map(s => s.freeSlots);

    for (let i = 0, n = sources.length; i < n; ++i)
    {
        console.log(`${sources[i].id}: ${freeSlots[i]}`);
    }

    Screeps.cleanup();
}
