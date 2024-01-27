import { Dictionary } from "./Collections";
import { Memories } from "./Memories";

interface NamesMemory
{
    creeps: Dictionary<number>;
}

const InitialNamesMemory: NamesMemory =
{
    creeps: {}
};

export class Names
{
    private static get memory(): NamesMemory { return Memories.get("names", InitialNamesMemory); }

    static next(prefix: string)
    {
        var memory = Names.memory;
        var id = memory.creeps[prefix] || 0;

        ++id;
        memory.creeps[prefix] = id;

        return `${prefix}${id}`;
    }
}