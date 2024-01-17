import { Spawns } from "./Spawns";
import { Suppliers } from "./Suppliers";
import { Wellers } from "./Wellers";

export class Scheduler
{
    static run()
    {
        Spawns.run();

        Wellers.run();
        Suppliers.run();
    }
}