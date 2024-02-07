
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
    Screeps.setup();

    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    Experiments.run();

    Profiler.stop();

    Screeps.cleanup();
}