import { Cached } from "./cache";
import { ControllerExt } from "./controller";
import { GameExt } from "./game";
import { MemoryExt } from "./memory";
import { SpawnExt } from "./spawn";

export class Screeps
{
    static setup()
    {
        MemoryExt.setup();
        ControllerExt.setup();
        SpawnExt.setup();
        GameExt.setup();
    }

    static cleanup()
    {
        Cached.cleanup();
    }
}