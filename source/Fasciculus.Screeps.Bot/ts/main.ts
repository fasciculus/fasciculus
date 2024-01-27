
import { Profiler } from "./Profiling";
import { Scheduler } from "./Scheduler";

export const loop = function ()
{
    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    Profiler.record("global", "main", Game.cpu.getUsed());
    Profiler.stop();
}