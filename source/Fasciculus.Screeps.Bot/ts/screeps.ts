
import "./screeps.types";

import { ScreepsGame } from "./screeps.game";
import { ScreepsMemory } from "./screeps.memory";
import { ScreepsRoom } from "./screeps.room";
import { ScreepsSource } from "./screeps.source";
import { ScreepsSpawn } from "./screeps.spawn";
import { Cached } from "./screeps.util";
import { ScreepsCreep } from "./screeps.creep";
import { ScreepsController } from "./screeps.controller";
import { ScreepsFlag } from "./screeps.flag";
import { profile } from "./Profiling";
import { ScreepsSite } from "./screeps.site";

export class Screeps
{
    @profile
    static setup()
    {
        ScreepsGame.setup();
        ScreepsMemory.setup();
        ScreepsRoom.setup();
        ScreepsFlag.setup();
        ScreepsSource.setup();
        ScreepsSpawn.setup();
        ScreepsController.setup();
        ScreepsSite.setup();
        ScreepsCreep.setup();
    }

    static cleanup(): void
    {
        Cached.cleanup();
    }
}
