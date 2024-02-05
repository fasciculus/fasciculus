import { Profiler } from "./Profiling";
import { Logger } from "./log";
import { Objects } from "./types.common";

interface ProfilerCLI
{
    log(): void;
    reset(): void;
}

export class CLI
{
    static profiler: ProfilerCLI =
    {
        log: Logger.logProfiler,
        reset: Profiler.reset
    };

    static setup()
    {
        Objects.setValue(Game, "profiler", CLI.profiler);
    }
}