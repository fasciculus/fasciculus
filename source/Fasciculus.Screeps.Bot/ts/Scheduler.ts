import { Builders } from "./Builders";
import { Spawns } from "./Spawns";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";

export class Scheduler
{
    static run()
    {
        Spawns.run();

        Wellers.run();
        Suppliers.run();
        Upgraders.run();
        Builders.run();
    }
}