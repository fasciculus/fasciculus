
import "./Types_Common";
import "./Types_Screeps";

import { Profiler } from "./Profiling";
import { Scheduler } from "./Scheduler";

export const loop = function ()
{
    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    Profiler.stop();
}