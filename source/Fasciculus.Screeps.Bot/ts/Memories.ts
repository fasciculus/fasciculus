import * as _ from "lodash";

export interface NamesMemory
{
    next: { [prefix: string]: number };
}

export interface SourceMemory
{
    container?: Id<StructureContainer>;
}

export interface ExtendedMemory
{
    names?: NamesMemory;

    sources?: { [id: Id<Source>]: SourceMemory }
}

export class Memories
{
    static cleanup()
    {
        var existing: Set<string> = new Set(_.keys(Game.creeps));

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

    static get names(): NamesMemory
    {
        var memory = Memories.memory;
        var result = memory.names;

        if (!result)
        {
            memory.names = result = { next: {} };
        }

        return result;
    }

    static source(source: Source): SourceMemory
    {
        var memory = Memories.memory;
        var root = memory.sources;

        if (!root)
        {
            memory.sources = root = {};
        }

        var id = source.id;
        var result = root[id];

        if (!result)
        {
            root[id] = result = {};
        }

        return result;
    }
}