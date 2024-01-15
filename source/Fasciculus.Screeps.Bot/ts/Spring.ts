import * as _ from "lodash";

import { Chamber } from "./Chamber";
import { IJobCreator } from "./IJobCreator";
import { ISpringMemory } from "./ISpringMemory";
import { Job } from "./Job";
import { JobType } from "./JobType";
import { MemoryManager } from "./MemoryManager";

export class Spring implements IJobCreator
{
    readonly source: Source;
    readonly chamber: Chamber;

    constructor(source: Source)
    {
        this.source = source;
        this.chamber = new Chamber(this.source.room);
    }

    get id(): Id<Source> { return this.source.id; }

    get memory(): ISpringMemory { return MemoryManager.spring(this.id); }

    get harvestSlots(): number
    {
        var result = this.memory.harvestSlots;

        if (!result)
        {
            result = this.countHarvestSlots();
            this.memory.harvestSlots = result;
        }

        return result;
    }

    private countHarvestSlots(): number
    {
        var pos = this.source.pos;
        var types = this.chamber.territory.around(pos.x, pos.y);

        return types.filter(t => t != TERRAIN_MASK_WALL).length;
    }

    createJobs(): Job[]
    {
        return _.range(1, 1 + this.harvestSlots).map(p => new Job(JobType.Harvest, this.id, p))
    }
}