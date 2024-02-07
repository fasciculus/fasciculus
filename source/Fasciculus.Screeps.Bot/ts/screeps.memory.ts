import { Objects } from "./types.util";

export class ScreepsMemory
{
    static get<T>(key: string, initial: T): T
    {
        var result: any | undefined = Memory[key];

        if (!result)
        {
            Memory[key] = result = initial;
        }

        return result as T;
    }

    static sub<T>(root: string, key: string, initial: T): T
    {
        const parent: { [index: string]: T } = ScreepsMemory.get(root, {});
        var result: T | undefined = parent[key];

        if (!result)
        {
            parent[key] = result = initial;
        }

        return result as T;
    }

    static setup()
    {
        Objects.setFunction(Memory, "get", ScreepsMemory.get);
        Objects.setFunction(Memory, "sub", ScreepsMemory.sub);
    }
}

