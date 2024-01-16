import { Initializer } from "./Initializer"
import { Memories } from "./Memories";
import { Names } from "./Names";
import { Scheduler } from "./Scheduler";
import { Spawns } from "./Spawns";

export const loop = function ()
{
    Initializer.run();
    Scheduler.run();

    console.log(Spawns.my);
}