import { Builders } from "./Builders";
import { Markers } from "./Markers";
import { Memories } from "./Memories";
import { Profiler } from "./Profiler";
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
        Memories.cleanup(); Profiler.add("Memories");

        Starters.run(); Profiler.add("Starters");
        Wellers.run(); Profiler.add("Wellers");
        Suppliers.run(); // Profiler.add("Suppliers");
        Upgraders.run(); Profiler.add("Upgraders");
        Builders.run(); Profiler.add("Builders");
        Repairers.run(); Profiler.add("Repairers");

        Spawning.run(); Profiler.add("Spawning");

        Statistics.run();
        Markers.run();
    }
}