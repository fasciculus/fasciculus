import { Builders } from "./Builders";
import { Memories } from "./Memories";
import { Spawning } from "./Spawning";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";

export class Scheduler
{
    static run()
    {
        Memories.cleanup();

        Wellers.run();
        Suppliers.run();
        Upgraders.run();
        Builders.run();

        Spawning.run();
    }
}