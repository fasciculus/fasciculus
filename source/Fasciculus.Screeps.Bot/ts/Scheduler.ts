import { Builders } from "./Builders";
import { GameWrap } from "./Common";
import { VERSION } from "./Config";
import { Bodies, Creeps } from "./Creeps";
import { Controllers, Extensions, Spawns, Walls } from "./Infrastructure";
import { Tankers } from "./Logistics";
import { Markers } from "./Markers";
import { Repairers } from "./Repairers";
import { Repairs } from "./Repairs";
import { Wells } from "./Resources";
import { Chambers } from "./Rooming";
import { Sites } from "./Sites";
import { Spawning } from "./Spawning";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";

export class Scheduler
{
    private static _version: string = "";

    static initialize()
    {
        const reset: boolean = Scheduler.updateVersion();

        GameWrap.initialize(reset);

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
    }

    private static updateVersion(): boolean
    {
        const changed: boolean = Scheduler._version != VERSION;

        Scheduler._version = VERSION;

        return changed;
    }

    // @profile
    static run()
    {
        switch (Game.time % 10)
        {
            case 1: Creeps.cleanup(); break;
        }

        Wellers.run();
        Upgraders.run();
        Builders.run();
        Repairers.run();
        Tankers.run();

        Spawning.run();

        Markers.run();
    }
}