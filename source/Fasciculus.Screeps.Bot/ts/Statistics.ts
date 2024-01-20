import { Memories } from "./Memories";

export class Statistics
{
    private static _supplied: number = 0;

    static initialize()
    {
        Statistics._supplied = 0;
    }

    static run()
    {
        let memory = Memories.statistics;

        memory.supplied = (memory.supplied * 49 + Statistics._supplied) / 50;
    }

    static get supplied(): number { return Memories.statistics.supplied; }
    static addSupplied(amount: number) { Statistics._supplied += amount; }
}