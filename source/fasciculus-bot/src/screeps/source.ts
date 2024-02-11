import { Objects } from "../es/object";
import { Assigns } from "./assign";
import { Cached } from "./cache";

export class Sources
{
    private static _known: Cached<Map<SourceId, Source>> = Cached.simple(Sources.getKnown);
    private static _safe: Cached<Map<SourceId, Source>> = Cached.simple(Sources.getSafe);
    private static _slots: Map<SourceId, Array<RoomPosition>> = new Map();

    private static getSlots(id: SourceId, hint?: Source): Array<RoomPosition>
    {
        const source: Source | undefined = hint || Game.get(id);

        if (!source) return new Array();

        const terrain: RoomTerrain = source.room.terrain;

        return terrain.around(source.pos, 1).filter(i => i.mask == 0 || i.mask == TERRAIN_MASK_SWAMP).map(i => i.pos);
    }

    private static slots(this: Source): Array<RoomPosition>
    {
        return Sources._slots.ensure(this.id, Sources.getSlots, this);
    }

    private static freeSlots(this: Source): number
    {
        return Math.max(0, this.slots.length - this.assignees.size);
    }

    private static freeWork(this: Source): number
    {
        const maxWork: number = this.energyCapacity / 600;
        const assignedWork: number = this.assignedCreeps.sum(c => c.workParts);

        return Math.max(0, maxWork - assignedWork);
    }

    private static getKnown(): Map<SourceId, Source>
    {
        return Array.flatten(Room.known.map(r => r.sources)).indexBy(s => s.id);
    }

    private static getSafe(): Map<SourceId, Source>
    {
        return Sources._known.value.filter(Sources.isSafe);
    }

    private static isSafe(id: SourceId, source: Source)
    {
        return source.room.safe;
    }

    private static known(): Array<Source>
    {
        return Sources._known.value.data;
    }

    private static safe(): Array<Source>
    {
        return Sources._safe.value.data;
    }

    private static assignees(this: Source): Set<CreepId> { return Assigns.assignees(this.id); }
    private static assignedCreeps(this: Source): Array<Creep> { return Game.all(this.assignees); }
    private static assign(this: Source, creep: CreepId): void { Assigns.assign(this.id, creep); }
    private static unassign(this: Source, creep: CreepId): void { Assigns.unassign(this.id, creep); }
    private static unassignAll(this: Source): void { Assigns.unassignAll(this.id); }

    static setup()
    {
        Objects.setGetter(Source.prototype, "slots", Sources.slots);
        Objects.setGetter(Source.prototype, "freeSlots", Sources.freeSlots);
        Objects.setGetter(Source.prototype, "freeWork", Sources.freeWork);
        Objects.setGetter(Source.prototype, "assignees", Sources.assignees);
        Objects.setGetter(Source.prototype, "assignedCreeps", Sources.assignedCreeps);
        Objects.setFunction(Source.prototype, "assign", Sources.assign);
        Objects.setFunction(Source.prototype, "unassign", Sources.unassign);
        Objects.setFunction(Source.prototype, "unassignAll", Sources.unassignAll);

        Objects.setGetter(Source, "known", Sources.known);
        Objects.setGetter(Source, "safe", Sources.safe);
    }
}