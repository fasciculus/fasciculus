
import * as _ from "lodash";

import { MemoryManager } from "./MemoryManager";
import { Jobs } from "./Jobs";
import { Spawns } from "./Spawns";
import { Initializer } from "./Initializer";
import { Walls } from "./Walls";

export const loop = function ()
{
    MemoryManager.cleanup();
    Initializer.run();
    
    Jobs.run();
    Spawns.spawn();

    if ((Game.time % 10) == 0)
    {
        Walls.report();
    }
}
