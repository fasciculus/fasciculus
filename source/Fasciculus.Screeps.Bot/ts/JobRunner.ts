import * as _ from "lodash";

import { Bot } from "./Bot";
import { Bots } from "./Bots";
import { Job } from "./Job";
import { JobType } from "./JobType";
import { TargetId } from "./TargetId";
import { Targets } from "./Targets";

export class JobRunner
{
    static run(jobs?: Job[])
    {
        var restrict: Set<string> | undefined = undefined;

        if (jobs)
        {
            restrict = new Set();
            jobs.forEach(j => restrict?.add(j.id));
        }

        Bots.busy.forEach(b => JobRunner.process(b, restrict));
    }

    private static process(bot: Bot, restrict: Set<string> | undefined)
    {
        var job = bot.job;

        if (!job) return;
        if (restrict && !restrict.has(job.id)) return;

        var done = false;

        switch (job.type)
        {
            case JobType.Upgrade: done = JobRunner.upgrade(bot, job.target); break;
            case JobType.Harvest: done = JobRunner.harvest(bot, job.target); break;
            case JobType.Supply: done = JobRunner.supply(bot, job.target); break;
            case JobType.Build: done = JobRunner.build(bot, job.target); break;
            case JobType.Repair: done = JobRunner.repair(bot, job.target); break;

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

        return controller ? JobRunner.result(bot, bot.upgradeController(controller), controller) : true;
    }

    private static harvest(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canHarvest) return true;

        let source = Targets.source(id);

        return source ? JobRunner.result(bot, bot.harvest(source), source) : true;
    }

    private static supply(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canSupply) return true;

        let target: StructureSpawn | StructureExtension | null
            = Targets.spawn(id) || Targets.extension(id);

        return target ? JobRunner.result(bot, bot.transfer(target, RESOURCE_ENERGY), target) : true;
    }

    private static build(bot: Bot, id: TargetId): boolean
    {
        if (!bot.capabilities.canSupply) return true;

        let site = Targets.site(id);

        return site ? JobRunner.result(bot, bot.build(site), site) : true;
    }

    private static repair(bot: Bot, id: TargetId)
    {
        if (!bot.capabilities.canRepair) return true;

        let target = Targets.structure(id);

        return target ? JobRunner.result(bot, bot.repair(target), target) : true;
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