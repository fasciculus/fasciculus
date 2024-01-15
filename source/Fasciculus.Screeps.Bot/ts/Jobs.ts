import * as _ from "lodash";

import { IJobCreator } from "./IJobCreator";
import { Springs } from "./Springs";
import { Job } from "./Job";
import { Bots } from "./Bots";
import { JobType } from "./JobType";
import { Bot } from "./Bot";
import { Controllers } from "./Controllers";
import { Spawns } from "./Spawns";
import { Constructions } from "./Constructions";

export class Jobs
{
    static update()
    {
        var jobs = Jobs.create();

        jobs = Jobs.filter(jobs);
        jobs = Jobs.sort(jobs);

        Jobs.assign(jobs);
    }

    private static get creators(): IJobCreator[]
    {
        var result: IJobCreator[] = [];

        result = result.concat(Springs.active);
        result = result.concat(Controllers.my);
        result = result.concat(Spawns.all);
        result = result.concat(Constructions.my);

        return result;
    }

    private static create(): Job[]
    {
        return _.flatten(Jobs.creators.map(c => c.createJobs()));
    }

    private static filter(jobs: Job[]): Job[]
    {
        var map: Map<string, Job> = new Map();

        jobs.forEach(j => map.set(j.id, j));
        Bots.busy.forEach(b => map.delete(b.job!.id));

        return Array.from(map.values());
    }

    private static sort(jobs: Job[]): Job[]
    {
        return jobs.sort((a, b) => a.order - b.order);
    }

    private static assign(jobs: Job[])
    {
        for (var job of jobs)
        {
            var bot: Bot | null = null;

            switch (job.type)
            {
                case JobType.Harvest: bot = Jobs.find(job, Bots.idleHarvesters); break;
                case JobType.Upgrade: bot = Jobs.find(job, Bots.idleUpgraders); break;
                case JobType.Supply: bot = Jobs.find(job, Bots.idleSuppliers); break;
                case JobType.Build: bot = Jobs.find(job, Bots.idleBuilders); break;
            }

            if (!bot) continue;

            bot.job = job;
        }
    }

    private static find(job: Job, bots: Bot[]): Bot | null
    {
        if (bots.length == 0) return null;

        var target = Game.getObjectById(job.target);

        if (!target) return null;

        return target.pos.findClosestByPath(bots);
    }
}