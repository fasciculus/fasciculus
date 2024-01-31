import { CreepState, CreepType, Dictionaries, Dictionary, ExtensionId, GameWrap, Names, SpawnId, Vector, WallId } from "./Common";
import { CreepBaseMemory } from "./Creeps";
import { profile } from "./Profiling";
import { Chambers } from "./Rooming";

export class Spawn
{
    readonly id: SpawnId;

    get spawn(): StructureSpawn { return GameWrap.get<StructureSpawn>(this.id)!; }
    get room(): Room { return this.spawn.room; }

    get idle(): boolean { return !this.spawn.spawning; }

    get roomEnergyAvailable(): number { return this.room.energyAvailable || 0; }
    get roomEnergyCapacity(): number { return this.room.energyCapacityAvailable; }

    constructor(id: SpawnId)
    {
        this.id = id;
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

    static get idle(): Vector<Spawn> { return Dictionaries.values(Spawns._spawns).filter(s => s.idle); }
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
