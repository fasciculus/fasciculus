import { Creeps } from "./Creeps";
import { Rooms } from "./Rooms";
import { Spawns } from "./Spawns";
import { Wellers } from "./Wellers";

export class Initializer
{
    static run()
    {
        Rooms.initialize();
        Spawns.initialize();

        Creeps.initialize();
        Wellers.initialize();
    }
}