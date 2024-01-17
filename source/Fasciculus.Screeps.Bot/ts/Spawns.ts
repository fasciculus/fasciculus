import * as _ from "lodash";

import { Rooms } from "./Rooms";
import { Weller, Wellers } from "./Wellers";
import { Sources } from "./Sources";
import { CreepState, CreepType, ICreepMemory } from "./Creeps";
import { Names } from "./Names";
import { Bodies } from "./Bodies";

export class Spawns
{
    private static _my: StructureSpawn[] = [];
    private static _idle: StructureSpawn[] = [];

    static get my(): StructureSpawn[] { return Spawns._my; }
    static get idle(): StructureSpawn[] { return Spawns._idle; }

    private static myOf(room: Room): StructureSpawn[]
    {
        return room.find<FIND_MY_SPAWNS, StructureSpawn>(FIND_MY_SPAWNS)
    }

    static initialize()
    {
        Spawns._my = _.flatten(Rooms.all.map(Spawns.myOf));
        Spawns._idle = Spawns._my.filter(s => s.spawning === null);
    }

    static run()
    {
        var spawn = Spawns.bestSpawn();

        if (!spawn) return;

        var wellerCount = Wellers.all.length;
        var sourceCount = Sources.all.length;

        if (wellerCount < sourceCount)
        {
            Spawns.spawnWeller(spawn);
            return;
        }
    }

    private static bestSpawn(): StructureSpawn | undefined
    {
        var spawns = Spawns.idle;

        if (spawns.length == 0) return undefined;

        return spawns.sort(Spawns.compareSpawns)[0];
    }

    private static compareSpawns(a: StructureSpawn, b: StructureSpawn): number
    {
        return b.room.energyAvailable - a.room.energyAvailable;
    }

    private static spawnWeller(spawn: StructureSpawn)
    {
        var sources = Wellers.findFreeSources();

        if (sources.length == 0) return;

        var source = sources[0];
        var memory: ICreepMemory = Wellers.createMemory(source);

        Spawns.spawnCreep(spawn, memory);
    }

    static spawnCreep(spawn: StructureSpawn, memory: ICreepMemory): ScreepsReturnCode
    {
        let energy = spawn.room.energyAvailable;
        let body = Bodies.create(memory.type, energy);

        if (!body) return ERR_NOT_ENOUGH_ENERGY;

        let name = Names.next(memory.type);
        let opts: SpawnOptions = { memory };

        return spawn.spawnCreep(body, name, opts);
    }
}