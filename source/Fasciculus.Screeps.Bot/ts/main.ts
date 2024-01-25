import * as _ from "lodash";

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
import { Upgraders } from "./Upgraders";
import { Walls } from "./Walls";
import { Wellers } from "./Wellers";
import { Wells } from "./Wells";
import { Profiler } from "./Profiler";

export const loop = function ()
{
    Profiler.start();
    Initializer.run();
    Profiler.add("Initializer");
    Scheduler.run();
    Profiler.stop();

    if ((Game.time % 10) == 0) Profiler.log();

    // console.log(`walls ${Walls.avg}`);
    // console.log(`bucket ${Game.cpu.bucket}`);
}