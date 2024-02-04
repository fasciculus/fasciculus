import { Bodies, Creeps } from "./Creeps";
import { Guards } from "./Forces";
import { Controllers, Spawns, Walls } from "./Infrastructure";
import { Tankers } from "./Logistics";
import { Markers } from "./Markers";
import { Repairs } from "./Repairs";
import { Wells } from "./Resources";
import { Chambers } from "./Rooms";
import { Sites } from "./Sites";
import { Spawning } from "./Spawning";
import { Paths } from "./Travelling";
import { Builders, Repairers, Upgraders, Wellers } from "./Workers";

export class Scheduler
{
    static initialize()
    {
        Chambers.initialize();
        Sites.initialize();
        Spawns.initialize();
        Walls.initialize();
        Repairs.initialize();
        Markers.initialize();

        Wells.initialize();
        Controllers.initialize();

        Bodies.initialize();
        Creeps.initialize();
        Wellers.initialize();
        Upgraders.initialize();
        Builders.initialize();
        Repairers.initialize();
        Tankers.initialize();

        Guards.initialize();
    }

    static run()
    {
        switch (Game.time % 10)
        {
            case 1: Creeps.cleanup(); break;
            case 2: Paths.cleanup(); break;
        }

        Wellers.run();
        Upgraders.run();
        Builders.run();
        Repairers.run();
        Tankers.run();

        Guards.run();

        Spawning.run();

        Markers.run();
    }
}