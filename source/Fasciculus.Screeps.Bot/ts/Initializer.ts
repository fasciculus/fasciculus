import { Builders } from "./Builders";
import { Chambers } from "./Chambers";
import { Constructions } from "./Constructions";
import { Controllers } from "./Controllers";
import { Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { Rooms } from "./Rooms";
import { Sources } from "./Sources";
import { Spawns } from "./Spawns";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";
import { Wells } from "./Wells";

export class Initializer
{
    static run()
    {
        Rooms.initialize();
        Controllers.initialize();
        Constructions.initialize();
        Spawns.initialize();
        Extensions.initialize();

        Sources.initialize();

        Chambers.initialize();
        Wells.initialize();

        Creeps.initialize();
        Wellers.initialize();
        Suppliers.initialize();
        Upgraders.initialize();
        Builders.initialize();
    }
}