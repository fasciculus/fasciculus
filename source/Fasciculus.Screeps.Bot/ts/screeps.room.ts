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

    static sources(room: Room | undefined): Set<SourceId>
    {
        return room ? Ids.get(room.find<FIND_SOURCES, Source>(FIND_SOURCES)) : new Set<SourceId>();
    }
}

class ScreepsRoomData
{
    private _sourceIds: Cached<Set<SourceId>>;

    get sourceIds(): Set<SourceId> { return this._sourceIds.value.clone(); }

    constructor(name: string)
    {
        this._sourceIds = Cached.withKey(ScreepsRoomData.fetchSourceIds, name, false);
    }

    private static fetchSourceIds(name: string): Set<SourceId>
    {
        return ScreepsFinder.sources(Game.knownRoom(name))
    }
}

export class ScreepsRoom
{
    private static _data: Map<string, ScreepsRoomData> = new Map();

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

    static sourceIds(this: Room): Set<SourceId>
    {
        return ScreepsRoom.getData(this.name).sourceIds;
    }

    static setup()
    {
        Objects.setGetter(Room.prototype, "rcl", ScreepsRoom.rcl);
        Objects.setGetter(Room.prototype, "sourceIds", ScreepsRoom.sourceIds);
    }
}
