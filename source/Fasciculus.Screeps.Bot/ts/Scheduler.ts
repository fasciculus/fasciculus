import { Bodies } from "./Creeps";
import { Guards } from "./Forces";
import { Controllers, Spawns } from "./Infrastructure";
import { Tankers } from "./Logistics";
import { Markers } from "./Markers";
import { Repairs } from "./Repairs";
import { Wells } from "./Resources";
import { Sites } from "./Sites";
import { Spawning } from "./Spawning";
import { Paths } from "./Travelling";
import { Builders, Repairers, Upgraders, Wellers } from "./Workers";
import { CLI } from "./cli";

export class Scheduler
{
    static initialize()
    {
        CLI.setup();

        Creep.cleanup();

        Sites.initialize();
        Spawns.initialize();
        Repairs.initialize();
        Markers.initialize();

        Wells.initialize();
        Controllers.initialize();

        Bodies.initialize();
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
            case 1: Paths.cleanup(); break;
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