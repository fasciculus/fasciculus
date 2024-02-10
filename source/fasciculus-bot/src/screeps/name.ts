
interface NamesMemory
{
    creeps: { [type: string]: number },
    flags: { [type: string]: number }
}

export class Names
{
    private static _initialNamesMemory: NamesMemory = { creeps: {}, flags: {} };

    private static get memory(): NamesMemory
    {
        return Memory.get("names", Names._initialNamesMemory);
    }

    private static nextName(type: string, names: { [type: string]: number }): string
    {
        const nextIndex: number = (names[type] || 0) + 1;

        names[type] = nextIndex;

        return type + nextIndex;
    }

    static nextCreepName(type: string): string
    {
        return Names.nextName(type, Names.memory.creeps);
    }

    static nextFlagName(type: string): string
    {
        return Names.nextName(type, Names.memory.flags);
    }
}