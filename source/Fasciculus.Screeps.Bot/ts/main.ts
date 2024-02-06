
import "types.common";

import { Profiler, profile } from "./Profiling";
import { Scheduler } from "./Scheduler";
import { Screeps } from "./screeps";

class Experiments
{
    @profile
    static slots()
    {
        const sourceIds = Set.flatten(Game.knownRooms.map(r => r.sourceIds));
        const sources = Array.defined(sourceIds.toArray().map(id => Game.get(id)));
        const infos = sources.map(s => ` ${s.id}: ${s.slots}`);

        console.log(infos);
    }
}

export const loop = function ()
{
    Screeps.setup();

    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    Experiments.slots();

    Profiler.stop();

    Screeps.cleanup();
}