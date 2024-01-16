
import { IJobMemory } from "./IJobMemory";

export interface IBotMemory extends CreepMemory
{
    job?: IJobMemory
}