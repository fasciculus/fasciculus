import { profile } from "./Profiling";
import { Cached } from "./screeps";

export class Finder
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

    static allStructures(room: Room, types: Set<string>): Array<AnyStructure>
    {
        return room.find<AnyStructure>(FIND_STRUCTURES).filter(s => types.has(s.structureType));
    }

    private static ids<T extends _HasId>(values: Array<T>): Set<Id<T>>
    {
        return values.map(v => v.id).toSet();
    }

    static walls(room: Room): Array<StructureWall>
    {
        return Finder.structures<StructureWall>(room, STRUCTURE_WALL);
    }

    static wallIds(room: Room): Set<WallId>
    {
        return Finder.ids(Finder.walls(room));
    }

    static obstacles(room: Room): Array<AnyStructure>
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
        this.sources = Game.rooms[name].find<FIND_SOURCES, Source>(FIND_SOURCES).map(s => s.id).toSet();
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
    private static _allChambers: Cached<Map<string, Chamber>> = new Cached(Chambers.fetchAllChambers);
    private static _allControllers: Cached<Set<ControllerId>> = new Cached(Chambers.fetchAllControllers, false);
    private static _allSources: Cached<Set<SourceId>> = new Cached(Chambers.fetchAllSources, false);

    private static _myChambers: Cached<Array<Chamber>> = new Cached(Chambers.fetchMyChambers);
    private static _myWalls: Cached<Set<WallId>> = new Cached(Chambers.fetchMyWalls);

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._allChambers.value.get(name) : undefined; }

    static get all(): Array<Chamber> { return Chambers._allChambers.value.vs(); }
    static get allControllers(): Set<ControllerId> { return Chambers._allControllers.value.clone(); }
    static get allSources(): Set<SourceId> { return Chambers._allSources.value.clone(); }

    static get my(): Array<Chamber> { return Chambers._myChambers.value.clone(); }
    static get myWalls(): Set<WallId> { return Chambers._myWalls.value.clone(); }

    @profile
    static fetchAllChambers(value: Map<string, Chamber> | undefined): Map<string, Chamber>
    {
        const result: Map<string, Chamber> = value || new Map();

        if (result.update(Game.knownRoomNames, name => new Chamber(name)))
        {
            Chambers._allControllers.reset();
            Chambers._allSources.reset();
        }

        return result;
    }

    @profile
    static fetchAllControllers(value: Set<ControllerId> | undefined): Set<ControllerId>
    {
        return Array.defined(Chambers.all.map(c => c.controller)).map(c => c.id).toSet();
    }

    @profile
    static fetchAllSources(): Set<SourceId>
    {
        return Set.flatten(Chambers.all.map(c => c.sources));
    }

    @profile
    static fetchMyChambers(): Array<Chamber>
    {
        return Chambers.all.filter(c => c.my);
    }

    @profile
    static fetchMyWalls(): Set<WallId>
    {
        return Set.flatten(Chambers.my.map(c => c.walls));
    }

    @profile
    static reset()
    {
        Chambers.all.forEach(c => c.reset());
    }
}
