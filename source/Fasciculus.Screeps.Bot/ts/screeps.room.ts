import { Cached, Ids } from "./screeps.util";
import { Objects } from "./types";

class ScreepsFind
{
    private static obstacleTypes: Set<string> = new Set([STRUCTURE_SPAWN, STRUCTURE_WALL, STRUCTURE_EXTENSION, STRUCTURE_LINK, STRUCTURE_STORAGE,
        STRUCTURE_TOWER, STRUCTURE_OBSERVER, STRUCTURE_POWER_SPAWN, STRUCTURE_POWER_BANK, STRUCTURE_LAB, STRUCTURE_TERMINAL, STRUCTURE_NUKER,
        STRUCTURE_FACTORY, STRUCTURE_INVADER_CORE]);

    static structures<T extends AnyStructure>(room: Room, type: string): Array<T>
    {
        const opts: FilterOptions<FIND_STRUCTURES> = { filter: { structureType: type } };
        const result: Array<T> | undefined | null = room.find<T>(FIND_STRUCTURES, opts);

        return result || new Array();
    }

    static sourceIds(room: Room | undefined): Set<SourceId>
    {
        return room ? Ids.get(room.find<FIND_SOURCES, Source>(FIND_SOURCES)) : new Set<SourceId>();
    }
}

export class ScreepsRoom
{
    private static _sources: Map<string, Set<SourceId>> = new Map();

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

    private static ensureSourceIds(roomName: string): Set<SourceId>
    {
        return ScreepsFind.sourceIds(Room.get(roomName));
    }

    private static sourceIds(this: Room): Set<SourceId>
    {
        return ScreepsRoom._sources.ensure(this.name, ScreepsRoom.ensureSourceIds);
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
        Objects.setGetter(Room.prototype, "safe", ScreepsRoom.safe);
        Objects.setGetter(Room.prototype, "level", ScreepsRoom.level);
        Objects.setGetter(Room.prototype, "energy", ScreepsRoom.energy);
        Objects.setGetter(Room.prototype, "energyCapacity", ScreepsRoom.energyCapacity);
        Objects.setGetter(Room.prototype, "sourceIds", ScreepsRoom.sourceIds);

        Objects.setGetter(Room, "all", ScreepsRoom.all);
        Objects.setGetter(Room, "names", ScreepsRoom.names);
        Objects.setGetter(Room, "safe", ScreepsRoom.safeRooms);
        Objects.setGetter(Room, "safeNames", ScreepsRoom.safeNames);
        Objects.setFunction(Room, "get", ScreepsRoom.get);
    }
}
