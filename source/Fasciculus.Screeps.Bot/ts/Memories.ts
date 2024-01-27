
import { Dictionaries } from "./Collections";
import { profile } from "./Profiling";
import { CreepState, CreepType } from "./Types";

interface ExtendedMemory extends Memory
{
    [index: string]: any;
}

export class Memories
{
    @profile
    static cleanup()
    {
        var existing: Set<string> = Dictionaries.keys(Game.creeps);

        for (let id in Memory.creeps)
        {
            if (!existing.has(id))
            {
                delete Memory.creeps[id];
            }
        }
    }

    static get memory(): ExtendedMemory
    {
        return Memory as ExtendedMemory;
    }

    static get<T>(key: string, initial: T): T
    {
        let result: any | undefined = Memories.memory[key];

        if (!result)
        {
            Memories.memory[key] = result = initial;
        }

        return result as T;
    }
}