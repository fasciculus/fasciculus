import { Cached } from "./screeps.util";
import { Objects } from "./types.util";

export class ScreepsGame
{
    private static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    private static all<T extends _HasId>(ids: Set<Id<T>>): Array<T>
    {
        return Array.defined(ids.map(ScreepsGame.get));
    }

    private static existing<T extends _HasId>(ids: Set<Id<T>>): Set<Id<T>>
    {
        return ids.filter(id => ScreepsGame.get(id) !== undefined);
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
        Objects.setFunction(Game, "all", ScreepsGame.all);
        Objects.setFunction(Game, "existing", ScreepsGame.existing);
        Objects.setGetter(Game, "username", ScreepsGame.username);
    }
}
