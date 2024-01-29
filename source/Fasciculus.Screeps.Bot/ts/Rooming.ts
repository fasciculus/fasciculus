import { Dictionary, GameWrap, Point, Vector } from "./Common";
import { profile } from "./Profiling";

export type FieldType = 0 | TERRAIN_MASK_WALL | TERRAIN_MASK_SWAMP;

export class Territory
{
    readonly terrain: RoomTerrain;

    constructor(terrain: RoomTerrain)
    {
        this.terrain = terrain;
    }

    at(p: Point): FieldType { return this.terrain.get(p.x, p.y); }
    around(p: Point): Vector<FieldType> { return p.around().map(q => this.at(q)); }
}

export class Territories
{
    private static _cache: Dictionary<Territory> = {};

    static get(room: Room): Territory
    {
        let name = room.name;
        let result: Territory | undefined = Territories._cache[name];

        if (!result)
        {
            result = Territories._cache[name] = new Territory(room.getTerrain());
        }

        return result;
    }
}

export class Chamber
{
    readonly room: Room;
    readonly name: string;
    readonly controller?: StructureController;
    readonly energyCapacityAvailable: number;

    readonly my: boolean;
    readonly controlled: boolean;

    private readonly _sources: Vector<Source>;
    private readonly _walls: Vector<StructureWall>;
    private readonly _myExtensions: Vector<StructureExtension>;

    get territory(): Territory { return Territories.get(this.room); }

    get sources(): Vector<Source> { return this._sources.clone(); }
    get walls(): Vector<StructureWall> { return this._walls.clone(); }
    get myExtensions(): Vector<StructureExtension> { return this._myExtensions.clone(); }

    constructor(room: Room)
    {
        this.room = room;
        this.name = room.name;
        this._sources = Vector.from(room.find<FIND_SOURCES, Source>(FIND_SOURCES));
        this._walls = Chamber.wallsOf(room);
        this._myExtensions = Chamber.myExtensionsOf(room);
        this.energyCapacityAvailable = room.energyCapacityAvailable;

        const controller = room.controller;

        this.controller = controller;

        if (controller)
        {
            const reservation = controller?.reservation;

            this.my = controller.my;
            this.controlled = this.my || reservation?.username == GameWrap.username;
        }
        else
        {
            this.my = false;
            this.controlled = false;
        }
    }

    static wallsOf(room: Room): Vector<StructureWall>
    {
        var structures = room.find<FIND_STRUCTURES, StructureWall>(FIND_STRUCTURES);

        if (structures.length == 0) return new Vector();

        let walls = structures.filter(s => s.structureType == STRUCTURE_WALL);

        return Vector.from(walls);
    }

    private static myExtensionsOf(room: Room): Vector<StructureExtension>
    {
        let structures: StructureExtension[] = room.find<FIND_MY_STRUCTURES, StructureExtension>(FIND_MY_STRUCTURES);

        if (structures.length == 0) return new Vector();

        let extensions: StructureExtension[] = structures.filter(s => s.structureType == STRUCTURE_EXTENSION);

        return Vector.from(extensions);
    }
}

export class Chambers
{
    private static _all: Vector<Chamber> = new Vector();
    private static _my: Vector<Chamber> = new Vector();
    private static _controlled: Vector<Chamber> = new Vector();
    private static _byName: Dictionary<Chamber> = {};

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._byName[name] : undefined; }

    static get all(): Vector<Chamber> { return Chambers._all.clone(); }
    static get my(): Vector<Chamber> { return Chambers._my.clone(); }
    static get controlled(): Vector<Chamber> { return Chambers._controlled.clone(); }

    @profile
    static initialize()
    {
        Chambers._all = GameWrap.rooms.map(r => new Chamber(r));
        Chambers._my = Chambers._all.filter(c => c.my);
        Chambers._controlled = Chambers._all.filter(c => c.controlled);
        Chambers._byName = Chambers._all.indexBy(c => c.name);
    }
}
