import { Rooms } from "./Rooms";
import { Vector } from "./Collections";

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

}

export class Spawns
{
    private static _my: Vector<Spawn> = new Vector();
    private static _idle: Vector<Spawn> = new Vector();

    static get my(): Vector<Spawn> { return Spawns._my.clone(); }
    static get idle(): Vector<Spawn> { return Spawns._idle.clone(); }

    static initialize()
    {
        Spawns._my = Rooms.mySpawns.map(s => new Spawn(s));
        Spawns._idle = Spawns._my.filter(s => s.idle);
    }
}