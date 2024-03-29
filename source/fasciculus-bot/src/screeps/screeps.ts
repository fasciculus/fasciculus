import { Assigns } from "./assign";
import { BodyInfos } from "./body";
import { Cached } from "./cache";
import { Controllers } from "./controller";
import { Creeps } from "./creep";
import { GameExt } from "./game";
import { Memories } from "./memory";
import { Positions } from "./pos";
import { Rooms } from "./room";
import { Sources } from "./source";
import { Spawns } from "./spawn";
import { Terrains } from "./terrain";

export class Screeps
{
    static setup()
    {
        GameExt.setup();

        Memories.setup();
        Terrains.setup();
        Positions.setup();
        Controllers.setup();
        Rooms.setup();
        Sources.setup();
        Spawns.setup();
        Creeps.setup();
    }

    static cleanup()
    {
        Cached.cleanup();
        Assigns.cleanup();
        BodyInfos.cleanup();
    }
}