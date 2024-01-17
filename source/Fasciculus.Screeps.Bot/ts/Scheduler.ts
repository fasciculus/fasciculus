import { Spawns } from "./Spawns";
import { Wellers } from "./Wellers";

export class Scheduler
{
    static run()
    {
        Spawns.run();

        Wellers.run();
    }
}