import { Objects } from "../es/object";
import { Cached } from "./cache";

export class GameExt
{
    private static _unknownUsername: string = "unknown";
    private static _username: Cached<string> = Cached.simple(GameExt.getUsername);

    private static getUsername(): string
    {
        const spawns: Array<StructureSpawn> = Objects.values(Game.spawns);

        return spawns.length == 0 ? GameExt._unknownUsername : spawns[0].owner.username;
    }

    private static username(): string
    {
        return GameExt._username.value;
    }

    private static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    private static all<T extends _HasId>(ids: Set<Id<T>> | undefined): Array<T>
    {
        return ids ? Array.defined(ids.map(GameExt.get)) : new Array();
    }

    static setup()
    {
        Objects.setGetter(Game, "username", GameExt.username);
        Objects.setFunction(Game, "get", GameExt.get)
        Objects.setFunction(Game, "all", GameExt.all)
    }
}