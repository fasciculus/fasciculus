import { Builders } from "./Builders";
import { Constructions } from "./Constructions";
import { Controllers } from "./Controllers";
import { Creeps } from "./Creeps";
import { Rooms } from "./Rooms";
import { Sources } from "./Sources";
import { Spawns } from "./Spawns";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";

export class Initializer
{
    static run()
    {
        Rooms.initialize();
        Controllers.initialize();
        Constructions.initialize();
        Spawns.initialize();

        Sources.initialize();

        Creeps.initialize();
        Wellers.initialize();
        Suppliers.initialize();
        Upgraders.initialize();
        Builders.initialize();
    }
}