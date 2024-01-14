
import * as _ from "lodash";
import { Job, createHarvestJob, createUpgradeJob } from "./class.Job";
import { getCreepJob } from "./creep";
import { getActiveSources } from "./source";
import { getMyControllers } from "./controller";

export function getNewJobs(): Job[]
{
    var harvestJobs = getHarvestJobs();
    var upgradeJobs = getUpgradeJobs();

    var allJobs = harvestJobs.concat(upgradeJobs);
    var newJobs = removeActiveJobs(allJobs);

    return newJobs;
}

function getHarvestJobs(): Job[]
{
    return _.flatten(getActiveSources().map(createHarvestJobs));
}

function createHarvestJobs(source: Source): Job[]
{
    return [createHarvestJob(source, 1)];
}

function getUpgradeJobs(): Job[]
{
    return _.flatten(getMyControllers().map(createUpgradeJobs));
}

function createUpgradeJobs(controller: StructureController): Job[]
{
    return [createUpgradeJob(controller, 1)];
}

function removeActiveJobs(jobs: Job[]): Job[]
{
    var jobMap: Map<string, Job> = new Map();

    jobs.forEach(j => jobMap.set(j.id, j));

    for (let name in Game.creeps)
    {
        let creep = Game.creeps[name];
        let id = getCreepJob(creep);

        if (id)
        {
            jobMap.delete(id);
        }
    }

    return Array.from(jobMap.values());
}