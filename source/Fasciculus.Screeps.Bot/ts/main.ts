
import * as _ from "lodash";

import { MemoryManager } from "./MemoryManager";
import { Jobs } from "./Jobs";
import { Spawns } from "./Spawns";
import { Bots } from "./Bots";

export const loop = function ()
{
    MemoryManager.cleanup();

    Bots.refresh();
    Jobs.run();
    Spawns.spawn();
}
