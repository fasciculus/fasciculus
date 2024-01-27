import { CreepState, CreepType, Vector } from "./Common";
import { CreepBaseMemory } from "./Creeps";
import { GameWrap } from "./GameWrap";
import { Names } from "./Names";

export class Spawn
{
    readonly spawn: StructureSpawn;

    get room(): Room { return this.spawn.room; }

    get idle(): boolean { return !this.spawn.spawning; }

    get roomEnergyAvailable(): number { return this.room.energyAvailable; }
    get roomEnergyCapacity(): number { return this.room.energyCapacityAvailable; }

    constructor(spawn: StructureSpawn)
    {
        this.spawn = spawn;
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
    private static _my: Vector<Spawn> = new Vector();
    private static _idle: Vector<Spawn> = new Vector();

    static get my(): Vector<Spawn> { return Spawns._my.clone(); }
    static get best(): Spawn | undefined { return Spawns._idle.max(s => s.roomEnergyAvailable); }

    static initialize()
    {
        Spawns._my = GameWrap.mySpawns.map(s => new Spawn(s));
        Spawns._idle = Spawns._my.filter(s => s.idle);
    }
}