
import * as _ from "lodash";

import { IMemory } from "./IMemory";
import { ISpringMemory } from "./ISpringMemory";
import { INameMemory } from "./INameMemory";

export class MemoryManager
{
    static get memory(): IMemory
    {
        return Memory as IMemory;
    }

    static get names(): INameMemory
    {
        var memory = MemoryManager.memory;
        var result: INameMemory | undefined = memory.names;

        if (!result)
        {
            result = { lastWorkerId: 0 };
            memory.names = result;
        }

        return result;
    }

    static get springs(): { [name: string]: ISpringMemory }
    {
        var memory = MemoryManager.memory;
        var result: { [name: string]: ISpringMemory } | undefined = memory.springs;

        if (!result)
        {
            result = {};
            memory.springs = result;
        }

        return result;
    }

    static spring(id: Id<Source>): ISpringMemory
    {
        var memory = MemoryManager.springs;
        var result = memory[id];

        if (!result)
        {
            result = { harvestSlots: 0 };
            memory[id] = result;
        }

        return result;
    }

    static cleanup()
    {
        MemoryManager.cleanupCreeps();
        // MemoryManager.cleanupSprings();
    }

    private static cleanupCreeps()
    {
        var existing: Set<string> = new Set(_.keys(Game.creeps));

        for (let name in Memory.creeps)
        {
            if (!existing.has(name))
            {
                delete Memory.creeps[name];
            }
        }
    }

    private static cleanupSprings()
    {
        for (let id in MemoryManager.memory.springs)
        {
            delete MemoryManager.memory.springs[id];
        }
    }
}