import { Objects } from "../es/object";
import { Cached } from "./cache";

export class Sources
{
    private static _known: Cached<Map<SourceId, Source>> = Cached.simple(Sources.getKnown);
    private static _safe: Cached<Map<SourceId, Source>> = Cached.simple(Sources.getSafe);

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

    static setup()
    {
        Objects.setGetter(Source, "known", Sources.known);
        Objects.setGetter(Source, "safe", Sources.safe);
    }
}