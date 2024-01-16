import { IJobCreator } from "./IJobCreator";
import { Job } from "./Job";
import { JobType } from "./JobType";
import { Utils } from "./Utils";

export class Extension implements IJobCreator
{
    readonly extension: StructureExtension;

    constructor(extension: StructureExtension)
    {
        this.extension = extension;
    }

    get freeCapacity(): number { return this.extension.store.getFreeCapacity(RESOURCE_ENERGY); }

    createJobs(): Job[]
    {
        if (this.freeCapacity == 0) return [];

        return [new Job(JobType.Supply, this.extension.id, 0)];
    }
}