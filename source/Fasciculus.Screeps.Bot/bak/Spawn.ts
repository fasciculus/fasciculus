
import * as _ from "lodash";
import { Names } from "./Names";
import { IJobCreator } from "./IJobCreator";
import { Job } from "./Job";
import { JobType } from "./JobType";
import { Bots } from "./Bots";
import { Bodies, MIN_WORKER_COST } from "./Bodies";

export class Spawn implements IJobCreator
{
    readonly spawner: StructureSpawn;

    constructor(spawner: StructureSpawn)
    {
        this.spawner = spawner;
    }

    get idle(): boolean { return !this.spawner.spawning; }

    get store(): Store<RESOURCE_ENERGY, false> { return this.spawner.store; }

    get capacity(): number { return this.store.getCapacity(RESOURCE_ENERGY); }
    get freeCapacity(): number { return this.store.getFreeCapacity(RESOURCE_ENERGY); }

    spawn()
    {
        if (Bots.idle.length > 1) return;

        let required = Bodies.workerEnergy(this.spawner.room.energyCapacityAvailable);

        if (Bots.my.length < 2)
        {
            required = MIN_WORKER_COST;
        }

        let energy = this.spawner.room.energyAvailable;

        if (energy < required) return;

        let body = Bodies.workerBody(energy);

        if (!body) return;

        this.spawner.spawnCreep(body, Names.nextWorkerName());
    }

    createJobs(): Job[]
    {
        if (this.freeCapacity == 0) return [];

        return _.range(2).map(p => new Job(JobType.Supply, this.spawner.id, p));
    }
}