import { IJobCreator } from "./IJobCreator";
import { Job } from "./Job";
import { JobType } from "./JobType";

export class Controller implements IJobCreator
{
    readonly controller: StructureController;

    constructor(controller: StructureController)
    {
        this.controller = controller;
    }

    get id(): Id<StructureController> { return this.controller.id; }

    get my(): boolean { return this.controller.my; }

    createJobs(): Job[]
    {
        return [new Job(JobType.Upgrade, this.id, 1)];
    }
}