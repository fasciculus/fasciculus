import { Dictionaries, Dictionary, Memories, Vector } from "./Common";
import { PROFILER_IGNORED_KEYS, PROFILER_LOG_INTERVAL, PROFILER_MAX_ENTRIES, PROFILER_SESSION, PROFILER_WARMUP } from "./Config";

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
}

interface MemoryWithProfiler
{
    profiler?: ProfilerMemory;
}

export class Profiler
{
    private static _start: number = 0;
    private static _warmup: number = PROFILER_WARMUP;

    static record(type: string, name: string, duration: number)
    {
        if (Profiler._warmup > 0) return;

        const key: string = `${type}:${name}`;
        const entry: ProfilerEntry = Profiler.getEntry(key);

        ++entry.calls;
        entry.duration += duration;
    }

    private static getEntry(key: string): ProfilerEntry
    {
        let result = Profiler.memory.entries[key];

        if (!result)
        {
            Profiler.memory.entries[key] = result = { key, calls: 0, duration: 0 };
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

            memory.profiler = result = { session, start, entries };
        }

        return result;
    }

    static start()
    {
        if (Profiler._warmup > 0)
        {
            Profiler.memory.entries = {};
        }
        else
        {
            Profiler._start = Game.cpu.getUsed();
            Profiler.record("global", "load", Profiler._start);
        }
    }

    static stop()
    {
        const memory: ProfilerMemory = Profiler.memory;

        if (Profiler._warmup > 0)
        {
            --Profiler._warmup;
            ++memory.start;
        }
        else
        {
            Profiler.record("global", "main", Game.cpu.getUsed() - Profiler._start);
        }

        Profiler.log(memory);
    }

    static log(memory: ProfilerMemory)
    {
        if (Profiler._warmup > 0)
        {
            console.log(`Profiler in warmup (${Profiler._warmup})`);
            return;
        }

        const ticks: number = Game.time - memory.start + 1;

        if (ticks == 0 || ticks % PROFILER_LOG_INTERVAL != 0)
        {
            return;
        }

        const entries: ProfilerEntries = Profiler.getLogEntries();
        const divider: string = "".padEnd(53, "-");
        let memoryUsed: string = (Memories.used / 1024).toFixed(1);
        let label: string = "method".padEnd(40);
        let duration: string = "cpu".padStart(6);
        let calls: string = "calls".padStart(7);

        console.log(divider);
        console.log(`Profile after ${ticks} ticks. Memory used: ${memoryUsed} KB.`);
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

        Dictionaries.removeAll(dictionary, PROFILER_IGNORED_KEYS);

        return Dictionaries.values(dictionary).sort(Profiler.compare).take(PROFILER_MAX_ENTRIES);
    }

    private static compare(a: ProfilerEntry, b: ProfilerEntry): number
    {
        return b.duration - a.duration;
    }
}