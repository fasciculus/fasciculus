import { CreepState, CreepType, Dictionaries, Dictionary, GameWrap, Names, SpawnId, Vector } from "./Common";
import { CreepBaseMemory } from "./Creeps";
import { profile } from "./Profiling";

export class Spawn
{
    readonly id: SpawnId;

    get spawn(): StructureSpawn { return GameWrap.get<StructureSpawn>(this.id)!; }
    get room(): Room { return this.spawn.room; }

    get idle(): boolean { return !this.spawn.spawning; }

    get roomEnergyAvailable(): number { return this.room.energyAvailable; }
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