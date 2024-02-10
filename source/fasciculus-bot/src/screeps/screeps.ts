import { Cached } from "./cache";
import { GameExt } from "./game";

export class Screeps
{
    static setup()
    {
        GameExt.setup();
    }

    static cleanup()
    {
        Cached.cleanup();
    }
}