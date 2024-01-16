import { CreepType, Creeps } from "./Creeps";
import { Initializer } from "./Initializer"
import { Memories } from "./Memories";
import { Names } from "./Names";
import { Rooms } from "./Rooms";
import { Scheduler } from "./Scheduler";
import { Spawns } from "./Spawns";

export const loop = function ()
{
    Initializer.run();
    Scheduler.run();

    console.log(Creeps.ofType(CreepType.Weller).length);
    console.log(`cpu used ${Game.cpu.getUsed()}`);
}