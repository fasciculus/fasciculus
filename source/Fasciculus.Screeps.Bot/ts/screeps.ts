
import "./screeps.types";

import { ScreepsGame } from "./screeps.game";
import { ScreepsMemory } from "./screeps.memory";
import { Rooms } from "./screeps.room";
import { Sources } from "./screeps.source";
import { ScreepsSpawn } from "./screeps.spawn";
import { Assignees, Cached } from "./screeps.util";
import { ScreepsCreep } from "./screeps.creep";
import { ScreepsController } from "./screeps.controller";
import { ScreepsFlag } from "./screeps.flag";
import { profile } from "./Profiling";
import { ScreepsSite } from "./screeps.site";
import { ScreepsWall } from "./screeps.wall";

export class Screeps
{
    @profile
    static setup()
    {
        ScreepsGame.setup();
        ScreepsMemory.setup();
        Rooms.setup();
        ScreepsFlag.setup();
        Sources.setup();
        ScreepsSpawn.setup();
        ScreepsController.setup();
        ScreepsWall.setup();
        ScreepsSite.setup();
        ScreepsCreep.setup();
    }

    @profile
    static cleanup(): void
    {
        Cached.cleanup();
        Assignees.cleanup();
    }
}
