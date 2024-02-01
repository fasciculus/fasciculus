
import { Profiler } from "./Profiling";
import { Chambers } from "./Rooming";
import { Scheduler } from "./Scheduler";

export const loop = function ()
{
    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    console.log(Chambers.all.map(c => `${c.name}: ${c.rcl}`).toArray());

    Profiler.stop();
}