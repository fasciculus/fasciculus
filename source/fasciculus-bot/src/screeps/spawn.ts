import { Objects } from "../es/object";
import { Cached } from "./cache";
import { Names } from "./name";

export class Spawns
{
    private static _my: Cached<Map<SpawnId, StructureSpawn>> = Cached.simple(Spawns.fetchMy);
    private static _idle: Cached<Map<SpawnId, StructureSpawn>> = Cached.simple(Spawns.fetchIdle);

    private static fetchMy(): Map<SpawnId, StructureSpawn>
    {
        return Objects.values(Game.spawns).indexBy(s => s.id);
    }

    private static fetchIdle(): Map<SpawnId, StructureSpawn>
    {
        return Spawns._my.value.filter((id, spawn) => !spawn.spawning);
    }

    private static spawn(this: StructureSpawn, type: string, body: Array<BodyPartConstant>): ScreepsReturnCode
    {
        const name = Names.nextCreepName(type);

        return this.spawnCreep(body, name);
    }

    private static my(): Array<StructureSpawn>
    {
        return Spawns._my.value.data;
    }

    private static idle(): Array<StructureSpawn>
    {
        return Spawns._idle.value.data;
    }

    static setup()
    {
        Objects.setFunction(Spawn.prototype, "spawn", Spawns.spawn);

        Objects.setGetter(Spawn, "my", Spawns.my);
        Objects.setGetter(Spawn, "idle", Spawns.idle);
    }
}