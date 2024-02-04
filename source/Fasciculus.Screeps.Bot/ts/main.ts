
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

    Profiler.stop();
}