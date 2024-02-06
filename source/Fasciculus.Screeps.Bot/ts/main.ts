
import { Profiler } from "./Profiling";
import { Scheduler } from "./Scheduler";
import { Screeps } from "./screeps";

export const loop = function ()
{
    Screeps.setup();

    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    Profiler.stop();

    Screeps.cleanup();
}