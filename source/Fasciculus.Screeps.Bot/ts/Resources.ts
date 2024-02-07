import { profile } from "./Profiling";

export class Well
{
    readonly id: SourceId;

    get source(): Source { return Game.get<Source>(this.id)!; }
    get pos(): RoomPosition { return this.source.pos; }

    get assignee(): Creep | undefined { return this.source.assignees[0]; }
    set assignee(value: Creep) { this.source.assign(value); }

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
        const sourceIds = Set.flatten(Room.safe.map(r => r.sourceIds));

        Wells._wells.update(sourceIds, id => new Well(id));
    }
}