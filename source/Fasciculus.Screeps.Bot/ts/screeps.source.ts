import { Assignees, Cached } from "./screeps.util";
import { Objects } from "./types.util";

export class Sources
{
    private static _slots: Map<SourceId, number> = new Map();

    private static countSlots(id: SourceId): number
    {
        const source: Source | undefined = Game.get(id);

        if (!source) return 0;

        const terrain: RoomTerrain = source.room.getTerrain();

        const pos: RoomPosition = source.pos;
        const x: number = pos.x;
        const y: number = pos.y;
        var result = 0;

        if (terrain.get(x - 1, y - 1) == 0) ++result;
        if (terrain.get(x, y - 1) == 0) ++result;
        if (terrain.get(x + 1, y - 1) == 0) ++result;
        if (terrain.get(x + 1, y) == 0) ++result;
        if (terrain.get(x + 1, y + 1) == 0) ++result;
        if (terrain.get(x, y + 1) == 0) ++result;
        if (terrain.get(x - 1, y + 1) == 0) ++result;
        if (terrain.get(x - 1, y) == 0) ++result;

        return result;
    }

    private static slots(this: Source): number
    {
        return Sources._slots.ensure(this.id, Sources.countSlots);
    }

    private static _freeSlots: Cached<Map<SourceId, number>> = Cached.simple(Sources.fetchFreeSlots);

    private static fetchFreeSlots(): Map<SourceId, number> { return new Map<SourceId, number>(); }

    private static countFreeSlots(id: SourceId): number
    {
        const source: Source | undefined = Game.get(id);

        return source ? (source.slots - source.assignees.length): 0;
    }

    private static freeSlots(this: Source): number
    {
        return Sources._freeSlots.value.ensure(this.id, Sources.countFreeSlots);
    }

    private static assignees(this: Source): Array<Creep>
    {
        return Assignees.get(this);
    }

    private static assignedWorkParts(this: Source): number
    {
        return Assignees.assignedWorkParts(this);
    }

    private static assign(this: Source, creep: Creep): void
    {
        Assignees.assign(this, creep);
    }

    private static unassign(this: Source, creep: Creep): void
    {
        Assignees.unassign(this, creep);
    }

    static setup()
    {
        Objects.setGetter(Source.prototype, "slots", Sources.slots);
        Objects.setGetter(Source.prototype, "freeSlots", Sources.freeSlots);
        Objects.setGetter(Source.prototype, "assignees", Sources.assignees);
        Objects.setGetter(Source.prototype, "assignedWorkParts", Sources.assignedWorkParts);
        Objects.setFunction(Source.prototype, "assign", Sources.assign);
        Objects.setFunction(Source.prototype, "unassign", Sources.unassign);
    }
}