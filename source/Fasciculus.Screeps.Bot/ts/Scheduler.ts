import { Builders } from "./Builders";
import { Chambers } from "./Chambers";
import { Controllers } from "./Controllers";
import { Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { Markers } from "./Markers";
import { Memories } from "./Memories";
import { Repairers } from "./Repairers";
import { Repairs } from "./Repairs";
import { Rooms } from "./Rooms";
import { Sites } from "./Sites";
import { Sources } from "./Sources";
import { Spawning } from "./Spawning";
import { Spawns } from "./Spawns";
import { Starters } from "./Starters";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Walls } from "./Walls";
import { Wellers } from "./Wellers";
import { Wells } from "./Wells";

export class Scheduler
{
    static initialize()
    {
        Rooms.initialize();
        Controllers.initialize();
        Sites.initialize();
        Spawns.initialize();
        Extensions.initialize();
        Walls.initialize();
        Repairs.initialize();
        Markers.initialize();

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

    // @profile
    static run()
    {
        switch (Game.time % 10)
        {
            case 1: Memories.cleanup(); break;
        }

        Starters.run();
        Wellers.run();
        Suppliers.run();
        Upgraders.run();
        Builders.run();
        Repairers.run();

        Spawning.run();

        Markers.run();
    }
}