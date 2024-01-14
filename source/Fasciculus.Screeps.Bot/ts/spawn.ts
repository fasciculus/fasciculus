
import * as _ from "lodash";
import { nextWorkerName } from "./name";

const WORKER_BODY = [WORK, CARRY, MOVE];
const WORKER_COST = _.sum(WORKER_BODY.map(b => BODYPART_COST[b]));

function spawnCreepsOfSpawn(spawn: StructureSpawn)
{
    if (spawn.spawning)
    {
        console.log(`spawn ${spawn.name} already spawning`);
        return;
    }

    let room = spawn.room;
    let energyAvailable = room.energyAvailable;

    if (energyAvailable < WORKER_COST)
    {
        // console.log(`not enough energy in room ${room.name}. required: ${WORKER_COST}, available: ${energyAvailable}`);
        return;
    }

    let name = nextWorkerName();
    let result = spawn.spawnCreep(WORKER_BODY, name);

    console.log(`spawn ${spawn.name} spawning ${name}. result ${result}`);
}

export function spawnCreeps()
{
    for (let name in Game.spawns)
    {
        let spawn = Game.spawns[name];

        spawnCreepsOfSpawn(spawn);
    }
}