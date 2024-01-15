
import * as _ from "lodash";
import { Names } from "./Names";
import { IJobCreator } from "./IJobCreator";
import { Job } from "./Job";
import { JobType } from "./JobType";
import { Bots } from "./Bots";

const WORKER_BODY = [WORK, CARRY, MOVE, MOVE];
const WORKER_COST = _.sum(WORKER_BODY.map(b => BODYPART_COST[b]));

export class Spawn implements IJobCreator
{
    readonly spawner: StructureSpawn;

    constructor(spawner: StructureSpawn)
    {
        this.spawner = spawner;
    }

    get idle(): boolean { return !this.spawner.spawning; }

    spawn()
    {
        if (Bots.idle.length > 2) return;

        let energy = this.spawner.room.energyAvailable;

        if (energy < WORKER_COST) return;

        let name = Names.nextWorkerName();

        this.spawner.spawnCreep(WORKER_BODY, name);
    }

    createJobs(): Job[]
    {
        if (this.spawner.store.getFreeCapacity() == 0) return [];

        return [new Job(JobType.Supply, this.spawner.id, 1)];
    }
}