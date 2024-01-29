import { BodyParts, ContainerId, Dictionary, Memories, Point, SourceId, Vector, Vectors } from "./Common";
import { Creeps } from "./Creeps";
import { profile } from "./Profiling";
import { Chamber, Chambers } from "./Rooming";

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

    private _slots?: number;
    get slots(): number { return this._slots !== undefined ? this._slots : this._slots = this.fetchSlots(); }

    private _assignees?: Vector<Creep>;
    get assignees(): Vector<Creep> { return this._assignees !== undefined ? this._assignees : this._assignees = this.fetchAssignees(); }
    private set assignees(value: Vector<Creep>) { this.memory.assignees = value.map(c => c.name).toArray(); this.clearLazies(); }

    private _freeSlots?: number;
    get freeSlots(): number { return this._freeSlots !== undefined ? this._freeSlots : this._freeSlots = this.fetchFreeSlots(); }

    private _assignable?: boolean;
    get assignable(): boolean { return this._assignable !== undefined ? this._assignable : this._assignable = this.fetchAssignable(); }

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
    }

    private fetchSlots(): number
    {
        return this.memory.slots || (this.memory.slots = Well.countSlots(this.chamber, this.pos));
    }

    private fetchAssignees(): Vector<Creep>
    {
        return Vectors.defined(Vector.from(this.memory.assignees).map(Creeps.get));
    }

    private fetchFreeSlots(): number
    {
        return this.slots - this.assignees.length;
    }

    private fetchAssignable(): boolean
    {
        const assignedWork: number = this.assignees.sum(BodyParts.workOf);
        const maxWork = this.energyCapacity / ENERGY_REGEN_TIME;
        const unassignedWork = maxWork - assignedWork;

        return this.freeSlots > 0 && unassignedWork > 0;
    }

    private clearLazies()
    {
        this._assignees = undefined;
        this._freeSlots = undefined;
        this._assignable = undefined;
    }

    assign(creep: Creep)
    {
        const assignees = this.assignees;

        assignees.append(creep);
        this.memory.assignees = assignees.map(c => c.name).toArray();
        this.clearLazies();
    }

    private static countSlots(chamber: Chamber | undefined, pos: RoomPosition): number
    {
        let territory = chamber?.territory;

        if (!territory) return 0;

        let types = territory.around(Point.from(pos));

        return types.filter(t => t == 0).length;
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

    @profile
    static initialize()
    {
        Wells._all = Vectors.flatten(Chambers.all.map(c => c.sources)).map(s => new Well(s));
        Wells._byId = Wells._all.indexBy(w => w.id);
    }
}