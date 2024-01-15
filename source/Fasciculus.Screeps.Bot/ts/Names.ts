
import { INameMemory } from "./INameMemory";
import { MemoryManager } from "./MemoryManager";

export class Names
{
    private static initialNameMemory: INameMemory =
    {
        lastWorkerId: 0
    }

    private static get memory(): INameMemory { return MemoryManager.names; }

    static nextWorkerName(): string
    {
        var memory = Names.memory;
        var nextWorkerId = memory.lastWorkerId + 1;

        memory.lastWorkerId = nextWorkerId;

        return `W${nextWorkerId}`;
    }
}