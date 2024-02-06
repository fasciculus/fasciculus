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

class ScreepsRoomData
{
    constructor(name: string)
    {
    }
}

export class ScreepsRoom
{
    private static _data: Map<string, ScreepsRoomData> = new Map();
    private static _sources: Map<string, Set<SourceId>> = new Map();

    private static createData(roomName: string): ScreepsRoomData
    {
        return new ScreepsRoomData(roomName);
    }

    private static getData(roomName: string): ScreepsRoomData
    {
        return ScreepsRoom._data.ensure(roomName, ScreepsRoom.createData);
    }

    static rcl(this: Room): number
    {
        return this.controller?.level || 0;
    }

    private static ensureSourceIds(roomName: string): Set<SourceId>
    {
        return ScreepsFinder.sourceIds(Game.knownRoom(roomName));
    }

    static sourceIds(this: Room): Set<SourceId>
    {
        return ScreepsRoom._sources.ensure(this.name, ScreepsRoom.ensureSourceIds);
    }

    static setup()
    {
        Objects.setGetter(Room.prototype, "rcl", ScreepsRoom.rcl);
        Objects.setGetter(Room.prototype, "sourceIds", ScreepsRoom.sourceIds);
    }
}
