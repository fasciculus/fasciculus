
import { GameWrap, Vector, Vectors } from "./Common";

export class Rooms
{
    private static _all: Vector<Room> = new Vector();
    private static _my: Vector<Room> = new Vector();

    static get all(): Vector<Room> { return Rooms._all.clone(); }
    static get my(): Vector<Room> { return Rooms._my.clone(); }

    static initialize()
    {
        Rooms._all = GameWrap.rooms;
        Rooms._my = Rooms._all.filter(Rooms.isMyRoom);
    }

    private static isMyRoom(room: Room): boolean
    {
        var controller = room.controller;

        return (controller !== undefined) && controller.my;
    }

    static mySpawnsOf(room: Room): Vector<StructureSpawn>
    {
        return Vector.from(room.find<FIND_MY_SPAWNS, StructureSpawn>(FIND_MY_SPAWNS));
    }

    static get mySpawns(): Vector<StructureSpawn>
    {
        return Vectors.flatten(Rooms.my.map(Rooms.mySpawnsOf));
    }

    static sourcesOf(room: Room): Vector<Source>
    {
        return Vector.from(room.find<FIND_SOURCES, Source>(FIND_SOURCES));
    }

    static get sources(): Vector<Source>
    {
        return Vectors.flatten(Rooms._all.map(Rooms.sourcesOf));
    }

    static myExtensionsOf(room: Room): Vector<StructureExtension>
    {
        let structures: StructureExtension[] = room.find<FIND_MY_STRUCTURES, StructureExtension>(FIND_MY_STRUCTURES);

        if (structures.length == 0) return new Vector();

        let extensions: StructureExtension[] = structures.filter(s => s.structureType == STRUCTURE_EXTENSION);

        return Vector.from(extensions);
    }

    static get myExtensions(): Vector<StructureExtension>
    {
        return Vectors.flatten(Rooms._my.map(Rooms.myExtensionsOf));
    }

    static wallsOf(room: Room): Vector<StructureWall>
    {
        var structures = room.find<FIND_STRUCTURES, StructureWall>(FIND_STRUCTURES);

        if (structures.length == 0) return new Vector();

        let walls = structures.filter(s => s.structureType == STRUCTURE_WALL);

        return Vector.from(walls);
    }

    static get myWalls(): Vector<StructureWall>
    {
        return Vectors.flatten(Rooms._my.map(Rooms.wallsOf));
    }
}