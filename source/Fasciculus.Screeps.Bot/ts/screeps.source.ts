import { Objects } from "./types";

export class ScreepsSource
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

    static slots(this: Source): number
    {
        return ScreepsSource._slots.ensure(this.id, ScreepsSource.countSlots);
    }

    static setup()
    {
        Objects.setGetter(Source.prototype, "slots", ScreepsSource.slots);
    }
}