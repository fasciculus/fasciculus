import { IJobCreator } from "./IJobCreator";
import { Job } from "./Job";
import { JobType } from "./JobType";

export class Wall implements IJobCreator
{
    readonly wall: StructureWall;

    constructor(wall: StructureWall)
    {
        this.wall = wall;
    }

    get id(): Id<StructureWall> { return this.wall.id; }

    get hits(): number { return this.wall.hits; }

    get my(): boolean
    {
        var controller = this.wall.room.controller;

        return controller ? controller.my : false;
    }

    createJobs(): Job[]
    {
        if (this.hits == WALL_HITS_MAX) return [];

        var prio = Math.floor(this.hits / 1000);

        return [new Job(JobType.Repair, this.id, prio)];
    }
}