import { Builders } from "./Builders";
import { GameWrap } from "./Common";
import { Controllers } from "./Controllers";
import { Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { Markers } from "./Markers";
import { profile } from "./Profiling";
import { Repairers } from "./Repairers";
import { Repairs } from "./Repairs";
import { Wells } from "./Resources";
import { Chambers } from "./Rooming";
import { Rooms } from "./Rooms";
import { Sites } from "./Sites";
import { Spawning } from "./Spawning";
import { Spawns } from "./Spawns";
import { Starters } from "./Starters";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Walls } from "./Walls";
import { Wellers } from "./Wellers";

export class Scheduler
{
    @profile
    static initialize()
    {
        GameWrap.initialize();

        Rooms.initialize();
        Controllers.initialize();
        Sites.initialize();
        Spawns.initialize();
        Extensions.initialize();
        Walls.initialize();
        Repairs.initialize();
        Markers.initialize();

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
            case 1: Creeps.cleanup(); break;
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