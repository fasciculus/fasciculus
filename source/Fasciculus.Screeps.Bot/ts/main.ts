import { CreepManager } from "./creep";
import { SpawnManager } from "./spawn";
import { SourceManager } from "./source";
import { JobManager } from "./job";

export const loop = function ()
{
    CreepManager.cleanup();
    SpawnManager.run();
    SourceManager.run();
    JobManager.run();
}