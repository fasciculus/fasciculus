import { Builders } from "./Builders";
import { Chambers } from "./Chambers";
import { Creeps } from "./Creeps";
import { CreepType } from "./Enums";
import { GameWrap } from "./GameWrap";
import { Initializer } from "./Initializer"
import { Memories } from "./Memories";
import { Names } from "./Names";
import { Repairs } from "./Repairs";
import { Rooms } from "./Rooms";
import { Scheduler } from "./Scheduler";
import { Sites } from "./Sites";
import { Spawning } from "./Spawning";
import { Spawns } from "./Spawns";
import { Statistics } from "./Statistics";
import { Suppliers } from "./Suppliers";
import { Utils } from "./Utils";
import { Walls } from "./Walls";
import { Wellers } from "./Wellers";
import { Wells } from "./Wells";

export const loop = function ()
{
    Initializer.run();
    Scheduler.run();

    // console.log(`walls ${Walls.avg}`);
    // console.log(`bucket ${Game.cpu.bucket}`);

    let welled = Utils.round(Statistics.welled, 1);
    let supplied = Utils.round(Statistics.supplied, 1);

    console.log(`${Game.time}: welled ${welled}, supplied ${supplied}`);
}