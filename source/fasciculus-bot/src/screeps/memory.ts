
import { Objects } from "../es/object";

declare global
{
    interface Memory
    {
        [index: string]: any;

        get<T>(key: string, initial: T): T;
    }
}

export class MemoryExt
{
    private static get<T>(key: string, initial: T): T
    {
        var result: any | undefined = Memory[key];

        if (!result)
        {
            Memory[key] = result = initial;
        }

        return result as T;
    }

    static setup()
    {
        Objects.setFunction(Memory, "get", MemoryExt.get);
    }
}