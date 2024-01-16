import { Rooms } from "./Rooms";
import { Spawns } from "./Spawns";

export class Initializer
{
    static run()
    {
        Rooms.initialize();
        Spawns.initialize();
    }
}