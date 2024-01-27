import { Chamber, Chambers } from "./Chambers";
import { BodyParts, ContainerId, Dictionary, Memories, SourceId, Vector, Vectors } from "./Common";
import { Creeps } from "./Creeps";
import { DIRECTIONS, Point } from "./Geometry";
import { Sources } from "./Sources";

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
    readonly memory: WellMemory; 

    get id(): SourceId { return this.source.id; }

    get pos(): RoomPosition { return this.source.pos; }
    get room(): Room { return this.source.room; }
    get chamber(): Chamber | undefined { return Chambers.get(this.room.name); }

    get energy(): number { return this.source.energy; }
    get energyCapacity(): number { return this.source.energyCapacity; }

    get slots(): number
    {
        let result = this.memory.slots;

        if (!result)
        {
            this.memory.slots = result = this.countSlots();
        }

        return result;
    }

    get assignees(): Vector<Creep> { return Vectors.defined(Vector.from(this.memory.assignees).map(Creeps.get)); }
    private set assignees(value: Vector<Creep>) { this.memory.assignees = value.map(c => c.name).toArray(); }
    get freeSlots(): number { return this.slots - this.assignees.length; }
    get assignedWork(): number { return this.assignees.sum(BodyParts.workOf); }
    get maxWork(): number { return this.energyCapacity / ENERGY_REGEN_TIME; }
    get unassignedWork(): number { return this.maxWork - this.assignedWork; }
    get assignable(): boolean { return this.freeSlots > 0 && this.unassignedWork > 0; }

    constructor(source: Source)
    {
        this.source = source;
        this.memory = Well.getMemory(this.id);
        this.assignees = this.assignees;
    }

    assign(creep: Creep)
    {
        this.assignees = this.assignees.append(creep);
    }

    private countSlots(): number
    {
        let territory = this.chamber?.territory;

        if (!territory) return 0;

        let types = territory.around(Point.from(this.pos));

        return DIRECTIONS.filter(d => types.at(d) == 0).length;
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
        Wells._all = Sources.all.map(s => new Well(s));
        Wells._byId = Wells._all.indexBy(w => w.id);
    }
}