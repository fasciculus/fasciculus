
import { Profiler } from "./Profiling";
import { Scheduler } from "./Scheduler";

export const loop = function ()
{
    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    Profiler.stop();

//    console.log(`---------- ${Game.time} ----------`);
//    Wells.all.map(w => `${w.id}: ${w.assignee?.name || "undefined"}`).forEach(s => console.log(s));
}