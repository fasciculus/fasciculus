
import "./Types";

import { Profiler } from "./Profiling";
import { Scheduler } from "./Scheduler";

export const loop = function ()
{
    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    const map: Map<string, number> = new Map();

    Profiler.stop();
}