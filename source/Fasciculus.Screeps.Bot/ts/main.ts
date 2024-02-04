
import "./types.common";
import "./types.screeps";

import { Profiler } from "./Profiling";
import { Scheduler } from "./Scheduler";
import { Screeps } from "./types.screeps";

export const loop = function ()
{
    Screeps.setup();

    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    const set1: Set<number> = Set.from([1, 2, 3]);

    console.log(`set1: [${set1.toArray()}]`);

    Profiler.stop();
}