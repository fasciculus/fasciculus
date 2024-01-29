import { BodyParts, ContainerId, Dictionary, Memories, Point, SourceId, Vector, Vectors } from "./Common";
import { Creeps } from "./Creeps";
import { Chamber, Chambers } from "./Rooming";
import { Rooms } from "./Rooms";

interface WellMemory
{
    slots?: number;
    container?: ContainerId;
    assignees?: string[];
}

type WellsMemory = Dictionary<WellMemory>;

export class Well
{
    readonly source: Source;
    readonly id: SourceId;
    readonly memory: WellMemory;

    readonly pos: RoomPosition;
    readonly chamber?: Chamber;

    readonly energy: number;
    readonly energyCapacity: number;

    readonly slots: number;

    private _assignees: Vector<Creep>;

    private _freeSlots: number;
    get freeSlots(): number { return this._freeSlots; }

    private _assignable: boolean;
    get assignable(): boolean { return this._assignable; }

    get assignees(): Vector<Creep> { return this._assignees.clone(); }
    private set assignees(value: Vector<Creep>) { this.memory.assignees = value.map(c => c.name).toArray(); }

    constructor(source: Source)
    {
        const id: SourceId = source.id;
        const memory = Well.getMemory(id);

        this.source = source;
        this.id = id;
        this.memory = memory;
        this.pos = this.source.pos;
        this.chamber = Chambers.get(this.source.room.name);
        this.energy = source.energy;
        this.energyCapacity = source.energyCapacity;
        this._assignees = Vectors.defined(Vector.from(memory.assignees).map(Creeps.get));
        memory.assignees = this._assignees.map(c => c.name).toArray();
        this.slots = memory.slots || (memory.slots = Well.countSlots(this.chamber, this.pos));
        this._freeSlots = this.slots - this._assignees.length;
        this._assignable = Well.isAssignable(this.freeSlots, this._assignees, this.energyCapacity);
    }

    assign(creep: Creep)
    {
        const assignees = this._assignees;

        assignees.append(creep);
        this.memory.assignees = this._assignees.map(c => c.name).toArray();
        --this._freeSlots;
        this._assignable = Well.isAssignable(this.freeSlots, assignees, this.energyCapacity);
    }

    private static countSlots(chamber: Chamber | undefined, pos: RoomPosition): number
    {
        let territory = chamber?.territory;

        if (!territory) return 0;

        let types = territory.around(Point.from(pos));

        return types.filter(t => t == 0).length;
    }

    private static isAssignable(freeSlots: number, assignees: Vector<Creep>, energyCapacity: number): boolean
    {
        const assignedWork: number = assignees.sum(BodyParts.workOf);
        const maxWork = energyCapacity / ENERGY_REGEN_TIME;
        const unassignedWork = maxWork - assignedWork;

        return freeSlots > 0 && unassignedWork > 0;
    }

    private static getMemory(id: SourceId): WellMemory
    {
        const wellsMemory: WellsMemory = Memories.get("wells", {});

        return wellsMemory[id] || (wellsMemory[id] = {});
    }
}

export class Wells
{
    private static _all: Vector<Well> = new Vector();
    private static _byId: Dictionary<Well> = {};

    static get(id?: SourceId): Well | undefined { return id ? Wells._byId[id] : undefined; }

    static get assignable(): Vector<Well> { return Wells._all.filter(w => w.assignable); }
    static get assignableCount(): number { return Wells.assignable.length; }

    static get maxEnergyPerTick(): number { return Wells._all.sum(w => w.energyCapacity) / ENERGY_REGEN_TIME; }

    static initialize()
    {
        Wells._all = Rooms.sources.map(s => new Well(s));
        Wells._byId = Wells._all.indexBy(w => w.id);
    }
}