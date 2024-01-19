import * as _ from "lodash";

import { Rooms } from "./Rooms";
import { Sources } from "./Sources";
import { Creeps } from "./Creeps";
import { Names } from "./Names";
import { Bodies } from "./Bodies";
import { CreepState, CreepType } from "./Enums";
import { CreepBaseMemory } from "./Memories";
import { Wells } from "./Wells";
import { Sites } from "./Sites";

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
    private static _my: Spawn[] = [];
    private static _idle: Spawn[] = [];

    static get my(): Spawn[] { return Spawns._my; }
    static get idle(): Spawn[] { return Spawns._idle; }

    static initialize()
    {
        let mySpawns: StructureSpawn[] = _.flatten(Rooms.all.map(r => r.find<FIND_MY_SPAWNS, StructureSpawn>(FIND_MY_SPAWNS)));

        Spawns._my = mySpawns.map(s => new Spawn(s));
        Spawns._idle = Spawns._my.filter(s => s.idle);
    }
}