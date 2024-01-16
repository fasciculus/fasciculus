
export interface INamesMemory
{
    next: { [prefix: string]: number };
}

export interface IMemory
{
    names?: INamesMemory;
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
            result = { next: {} };
            memory.names = result;
        }

        return result;
    }
}