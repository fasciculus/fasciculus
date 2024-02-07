import { CreepState, CreepType, Stores } from "./Common";
import { CreepBaseMemory } from "./Creeps";
import { profile } from "./Profiling";
import { Chambers } from "./Rooms";

export class Controller
{
    readonly id: ControllerId;

    get controller(): StructureController { return Game.get<StructureController>(this.id)!; }

    get my(): boolean { return this.controller.my; }
    get pos(): RoomPosition { return this.controller.pos; }

    constructor(id: ControllerId)
    {
        this.id = id;
    }
}

export class Controllers
{
    private static _allControllers: Map<ControllerId, Controller> = new Map();

    static get(id: ControllerId | undefined): Controller | undefined { return id ? Controllers._allControllers.get(id) : undefined; }

    static get all(): Array<Controller> { return Controllers._allControllers.vs(); }
    static get my(): Array<Controller> { return Controllers.all.filter(c => c.my); }
    static get myCount(): number { return Controllers.my.length; }

    @profile
    static initialize()
    {
        Controllers._allControllers.update(Chambers.allControllers, id => new Controller(id));
    }
}

export class Spawn
{
    readonly id: SpawnId;
    readonly pos: RoomPosition;

    get spawn(): StructureSpawn { return Game.get<StructureSpawn>(this.id)!; }
    get room(): Room { return this.spawn.room; }

    get idle(): boolean { return !this.spawn.spawning; }

    get level(): number { return this.spawn.level; }

    get energy(): number { return Stores.energy(this.spawn); }
    get freeEnergyCapacity(): number { return Stores.freeEnergyCapacity(this.spawn); }

    get roomEnergyAvailable(): number { return this.room.energy; }
    get roomEnergyCapacity(): number { return this.room.energyCapacity; }

    constructor(id: SpawnId)
    {
        this.id = id;
        this.pos = this.spawn.pos;
    }

    spawnCreep(type: CreepType, body: Array<BodyPartConstant>)
    {
        let name = Creep.newName(type);
        let memory: CreepBaseMemory = { state: CreepState.Idle };
        let opts: SpawnOptions = { memory };

        this.spawn.spawnCreep(body, name, opts);
    }
}

export class Spawns
{
    private static _spawns: Map<SpawnId, Spawn> = new Map();

    static get all(): Array<Spawn> { return Spawns._spawns.vs(); }
    static get idle(): Array<Spawn> { return Spawns.all.filter(s => s.idle); }
    static get best(): Spawn | undefined { return Spawns.idle.max(s => s.roomEnergyAvailable); }

    @profile
    static initialize()
    {
        Spawns._spawns.update(StructureSpawn.myIds, id => new Spawn(id));
    }
}

export class Wall
{
    readonly id: WallId;

    get wall(): StructureWall { return Game.get<StructureWall>(this.id)!; }

    get hits(): number { return this.wall.hits; }

    constructor(id: WallId)
    {
        this.id = id;
    }
}

export class Walls
{
    private static _myWalls: Map<WallId, Wall> = new Map();

    static get my(): Array<Wall> { return Walls._myWalls.vs(); }
    static get avg(): number { return Walls.my.avg(w => w.hits); }

    static get newest(): Array<Wall> { return Walls.my.filter(w => w.hits == 1); }

    @profile
    static initialize()
    {
        Walls._myWalls.update(Chambers.myWalls, id => new Wall(id));
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
