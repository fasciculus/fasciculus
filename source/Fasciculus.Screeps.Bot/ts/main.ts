
import * as _ from "lodash";

import { MemoryManager } from "./MemoryManager";
import { Jobs } from "./Jobs";
import { Executor } from "./Executor";
import { Spawns } from "./Spawns";
import { Springs } from "./Springs";

export const loop = function ()
{
    MemoryManager.cleanup();

    Spawns.spawn();
    Jobs.update();
    Executor.run();
}
