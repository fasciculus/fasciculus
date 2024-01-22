import * as _ from "lodash";

import { CreepBase, Creeps } from "./Creeps";
import { Bodies, BodyTemplate } from "./Bodies";
import { CreepState, CreepType } from "./Enums";
import { WellerMemory } from "./Memories";
import { Well, Wells } from "./Wells";
import { Statistics } from "./Statistics";

const WELLER_TEMPLATE: BodyTemplate =
{
    chunks:
        [
            { cost: 300, parts: [WORK, CARRY, CARRY, MOVE] },
            { cost: 150, parts: [WORK, MOVE] },
            { cost: 150, parts: [WORK, MOVE] },
            { cost: 150, parts: [WORK, MOVE] },
            { cost: 150, parts: [WORK, MOVE] },
        ]
};

export class Weller extends CreepBase
{
    get memory(): WellerMemory { return super.memory as WellerMemory; }

    get well(): Well | undefined { return Wells.get(this.memory.well); }
    set well(value: Well | undefined) { this.memory.well = value?.id; }

    get harvestCapability(): number { return this.capabilities.work * 2; }
    get full(): boolean { return this.freeEnergyCapacity < this.harvestCapability; }

    constructor(creep: Creep)
    {
        super(creep);
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToWell: this.executeToWell(); break;
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

        if (!well) return;

        let amount = Math.min(well.energy, this.harvestCapability);

        this.harvest(well.source);
        Statistics.addWelled(amount);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.ToWell: this.state = this.prepareToWell(); break;
            case CreepState.Harvest: this.state = this.prepareHarvest(); break;
            default: this.state = this.prepareIdle(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        if (this.full) return CreepState.Idle;

        let well = this.well;

        if (!well) return CreepState.Idle;

        return this.inRangeTo(well) ? CreepState.Harvest : CreepState.ToWell
    }

    private prepareToWell(): CreepState
    {
        let well = this.well;

        if (!well) return this.prepareIdle();

        return this.inRangeTo(well) ? CreepState.Harvest : CreepState.ToWell;
    }

    private prepareHarvest(): CreepState
    {
        if (this.full) return this.prepareIdle();

        let well = this.well;

        if (!well) return this.prepareIdle();

        return this.inRangeTo(well) ? CreepState.Harvest : CreepState.ToWell;
    }

    private inRangeTo(target: Well | StructureContainer): boolean
    {
        return this.pos.inRangeTo(target, 1);
    }
}

export class Wellers
{
    private static _all: Weller[] = [];

    static get all(): Weller[] { return Wellers._all; }

    static initialize()
    {
        Wellers._all = Creeps.ofType(CreepType.Weller).map(c => new Weller(c));

        Bodies.register(CreepType.Weller, WELLER_TEMPLATE);
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
            let remainingSlots = well.slots - assignees.length;
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