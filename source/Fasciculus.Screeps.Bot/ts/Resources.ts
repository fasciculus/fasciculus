import { Dictionaries, Dictionary, GameWrap, Memories, Sets, SourceId, Vector } from "./Common";
import { profile } from "./Profiling";

interface WellMemory
{
    assignee?: string;
}

type WellsMemory = Dictionary<WellMemory>;

export class Well
{
    readonly id: SourceId;
    readonly memory: WellMemory;

    get source(): Source { return GameWrap.get<Source>(this.id)!; }
    get pos(): RoomPosition { return this.source.pos; }

    get assignee(): Creep | undefined { return GameWrap.myCreep(this.memory.assignee); }
    set assignee(value: Creep | undefined) { this.memory.assignee = value?.name; }

    constructor(id: SourceId)
    {
        this.id = id;
        this.memory = Well.getMemory(id);
    }

    private static getMemory(id: SourceId): WellMemory
    {
        const wellsMemory: WellsMemory = Memories.get("wells", {});

        return wellsMemory[id] || (wellsMemory[id] = {});
    }
}

export class Wells
{
    private static _roomSources: Dictionary<Set<SourceId>> = {};
    private static _sources: Set<SourceId> = new Set();
    private static _wells: Dictionary<Well> = {};

    //private static _all: Vector<Well> = new Vector();
    //private static _byId: Dictionary<Well> = {};

    //static get(id?: SourceId): Well | undefined { return id ? Wells._byId[id] : undefined; }

    //static get assignable(): Vector<Well> { return Wells._all.filter(w => !w.assignee); }
    //static get assignableCount(): number { return Wells.assignable.length; }

    static get(id?: SourceId): Well | undefined { return id ? Wells._wells[id] : undefined; }
    static get assignable(): Vector<Well> { return Dictionaries.values(Wells._wells).filter(w => !w.assignee); }
    static get assignableCount(): number { return Wells.assignable.length; }

    @profile
    static initialize(clear: boolean)
    {
        //Wells._all = Vectors.flatten(Chambers.all.map(c => c.sources)).map(s => new Well(s.id));
        //Wells._byId = Wells._all.indexBy(w => w.id);

        Wells.updateSources();
        Wells.updateWells();
    }

    @profile
    private static updateSources()
    {
        const existing: Set<string> = new Set(GameWrap.rooms.map(r => r.name));
        const toDelete: Set<string> = Sets.difference(Dictionaries.keys(Wells._roomSources), existing);
        const toCreate: Set<string> = Sets.difference(existing, Dictionaries.keys(Wells._roomSources));

        Dictionaries.removeAll(Wells._roomSources, toDelete);

        for (const name of toCreate)
        {
            const room = Game.rooms[name];
            const sources: Vector<Source> = Vector.from(room.find<FIND_SOURCES, Source>(FIND_SOURCES));
            const ids: Set<SourceId> = sources.map(s => s.id).toSet();

            Wells._roomSources[name] = ids;
        }

        if (toDelete.size > 0 || toCreate.size > 0)
        {
            Wells._sources = Sets.unionAll(Dictionaries.values(Wells._roomSources));
        }
    }

    private static updateWells()
    {
        const existing: Set<string> = Wells._sources;
        const toDelete: Set<string> = Sets.difference(Dictionaries.keys(Wells._wells), existing);
        const toCreate: Set<string> = Sets.difference(existing, Dictionaries.keys(Wells._wells));

        Dictionaries.removeAll(Wells._wells, toDelete);

        for (const id of toCreate)
        {
            Wells._wells[id] = new Well(id as SourceId);
        }
    }
}