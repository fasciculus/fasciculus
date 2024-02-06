import { profile } from "./Profiling";
import { Cached, Ids } from "./screeps.util";

export class OldFinder
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

    static walls(room: Room): Array<StructureWall>
    {
        return OldFinder.structures<StructureWall>(room, STRUCTURE_WALL);
    }

    static wallIds(room: Room): Set<WallId>
    {
        return Ids.get(OldFinder.walls(room));
    }

    static obstacles(room: Room): Array<AnyStructure>
    {
        return OldFinder.allStructures(room, OldFinder.obstacleTypes);
    }
}

export class Chamber
{
    readonly name: string;
    // readonly sources: Set<SourceId>;

    get room(): Room { return Game.rooms[this.name]; }
    get controller(): StructureController | undefined { return this.room.controller; }
    get my(): boolean { return this.controller?.my || false; }
    get sourceIds(): Set<SourceId> { return this.room.sourceIds; }

    get energyAvailable(): number { return this.room.energyAvailable || 0; }
    get energyCapacityAvailable(): number { return this.room.energyCapacityAvailable || 0; }

    constructor(name: string)
    {
        this.name = name;
        // this.sources = Ids.get(Game.rooms[name].find<FIND_SOURCES, Source>(FIND_SOURCES));
    }

    reset()
    {
        this._costMatrix = undefined;
        this._walls = undefined;
    }

    private _costMatrix?: CostMatrix = undefined;
    private _walls?: Set<WallId> = undefined;

    get costMatrix(): CostMatrix { return this._costMatrix || (this._costMatrix = this.createCostMatrix()); }
    get walls(): Set<WallId> { return this._walls || (this._walls = OldFinder.wallIds(this.room)); }

    private createCostMatrix(): CostMatrix
    {
        const cm = new PathFinder.CostMatrix();

        OldFinder.obstacles(this.room).map(o => o.pos).forEach(p => cm.set(p.x, p.y, 255));

        return cm;
    }
}

export class Chambers
{
    private static _allChambers: Cached<Map<string, Chamber>> = Cached.withValue(Chambers.fetchAllChambers);
    private static _allControllers: Cached<Set<ControllerId>> = Cached.simple(Chambers.fetchAllControllers, false);
    private static _allSourceIds: Cached<Set<SourceId>> = Cached.simple(Chambers.fetchAllSourceIds, false);

    private static _myChambers: Cached<Array<Chamber>> = Cached.simple(Chambers.fetchMyChambers);
    private static _myWalls: Cached<Set<WallId>> = Cached.simple(Chambers.fetchMyWalls);

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._allChambers.value.get(name) : undefined; }

    static get all(): Array<Chamber> { return Chambers._allChambers.value.vs(); }
    static get allControllers(): Set<ControllerId> { return Chambers._allControllers.value.clone(); }
    static get allSourceIds(): Set<SourceId> { return Chambers._allSourceIds.value.clone(); }

    static get my(): Array<Chamber> { return Chambers._myChambers.value.clone(); }
    static get myWalls(): Set<WallId> { return Chambers._myWalls.value.clone(); }

    @profile
    static fetchAllChambers(value: Map<string, Chamber> | undefined): Map<string, Chamber>
    {
        const result: Map<string, Chamber> = value || new Map();

        if (result.update(Game.knownRoomNames, name => new Chamber(name)))
        {
            Chambers._allControllers.reset();
            Chambers._allSourceIds.reset();
        }

        return result;
    }

    @profile
    static fetchAllControllers(): Set<ControllerId>
    {
        return Ids.get(Array.defined(Chambers.all.map(c => c.controller)));
    }

    @profile
    static fetchAllSourceIds(): Set<SourceId>
    {
        return Set.flatten(Chambers.all.map(c => c.sourceIds));
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
