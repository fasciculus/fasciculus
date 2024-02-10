import { Cached } from "./cache";
import { GameExt } from "./game";
import { MemoryExt } from "./memory";
import { SpawnExt } from "./spawn";

export class Screeps
{
    static setup()
    {
        MemoryExt.setup();
        SpawnExt.setup();
        GameExt.setup();
    }

    static cleanup()
    {
        Cached.cleanup();
    }
}