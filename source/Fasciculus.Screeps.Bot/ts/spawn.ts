import { BodyManager } from "./body";
import { NameManager } from "./name";


var _spawns: StructureSpawn[];

function getSpawns(): StructureSpawn[]
{
    if (!_spawns)
    {
        _spawns = [];

        for (var name in Game.spawns)
        {
            _spawns.push(Game.spawns[name]);
        }
    }

    return _spawns;
}

export const Spawns: StructureSpawn[] = getSpawns();
export const IdleSpawns: StructureSpawn[] = Spawns.filter(spawn => !spawn.spawning);

export class SpawnManager
{
    static run()
    {
        var usedRooms: Set<string> = new Set<string>();

        for (let spawn of IdleSpawns)
        {
            let room = spawn.room;
            let roomName = room.name;

            if (usedRooms.has(roomName)) continue;

            let body = BodyManager.createWorkerBody(room);

            if (body.length == 0) continue;

            let name = NameManager.nextWorkerName();

            if (spawn.spawnCreep(body, name) == OK)
            {
                usedRooms.add(roomName);
            }
        }
    }
}