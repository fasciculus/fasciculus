import { Dictionaries, Dictionary, Vector } from "./Collections";

export function profile<T extends new (...args: any[]) => any, A extends any[], R>(target: (this: T, ...args: A) => R,
    context: ClassMemberDecoratorContext)
{
    const name: string = String(context.name);

    function replacement(this: T, ...args: A)
    {
        var type: string = "unknown";

        if (typeof (this) == "function")
        {
            type = this.name;
        }
        else if (typeof (this) == "object")
        {
            let proto = Object.getPrototypeOf(this) || {};
            let ctor = proto["constructor"] || {};

            type = ctor["name"] || type;
        }

        const start: number = Game.cpu.getUsed();
        const result: R = target.call(this, ...args);
        const duration: number = Game.cpu.getUsed() - start;

        Profiler.record(type, name, duration);

        return result;
    }

    return replacement;
}

interface ProfilerEntry
{
    key: string;
    calls: number;
    duration: number;
}

type ProfilerDictionary = Dictionary<ProfilerEntry>;
type ProfilerEntries = Vector<ProfilerEntry>;

interface ProfilerMemory
{
    session: number;
    start: number;
    entries: ProfilerDictionary;
}

interface MemoryWithProfiler
{
    profiler?: ProfilerMemory;
}

export class Profiler
{
    private static _session: number = 0;
    private static _entries: ProfilerDictionary = {};

    static record(type: string, name: string, duration: number)
    {
        const key: string = `${type}:${name}`;
        const entry: ProfilerEntry = Profiler.getEntry(key);

        ++entry.calls;
        entry.duration += duration;
    }

    private static getEntry(key: string): ProfilerEntry
    {
        let result = Profiler._entries[key];

        if (!result)
        {
            Profiler._entries[key] = result = { key, calls: 0, duration: 0 };
        }

        return result;
    }

    private static get memory(): ProfilerMemory
    {
        let memory: MemoryWithProfiler = Memory as MemoryWithProfiler;
        let result: ProfilerMemory | undefined = memory.profiler;
        let session: number = Profiler._session;

        if (!result || result.session != Profiler._session)
        {
            memory.profiler = result = { session, start: Game.time, entries: {} };
        }

        return result;
    }

    static start(session: number)
    {
        Profiler._session = session;
        Profiler._entries = {};
    }

    static stop()
    {
        let memory: ProfilerMemory = Profiler.memory;
        let memoryEntries: ProfilerDictionary = memory.entries;
        let entries: ProfilerDictionary = Profiler._entries;

        for (let key in entries)
        {
            let entry: ProfilerEntry = entries[key];
            let memoryEntry: ProfilerEntry | undefined = memoryEntries[key];

            if (memoryEntry === undefined)
            {
                memoryEntries[key] = entry;
            }
            else
            {
                memoryEntry.calls += entry.calls;
                memoryEntry.duration += entry.duration;
            }
        }

        Profiler._entries = {};
    }

    static log()
    {
        const memory: ProfilerMemory = Profiler.memory;
        const ticks: number = Game.time - memory.start + 1;
        const entries: ProfilerEntries = Dictionaries.values(memory.entries).sort(Profiler.compare).take(10);
        const divider: string = "".padEnd(53, "-");
        let label: string = "method".padEnd(40);
        let duration: string = "cpu".padStart(6);
        let calls: string = "calls".padStart(7);

        console.log(divider);
        console.log(`profile after ${ticks} ticks`);
        console.log(`${label}${duration}${calls}`);
        console.log(divider);

        for (const entry of entries)
        {
            let label: string = entry.key.padEnd(40);
            let duration: string = (entry.duration / ticks).toFixed(1).padStart(6);
            let calls: string = (entry.calls / ticks).toFixed(1).padStart(7);

            console.log(`${label}${duration}${calls}`);
        }
    }

    private static compare(a: ProfilerEntry, b: ProfilerEntry): number
    {
        return b.duration - a.duration;
    }
}