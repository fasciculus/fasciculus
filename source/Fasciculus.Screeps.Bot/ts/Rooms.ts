
import { GameWrap, Vector, Vectors } from "./Common";

export class Rooms
{
    private static _all: Vector<Room> = new Vector();
    private static _my: Vector<Room> = new Vector();

    private static _sources: Vector<Source> = new Vector();
    private static _myExtensions: Vector<StructureExtension> = new Vector();
    private static _myWalls: Vector<StructureWall> = new Vector();

    static get all(): Vector<Room> { return Rooms._all.clone(); }
    static get my(): Vector<Room> { return Rooms._my.clone(); }

    static get sources(): Vector<Source> { return Rooms._sources.clone(); }
    static get myExtensions(): Vector<StructureExtension> { return Rooms._myExtensions.clone(); }
    static get myWalls(): Vector<StructureWall> { return Rooms._myWalls.clone(); }

    static initialize()
    {
        Rooms._all = GameWrap.rooms;
        Rooms._my = Rooms._all.filter(Rooms.isMyRoom);
        Rooms._sources = Vectors.flatten(Rooms._all.map(Rooms.sourcesOf));
        Rooms._myExtensions = Vectors.flatten(Rooms._my.map(Rooms.myExtensionsOf));
        Rooms._myWalls = Vectors.flatten(Rooms._my.map(Rooms.wallsOf));
    }

    private static isMyRoom(room: Room): boolean
    {
        var controller = room.controller;

        return (controller !== undefined) && controller.my;
    }

    private static sourcesOf(room: Room): Vector<Source>
    {
        return Vector.from(room.find<FIND_SOURCES, Source>(FIND_SOURCES));
    }

    static myExtensionsOf(room: Room): Vector<StructureExtension>
    {
        let structures: StructureExtension[] = room.find<FIND_MY_STRUCTURES, StructureExtension>(FIND_MY_STRUCTURES);

        if (structures.length == 0) return new Vector();

        let extensions: StructureExtension[] = structures.filter(s => s.structureType == STRUCTURE_EXTENSION);

        return Vector.from(extensions);
    }

    static wallsOf(room: Room): Vector<StructureWall>
    {
        var structures = room.find<FIND_STRUCTURES, StructureWall>(FIND_STRUCTURES);

        if (structures.length == 0) return new Vector();

        let walls = structures.filter(s => s.structureType == STRUCTURE_WALL);

        return Vector.from(walls);
    }
}