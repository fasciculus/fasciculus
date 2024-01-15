import { Bot } from "./Bot";
import { Bots } from "./Bots";
import { JobType } from "./JobType";
import { TargetId } from "./TargetId";
import { Targets } from "./Targets";

export class Executor
{
    static run()
    {
        Bots.busy.forEach(Executor.process);
    }

    private static process(bot: Bot)
    {
        var job = bot.job;

        if (!job) return;

        var done = false;

        switch (job.type)
        {
            case JobType.Upgrade: done = Executor.upgrade(bot, job.target); break;
            case JobType.Harvest: done = Executor.harvest(bot, job.target); break;
            case JobType.Supply: done = Executor.supply(bot, job.target); break;
            case JobType.Build: done = Executor.build(bot, job.target); break;

            default: done = true; break;
        }

        if (done)
        {
            bot.job = undefined;
        }
    }

    private static upgrade(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canUpgrade) return true;

        let controller = Targets.controller(id);

        return controller ? Executor.result(bot, bot.upgradeController(controller), controller) : true;
    }

    private static harvest(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canHarvest) return true;

        let source = Targets.source(id);

        return source ? Executor.result(bot, bot.harvest(source), source) : true;
    }

    private static supply(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canSupply) return true;

        let target: StructureSpawn | StructureExtension | null
            = Targets.spawn(id) || Targets.extension(id);

        return target ? Executor.result(bot, bot.transfer(target, RESOURCE_ENERGY), target) : true;
    }

    private static build(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canSupply) return true;

        let site = Targets.site(id);

        return site ? Executor.result(bot, bot.build(site), site) : true;
    }

    private static result(bot: Bot, code: ScreepsReturnCode, target: { pos: RoomPosition }): boolean
    {
        if (code == ERR_NOT_IN_RANGE)
        {
            bot.moveTo(target);
            return false;
        }

        return code != OK;
    }
}