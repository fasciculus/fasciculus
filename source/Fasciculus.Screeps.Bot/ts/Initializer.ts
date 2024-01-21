import { Builders } from "./Builders";
import { Chambers } from "./Chambers";
import { Controllers } from "./Controllers";
import { Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { Repairers } from "./Repairers";
import { Repairs } from "./Repairs";
import { Rooms } from "./Rooms";
import { Sites } from "./Sites";
import { Sources } from "./Sources";
import { Spawning } from "./Spawning";
import { Spawns } from "./Spawns";
import { Starters } from "./Starters";
import { Statistics } from "./Statistics";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Walls } from "./Walls";
import { Wellers } from "./Wellers";
import { Wells } from "./Wells";

export class Initializer
{
    static run()
    {
        Statistics.initialize();
        Rooms.initialize();
        Controllers.initialize();
        Sites.initialize();
        Spawns.initialize();
        Extensions.initialize();
        Walls.initialize();
        Repairs.initialize();

        Sources.initialize();

        Chambers.initialize();
        Wells.initialize();

        Creeps.initialize();
        Starters.initialize();
        Wellers.initialize();
        Suppliers.initialize();
        Upgraders.initialize();
        Builders.initialize();
        Repairers.initialize();
    }
}