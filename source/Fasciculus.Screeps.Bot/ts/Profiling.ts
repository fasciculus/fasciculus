import { Dictionaries, Dictionary, Vector } from "./Collections";
import { PROFILER_SESSION } from "./ProfilerSession";
import { PROFILER_IGNORED_KEYS, PROFILER_LOG_INTERVAL, PROFILER_MAX_ENTRIES, PROFILER_WARMUP } from "./_Config";

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
    session: string;
    start: number;
    entries: ProfilerDictionary;
    warmup: number;
}

interface MemoryWithProfiler
{
    profiler?: ProfilerMemory;
}

export class Profiler
{
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
        let session: string = PROFILER_SESSION;

        if (!result || result.session != session)
        {
            let start = Game.time;
            let entries: ProfilerDictionary = {};
            let warmup = PROFILER_WARMUP;

            memory.profiler = result = { session, start, entries, warmup };
        }

        return result;
    }

    static start()
    {
        Profiler._entries = {};
    }

    static stop()
    {
        let memory: ProfilerMemory = Profiler.memory;

        if (memory.warmup > 0)
        {
            --memory.warmup;
            ++memory.start;
        }
        else
        {
            Profiler.merge(memory);
        }

        Profiler._entries = {};
        Profiler.log(memory);
    }

    private static merge(memory: ProfilerMemory)
    {
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
    }

    static log(memory: ProfilerMemory)
    {
        if (memory.warmup > 0)
        {
            console.log(`Profiler in warmup (${memory.warmup})`);
            return;
        }

        const ticks: number = Game.time - memory.start + 1;

        if (ticks == 0 || ticks % PROFILER_LOG_INTERVAL != 0)
        {
            return;
        }

        const entries: ProfilerEntries = Profiler.getLogEntries();
        const divider: string = "".padEnd(53, "-");
        let label: string = "method".padEnd(40);
        let duration: string = "cpu".padStart(6);
        let calls: string = "calls".padStart(7);

        console.log(divider);
        console.log(`Profile after ${ticks} ticks`);
        console.log(`${label}${duration}${calls}`);
        console.log(divider);

        for (const entry of entries)
        {
            let label: string = entry.key.padEnd(40);
            let duration: string = (entry.duration / ticks).toFixed(2).padStart(6);
            let calls: string = (entry.calls / ticks).toFixed(1).padStart(7);

            console.log(`${label}${duration}${calls}`);
        }
    }

    private static getLogEntries(): ProfilerEntries
    {
        let dictionary: ProfilerDictionary = Dictionaries.clone(Profiler.memory.entries);

        for (let key of PROFILER_IGNORED_KEYS)
        {
            delete dictionary[key];
        }

        return Dictionaries.values(dictionary).sort(Profiler.compare).take(PROFILER_MAX_ENTRIES);
    }

    private static compare(a: ProfilerEntry, b: ProfilerEntry): number
    {
        return b.duration - a.duration;
    }
}