
import "types.common";

import { Profiler, profile } from "./Profiling";
import { Scheduler } from "./Scheduler";
import { Screeps } from "./screeps";

class Experiments
{
    @profile
    static run()
    {
    }
}

export const loop = function ()
{
    Profiler.start();

    Screeps.setup();
    Scheduler.initialize();
    Scheduler.run();
    Experiments.run();
    Screeps.cleanup();

    Profiler.stop();
}