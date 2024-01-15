
import * as _ from "lodash";

import { MemoryManager } from "./MemoryManager";
import { Jobs } from "./Jobs";
import { Executor } from "./Executor";
import { Spawns } from "./Spawns";
import { Constructions } from "./Constructions";
import { Bots } from "./Bots";

export const loop = function ()
{
    MemoryManager.cleanup();

    Spawns.spawn();
    Jobs.update();
    Executor.run();

    console.log(`${Constructions.my.length}, ${Bots.idleBuilders.length}`);
}
