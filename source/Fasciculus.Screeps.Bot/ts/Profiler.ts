import { Dictionaries, DictionaryEntry, Vector } from "./Collections";
import { Memories, ProfilerMemory } from "./Memories";

const PROFILER_TICK_KEY = "Profiler_tick";
const PROFILER_SESSION_KEY = "Profiler_session";
const PROFILER_SESSION = 3;

export class Profiler
{
    private static _data: ProfilerMemory = {};
    private static _lastUsed: number;

    static start()
    {
        Profiler._data = {};
        Profiler.prepare();
        Profiler._lastUsed = Game.cpu.getUsed();
    }

    static resetUsed()
    {
        Profiler._lastUsed = Game.cpu.getUsed();
    }

    static add(key: string)
    {
        let data = Profiler._data;
        let used = Game.cpu.getUsed();

        data[key] = (data[key] || 0) + (used - Profiler._lastUsed);
        Profiler._lastUsed = used;
    }

    static stop()
    {
        let data: ProfilerMemory = Profiler._data;
        let memory: ProfilerMemory = Memories.profiler;

        for (let key in data)
        {
            memory[key] = (memory[key] || 0) + data[key];
        }
    }

    static log()
    {
        let data: ProfilerMemory = Object.assign({}, Memories.profiler);
        let startTick: number = data[PROFILER_TICK_KEY] || 0;
        let ticks = Game.time - startTick + 1;

        delete data[PROFILER_SESSION_KEY];
        delete data[PROFILER_TICK_KEY];

        var entries: Vector<DictionaryEntry<number>> = Dictionaries.entries(data);

        entries = entries.sort((a, b) => b.value - a.value).take(5);
        entries = entries.forEach(e => e.value /= ticks);

        let used = Math.max(1, entries.sum(e => e.value));

        console.log(`Profiler data for ${Game.time} after ${ticks} ticks`);

        for (let entry of entries)
        {
            let percentage = (entry.value / used * 100).toFixed(1);
            let cpu = entry.value.toFixed(1);

            console.log(`  ${percentage} %: cpu: ${cpu}, ${entry.key}`);
        }
    }

    private static prepare()
    {
        let memory = Memories.profiler;

        if ((memory[PROFILER_SESSION_KEY] || 0) >= PROFILER_SESSION) return;

        let keys = Object.keys(memory);

        for (let key of keys)
        {
            delete memory[key];
        }

        memory[PROFILER_SESSION_KEY] = PROFILER_SESSION;
        memory[PROFILER_TICK_KEY] = Game.time;
    }
}