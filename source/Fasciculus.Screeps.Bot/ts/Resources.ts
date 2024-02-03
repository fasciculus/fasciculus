import { Dictionaries, Dictionary, GameWrap, SourceId, Vector } from "./Common";
import { profile } from "./Profiling";
import { Chambers } from "./Rooms";

interface WellMemory
{
    assignee?: string;
}

export class Well
{
    readonly id: SourceId;

    get memory(): WellMemory { return Memory.sub("wells", this.id, {}); }

    get source(): Source { return Game.get<Source>(this.id)!; }
    get pos(): RoomPosition { return this.source.pos; }

    get assignee(): Creep | undefined { return Game.myCreep(this.memory.assignee); }
    set assignee(value: Creep) { this.memory.assignee = value.name; }

    constructor(id: SourceId)
    {
        this.id = id;
    }
}

export class Wells
{
    private static _wells: Dictionary<Well> = {};

    static get(id?: SourceId): Well | undefined { return id ? Wells._wells[id] : undefined; }
    static get all(): Vector<Well> { return Dictionaries.values(Wells._wells); }
    static get assignable(): Vector<Well> { return Dictionaries.values(Wells._wells).filter(w => !w.assignee); }
    static get assignableCount(): number { return Wells.assignable.length; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Wells._wells = {};
        }

        Dictionaries.update(Wells._wells, Chambers.allSources, id => new Well(id as SourceId));
    }
}