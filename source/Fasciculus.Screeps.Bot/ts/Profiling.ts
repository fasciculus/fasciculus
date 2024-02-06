import { PROFILER_WARMUP } from "./Config";

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

export interface ProfilerEntry
{
    key: string;
    calls: number;
    duration: number;
}

export class Profiler
{
    private static _warmup: number = PROFILER_WARMUP;

    private static _loadUsage: number = 0;
    private static _startTime: number = Game.time;

    private static _entries: Map<string, ProfilerEntry> = new Map();

    private static _resetRequested: boolean = false;

    static get warmup(): number { return Profiler._warmup; }
    static get ticks(): number { return Math.max(1, Game.time - Profiler._startTime + 1); }

    private static createEntry(key: string): ProfilerEntry { return { key, calls: 0, duration: 0 }; }

    static record(type: string, name: string, duration: number): void
    {
        if (Profiler._warmup > 0) return;

        const key: string = `${type}:${name}`;
        const entry: ProfilerEntry = Profiler._entries.ensure(key, Profiler.createEntry);

        ++entry.calls;
        entry.duration += duration;
    }

    static start(): void
    {
        if (Profiler._resetRequested)
        {
            Profiler._entries.clear();
            Profiler._startTime = Game.time;
            Profiler._resetRequested = false;
        }

        if (Profiler._warmup == 0)
        {
            Profiler._loadUsage = Game.cpu.getUsed();
            Profiler.record("global", "load", Profiler._loadUsage);
        }
    }

    static stop(): void
    {
        if (Profiler._warmup > 0)
        {
            --Profiler._warmup;
            ++Profiler._startTime;
        }
        else
        {
            Profiler.record("global", "main", Game.cpu.getUsed() - Profiler._loadUsage);
        }
    }

    static reset(): void
    {
        Profiler._resetRequested = true;
    }

    static entries(ignore: Array<string>, count: number): Array<ProfilerEntry>
    {
        const entries: Map<string, ProfilerEntry> = new Map(Profiler._entries);

        ignore.forEach(key => entries.delete(key));

        return entries.vs().sort((a, b) => b.duration - a.duration).take(count);
    }
}