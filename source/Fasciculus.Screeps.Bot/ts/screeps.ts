
import "./types.screeps";

import { ScreepsGame } from "./screeps.game";
import { ScreepsMemory } from "./screeps.memory";
import { ScreepsRoom } from "./screeps.room";
import { ScreepsSource } from "./screeps.source";
import { ScreepsSpawn } from "./screeps.spawn";
import { Cached } from "./screeps.util";
import { ScreepsCreep } from "./screeps.creep";

export class Screeps
{
    static setup()
    {
        ScreepsGame.setup();
        ScreepsMemory.setup();
        ScreepsRoom.setup();
        ScreepsSource.setup();
        ScreepsSpawn.setup();
        ScreepsCreep.setup();
    }

    static cleanup(): void
    {
        Cached.cleanup();
    }
}
