
import { Objects } from "../es/object";

export class Memories
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
        Objects.setFunction(Memory, "get", Memories.get);
    }
}