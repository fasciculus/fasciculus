import { Cached, Ids } from "./screeps.util";
import { Objects } from "./types.util";

class Finder
{
    private static obstacleTypes: Set<string> = new Set([STRUCTURE_SPAWN, STRUCTURE_WALL, STRUCTURE_EXTENSION, STRUCTURE_LINK, STRUCTURE_STORAGE,
        STRUCTURE_TOWER, STRUCTURE_OBSERVER, STRUCTURE_POWER_SPAWN, STRUCTURE_POWER_BANK, STRUCTURE_LAB, STRUCTURE_TERMINAL, STRUCTURE_NUKER,
        STRUCTURE_FACTORY, STRUCTURE_INVADER_CORE]);

    static allStructuresOfType<T extends AnyStructure>(room: Room | undefined, type: string): Array<T>
    {
        if (!room) return new Array();

        const opts: FilterOptions<FIND_STRUCTURES> = { filter: { structureType: type } };
        const result: Array<T> | undefined | null = room.find<T>(FIND_STRUCTURES, opts);

        return result || new Array();
    }

    static allStructuresOfTypes(room: Room | undefined, types: Set<string>): Array<AnyStructure>
    {
        if (!room) return new Array();

        return room.find<AnyStructure>(FIND_STRUCTURES).filter(s => types.has(s.structureType));
    }

    static sources(room: Room | undefined): Array<Source>
    {
        return room ? room.find<FIND_SOURCES, Source>(FIND_SOURCES) : new Array<Source>();
    }

    static sourceIds(room: Room | undefined): Set<SourceId>
    {
        return Ids.get(Finder.sources(room));
    }

    static roads(room: Room | undefined): Array<StructureRoad>
    {
        return Finder.allStructuresOfType(room, STRUCTURE_ROAD);
    }

    static walls(room: Room | undefined): Array<StructureWall>
    {
        return Finder.allStructuresOfType(room, STRUCTURE_WALL);
    }

    static obstacles(room: Room | undefined): Array<AnyStructure>
    {
        return Finder.allStructuresOfTypes(room, Finder.obstacleTypes);
    }
}

export class Rooms
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

    private static createSourceIds(name: string): Set<SourceId>
    {
        return Finder.sourceIds(Room.get(name));
    }

    private static sourceIds(this: Room): Set<SourceId>
    {
        return Rooms._sources.ensure(this.name, Rooms.createSourceIds).clone();
    }

    private static _walls: Cached<Map<string, Array<StructureWall>>> = Cached.simple(Rooms.fetchWalls);

    private static fetchWalls(): Map<string, Array<StructureWall>>
    {
        return new Map<string, Array<StructureWall>>();
    }

    private static createWalls(name: string): Array<StructureWall>
    {
        return Finder.walls(Room.get(name));
    }

    private static walls(this: Room): Array<StructureWall>
    {
        return Rooms._walls.value.ensure(this.name, Rooms.createWalls).clone();
    }

    private static _costMatrices: Cached<Map<string, CostMatrix>> = Cached.simple(Rooms.fetchCostMatrix);

    private static fetchCostMatrix(): Map<string, CostMatrix>
    {
        return new Map<string, CostMatrix>();
    }

    private static createCostMatrix(name: string): CostMatrix
    {
        const result: CostMatrix = new PathFinder.CostMatrix();
        const room: Room | undefined = Room.get(name);

        if (room)
        {
            const obstacles: Array<RoomPosition> = Finder.obstacles(Room.get(name)).map(o => o.pos);
            const roads: Array<RoomPosition> = Finder.roads(room).map(o => o.pos);

            obstacles.forEach(p => result.set(p.x, p.y, 255));
            roads.forEach(p => result.set(p.x, p.y, 1));
        }

        return result;
    }

    private static costMatrix(this: Room): CostMatrix
    {
        return Rooms._costMatrices.value.ensure(this.name, Rooms.createCostMatrix).clone();
    }

    private static _all: Cached<Map<string, Room>> = Cached.simple(Rooms.fetchAll);

    private static fetchAll(): Map<string, Room>
    {
        return Objects.values(Game.rooms).indexBy(r => r.name);
    }

    private static all(): Array<Room>
    {
        return Rooms._all.value.vs();
    }

    private static names(): Set<string>
    {
        return Rooms._all.value.ks();
    }

    private static get(name: string): Room | undefined
    {
        return Rooms._all.value.get(name);
    }

    private static _my: Cached<Map<string, Room>> = Cached.simple(Rooms.fetchMy);

    private static fetchMy(): Map<string, Room>
    {
        return Rooms._all.value.filter((k, v) => v.my);
    }

    private static myRooms(): Array<Room>
    {
        return Rooms._my.value.vs();
    }

    private static myNames(): Set<string>
    {
        return Rooms._my.value.ks();
    }

    private static _safe: Cached<Map<string, Room>> = Cached.simple(Rooms.fetchSafe);

    private static fetchSafe(): Map<string, Room>
    {
        return Rooms._all.value.filter((k, v) => v.safe);
    }

    private static safeRooms(): Array<Room>
    {
        return Rooms._safe.value.vs();
    }

    private static safeNames(): Set<string>
    {
        return Rooms._safe.value.ks();
    }

    static setup()
    {
        Objects.setGetter(Room.prototype, "my", Rooms.my);
        Objects.setGetter(Room.prototype, "safe", Rooms.safe);
        Objects.setGetter(Room.prototype, "level", Rooms.level);
        Objects.setGetter(Room.prototype, "energy", Rooms.energy);
        Objects.setGetter(Room.prototype, "energyCapacity", Rooms.energyCapacity);
        Objects.setGetter(Room.prototype, "sourceIds", Rooms.sourceIds);
        Objects.setGetter(Room.prototype, "walls", Rooms.walls);
        Objects.setGetter(Room.prototype, "costMatrix", Rooms.costMatrix);

        Objects.setGetter(Room, "all", Rooms.all);
        Objects.setGetter(Room, "names", Rooms.names);
        Objects.setGetter(Room, "my", Rooms.myRooms);
        Objects.setGetter(Room, "myNames", Rooms.myNames);
        Objects.setGetter(Room, "safe", Rooms.safeRooms);
        Objects.setGetter(Room, "safeNames", Rooms.safeNames);
        Objects.setFunction(Room, "get", Rooms.get);
    }
}
