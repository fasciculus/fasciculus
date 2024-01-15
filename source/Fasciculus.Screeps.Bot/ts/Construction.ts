import * as _ from "lodash";

import { Bots } from "./Bots";
import { IJobCreator } from "./IJobCreator";
import { Job } from "./Job";
import { JobType } from "./JobType";

export class Construction implements IJobCreator
{
    readonly site: ConstructionSite;

    constructor(site: ConstructionSite)
    {
        this.site = site;
    }

    get progress(): number { return this.site.progress; }
    get remaining(): number { return this.site.progressTotal - this.progress; }

    createJobs(): Job[]
    {
        var n = 1 + Math.floor(Bots.idleBuilders.length / 2);

        return _.range(1, 1 + n).map(p => new Job(JobType.Build, this.site.id, p));
    }
}