import { PROFILER_IGNORED_KEYS, PROFILER_MAX_ENTRIES, PROFILER_WARMUP } from "./Config";
import { Objects } from "./types.common";

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

interface ProfilerCLI
{
    log(): void;
    reset(): void;
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

    private static resetRequested: boolean = false;

    private static cli: ProfilerCLI = { log: Profiler.log, reset: Profiler.reset };

    static record(type: string, name: string, duration: number): void
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

    static start(): void
    {
        Objects.setValue(Game, "profiler", Profiler.cli);

        if (Profiler.resetRequested)
        {
            Profiler.entries.clear();
            Profiler.startTime = Game.time;
            Profiler.resetRequested = false;
        }

        if (Profiler.warmup == 0)
        {
            Profiler.loadUsage = Game.cpu.getUsed();
            Profiler.record("global", "load", Profiler.loadUsage);
        }
    }

    static stop(): void
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
    }

    static reset(): void
    {
        Profiler.resetRequested = true;
    }

    static log(): void
    {
        if (Profiler.warmup > 0)
        {
            console.log(`Profiler in warmup. ${Profiler.warmup} ticks to go.`);
            return;
        }

        const ticks: number = Math.max(1, Game.time - Profiler.startTime + 1);
        const entries: ProfilerEntry[] = Profiler.getLogEntries();
        const divider: string = "".padEnd(62, "-");
        let memoryUsed: string = "n/a";
        let label: string = "method".padEnd(40);
        let duration: string = "cpu".padStart(6);
        let calls: string = "calls".padStart(7);
        let cpuPerCall: string = "cpu/call".padStart(9);

        console.log(divider);
        console.log(`Profile after ${ticks} ticks. Memory used: ${memoryUsed} KB.`);
        console.log(`${label}${duration}${calls}${cpuPerCall}`);
        console.log(divider);

        for (const entry of entries)
        {
            let label: string = entry.key.padEnd(40);
            let duration: string = (entry.duration / ticks).toFixed(2).padStart(6);
            let calls: string = (entry.calls / ticks).toFixed(1).padStart(7);
            let cpuPerCall: string = (entry.duration / Math.max(1, entry.calls)).toFixed(2).padStart(9);

            console.log(`${label}${duration}${calls}${cpuPerCall}`);
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