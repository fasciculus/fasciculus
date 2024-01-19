import * as _ from "lodash";

import { CreepBase, Creeps } from "./Creeps";
import { Bodies } from "./Bodies";
import { CreepState, CreepType } from "./Enums";
import { WellerMemory } from "./Memories";
import { Well, Wells } from "./Wells";

export class Weller extends CreepBase
{
    get memory(): WellerMemory { return super.memory as WellerMemory; }

    get well(): Well | undefined { return Wells.get(this.memory.well); }
    set well(value: Well | undefined) { this.memory.well = value?.id; }

    constructor(creep: Creep)
    {
        super(creep);
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToSource: this.executeToWell(); break;
            case CreepState.Harvest: this.executeHarvest(); break;
        }
    }

    private executeToWell()
    {
        let well = this.well;

        if (well)
        {
            this.moveTo(well);
        }
    }

    private executeHarvest()
    {
        let well = this.well;

        if (well)
        {
            this.harvest(well.source);
        }
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToSource: this.state = this.prepareToWell(); break;
            case CreepState.Harvest: this.state = this.prepareHarvest(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        let well = this.well;

        if (well && this.freeEnergyCapacity > 0)
        {
            return this.inRangeTo(well) ? CreepState.Harvest : CreepState.ToSource;
        }
        else
        {
            return CreepState.Idle;
        }
    }

    private prepareToWell(): CreepState
    {
        return this.inRangeTo(this.well) ? this.prepareIdle() : CreepState.ToSource;
    }

    private prepareHarvest(): CreepState
    {
        return this.freeEnergyCapacity == 0 ? this.prepareIdle() : CreepState.Harvest;
    }

    private inRangeTo(target: Well | StructureContainer | undefined): boolean
    {
        if (!target) return false;

        return this.pos.inRangeTo(target, 1);
    }
}

const WELLER_PARTS: BodyPartConstant[] = [WORK, MOVE, CARRY, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK, WORK, MOVE, WORK];
const WELLER_MIN_SIZE = 3;

export class Wellers
{
    private static _all: Weller[] = [];

    static get all(): Weller[] { return Wellers._all; }

    static initialize()
    {
        Wellers._all = Creeps.ofType(CreepType.Weller).map(c => new Weller(c));

        Bodies.register(CreepType.Weller, WELLER_MIN_SIZE, WELLER_PARTS);
    }

    static run()
    {
        Wellers._all.forEach(w => w.prepare());
        Wellers.assign();
        Wellers._all.forEach(w => w.execute());
    }

    private static assign()
    {
        let assignments: _.Dictionary<Weller[]> = _.groupBy(Wellers.all, w => w.well?.id || "unassigned");
        let unassigned: Weller[] = assignments["unassigned"] || [];

        for (let well of Wells.all)
        {
            let assignees: Weller[] = assignments[well.id] || [];
            let remainingSlots = well.slots.length - assignees.length;
            let assignedWork = _.sum(assignees.map(w => w.capabilities.work));

            while (remainingSlots > 0 && assignedWork < 5)
            {
                let weller = unassigned.pop();

                if (!weller) break;

                weller.well = well;

                assignees.push(weller);
                --remainingSlots;
                assignedWork += weller.capabilities.work;
            }

            well.assignees = assignees.map(w => w.name);
            well.assignedWork = assignedWork;
        }
    }
}