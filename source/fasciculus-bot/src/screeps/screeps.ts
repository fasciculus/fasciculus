import { Cached } from "./cache";
import { Controllers } from "./controller";
import { GameExt } from "./game";
import { Memories } from "./memory";
import { Rooms } from "./room";
import { Sources } from "./source";
import { Spawns } from "./spawn";

export class Screeps
{
    static setup()
    {
        GameExt.setup();

        Memories.setup();
        Controllers.setup();
        Rooms.setup();
        Sources.setup();
        Spawns.setup();
    }

    static cleanup()
    {
        Cached.cleanup();
    }
}