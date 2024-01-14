
import { getCreeps, getCreepJob, setCreepJob } from "./creep";
import { Job, JobType } from "./class.Job";

export function executeJobs()
{
    getCreeps().forEach(executeCreepJob);
}

function executeCreepJob(creep: Creep)
{
    let job = Job.decode(getCreepJob(creep));

    if (!job) return;

    console.log(`executing ${job.id}`);

    var done = false;

    switch (job.type)
    {
        case JobType.Harvest: done = executeHarvest(creep, job.source); break;
        case JobType.Upgrade: done = executeUpgrade(creep, job.controller); break;
    }

    if (done)
    {
        setCreepJob(creep, undefined);
    }
}

function executeHarvest(creep: Creep, source: Source | null): boolean
{
    if (!source) return true;

    var result = creep.harvest(source);

    if (result == ERR_NOT_IN_RANGE)
    {
        creep.moveTo(source);
        return false;
    }

    if (result != OK) return true;

    return creep.store.getFreeCapacity(RESOURCE_ENERGY) > 0;
}

function executeUpgrade(creep: Creep, controller: StructureController | null): boolean
{
    if (!controller) return true;

    var result = creep.upgradeController(controller);

    if (result == ERR_NOT_IN_RANGE)
    {
        creep.moveTo(controller);
        return false;
    }

    if (result != OK) return true;

    return creep.store.energy == 0;
}