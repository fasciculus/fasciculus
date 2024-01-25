import { Memories } from "./Memories";
import { profile } from "./Profiling";

export class Statistics
{
    private static _supplied: number = 0;
    private static _welled: number = 0;

    static initialize()
    {
        Statistics._supplied = 0;
        Statistics._welled = 0;
    }

    @profile
    static run()
    {
        let memory = Memories.statistics;

        memory.supplied = (Statistics.supplied * 49 + Statistics._supplied) / 50;
        memory.welled = (Statistics.welled * 49 + Statistics._welled) / 50;
    }

    static get supplied(): number { return Memories.statistics.supplied || 0; }
    static addSupplied(amount: number) { Statistics._supplied += amount; }

    static get welled(): number { return Memories.statistics.welled || 0; }
    static addWelled(amount: number) { Statistics._welled += amount; }
}