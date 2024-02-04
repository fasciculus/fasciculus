import { VERSION } from "./Config";
import { Bodies, Creeps } from "./Creeps";
import { Guards } from "./Forces";
import { Controllers, Extensions, Spawns, Walls } from "./Infrastructure";
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
    private static _version: string = "";

    private static updateVersion(): boolean
    {
        const changed: boolean = Scheduler._version != VERSION;

        Scheduler._version = VERSION;

        return changed;
    }

    static initialize()
    {
        const reset: boolean = Scheduler.updateVersion();

        Chambers.initialize(reset);
        Sites.initialize(reset);
        Spawns.initialize(reset);
        Extensions.initialize(reset);
        Walls.initialize(reset);
        Repairs.initialize();
        Markers.initialize(reset);

        Wells.initialize(reset);
        Controllers.initialize(reset);

        Bodies.initialize(reset);
        Creeps.initialize(reset);
        Wellers.initialize(reset);
        Upgraders.initialize(reset);
        Builders.initialize(reset);
        Repairers.initialize(reset);
        Tankers.initialize(reset);

        Guards.initialize(reset);
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