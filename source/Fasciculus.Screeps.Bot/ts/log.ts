import { PROFILER_IGNORED_KEYS, PROFILER_MAX_ENTRIES } from "./Config";
import { Profiler, ProfilerEntry } from "./Profiling";

export class Logger
{
    private static _divider: string = "".padEnd(79, "-");

    static logProfiler(): void
    {
        if (Profiler.warmup > 0)
        {
            console.log(`Profiler in warmup. ${Profiler.warmup} ticks to go.`);
            return;
        }

        const ticks: number = Profiler.ticks;
        const entries: Array<ProfilerEntry> = Profiler.entries(PROFILER_IGNORED_KEYS, PROFILER_MAX_ENTRIES);

        Logger.profilerHeader(ticks);
        entries.forEach(e => Logger.logProfilerEntry(e, ticks));
    }

    private static logProfilerEntry(entry: ProfilerEntry, ticks: number): void
    {
        const label: string = entry.key.padEnd(40);
        const duration: string = (entry.duration / ticks).toFixed(2).padStart(6);
        const calls: string = (entry.calls / ticks).toFixed(1).padStart(7);
        const cpuPerCall: string = (entry.duration / Math.max(1, entry.calls)).toFixed(2).padStart(9);

        console.log(`${label}${duration}${calls}${cpuPerCall}`);
    }

    private static profilerHeader(ticks: number): void
    {
        let label: string = "method".padEnd(40);
        let duration: string = "cpu".padStart(6);
        let calls: string = "calls".padStart(7);
        let cpuPerCall: string = "cpu/call".padStart(9);

        console.log(Logger._divider);
        console.log(`Profile after ${ticks} ticks.`);
        console.log(`${label}${duration}${calls}${cpuPerCall}`);
        console.log(Logger._divider);
    }
}