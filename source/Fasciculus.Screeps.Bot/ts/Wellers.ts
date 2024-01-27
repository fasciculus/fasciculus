import { Bodies, BodyTemplate } from "./Bodies";
import { Vector } from "./Collections";
import { CreepState, CreepType, SourceId } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Positions } from "./Positions";
import { profile } from "./Profiling";
import { Well, Wells } from "./Wells";

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

interface WellerMemory extends CreepBaseMemory
{
    well?: SourceId;
}

export class Weller extends CreepBase<WellerMemory>
{
    private _well?: Well;

    get well(): Well | undefined { return this._well; }
    set well(value: Well | undefined) { this._well = value; this.memory.well = value?.id; }

    get maxEnergyPerTick(): number { return this.workParts * HARVEST_POWER; }
    get full(): boolean { return this.freeEnergyCapacity < this.maxEnergyPerTick; }

    constructor(creep: Creep)
    {
        super(creep, CreepType.Weller);

        this._well = Wells.get(this.memory.well);
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToWell: this.executeToWell(); break;
            case CreepState.Harvest: this.executeHarvest(); break;
        }
    }

    @profile
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

        this.harvest(well.source);
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

    @profile
    private prepareIdle(): CreepState
    {
        if (this.full) return CreepState.Idle;

        let well = this.well;

        if (!well) return CreepState.Idle;

        return this.inRangeTo(well) ? CreepState.Harvest : CreepState.ToWell
    }

    @profile
    private prepareToWell(): CreepState
    {
        let well = this.well;

        if (!well) return this.prepareIdle();

        return this.inRangeTo(well) ? CreepState.Harvest : CreepState.ToWell;
    }

    @profile
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
        Wellers.prepare(Wellers._all);
        Wellers.prepare(Wellers.assign());
        Wellers.execute(Wellers._all);
    }

    @profile
    private static prepare(wellers: Vector<Weller>)
    {
        wellers.forEach(w => w.prepare());
    }

    private static execute(wellers: Vector<Weller>)
    {
        wellers.forEach(w => w.execute());
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