import { Cached, Ids } from "./screeps.util";
import { Objects } from "./types";

export class ScreepsGame
{
    static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    private static _knownRooms: Cached<Map<string, Room>> = new Cached(ScreepsGame.fetchKnownRooms);

    private static fetchKnownRooms(): Map<string, Room>
    {
        const result: Map<string, Room> = new Map<string, Room>();

        Objects.keys(Game.rooms).forEach(k => result.set(k, Game.rooms[k]));

        return result;
    }

    static knownRooms(): Array<Room>
    {
        return ScreepsGame._knownRooms.value.vs();
    }

    static knownRoomNames(): Set<string>
    {
        return ScreepsGame._knownRooms.value.ks();
    }

    static knownRoom(roomName: string): Room | undefined
    {
        return ScreepsGame._knownRooms.value.get(roomName);
    }

    private static _myFlagNames: Cached<Set<string>> = new Cached(ScreepsGame.fetchMyFlagNames);

    private static fetchMyFlagNames(): Set<string>
    {
        return Objects.keys(Game.flags);
    }

    static myFlagNames(): Set<string>
    {
        return ScreepsGame._myFlagNames.value;
    }

    private static _mySpawns: Cached<Array<StructureSpawn>> = new Cached(ScreepsGame.fetchMySpawns);

    private static fetchMySpawns(): Array<StructureSpawn>
    {
        return Objects.values(Game.spawns);
    }

    static mySpawns(): Array<StructureSpawn>
    {
        return ScreepsGame._mySpawns.value;
    }

    private static _mySpawnIds: Cached<Set<SpawnId>> = new Cached(ScreepsGame.fetchMySpawnIds);

    private static fetchMySpawnIds(): Set<SpawnId>
    {
        return Ids.get(ScreepsGame.mySpawns());
    }

    static mySpawnIds(): Set<SpawnId>
    {
        return ScreepsGame._mySpawnIds.value;
    }

    private static _mySites: Cached<Array<ConstructionSite>> = new Cached(ScreepsGame.fetchMySites);

    private static fetchMySites(): Array<ConstructionSite>
    {
        return Objects.values(Game.constructionSites);
    }

    static mySites(): Array<ConstructionSite>
    {
        return ScreepsGame._mySites.value;
    }

    private static _mySiteIds: Cached<Set<SiteId>> = new Cached(ScreepsGame.fetchMySiteIds);

    private static fetchMySiteIds(): Set<SiteId>
    {
        return Ids.get(ScreepsGame.mySites());
    }

    static mySiteIds(): Set<SiteId>
    {
        return ScreepsGame._mySiteIds.value;
    }

    static myCreep(name: string | undefined): Creep | undefined
    {
        if (!name || !Game.creeps) return undefined;

        return Game.creeps[name] || undefined;
    }

    static myCreeps(): Array<Creep>
    {
        return Objects.values(Game.creeps);;
    }

    static myCreepNames(): Set<string>
    {
        return Objects.keys(Game.creeps);
    }

    private static _unknownUsername: string = "unknown";
    private static _username: Cached<string> = new Cached(ScreepsGame.fetchUsername, false);

    private static fetchUsername(): string
    {
        const spawns = ScreepsGame.mySpawns();

        if (spawns.length == 0) return ScreepsGame._unknownUsername;

        return spawns[0].owner.username;
    }

    static username(): string
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
        Objects.setGetter(Game, "knownRooms", ScreepsGame.knownRooms);
        Objects.setGetter(Game, "knownRoomNames", ScreepsGame.knownRoomNames);
        Objects.setFunction(Game, "knownRoom", ScreepsGame.knownRoom);
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
