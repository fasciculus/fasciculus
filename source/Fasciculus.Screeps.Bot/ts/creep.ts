
import { NameManager } from "./name";

var _creeps: Creep[];
var _workers: Creep[];

function getCreeps(): Creep[]
{
    if (!_creeps)
    {
        _creeps = [];

        for (var name in Game.creeps)
        {
            _creeps.push(Game.creeps[name]);
        }
    }

    return _creeps;
}

function getWorkers(): Creep[]
{
    if (!_workers)
    {
        Creeps.filter(creep => NameManager.isWorkerName(creep.name));
    }

    return _workers;
}

export const Creeps: Creep[] = getCreeps();
export const Workers: Creep[] = getWorkers();

export class CreepManager
{
    static cleanup()
    {
        var existing: Set<string> = new Set();

        Creeps.forEach(creep => existing.add(creep.name));

        for (var name in Memory.creeps)
        {
            if (!existing.has(name))
            {
                delete Memory.creeps[name];
            }
        }
    }
}