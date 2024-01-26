
import { Initializer } from "./Initializer"
import { Scheduler } from "./Scheduler";
import { Profiler } from "./Profiling";

export const loop = function ()
{
    Profiler.start(5);

    Initializer.run();
    Scheduler.run();

    Profiler.stop();

    if (Game.time % 10 == 0)
    {
        Profiler.log();
    }
}