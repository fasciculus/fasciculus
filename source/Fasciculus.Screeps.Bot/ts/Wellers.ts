import { CreepBase, Creeps } from "./Creeps";
import { Bodies, BodyTemplate } from "./Bodies";
import { CreepState, CreepType } from "./Enums";
import { WellerMemory } from "./Memories";
import { Well, Wells } from "./Wells";
import { Statistics } from "./Statistics";
import { Vector } from "./Collections";
import { Positions } from "./Positions";

const WELLER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE])
    .add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE], 5);

const WELLER_MOVE_TO_OPTS: MoveToOpts =
{
    reusePath: 0,

    visualizePathStyle:
    {
        stroke: "#fff"
    }
};

export class Weller extends CreepBase
{
    get memory(): WellerMemory { return super.memory as WellerMemory; }

    get well(): Well | undefined { return Wells.get(this.memory.well); }
    set well(value: Well | undefined) { this.memory.well = value?.id; }

    get maxEnergyPerTick(): number { return this.workParts * HARVEST_POWER; }
    get full(): boolean { return this.freeEnergyCapacity < this.maxEnergyPerTick; }

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
            this.moveTo(well, WELLER_MOVE_TO_OPTS);
        }
    }

    private executeHarvest()
    {
        let well = this.well;

        if (!well) return;

        let amount = Math.min(well.energy, this.maxEnergyPerTick);

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

const FIND_CLOSEST_WELL_OPTS: FindPathOpts =
{
    ignoreCreeps: true
};

export class Wellers
{
    private static _all: Vector<Weller> = new Vector();

    static get count(): number { return Wellers._all.length; }
    static get all(): Vector<Weller> { return Wellers._all.clone(); }

    static get maxEnergyPerTick(): number { return Wellers._all.sum(w => w.maxEnergyPerTick); }

    static initialize()
    {
        Wellers._all = Creeps.ofType(CreepType.Weller).map(c => new Weller(c));

        Bodies.register(CreepType.Weller, WELLER_TEMPLATE);
    }

    static run()
    {
        Wellers._all.forEach(w => w.prepare());
        Wellers.assign().forEach(w => w.prepare());
        Wellers._all.forEach(w => w.execute());
    }

    private static assign(): Vector<Weller>
    {
        var result: Vector<Weller> = new Vector();
        let unassignedWellers: Vector<Weller> = Wellers.all.filter(w => !w.well);

        for (let weller of unassignedWellers)
        {
            let assignableWells: Vector<Well> = Wells.assignable;
            let nearestWell: Well | undefined = Positions.closestByPath(weller, assignableWells, FIND_CLOSEST_WELL_OPTS);

            if (!nearestWell) continue;

            weller.well = nearestWell;
            nearestWell.assign(weller.creep);
            result.append(weller);
        }

        return result;
    }
}