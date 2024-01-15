import { Bot } from "./Bot";
import { Bots } from "./Bots";
import { JobType } from "./JobType";
import { TargetId } from "./TargetId";

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

        let controller = Game.getObjectById<StructureController>(id as Id<StructureController>);

        if (!controller) return true;

        var creep = bot.creep;
        var code = creep.upgradeController(controller);

        if (code == ERR_NOT_IN_RANGE)
        {
            creep.moveTo(controller);
            return false;
        }

        return code != OK;
    }

    private static harvest(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canHarvest) return true;

        let source = Game.getObjectById<Source>(id as Id<Source>);

        if (!source) return true;

        var creep = bot.creep;
        var code = creep.harvest(source);

        if (code == ERR_NOT_IN_RANGE)
        {
            creep.moveTo(source);
            return false;
        }

        return code != OK;
    }

    private static supply(bot: Bot, id: TargetId)
    {
        if (!bot.capabilities.canSupply) return true;

        let spawn = Game.getObjectById<StructureSpawn>(id as Id<StructureSpawn>);

        if (!spawn) return true;

        var creep = bot.creep;
        var code = creep.transfer(spawn, RESOURCE_ENERGY);

        if (code == ERR_NOT_IN_RANGE)
        {
            creep.moveTo(spawn);
            return false;
        }

        return code != OK || spawn.store.getFreeCapacity() == 0;
    }
}