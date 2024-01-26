
import { Initializer } from "./Initializer"
import { Scheduler } from "./Scheduler";
import { Profiler } from "./Profiling";
import { PROFILER_IGNORED_KEYS, PROFILER_SESSION } from "./_Config";

export const loop = function ()
{
    Profiler.start(PROFILER_SESSION);

    Initializer.run();
    Scheduler.run();

    Profiler.record("global", "main", Game.cpu.getUsed());
    Profiler.stop();

    if (Game.time % 10 == 0)
    {
        Profiler.log(PROFILER_IGNORED_KEYS);
    }
}