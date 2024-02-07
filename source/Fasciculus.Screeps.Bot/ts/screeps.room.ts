import { Cached, Ids } from "./screeps.util";
import { Objects } from "./types.util";

class ScreepsFind
{
    private static obstacleTypes: Set<string> = new Set([STRUCTURE_SPAWN, STRUCTURE_WALL, STRUCTURE_EXTENSION, STRUCTURE_LINK, STRUCTURE_STORAGE,
        STRUCTURE_TOWER, STRUCTURE_OBSERVER, STRUCTURE_POWER_SPAWN, STRUCTURE_POWER_BANK, STRUCTURE_LAB, STRUCTURE_TERMINAL, STRUCTURE_NUKER,
        STRUCTURE_FACTORY, STRUCTURE_INVADER_CORE]);

    static structures<T extends AnyStructure>(room: Room | undefined, type: string): Array<T>
    {
        if (!room) return new Array();

        const opts: FilterOptions<FIND_STRUCTURES> = { filter: { structureType: type } };
        const result: Array<T> | undefined | null = room.find<T>(FIND_STRUCTURES, opts);

        return result || new Array();
    }

    static sources(room: Room | undefined): Array<Source>
    {
        return room ? room.find<FIND_SOURCES, Source>(FIND_SOURCES) : new Array<Source>();
    }

    static sourceIds(room: Room | undefined): Set<SourceId>
    {
        return Ids.get(ScreepsFind.sources(room));
    }

    static walls(room: Room | undefined): Array<StructureWall>
    {
        return ScreepsFind.structures(room, STRUCTURE_WALL);
    }
}

export class ScreepsRoom
{
    private static my(this: Room): boolean
    {
        const controller: StructureController | undefined = this.controller;

        return (controller !== undefined) && controller.my;
    }

    private static safe(this: Room): boolean
    {
        const controller: StructureController | undefined = this.controller;

        return controller ? controller.safe : true;
    }

    private static level(this: Room): number
    {
        return this.controller?.level || 0;
    }

    private static energy(this: Room): number
    {
        return this.energyAvailable || 0;
    }

    private static energyCapacity(this: Room): number
    {
        return this.energyCapacityAvailable || 0;
    }

    private static _sources: Map<string, Set<SourceId>> = new Map();

    private static ensureSourceIds(name: string): Set<SourceId>
    {
        return ScreepsFind.sourceIds(Room.get(name));
    }

    private static sourceIds(this: Room): Set<SourceId>
    {
        return ScreepsRoom._sources.ensure(this.name, ScreepsRoom.ensureSourceIds);
    }

    private static _walls: Cached<Map<string, Array<StructureWall>>> = Cached.simple(ScreepsRoom.fetchWalls);

    private static fetchWalls(): Map<string, Array<StructureWall>>
    {
        return new Map<string, Array<StructureWall>>();
    }

    private static ensureWalls(name: string): Array<StructureWall>
    {
        return ScreepsFind.walls(Room.get(name));
    }

    private static walls(this: Room): Array<StructureWall>
    {
        return ScreepsRoom._walls.value.ensure(this.name, ScreepsRoom.ensureWalls);
    }

    private static _all: Cached<Map<string, Room>> = Cached.simple(ScreepsRoom.fetchAll);

    private static fetchAll(): Map<string, Room>
    {
        return Objects.values(Game.rooms).indexBy(r => r.name);
    }

    private static all(): Array<Room>
    {
        return ScreepsRoom._all.value.vs();
    }

    private static names(): Set<string>
    {
        return ScreepsRoom._all.value.ks();
    }

    private static get(name: string): Room | undefined
    {
        return ScreepsRoom._all.value.get(name);
    }

    private static _my: Cached<Map<string, Room>> = Cached.simple(ScreepsRoom.fetchMy);

    private static fetchMy(): Map<string, Room>
    {
        return ScreepsRoom._all.value.filter((k, v) => v.my);
    }

    private static myRooms(): Array<Room>
    {
        return ScreepsRoom._my.value.vs();
    }

    private static myNames(): Set<string>
    {
        return ScreepsRoom._my.value.ks();
    }

    private static _safe: Cached<Map<string, Room>> = Cached.simple(ScreepsRoom.fetchSafe);

    private static fetchSafe(): Map<string, Room>
    {
        return ScreepsRoom._all.value.filter((k, v) => v.safe);
    }

    private static safeRooms(): Array<Room>
    {
        return ScreepsRoom._safe.value.vs();
    }

    private static safeNames(): Set<string>
    {
        return ScreepsRoom._safe.value.ks();
    }

    static setup()
    {
        Objects.setGetter(Room.prototype, "my", ScreepsRoom.my);
        Objects.setGetter(Room.prototype, "safe", ScreepsRoom.safe);
        Objects.setGetter(Room.prototype, "level", ScreepsRoom.level);
        Objects.setGetter(Room.prototype, "energy", ScreepsRoom.energy);
        Objects.setGetter(Room.prototype, "energyCapacity", ScreepsRoom.energyCapacity);
        Objects.setGetter(Room.prototype, "sourceIds", ScreepsRoom.sourceIds);
        Objects.setGetter(Room.prototype, "walls", ScreepsRoom.walls);

        Objects.setGetter(Room, "all", ScreepsRoom.all);
        Objects.setGetter(Room, "names", ScreepsRoom.names);
        Objects.setGetter(Room, "my", ScreepsRoom.myRooms);
        Objects.setGetter(Room, "myNames", ScreepsRoom.myNames);
        Objects.setGetter(Room, "safe", ScreepsRoom.safeRooms);
        Objects.setGetter(Room, "safeNames", ScreepsRoom.safeNames);
        Objects.setFunction(Room, "get", ScreepsRoom.get);
    }
}
