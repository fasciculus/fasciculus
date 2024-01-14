
import { cleanupCreepsMemory } from "./creep";
import { spawnCreeps } from "./spawn";
import { assignNewJobs } from "./job.assign";
import { executeJobs } from "./job.execute";

export const loop = function ()
{
    cleanupCreepsMemory();
    spawnCreeps();
    assignNewJobs();
    executeJobs();
}
