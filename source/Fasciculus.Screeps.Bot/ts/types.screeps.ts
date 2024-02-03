import { Objects } from "./types.common";

declare global
{
    type SpawnId = Id<StructureSpawn>;

    interface Game
    {
        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;

        get knownRooms(): Array<Room>;
        get knownRoomNames(): Set<string>;

        get myFlagNames(): Set<string>;

        get mySpawns(): Array<StructureSpawn>;
        get mySpawnIds(): Set<SpawnId>;

        myCreep(name: string | undefined): Creep | undefined;
        get myCreeps(): Array<Creep>;

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

        Objects.setGetter(Game, "knownRooms", function (): Array<Room>
        {
            if (!Game.rooms) return new Array();

            const rooms = Object.values(Game.rooms);

            if (!rooms || !Array.isArray(rooms) || rooms.length == 0) return new Array();

            return rooms;
        });

        Objects.setGetter(Game, "knownRoomNames", function (): Set<string>
        {
            if (!Game.rooms) return new Set();

            const names = Object.keys(Game.rooms);

            if (!names || !Array.isArray(names) || names.length == 0) return new Set();

            return names.toSet();
        });

        Objects.setGetter(Game, "myFlagNames", function (): Set<string>
        {
            if (!Game.flags) return new Set();

            const names = Object.keys(Game.flags);

            if (!names || !Array.isArray(names) || names.length == 0) return new Set();

            return names.toSet();
        });

        Objects.setGetter(Game, "mySpawns", function (): Array<StructureSpawn>
        {
            if (!Game.spawns) return new Array();

            const spawns = Object.values(Game.spawns);

            if (!spawns || !Array.isArray(spawns) || spawns.length == 0) return new Array();

            return spawns;
        });

        Objects.setGetter(Game, "mySpawnIds", function (): Set<SpawnId>
        {
            const spawns = Game.mySpawns;

            if (spawns.length == 0) return new Set();

            return spawns.map(s => s.id).toSet();
        });

        Objects.setFunction(Game, "myCreep", function (name: string | undefined): Creep | undefined
        {
            if (!name || !Game.creeps) return undefined;

            return Game.creeps[name] || undefined;
        });

        Objects.setGetter(Game, "myCreeps", function (): Array<Creep>
        {
            if (!Game.creeps) return new Array();

            const creeps = Object.values(Game.creeps);

            if (!creeps || !Array.isArray(creeps) || creeps.length == 0) return new Array();

            return creeps;
        });

        Objects.setGetter(Game, "username", function (): string
        {
            if (!Screeps._username)
            {
                const spawns = Game.mySpawns;

                if (spawns.length == 0) return "unknown";

                Screeps._username = spawns[0].owner.username;
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
