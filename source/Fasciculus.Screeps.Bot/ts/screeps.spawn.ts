import { Cached } from "./screeps.util";
import { Objects } from "./types";

export class ScreepsSpawn
{
    private static level(this: StructureSpawn): number
    {
        return this.room.level;
    }

    private static _my: Cached<Map<SpawnId, StructureSpawn>> = Cached.simple(ScreepsSpawn.fetchMy);

    private static fetchMy(): Map<SpawnId, StructureSpawn>
    {
        return Objects.values(Game.spawns).indexBy(s => s.id);
    }

    private static my(): Array<StructureSpawn>
    {
        return ScreepsSpawn._my.value.vs();
    }

    private static myIds(): Set<SpawnId>
    {
        return ScreepsSpawn._my.value.ks();
    }

    static setup()
    {
        Objects.setGetter(StructureSpawn.prototype, "level", ScreepsSpawn.level);

        Objects.setGetter(StructureSpawn, "my", ScreepsSpawn.my);
        Objects.setGetter(StructureSpawn, "myIds", ScreepsSpawn.myIds);
    }
}
