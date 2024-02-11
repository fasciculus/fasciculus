import { Objects } from "../es/object";
import { Cached } from "./cache";
import { Terrains } from "./terrain";

export class Finder
{
    static sources(room: Room | undefined): Array<Source>
    {
        return room ? room.find<FIND_SOURCES, Source>(FIND_SOURCES) : new Array();
    }
}

export class Rooms
{
    private static _known: Cached<Map<string, Room>> = Cached.simple(Rooms.getKnown);
    private static _safe: Cached<Map<string, Room>> = Cached.simple(Rooms.getSafe);

    private static _sources: Map<string, Set<SourceId>> = new Map();

    private static safe(this: Room): boolean
    {
        return Rooms._safe.value.has(this.name);
    }

    private static energy(this: Room): number
    {
        return this.energyAvailable || 0;
    }

    private static energyCapacity(this: Room): number
    {
        return this.energyCapacityAvailable || 0;
    }

    private static terrain(this: Room): RoomTerrain
    {
        return Terrains.ofRoom(this);
    }

    private static getSources(name: string, hint?: Room): Set<SourceId>
    {
        const room: Room | undefined = hint || Rooms.get(name);

        return Set.from(Finder.sources(room).map(s => s.id));
    }

    private static sources(this: Room): Array<Source>
    {
        return Game.all(Rooms._sources.ensure(this.name, Rooms.getSources, this));
    }

    private static getKnown(): Map<string, Room>
    {
        return Objects.values(Game.rooms).indexBy(s => s.name);
    }

    private static known(): Array<Room>
    {
        return Rooms._known.value.data;
    }

    private static getSafe(): Map<string, Room>
    {
        return Rooms._known.value.filter(Rooms.isSafe);
    }

    private static isSafe(name: string, room: Room): boolean
    {
        const controller: StructureController | undefined = room.controller;

        return controller ? controller.safe : true;
    }

    private static safeRooms(): Array<Room>
    {
        return Rooms._safe.value.data;
    }

    private static get(name: string): Room | undefined
    {
        return Rooms._known.value.get(name);
    }

    static setup()
    {
        Objects.setGetter(Room.prototype, "safe", Rooms.safe);
        Objects.setGetter(Room.prototype, "energy", Rooms.energy);
        Objects.setGetter(Room.prototype, "energyCapacity", Rooms.energyCapacity);
        Objects.setGetter(Room.prototype, "terrain", Rooms.terrain);
        Objects.setGetter(Room.prototype, "sources", Rooms.sources);

        Objects.setFunction(Room, "get", Rooms.get);
        Objects.setGetter(Room, "known", Rooms.known);
        Objects.setGetter(Room, "safe", Rooms.safeRooms);
    }
}