import * as _ from "lodash";
import { IJobCreator } from "./IJobCreator";
import { Springs } from "./Springs";
import { Controllers } from "./Controllers";
import { Spawns } from "./Spawns";
import { Extensions } from "./Extensions";
import { Constructions } from "./Constructions";
import { Job } from "./Job";
import { Bots } from "./Bots";
import { Walls } from "./Walls";

export class JobFactory
{
    private static get creators(): IJobCreator[]
    {
        var result: IJobCreator[] = [];

        result = result.concat(Springs.active);
        result = result.concat(Controllers.my);
        result = result.concat(Spawns.all);
        result = result.concat(Extensions.my);
        result = result.concat(Constructions.my);
        result = result.concat(Walls.my);

        return result;
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

    static create(): Job[]
    {
        var jobs = _.flatten(JobFactory.creators.map(c => c.createJobs()));

        jobs = JobFactory.filter(jobs);
        jobs = JobFactory.sort(jobs);

        return jobs;
    }
}