import * as _ from "lodash";

import { IJobCreator } from "./IJobCreator";
import { Job } from "./Job";
import { JobType } from "./JobType";
import { Bots } from "./Bots";

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
        var n = 1 + Math.floor(Bots.idleUpgraders.length / 2);

        return _.range(1, 1 + n).map(p => new Job(JobType.Upgrade, this.id, p));
    }
}