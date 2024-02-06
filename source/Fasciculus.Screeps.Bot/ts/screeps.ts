
import { Objects } from "./types";
import "./types.screeps";

export class Ids
{
    static get<T extends _HasId>(values: Iterable<T>): Set<Id<T>>
    {
        const result: Set<Id<T>> = Set.empty();

        for (const value of values)
        {
            result.add(value.id);
        }

        return result;
    }
}

export class Cached<V>
{
    private fetch: (value: V | undefined, key: string) => V;
    private key: string;
    private ticked: boolean;

    private _time: number;
    private _value?: V;

    constructor(fetch: (value: V | undefined, key: string) => V, ticked: boolean = true, key: string = "")
    {
        this.key = key;
        this.fetch = fetch;
        this.ticked = ticked;

        this._time = -1;
        this._value = undefined;
    }

    get value(): V
    {
        const time: number = Game.time;

        if (this._value === undefined || (this.ticked && this._time != time))
        {
            this._time = time;
            this._value = this.fetch(this._value, this.key);
        }

        return this._value;
    }

    reset()
    {
        this._time = -1;
        this._value = undefined;
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
        return Objects.keys(Game.rooms);
    }

    static myFlagNames(): Set<string>
    {
        return Objects.keys(Game.flags);
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
        return Ids.get(ScreepsGame.mySpawns());
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
        return Ids.get(ScreepsGame.mySites());
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
        return Objects.keys(Game.creeps);
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
