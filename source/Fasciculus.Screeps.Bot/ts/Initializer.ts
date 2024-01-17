import { Creeps } from "./Creeps";
import { Rooms } from "./Rooms";
import { Sources } from "./Sources";
import { Spawns } from "./Spawns";
import { Suppliers } from "./Suppliers";
import { Wellers } from "./Wellers";

export class Initializer
{
    static run()
    {
        Rooms.initialize();
        Spawns.initialize();

        Sources.initialize();

        Creeps.initialize();
        Wellers.initialize();
        Suppliers.initialize();
    }
}