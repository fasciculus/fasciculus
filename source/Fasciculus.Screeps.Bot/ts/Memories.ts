
export interface INamesMemory
{
    next: { [prefix: string]: number };
}

export interface ISourceMemory
{
    container?: Id<StructureContainer>;
}

export interface IMemory
{
    names?: INamesMemory;

    sources?: { [id: Id<Source>]: ISourceMemory }
}

export class Memories
{
    static get memory(): IMemory
    {
        return Memory as IMemory;
    }

    static get names(): INamesMemory
    {
        var memory = Memories.memory;
        var result = memory.names;

        if (!result)
        {
            memory.names = result = { next: {} };
        }

        return result;
    }

    static source(source: Source): ISourceMemory
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