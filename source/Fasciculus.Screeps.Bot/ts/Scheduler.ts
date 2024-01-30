import { Builders } from "./Builders";
import { GameWrap } from "./Common";
import { VERSION } from "./Config";
import { Controllers } from "./Controllers";
import { Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { Spawns } from "./Infrastructure";
import { Markers } from "./Markers";
import { Repairers } from "./Repairers";
import { Repairs } from "./Repairs";
import { Wells } from "./Resources";
import { Chambers } from "./Rooming";
import { Sites } from "./Sites";
import { Spawning } from "./Spawning";
import { Upgraders } from "./Upgraders";
import { Walls } from "./Walls";
import { Wellers } from "./Wellers";

export class Scheduler
{
    private static _version: string = "";

    static initialize()
    {
        const reset: boolean = Scheduler.updateVersion();

        GameWrap.initialize();

        Chambers.initialize();
        Sites.initialize();
        Spawns.initialize(reset);
        Extensions.initialize();
        Walls.initialize();
        Repairs.initialize();
        Markers.initialize();

        Wells.initialize(reset);
        Controllers.initialize();

        Creeps.initialize(reset);
        Wellers.initialize(reset);
        Upgraders.initialize(reset);
        Builders.initialize(reset);
        Repairers.initialize(reset);
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

        Spawning.run();

        Markers.run();
    }
}