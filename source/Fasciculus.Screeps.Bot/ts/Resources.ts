import { Dictionaries, Dictionary, Vector } from "./Common";
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
    private static _wells: Map<SourceId, Well> = new Map();

    static get(id?: SourceId): Well | undefined { return id ? Wells._wells.get(id) : undefined; }
    static get all(): Array<Well> { return Wells._wells.vs(); }
    static get assignable(): Array<Well> { return Wells.all.filter(w => !w.assignee); }
    static get assignableCount(): number { return Wells.assignable.length; }

    @profile
    static initialize()
    {
        Wells._wells.update(Chambers.allSources, id => new Well(id));
    }
}