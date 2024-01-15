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

        if (!controller) return true;

        var creep = bot.creep;
        var code = creep.upgradeController(controller);

        return Executor.result(creep, code, controller.pos);
    }

    private static harvest(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canHarvest) return true;

        let source = Targets.source(id);

        if (!source) return true;

        var creep = bot.creep;
        var code = creep.harvest(source);

        return Executor.result(creep, code, source.pos);
    }

    private static supply(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canSupply) return true;

        let target: StructureSpawn | StructureExtension | null
            = Targets.spawn(id) || Targets.extension(id);

        if (!target) return true;

        var creep = bot.creep;
        var code = creep.transfer(target, RESOURCE_ENERGY);

        return Executor.result(creep, code, target.pos) || target.store.getFreeCapacity() == 0;
    }

    private static build(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canSupply) return true;

        let site = Targets.site(id);

        if (!site) return true;

        var creep = bot.creep;
        var code = creep.build(site);

        return Executor.result(creep, code, site.pos);
    }

    private static result(creep: Creep, code: ScreepsReturnCode, dest: RoomPosition): boolean
    {
        if (code == ERR_NOT_IN_RANGE)
        {
            creep.moveTo(dest);
            return false;
        }

        return code != OK;
    }
}