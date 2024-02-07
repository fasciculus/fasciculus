import { Cached } from "./screeps.util";
import { Objects } from "./types.util";

export class ScreepsGame
{
    private static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    private static myCreeps(): Array<Creep>
    {
        return Objects.values(Game.creeps);;
    }

    private static _myCreepsOfType: Cached<Map<string, Array<Creep>>> = Cached.simple(ScreepsGame.fetchMyCreepsOfType);
    private static _myCreepNamesOfType: Cached<Map<string, Set<string>>> = Cached.simple(ScreepsGame.fetchMyCreepNamesOfType);

    private static fetchMyCreepsOfType(): Map<string, Array<Creep>>
    {
        return ScreepsGame.myCreeps().groupBy(c => c.type);
    }

    private static fetchMyCreepNamesOfType(): Map<string, Set<string>>
    {
        return ScreepsGame._myCreepsOfType.value.map(cs => cs.map(c => c.name).toSet());
    }

    private static myCreepsOfType(type: string): Array<Creep>
    {
        return ScreepsGame._myCreepsOfType.value.get(type)?.clone() || new Array<Creep>();
    }

    private static myCreepNamesOfType(type: string): Set<string>
    {
        return ScreepsGame._myCreepNamesOfType.value.get(type)?.clone() || new Set<string>();
    }

    private static _unknownUsername: string = "unknown";
    private static _username: Cached<string> = Cached.simple(ScreepsGame.fetchUsername);

    private static fetchUsername(): string
    {
        const spawns = StructureSpawn.my;

        return spawns.length == 0 ? ScreepsGame._unknownUsername : spawns[0].owner.username;
    }

    private static username(): string
    {
        const result: string = ScreepsGame._username.value;

        if (result == ScreepsGame._unknownUsername)
        {
            ScreepsGame._username.reset();
        }

        return result;
    }

    static setup()
    {
        Objects.setFunction(Game, "get", ScreepsGame.get);
        Objects.setFunction(Game, "myCreepsOfType", ScreepsGame.myCreepsOfType);
        Objects.setFunction(Game, "myCreepNamesOfType", ScreepsGame.myCreepNamesOfType);
        Objects.setGetter(Game, "username", ScreepsGame.username);
    }
}
