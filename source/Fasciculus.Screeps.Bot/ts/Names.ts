import { Memories } from "./Memories";

export class Names
{
    static next(prefix: string)
    {
        var memory = Memories.names;
        var id = memory.next[prefix] || 0;

        ++id;
        memory.next[prefix] = id;

        return `${prefix}${id}`;
    }
}