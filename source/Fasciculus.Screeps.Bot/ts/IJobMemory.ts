
import { JobType } from "./JobType";
import { TargetId } from "./TargetId";

export interface IJobMemory
{
    type: JobType,
    target: TargetId,
    priority: number
}