import { CreepState, Creeps, ICreepMemory } from "./Creeps";
import { Initializer } from "./Initializer"
import { Memories } from "./Memories";
import { Names } from "./Names";
import { Rooms } from "./Rooms";
import { Scheduler } from "./Scheduler";
import { Spawns } from "./Spawns";
import { Utils } from "./Utils";
import { Wellers } from "./Wellers";

export const loop = function ()
{
    Initializer.run();
    // Creeps.resetStates();
    Scheduler.run();

    // console.log(`cpu used ${Math.ceil(Game.cpu.getUsed())}`);
}