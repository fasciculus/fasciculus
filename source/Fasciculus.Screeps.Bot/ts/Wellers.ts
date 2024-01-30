import { Bodies, BodyTemplate, CreepState, CreepType, Dictionaries, Dictionary, Positions, SourceId, Vector } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { profile } from "./Profiling";
import { Well, Wells } from "./Resources";

const WELLER_TEMPLATE: BodyTemplate = BodyTemplate.create([WORK, CARRY, MOVE])
    .add([WORK, MOVE]).add([WORK, CARRY, MOVE]).add([WORK, MOVE], 3);

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
    ready?: boolean;
}

export class Weller extends CreepBase<WellerMemory>
{
    readonly maxEnergyPerTick: number;

    get well(): Well | undefined { return Wells.get(this.memory.well); }
    set well(value: Well | undefined) { this.memory.well = value?.id; this.ready = false; }

    get ready(): boolean { return this.memory.ready || false; }
    private set ready(value: boolean) { this.memory.ready = value; }

    private get full(): boolean { return this.freeEnergyCapacity < this.maxEnergyPerTick; }

    constructor(name: string)
    {
        super(name);

        this.maxEnergyPerTick = this.workParts * HARVEST_POWER;
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

        if (!well) return;

        this.moveTo(well, WELLER_MOVE_TO_OPTS);
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
        if (this.ready) return CreepState.Harvest;
        if (!this.well) return CreepState.Idle;

        return CreepState.ToWell;
    }

    @profile
    private prepareToWell(): CreepState
    {
        let well = this.well;

        if (!well) return this.prepareIdle();

        if (this.inRangeTo(well))
        {
            this.ready = true;
            return CreepState.Harvest;
        }

        return CreepState.ToWell;
    }

    @profile
    private prepareHarvest(): CreepState
    {
        return this.full ? CreepState.Idle : CreepState.Harvest;
    }

    private inRangeTo(target: Well): boolean
    {
        return this.pos.inRangeTo(target.pos, 1);
    }
}

const FIND_CLOSEST_WELL_OPTS: FindPathOpts =
{
    ignoreCreeps: true
};

export class Wellers
{
    private static _wellers: Dictionary<Weller> = {};

    private static _all: Vector<Weller> = new Vector();
    private static _maxEnergyPerTick: number = 0;
    private static _maxEnergyCapacity: number = 0;

    static get count(): number { return Wellers._all.length; }
    static get all(): Vector<Weller> { return Wellers._all.clone(); }

    static get maxEnergyPerTick(): number { return Wellers._maxEnergyPerTick; }
    static get maxEnergyCapacity(): number { return Wellers._maxEnergyCapacity; }

    private static clear(clear: boolean)
    {
        if (clear)
        {
            Wellers._wellers = {};
            Wellers._all = new Vector();
            Wellers._maxEnergyPerTick = 0;
            Wellers._maxEnergyCapacity = 0;
        }
    }

    @profile
    static initialize(clear: boolean)
    {
        Wellers.clear(clear);

        if (Creeps.update(Wellers._wellers, CreepType.Weller, name => new Weller(name)))
        {
            Wellers._all = Dictionaries.values(Wellers._wellers);
            Wellers._maxEnergyPerTick = Wellers._all.sum(w => w.maxEnergyPerTick);
            Wellers._maxEnergyCapacity = Wellers._all.max(w => w.energyCapacity)?.energyCapacity || 0;
        }

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

    @profile
    private static execute(wellers: Vector<Weller>)
    {
        wellers.forEach(w => w.execute());
    }

    @profile
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
            nearestWell.assignee = weller.creep;
            result.add(weller);
        }

        return result;
    }
}