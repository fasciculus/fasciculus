import { BodyParts, ContainerId, Dictionary, GameWrap, Memories, Point, SourceId, Vector, Vectors } from "./Common";
import { Creeps } from "./Creeps";
import { profile } from "./Profiling";
import { Chamber, Chambers } from "./Rooming";

interface WellMemory
{
    assignee?: string;
}

type WellsMemory = Dictionary<WellMemory>;

export class Well
{
    readonly source: Source;
    readonly id: SourceId;
    readonly memory: WellMemory;

    readonly chamber?: Chamber;

    get pos(): RoomPosition { return this.source.pos; }

    private _assignee?: Creep;
    get assignee(): Creep | undefined { return this._assignee; }
    set assignee(value: Creep | undefined) { this._assignee = value; this.memory.assignee = value?.name; }

    constructor(source: Source)
    {
        const id: SourceId = source.id;
        const memory = Well.getMemory(id);

        this.source = source;
        this.id = id;
        this.memory = memory;
        this.chamber = Chambers.get(this.source.room.name);

        this._assignee = GameWrap.myCreep(memory.assignee);
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

    static get assignable(): Vector<Well> { return Wells._all.filter(w => !w.assignee); }
    static get assignableCount(): number { return Wells.assignable.length; }

    @profile
    static initialize(clear: boolean)
    {
        Wells._all = Vectors.flatten(Chambers.all.map(c => c.sources)).map(s => new Well(s));
        Wells._byId = Wells._all.indexBy(w => w.id);
    }
}