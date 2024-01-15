
import * as _ from "lodash";

import { MemoryManager } from "./MemoryManager";
import { Jobs } from "./Jobs";
import { Executor } from "./Executor";
import { Spawns } from "./Spawns";
import { Bots } from "./Bots";

export const loop = function ()
{
    MemoryManager.cleanup();

    Bots.refresh();
    Spawns.spawn();
    Jobs.update();
    Executor.run();
}
