
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

    moveTo(target: RoomPosition | { pos: RoomPosition }): CreepMoveReturnCode | ERR_NO_PATH | ERR_INVALID_TARGET | ERR_NOT_FOUND
    {
        return this.creep.moveTo(target);
    }

    upgradeController(target: StructureController): ScreepsReturnCode
    {
        return this.creep.upgradeController(target);
    }

    harvest(target: Source | Mineral | Deposit): CreepActionReturnCode | ERR_NOT_FOUND | ERR_NOT_ENOUGH_RESOURCES
    {
        return this.creep.harvest(target);
    }

    transfer(target: AnyCreep | Structure, resourceType: ResourceConstant): ScreepsReturnCode
    {
        return this.creep.transfer(target, resourceType);
    }

    build(target: ConstructionSite): CreepActionReturnCode | ERR_NOT_ENOUGH_RESOURCES | ERR_RCL_NOT_ENOUGH
    {
        return this.creep.build(target);
    }

    repair(target: Structure): CreepActionReturnCode | ERR_NOT_ENOUGH_RESOURCES
    {
        return this.creep.repair(target);
    }
}