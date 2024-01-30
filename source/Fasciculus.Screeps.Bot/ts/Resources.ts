import { Dictionaries, Dictionary, GameWrap, Memories, Sets, SourceId, Vector } from "./Common";
import { profile } from "./Profiling";

interface WellMemory
{
    assignee?: string;
}

export class Well
{
    readonly id: SourceId;

    get memory(): WellMemory { return Memories.sub("wells", this.id, {}); }

    get source(): Source { return GameWrap.get<Source>(this.id)!; }
    get pos(): RoomPosition { return this.source.pos; }

    get assignee(): Creep | undefined { return GameWrap.myCreep(this.memory.assignee); }
    set assignee(value: Creep) { this.memory.assignee = value.name; }

    constructor(id: SourceId)
    {
        this.id = id;
    }
}

export class Wells
{
    private static _roomSources: Dictionary<Set<SourceId>> = {};
    private static _sources: Set<SourceId> = new Set();
    private static _wells: Dictionary<Well> = {};

    static get(id?: SourceId): Well | undefined { return id ? Wells._wells[id] : undefined; }
    static get all(): Vector<Well> { return Dictionaries.values(Wells._wells); }
    static get assignable(): Vector<Well> { return Dictionaries.values(Wells._wells).filter(w => !w.assignee); }
    static get assignableCount(): number { return Wells.assignable.length; }

    @profile
    static initialize(clear: boolean)
    {
        Wells.clear(clear);
        Wells.updateSources();
        Wells.updateWells();
    }

    private static clear(clear: boolean)
    {
        if (clear)
        {
            Wells._roomSources = {};
            Wells._sources = new Set();
            Wells._wells = {};
        }
    }

    @profile
    private static updateSources()
    {
        const existing: Set<string> = new Set(GameWrap.rooms.map(r => r.name));

        if (Dictionaries.update(Wells._roomSources, existing, Wells.sourceIdsOf))
        {
            Wells._sources = Sets.unionAll(Dictionaries.values(Wells._roomSources));
        }
    }

    private static sourceIdsOf(roomName: string): Set<SourceId>
    {
        const room = Game.rooms[roomName];
        const sources: Vector<Source> = Vector.from(room.find<FIND_SOURCES, Source>(FIND_SOURCES));

        return sources.map(s => s.id).toSet();
    }

    private static updateWells()
    {
        Dictionaries.update(Wells._wells, Wells._sources, id => new Well(id as SourceId));
    }
}