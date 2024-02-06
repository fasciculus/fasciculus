import { Cached, Ids } from "./screeps.util";
import { Objects } from "./types";

class ScreepsFinder
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

    private static rcl(this: Room): number
    {
        return this.controller?.level || 0;
    }

    private static ensureSourceIds(roomName: string): Set<SourceId>
    {
        return ScreepsFinder.sourceIds(Room.get(roomName));
    }

    private static sourceIds(this: Room): Set<SourceId>
    {
        return ScreepsRoom._sources.ensure(this.name, ScreepsRoom.ensureSourceIds);
    }

    private static _all: Cached<Map<string, Room>> = Cached.simple(ScreepsRoom.fetchAll);

    private static fetchAll(): Map<string, Room>
    {
        const result: Map<string, Room> = new Map<string, Room>();

        Objects.keys(Game.rooms).forEach(k => result.set(k, Game.rooms[k]));

        return result;
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

    static setup()
    {
        Objects.setGetter(Room.prototype, "rcl", ScreepsRoom.rcl);
        Objects.setGetter(Room.prototype, "sourceIds", ScreepsRoom.sourceIds);

        Objects.setGetter(Room, "all", ScreepsRoom.all);
        Objects.setGetter(Room, "names", ScreepsRoom.names);
        Objects.setFunction(Room, "get", ScreepsRoom.get);
    }
}
