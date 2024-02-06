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

    static wallIds(room: Room | undefined): Set<WallId>
    {
        return room ? Ids.get(OldFinder.walls(room)) : new Set<WallId>();
    }

    static obstacles(room: Room | undefined): Array<AnyStructure>
    {
        return room ? OldFinder.allStructures(room, OldFinder.obstacleTypes) : new Array<AnyStructure>();
    }
}

export class Chamber
{
    readonly name: string;
    // readonly sources: Set<SourceId>;

    get room(): Room | undefined { return Room.get(this.name); }
    get controller(): StructureController | undefined { return this.room?.controller; }
    get my(): boolean { return this.controller?.my || false; }
    get sourceIds(): Set<SourceId> { return this.room?.sourceIds || new Set<SourceId>(); }

    get energyAvailable(): number { return this.room?.energyAvailable || 0; }
    get energyCapacityAvailable(): number { return this.room?.energyCapacityAvailable || 0; }

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
    private static _allControllers: Cached<Set<ControllerId>> = Cached.simple(Chambers.fetchAllControllers);

    private static _myChambers: Cached<Array<Chamber>> = Cached.simple(Chambers.fetchMyChambers);
    private static _myWalls: Cached<Set<WallId>> = Cached.simple(Chambers.fetchMyWalls);

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._allChambers.value.get(name) : undefined; }

    static get all(): Array<Chamber> { return Chambers._allChambers.value.vs(); }
    static get allControllers(): Set<ControllerId> { return Chambers._allControllers.value.clone(); }

    static get my(): Array<Chamber> { return Chambers._myChambers.value.clone(); }
    static get myWalls(): Set<WallId> { return Chambers._myWalls.value.clone(); }

    @profile
    private static fetchAllChambers(value: Map<string, Chamber> | undefined): Map<string, Chamber>
    {
        const result: Map<string, Chamber> = value || new Map();

        if (result.update(Room.names, name => new Chamber(name)))
        {
            Chambers._allControllers.reset();
        }

        return result;
    }

    private static fetchAllControllers(): Set<ControllerId>
    {
        return Array.defined(Chambers.all.map(c => c.controller)).map(c => c.id).toSet();
    }

    @profile
    private static fetchMyChambers(): Array<Chamber>
    {
        return Chambers.all.filter(c => c.my);
    }

    @profile
    private static fetchMyWalls(): Set<WallId>
    {
        return Set.flatten(Chambers.my.map(c => c.walls));
    }

    @profile
    static reset()
    {
        Chambers.all.forEach(c => c.reset());
    }
}
