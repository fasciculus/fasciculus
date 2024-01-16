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
        var prio = Math.floor(this.remaining / 1000);

        return [new Job(JobType.Build, this.site.id, prio)];
    }
}