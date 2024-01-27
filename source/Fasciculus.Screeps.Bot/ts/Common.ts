
export type ContainerId = Id<StructureContainer>;
export type ControllerId = Id<StructureController>;
export type SiteId = Id<ConstructionSite>;
export type SourceId = Id<Source>;

interface ExtendedMemory extends Memory
{
    [index: string]: any;
}

export class Memories
{
    static get<T>(key: string, initial: T): T
    {
        const memory: ExtendedMemory = Memory as ExtendedMemory;
        let result: any | undefined = memory[key];

        if (!result)
        {
            memory[key] = result = initial;
        }

        return result as T;
    }
}