import { Objects } from "./types.common";

declare global
{
    type SpawnId = Id<StructureSpawn>;

    interface Game
    {
        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;

        get mySpawns(): Set<SpawnId>;

        get username(): string;
    }

    interface Memory
    {
        [index: string]: any;

        get<T>(key: string, initial: T): T;
        sub<T>(root: string, key: string, initial: T): T;
    }
}

export class Screeps
{
    private static _username?: string = undefined;

    static setup()
    {
        Objects.setFunction(Game, "get", function <T extends _HasId>(id: Id<T> | undefined): T | undefined
        {
            let result: T | null = id ? Game.getObjectById(id) : null;

            return result || undefined;
        });

        Objects.setGetter(Game, "mySpawns", function (): Set<SpawnId>
        {
            return Object.values(Game.spawns).map(s => s.id).toSet();
        });

        Objects.setGetter(Game, "username", function (): string
        {
            if (!Screeps._username)
            {
                const spawns = Game.mySpawns.toArray();
                const spawn = spawns.length > 0 ? Game.get(spawns[0]) : undefined;

                Screeps._username = spawn?.owner.username || "unknown";
            }

            return Screeps._username;
        });

        Objects.setFunction(Memory, "get", function <T>(key: string, initial: T): T
        {
            var result: any | undefined = Memory[key];

            if (!result)
            {
                Memory[key] = result = initial;
            }

            return result as T;
        });

        Objects.setFunction(Memory, "sub", function <T>(root: string, key: string, initial: T): T
        {
            const parent: { [index: string]: T } = Memory.get(root, {});
            var result: T | undefined = parent[key];

            if (!result)
            {
                parent[key] = result = initial;
            }

            return result as T;
        });
    }
}

export { };
