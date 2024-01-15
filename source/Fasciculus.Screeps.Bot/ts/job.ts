
import { IJobMemory } from "./IJobMemory";
import { JobType } from "./JobType";
import { TargetId } from "./TargetId";

export class Job
{
    private _id?: string;

    readonly type: JobType;
    readonly target: TargetId;
    readonly priority: number;
    readonly order: number;

    constructor(type: JobType, target: TargetId, priority: number)
    {
        this.type = type;
        this.target = target;
        this.priority = priority;
        this.order = type + priority * 100;
    }

    get id(): string
    {
        if (!this._id)
        {
            this._id = `${this.type}.${this.target}.${this.priority}`;
        }

        return this._id;
    }

    public toMemory(): IJobMemory
    {
        return { type: this.type, target: this.target, priority: this.priority } as IJobMemory;
    }

    static fromMemory(memory: IJobMemory | undefined): Job | undefined
    {
        if (!memory) return undefined;

        return new Job(memory.type, memory.target, memory.priority);
    }
}