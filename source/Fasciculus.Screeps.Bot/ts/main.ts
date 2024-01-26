
import { Scheduler } from "./Scheduler";
import { Profiler } from "./Profiling";
import { PROFILER_IGNORED_KEYS, PROFILER_MAX_ENTRIES, PROFILER_SESSION } from "./_Config";

export const loop = function ()
{
    Profiler.start(PROFILER_SESSION);

    Scheduler.initialize();
    Scheduler.run();

    Profiler.record("global", "main", Game.cpu.getUsed());
    Profiler.stop();

    if (Game.time % 10 == 0)
    {
        Profiler.log(PROFILER_MAX_ENTRIES, PROFILER_IGNORED_KEYS);
    }
}