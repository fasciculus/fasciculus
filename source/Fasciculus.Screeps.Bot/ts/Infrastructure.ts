import { ControllerId, CreepState, CreepType, Dictionaries, Dictionary, ExtensionId, GameWrap, Names, SpawnId, Stores, Vector, WallId } from "./Common";
import { CreepBaseMemory } from "./Creeps";
import { profile } from "./Profiling";
import { Chamber, Chambers } from "./Rooming";

export class Controller
{
    readonly id: ControllerId;

    get controller(): StructureController { return GameWrap.get<StructureController>(this.id)!; }

    get my(): boolean { return this.controller.my; }
    get pos(): RoomPosition { return this.controller.pos; }

    constructor(id: ControllerId)
    {
        this.id = id;
    }
}

export class Controllers
{
    private static _allControllers: Dictionary<Controller> = {};

    static get(id: ControllerId | undefined): Controller | undefined { return id ? Controllers._allControllers[id] : undefined; }

    static get all(): Vector<Controller> { return Dictionaries.values(Controllers._allControllers); }
    static get my(): Vector<Controller> { return Controllers.all.filter(c => c.my); }
    static get myCount(): number { return Controllers.my.length; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Controllers._allControllers = {};
        }

        Dictionaries.update(Controllers._allControllers, Chambers.allControllers, id => new Controller(id as ControllerId));
    }
}

export class Spawn
{
    readonly id: SpawnId;
    readonly pos: RoomPosition;

    get spawn(): StructureSpawn { return GameWrap.get<StructureSpawn>(this.id)!; }
    get chamber(): Chamber { return Chambers.get(this.spawn.room.name)!; }

    get idle(): boolean { return !this.spawn.spawning; }

    get rcl(): number { return this.chamber.rcl; }

    get energy(): number { return Stores.energy(this.spawn); }
    get freeEnergyCapacity(): number { return Stores.freeEnergyCapacity(this.spawn); }

    get roomEnergyAvailable(): number { return this.chamber.energyAvailable; }
    get roomEnergyCapacity(): number { return this.chamber.energyCapacityAvailable; }

    constructor(id: SpawnId)
    {
        this.id = id;
        this.pos = this.spawn.pos;
    }

    spawnCreep(type: CreepType, body: Vector<BodyPartConstant>)
    {
        let name = Names.next(type);
        let memory: CreepBaseMemory = { type, state: CreepState.Idle };
        let opts: SpawnOptions = { memory };

        body.call(parts => this.spawn.spawnCreep(parts, name, opts));
    }
}

export class Spawns
{
    private static _spawns: Dictionary<Spawn> = {};

    static get all(): Vector<Spawn> { return Dictionaries.values(Spawns._spawns); }
    static get idle(): Vector<Spawn> { return Spawns.all.filter(s => s.idle); }
    static get best(): Spawn | undefined { return Spawns.idle.max(s => s.roomEnergyAvailable); }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Spawns._spawns = {};
        }

        const existing: Set<string> = GameWrap.mySpawns.map(s => s.id).toSet();

        Dictionaries.update(Spawns._spawns, existing, id => new Spawn(id as SpawnId));
    }
}

export class Extension
{
    readonly id: ExtensionId;

    get extension(): StructureExtension { return GameWrap.get<StructureExtension>(this.id)!; }

    constructor(id: ExtensionId)
    {
        this.id = id;
    }
}

export class Extensions
{
    private static _extensions: Dictionary<Extension> = {};

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Extensions._extensions = {};
        }

        Dictionaries.update(Extensions._extensions, Extensions.existing, id => new Extension(id as ExtensionId));
    }

    // todo: replace with Chambers.extensions
    private static get existing(): Set<string>
    {
        const result: Set<string> = new Set();

        for (const room of GameWrap.rooms)
        {
            const structures: StructureExtension[] = room.find<FIND_MY_STRUCTURES, StructureExtension>(FIND_MY_STRUCTURES);

            if (structures.length == 0) continue;

            const extensions: StructureExtension[] = structures.filter(s => s.structureType == STRUCTURE_EXTENSION);

            if (extensions.length == 0) continue;

            extensions.forEach(e => result.add(e.id));
        }

        return result;
    }
}

export class Wall
{
    readonly id: WallId;

    get wall(): StructureWall { return GameWrap.get<StructureWall>(this.id)!; }

    get hits(): number { return this.wall.hits; }

    constructor(id: WallId)
    {
        this.id = id;
    }
}

export class Walls
{
    private static _myWalls: Dictionary<Wall> = {};

    static get my(): Vector<Wall> { return Dictionaries.values(Walls._myWalls); }
    static get avg(): number { return Walls.my.avg(w => w.hits); }

    static get newest(): Vector<Wall> { return Walls.my.filter(w => w.hits == 1); }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Walls._myWalls = {};
        }

        Dictionaries.update(Walls._myWalls, Chambers.myWalls, id => new Wall(id as WallId));
    }

    static get myWeakest(): Wall | undefined
    {
        var result: Wall | undefined = Walls.my.min(w => w.hits);

        if (result && result.hits == WALL_HITS_MAX)
        {
            result = undefined;
        }

        return result;
    }
}
