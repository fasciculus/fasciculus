import { ControllerId, Dictionaries, Dictionary, GameWrap, Point, Sets, SourceId, Vector, Vectors, WallId } from "./Common";
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
    readonly name: string;
    readonly sources: Set<SourceId>;

    get room(): Room { return Game.rooms[this.name]; }
    get controller(): StructureController | undefined { return this.room.controller; }
    get my(): boolean { return this.controller?.my || false; }
    get reservation(): ReservationDefinition | undefined { return this.controller?.reservation; }
    get controlled(): boolean { return this.my || this.reservation?.username == GameWrap.username; }
    get rcl(): number { return this.controller?.level || 0; }

    get energyAvailable(): number { return this.room.energyAvailable || 0; }
    get energyCapacityAvailable(): number { return this.room.energyCapacityAvailable || 0; }

    get territory(): Territory { return Territories.get(this.room); }

    constructor(name: string)
    {
        this.name = name;
        this.sources = Vector.from(Game.rooms[name].find<FIND_SOURCES, Source>(FIND_SOURCES)).map(s => s.id).toSet();
    }

    get walls(): Set<WallId>
    {
        const structures = Vector.from(this.room.find<FIND_STRUCTURES, StructureWall>(FIND_STRUCTURES));

        return structures.filter(s => s.structureType == STRUCTURE_WALL).map(w => w.id).toSet();
    }
}

export class Chambers
{
    private static _allChambers: Dictionary<Chamber> = {};
    private static _allControllers: Set<ControllerId> = new Set();
    private static _allSources: Set<SourceId> = new Set();

    private static _myWalls: Set<WallId> = new Set();

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._allChambers[name] : undefined; }

    static get all(): Vector<Chamber> { return Dictionaries.values(Chambers._allChambers); }
    static get my(): Vector<Chamber> { return Chambers.all.filter(c => c.my); }

    static get allControllers(): Set<ControllerId> { return Sets.clone(Chambers._allControllers); }
    static get allSources(): Set<SourceId> { return Sets.clone(Chambers._allSources); }

    static get myWalls(): Set<WallId> { return Sets.clone(Chambers._myWalls); }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Chambers._allChambers = {};
            Chambers._allControllers = new Set();
            Chambers._allSources = new Set();
        }

        const existing: Set<string> = GameWrap.rooms.map(r => r.name).toSet();

        if (Dictionaries.update(Chambers._allChambers, existing, name => new Chamber(name)))
        {
            // todo: modify only changed values
            Chambers._allControllers = Vectors.defined(Chambers.all.map(c => c.controller)).map(c => c.id).toSet();
            Chambers._allSources = Sets.unionAll(Chambers.all.map(c => c.sources));
        }
    }

    @profile
    static updateStructures()
    {
        Chambers._myWalls = Sets.unionAll(Chambers.my.map(c => c.walls));
    }
}
