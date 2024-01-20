import { Builders } from "./Builders";
import { Memories } from "./Memories";
import { Repairers } from "./Repairers";
import { Spawning } from "./Spawning";
import { Starters } from "./Starters";
import { Statistics } from "./Statistics";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";

export class Scheduler
{
    static run()
    {
        Memories.cleanup();

        Starters.run();
        Wellers.run();
        Suppliers.run();
        Upgraders.run();
        Builders.run();
        Repairers.run();

        Spawning.run();

        Statistics.run();
    }
}