
import { BotCapabilities } from "./BotCapabilities";
import { IBotMemory } from "./IBotMemory";
import { Job } from "./Job";
import { JobType } from "./JobType";

export class Bot
{
    readonly creep: Creep;

    private _capabilities?: BotCapabilities
    private _job?: Job;

    constructor(creep: Creep)
    {
        this.creep = creep;
        this._job = Job.fromMemory(this.memory.job);
    }

    get name(): string { return this.creep.name; }

    get my(): boolean { return this.creep.my; }

    get pos(): RoomPosition { return this.creep.pos; }

    get memory(): IBotMemory { return this.creep.memory as IBotMemory; }

    get capabilities() { return this._capabilities || (this._capabilities = new BotCapabilities(this.creep)); }

    get idle(): boolean { return this._job === undefined; }

    get job(): Job | undefined { return this._job; }

    set job(job: Job | undefined)
    {
        this.memory.job = job ? job.toMemory() : undefined;
        this._job = job;

        this.sayJob(job);
    }

    private sayJob(job: Job | undefined)
    {
        var message = job ? JobType[job.type] : "Idle";

        this.creep.say(message, false);
    }
}