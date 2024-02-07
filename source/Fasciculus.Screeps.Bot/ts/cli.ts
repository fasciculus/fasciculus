import { Profiler } from "./Profiling";
import { Logger } from "./log";
import { Objects } from "./types.util";

interface ProfilerCLI
{
    log(): void;
    reset(): void;
}

interface RouterCLI
{
    log(): void;
}

export class CLI
{
    static profilerCLI: ProfilerCLI = { log: Logger.logProfiler, reset: Profiler.reset };

    static routerCLI: RouterCLI = { log: Logger.logRouter };

    static setup()
    {
        Objects.setValue(Game, "profiler", CLI.profilerCLI);
        Objects.setValue(Game, "router", CLI.routerCLI);
    }
}