import { Dictionaries, Dictionary, Vector, Vectors } from "./Common";
import { profile } from "./Profiling";

export class Finder
{
    private static obstacleTypes: Set<string> = new Set([STRUCTURE_SPAWN, STRUCTURE_WALL, STRUCTURE_EXTENSION, STRUCTURE_LINK, STRUCTURE_STORAGE,
        STRUCTURE_TOWER, STRUCTURE_OBSERVER, STRUCTURE_POWER_SPAWN, STRUCTURE_POWER_BANK, STRUCTURE_LAB, STRUCTURE_TERMINAL, STRUCTURE_NUKER,
        STRUCTURE_FACTORY, STRUCTURE_INVADER_CORE]);

    static structures<T extends AnyStructure>(room: Room, type: string): Vector<T>
    {
        const opts: FilterOptions<FIND_STRUCTURES> = { filter: { structureType: type } };

        return Vector.from(room.find<T>(FIND_STRUCTURES, opts));
    }

    static allStructures(room: Room, types: Set<string>): Vector<AnyStructure>
    {
        return Vector.from(room.find<AnyStructure>(FIND_STRUCTURES)).filter(s => types.has(s.structureType));
    }

    private static ids<T extends _HasId>(values: Vector<T>): Set<Id<T>>
    {
        return values.map(v => v.id).toSet();
    }

    static walls(room: Room) { return Finder.structures<StructureWall>(room, STRUCTURE_WALL); }
    static wallIds(room: Room): Set<WallId> { return Finder.ids(Finder.walls(room)); }

    static obstacles(room: Room):  Vector<AnyStructure>
    {
        return Finder.allStructures(room, Finder.obstacleTypes);
    }
}

export class Chamber
{
    readonly name: string;
    readonly sources: Set<SourceId>;

    get room(): Room { return Game.rooms[this.name]; }
    get controller(): StructureController | undefined { return this.room.controller; }
    get my(): boolean { return this.controller?.my || false; }

    get energyAvailable(): number { return this.room.energyAvailable || 0; }
    get energyCapacityAvailable(): number { return this.room.energyCapacityAvailable || 0; }

    constructor(name: string)
    {
        this.name = name;
        this.sources = Vector.from(Game.rooms[name].find<FIND_SOURCES, Source>(FIND_SOURCES)).map(s => s.id).toSet();
    }

    reset()
    {
        this._costMatrix = undefined;
        this._walls = undefined;
    }

    private _costMatrix?: CostMatrix = undefined;
    private _walls?: Set<WallId> = undefined;

    get costMatrix(): CostMatrix { return this._costMatrix || (this._costMatrix = this.createCostMatrix()); }
    get walls(): Set<WallId> { return this._walls || (this._walls = Finder.wallIds(this.room)); }

    private createCostMatrix(): CostMatrix
    {
        const cm = new PathFinder.CostMatrix();

        Finder.obstacles(this.room).map(o => o.pos).forEach(p => cm.set(p.x, p.y, 255));

        return cm;
    }
}

export class Chambers
{
    private static _allChambers: Dictionary<Chamber> = {};
    private static _allControllers: Set<ControllerId> = new Set();
    private static _allSources: Set<SourceId> = new Set();

    private static _myWalls: Set<WallId> = new Set();

    private static _reset: boolean = false;

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._allChambers[name] : undefined; }

    static get all(): Vector<Chamber> { return Dictionaries.values(Chambers._allChambers); }
    static get my(): Vector<Chamber> { return Chambers.all.filter(c => c.my); }

    static get allControllers(): Set<ControllerId> { return Chambers._allControllers.clone(); }
    static get allSources(): Set<SourceId> { return Chambers._allSources.clone(); }

    static get myWalls(): Set<WallId> { return Chambers._myWalls.clone(); }

    @profile
    static initialize()
    {
        const existing: Set<string> = Game.knownRoomNames;

        if (Dictionaries.update(Chambers._allChambers, existing, name => new Chamber(name)))
        {
            Chambers._allControllers = Vectors.defined(Chambers.all.map(c => c.controller)).map(c => c.id).toSet();
            Chambers._allSources = Set.flatten(Chambers.all.map(c => c.sources));
        }

        Chambers._reset = false;
    }

    @profile
    static reset()
    {
        if (Chambers._reset) return;

        Chambers._myWalls = Set.flatten(Chambers.my.map(c => c.walls));

        Chambers.all.forEach(c => c.reset());

        Chambers._reset = true;
    }
}
