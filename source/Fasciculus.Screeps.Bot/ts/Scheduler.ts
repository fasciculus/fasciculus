import { Builders } from "./Builders";
import { GameWrap } from "./Common";
import { VERSION } from "./Config";
import { Controllers } from "./Controllers";
import { Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { Markers } from "./Markers";
import { Repairers } from "./Repairers";
import { Repairs } from "./Repairs";
import { Wells } from "./Resources";
import { Chambers } from "./Rooming";
import { Sites } from "./Sites";
import { Spawning } from "./Spawning";
import { Spawns } from "./Spawns";
import { Upgraders } from "./Upgraders";
import { Walls } from "./Walls";
import { Wellers } from "./Wellers";

export class Scheduler
{
    private static _version: string = "";

    static initialize()
    {
        const clear: boolean = Scheduler.updateVersion();

        if (clear)
        {
            console.log(`clean initialization`);
        }

        GameWrap.initialize();

        Chambers.initialize();
        Sites.initialize();
        Spawns.initialize();
        Extensions.initialize();
        Walls.initialize();
        Repairs.initialize();
        Markers.initialize();

        Wells.initialize(clear);
        Controllers.initialize();

        Creeps.initialize(clear);
        Wellers.initialize(clear);
        Upgraders.initialize(clear);
        Builders.initialize(clear);
        Repairers.initialize(clear);
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