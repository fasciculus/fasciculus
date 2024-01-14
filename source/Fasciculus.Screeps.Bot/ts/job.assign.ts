
import { getNewJobs } from "./job.collect";
import { Job, JobType } from "./class.Job";
import { getIdleHarvesters, getIdleUpgraders, setCreepJob } from "./creep";

export function assignNewJobs()
{
    getNewJobs().forEach(assignNewJob);
}

function assignNewJob(job: Job)
{
    switch (job.type)
    {
        case JobType.Harvest: assignNewHarvestJob(job); break;
        case JobType.Upgrade: assignNewUpgradeJob(job); break;
    }
}

function assignNewHarvestJob(job: Job)
{
    var source = job.source;

    if (!source) return;

    var creep = source.pos.findClosestByPath(getIdleHarvesters());

    if (!creep) return;

    setCreepJob(creep, job.id);
}

function assignNewUpgradeJob(job: Job)
{
    var controller = job.controller;

    if (!controller) return;

    var creep = controller.pos.findClosestByPath(getIdleUpgraders());

    if (!creep) return;

    setCreepJob(creep, job.id);
}