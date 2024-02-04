
import "./types.common";
import "./types.screeps";

import { Profiler } from "./Profiling";
import { Scheduler } from "./Scheduler";
import { Screeps } from "./types.screeps";

export const loop = function ()
{
    Screeps.setup();

    Profiler.start();

    Scheduler.initialize();
    Scheduler.run();

    const a: Set<number> = Set.from([1, 2, 3]);
    const b: Set<number> = Set.from([2, 3, 4]);
    const c: Set<number> = Set.from([3, 4, 5]);
    const u1: Set<number> = Set.union(a, b);
    const u2: Set<number> = Set.unionAll([a, b, c]);
    const f1: Array<number> = Array.flatten([a, b, c]);

    console.log(`a: [${a.toArray()}], a+b: [${u1.toArray()}], a+b+c: [${u2.toArray()}]`);
    console.log(`f1: [${f1}]`);

    Profiler.stop();
}