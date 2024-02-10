import { Cached } from "./cache";
import { GameExt } from "./game";
import { MemoryExt } from "./memory";

export class Screeps
{
    static setup()
    {
        MemoryExt.setup();
        GameExt.setup();
    }

    static cleanup()
    {
        Cached.cleanup();
    }
}