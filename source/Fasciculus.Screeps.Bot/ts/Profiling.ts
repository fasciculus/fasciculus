import { Memories } from "./Common";
import { PROFILER_IGNORED_KEYS, PROFILER_LOG_INTERVAL, PROFILER_MAX_ENTRIES, PROFILER_WARMUP } from "./Config";

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

type ProfilerMap = Map<string, ProfilerEntry>;

export class Profiler
{
    private static warmup: number = PROFILER_WARMUP;

    private static loadUsage: number = 0;
    private static startTime: number = Game.time;

    private static entries: ProfilerMap = new Map();

    static record(type: string, name: string, duration: number)
    {
        if (Profiler.warmup > 0) return;

        const key: string = `${type}:${name}`;
        const entry: ProfilerEntry = Profiler.getEntry(key);

        ++entry.calls;
        entry.duration += duration;
    }

    private static getEntry(key: string): ProfilerEntry
    {
        var result: ProfilerEntry | undefined = Profiler.entries.get(key);

        if (!result)
        {
            result = { key, calls: 0, duration: 0 };
            Profiler.entries.set(key, result)
        }

        return result;
    }

    static start()
    {
        if (Profiler.warmup > 0)
        {
            Profiler.entries.clear();
        }
        else
        {
            Profiler.loadUsage = Game.cpu.getUsed();
            Profiler.record("global", "load", Profiler.loadUsage);
        }
    }

    static stop()
    {
        if (Profiler.warmup > 0)
        {
            --Profiler.warmup;
            ++Profiler.startTime;
        }
        else
        {
            Profiler.record("global", "main", Game.cpu.getUsed() - Profiler.loadUsage);
        }

        Profiler.log();
    }

    static log()
    {
        if (Profiler.warmup > 0)
        {
            console.log(`Profiler in warmup (${Profiler.warmup})`);
            return;
        }

        const ticks: number = Game.time - Profiler.startTime + 1;

        if (ticks == 0 || ticks % PROFILER_LOG_INTERVAL != 0)
        {
            return;
        }

        const entries: ProfilerEntry[] = Profiler.getLogEntries();
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

    private static getLogEntries(): ProfilerEntry[]
    {
        const entryMap: ProfilerMap = new Map(Profiler.entries);

        PROFILER_IGNORED_KEYS.forEach(key => entryMap.delete(key));

        const result: ProfilerEntry[] = Array.from(entryMap.values());

        return result.sort(Profiler.compare).slice(0, PROFILER_MAX_ENTRIES);
    }

    private static compare(a: ProfilerEntry, b: ProfilerEntry): number
    {
        return b.duration - a.duration;
    }
}