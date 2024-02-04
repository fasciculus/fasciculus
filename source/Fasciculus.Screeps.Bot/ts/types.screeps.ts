import { Objects } from "./types.common";

declare global
{
    type ControllerId = Id<StructureController>;
    type SpawnId = Id<StructureSpawn>;
    type SiteId = Id<ConstructionSite>;
}

declare global
{
    interface Game
    {
        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;

        get knownRooms(): Array<Room>;
        get knownRoomNames(): Set<string>;

        get myFlagNames(): Set<string>;

        get mySpawns(): Array<StructureSpawn>;
        get mySpawnIds(): Set<SpawnId>;

        get mySites(): Array<ConstructionSite>;
        get mySiteIds(): Set<SiteId>;

        myCreep(name: string | undefined): Creep | undefined;
        get myCreeps(): Array<Creep>;
        get myCreepNames(): Set<string>;

        get username(): string;
    }
}

class ScreepsGame
{
    static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    static knownRooms(): Array<Room>
    {
        if (!Game.rooms) return new Array();

        const rooms = Object.values(Game.rooms);

        if (!rooms || !Array.isArray(rooms) || rooms.length == 0) return new Array();

        return rooms;
    }

    static knownRoomNames(): Set<string>
    {
        if (!Game.rooms) return new Set();

        const names = Object.keys(Game.rooms);

        if (!names || !Array.isArray(names) || names.length == 0) return new Set();

        return names.toSet();
    }

    static myFlagNames(): Set<string>
    {
        if (!Game.flags) return new Set();

        const names = Object.keys(Game.flags);

        if (!names || !Array.isArray(names) || names.length == 0) return new Set();

        return names.toSet();
    }

    static mySpawns(): Array<StructureSpawn>
    {
        if (!Game.spawns) return new Array();

        const spawns = Object.values(Game.spawns);

        if (!spawns || !Array.isArray(spawns) || spawns.length == 0) return new Array();

        return spawns;
    }

    static mySpawnIds(): Set<SpawnId>
    {
        const spawns = ScreepsGame.mySpawns();

        if (spawns.length == 0) return new Set();

        return spawns.map(s => s.id).toSet();
    }

    static mySites(): Array<ConstructionSite>
    {
        if (!Game.constructionSites) return new Array();

        const sites = Object.values(Game.constructionSites);

        if (!sites || !Array.isArray(sites) || sites.length == 0) return new Array();

        return sites;
    }

    static mySiteIds(): Set<SiteId>
    {
        const sites = ScreepsGame.mySites();

        if (sites.length == 0) return new Set();

        return sites.map(s => s.id).toSet();
    }

    static myCreep(name: string | undefined): Creep | undefined
    {
        if (!name || !Game.creeps) return undefined;

        return Game.creeps[name] || undefined;
    }

    static myCreeps(): Array<Creep>
    {
        if (!Game.creeps) return new Array();

        const creeps = Object.values(Game.creeps);

        if (!creeps || !Array.isArray(creeps) || creeps.length == 0) return new Array();

        return creeps;
    }

    static myCreepNames(): Set<string>
    {
        if (!Game.creeps) return new Set();

        const names = Object.keys(Game.creeps);

        if (!names || !Array.isArray(names) || names.length == 0) return new Set();

        return Set.from(names);
    }

    private static _username?: string = undefined;

    static username(): string
    {
        if (!ScreepsGame._username)
        {
            const spawns = ScreepsGame.mySpawns();

            if (spawns.length == 0) return "unknown";

            ScreepsGame._username = spawns[0].owner.username;
        }

        return ScreepsGame._username;
    }

    static setup()
    {
        Objects.setFunction(Game, "get", ScreepsGame.get);
        Objects.setGetter(Game, "knownRooms", ScreepsGame.knownRooms);
        Objects.setGetter(Game, "knownRoomNames", ScreepsGame.knownRoomNames);
        Objects.setGetter(Game, "myFlagNames", ScreepsGame.myFlagNames);
        Objects.setGetter(Game, "mySpawns", ScreepsGame.mySpawns);
        Objects.setGetter(Game, "mySpawnIds", ScreepsGame.mySpawnIds);
        Objects.setGetter(Game, "mySites", ScreepsGame.mySites);
        Objects.setGetter(Game, "mySiteIds", ScreepsGame.mySiteIds);
        Objects.setFunction(Game, "myCreep", ScreepsGame.myCreep);
        Objects.setGetter(Game, "myCreeps", ScreepsGame.myCreeps);
        Objects.setGetter(Game, "myCreepNames", ScreepsGame.myCreepNames);
        Objects.setGetter(Game, "username", ScreepsGame.username);
    }
}

declare global
{
    interface Memory
    {
        [index: string]: any;

        get<T>(key: string, initial: T): T;
        sub<T>(root: string, key: string, initial: T): T;
    }
}

class ScreepsMemory
{
    static get<T>(key: string, initial: T): T
    {
        var result: any | undefined = Memory[key];

        if (!result)
        {
            Memory[key] = result = initial;
        }

        return result as T;
    }

    static sub<T>(root: string, key: string, initial: T): T
    {
        const parent: { [index: string]: T } = ScreepsMemory.get(root, {});
        var result: T | undefined = parent[key];

        if (!result)
        {
            parent[key] = result = initial;
        }

        return result as T;
    }

    static setup()
    {
        Objects.setFunction(Memory, "get", ScreepsMemory.get);
        Objects.setFunction(Memory, "sub", ScreepsMemory.sub);
    }
}

declare global
{
    interface Room
    {
        get rcl(): number;
    }
}

class ScreepsRoom
{
    static rcl(this: Room): number
    {
        return this.controller?.level || 0;
    }

    static setup()
    {
        Objects.setGetter(Room.prototype, "rcl", ScreepsRoom.rcl);
    }
}

declare global
{
    interface StructureSpawn
    {
        get rcl(): number;
    }
}

class ScreepsSpawn
{
    static rcl(this: StructureSpawn): number
    {
        return this.room.rcl;
    }

    static setup()
    {
        Objects.setGetter(StructureSpawn.prototype, "rcl", ScreepsSpawn.rcl);
    }
}

export class Screeps
{
    static setup()
    {
        ScreepsGame.setup();
        ScreepsMemory.setup();
        ScreepsRoom.setup();
        ScreepsSpawn.setup();
    }
}

export { };
