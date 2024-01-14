
export enum JobType
{
    Harvest = "H",
    Upgrade = "U"
}

export function decodeJobType(value: string): JobType | undefined
{
    if (!value) return undefined;

    switch (value)
    {
        case JobType.Harvest: return JobType.Harvest;
        case JobType.Upgrade: return JobType.Upgrade;
    }

    return undefined;
}

export class Job
{
    readonly type: JobType;
    readonly target: string;
    readonly prio: number;

    private _id?: string;

    constructor(type: JobType, target: string, prio: number)
    {
        this.type = type;
        this.target = target;
        this.prio = prio;
    }

    get id(): string
    {
        if (!this._id)
        {
            this._id = this.encode();
        }

        return this._id;
    }

    get source(): Source | null
    {
        return Game.getObjectById<Source>(this.target as Id<Source>);
    }

    get controller(): StructureController | null
    {
        return Game.getObjectById<StructureController>(this.target as Id<StructureController>);
    }

    private encode(): string
    {
        return `${this.type}.${this.target}.${this.prio}`;
    }

    static decode(id: string | undefined): Job | undefined
    {
        if (!id) return undefined;

        const parts = id.split(".", 3);

        if (parts.length != 3) return undefined;

        var type = decodeJobType(parts[0]);

        if (!type) return undefined;

        var target = parts[1];
        var prio = Number.parseInt(parts[2]);

        return new Job(type, target, prio);
    }
}

export function createHarvestJob(source: Source, prio: number): Job
{
    return new Job(JobType.Harvest, source.id, prio);
}

export function createUpgradeJob(controller: StructureController, prio: number): Job
{
    return new Job(JobType.Upgrade, controller.id, prio);
}