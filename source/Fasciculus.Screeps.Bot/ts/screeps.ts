
import "./types.screeps";

import { ScreepsGame } from "./screeps.game";
import { Cached, Ids } from "./screeps.util";
import { Objects } from "./types";

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

class RoomFinder
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

class RoomData
{
    private _sourceIds: Cached<Set<SourceId>>;

    get sourceIds(): Set<SourceId> { return this._sourceIds.value.clone(); }

    constructor(name: string)
    {
        this._sourceIds = new Cached<Set<SourceId>>(RoomData.fetchSourceIds, false, name);
    }

    private static fetchSourceIds(value: Set<SourceId> | undefined, name: string): Set<SourceId>
    {
        return RoomFinder.sources(Game.knownRoom(name))
    }
}

class ScreepsRoom
{
    private static _data: Map<string, RoomData> = new Map();

    private static createData(roomName: string): RoomData
    {
        return new RoomData(roomName);
    }

    private static getData(roomName: string): RoomData
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

    static cleanup(): void
    {
        Cached.cleanup();
    }
}
