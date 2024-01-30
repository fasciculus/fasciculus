import { Dictionaries, Dictionary, GameWrap, Point, Vector } from "./Common";
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

    get room(): Room { return Game.rooms[this.name]; }
    get controller(): StructureController | undefined { return this.room.controller; }
    get my(): boolean { return this.controller?.my || false; }
    get reservation(): ReservationDefinition | undefined { return this.controller?.reservation; }
    get controlled(): boolean { return this.my || this.reservation?.username == GameWrap.username; }

    get energyCapacityAvailable(): number { return this.room.energyCapacityAvailable; }

    get territory(): Territory { return Territories.get(this.room); }

    get walls(): Vector<StructureWall> { return Chamber.wallsOf(this.room); }

    constructor(name: string)
    {
        this.name = name;
    }

    static wallsOf(room: Room): Vector<StructureWall>
    {
        var structures = room.find<FIND_STRUCTURES, StructureWall>(FIND_STRUCTURES);

        if (structures.length == 0) return new Vector();

        let walls = structures.filter(s => s.structureType == STRUCTURE_WALL);

        return Vector.from(walls);
    }
}

export class Chambers
{
    private static _chambers: Dictionary<Chamber> = {};

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._chambers[name] : undefined; }

    static get all(): Vector<Chamber> { return Dictionaries.values(Chambers._chambers); }
    static get my(): Vector<Chamber> { return Chambers.all.filter(c => c.my); }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Chambers._chambers = {};
        }

        const existing: Set<string> = GameWrap.rooms.map(r => r.name).toSet();

        Dictionaries.update(Chambers._chambers, existing, name => new Chamber(name));
    }
}
