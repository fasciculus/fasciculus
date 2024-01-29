import { Bodies, BodyTemplate, CreepState, CreepType, Positions, SourceId, Supply, Vector, _Supply } from "./Common";
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

export class Weller extends CreepBase<WellerMemory> implements _Supply
{
    private _well?: Well;
    private _ready: boolean;

    private _maxEnergyPerTick: number;
    private _full: boolean;

    get well(): Well | undefined { return this._well; }
    set well(value: Well | undefined) { this._well = value; this.memory.well = value?.id; this.ready = false; }

    get ready(): boolean { return this._ready; }
    private set ready(value: boolean) { this._ready = this.memory.ready = value; }

    get maxEnergyPerTick(): number { return this._maxEnergyPerTick; }

    readonly supply: Supply;
    offer: number;

    constructor(creep: Creep)
    {
        super(creep, CreepType.Weller);

        let memory = this.memory;

        this._well = Wells.get(memory.well);
        this._ready = this.initReady();
        this._maxEnergyPerTick = this.workParts * HARVEST_POWER;
        this._full = this.freeEnergyCapacity < this._maxEnergyPerTick;

        this.supply = creep;
        this.offer = this.energy;
    }

    private initReady(): boolean
    {
        let memory = this.memory;
        let result = memory.ready;

        if (result === undefined)
        {
            result = this.inRangeTo(this._well);
            memory.ready = result;
        }

        return result;
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
        let well = this._well;

        if (!well) return;

        this.moveTo(well, WELLER_MOVE_TO_OPTS);
    }

    private executeHarvest()
    {
        let well = this._well;

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
        if (this._full) return CreepState.Idle;
        if (this._ready) return CreepState.Harvest;

        let well = this.well;

        if (!well) return CreepState.Idle;

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
        return this._full ? CreepState.Idle : CreepState.Harvest;
    }

    private inRangeTo(target: Well | undefined): boolean
    {
        return (target !== undefined) && this.pos.inRangeTo(target, 1);
    }
}

const FIND_CLOSEST_WELL_OPTS: FindPathOpts =
{
    ignoreCreeps: true
};

export class Wellers
{
    private static _all: Vector<Weller> = new Vector();
    private static _ready: Vector<Weller> = new Vector();

    static get count(): number { return Wellers._all.length; }
    static get all(): Vector<Weller> { return Wellers._all.clone(); }
    static get ready(): Vector<Weller> { return Wellers._ready.clone(); }

    static get maxEnergyPerTick(): number { return Wellers._all.sum(w => w.maxEnergyPerTick); }

    @profile
    static initialize()
    {
        Wellers._all = Creeps.ofType(CreepType.Weller).map(c => new Weller(c));
        Wellers._ready = Wellers._all.filter(w => w.ready);

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