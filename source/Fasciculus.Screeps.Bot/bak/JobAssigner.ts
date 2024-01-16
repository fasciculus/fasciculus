import { Bot } from "./Bot";
import { Bots } from "./Bots";
import { Job } from "./Job";
import { JobType } from "./JobType";

export class JobAssigner
{
    static assign(jobs: Job[]): Job[]
    {
        var unassigned: Job[] = [];

        for (var job of jobs)
        {
            var bot: Bot | null = null;

            switch (job.type)
            {
                case JobType.Harvest: bot = JobAssigner.find(job, Bots.idleHarvesters); break;
                case JobType.Upgrade: bot = JobAssigner.find(job, Bots.idleUpgraders); break;
                case JobType.Supply: bot = JobAssigner.find(job, Bots.idleSuppliers); break;
                case JobType.Build: bot = JobAssigner.find(job, Bots.idleBuilders); break;
                case JobType.Repair: bot = JobAssigner.find(job, Bots.idleRepairers); break;
            }

            if (bot)
            {
                bot.job = job;
                Bots.refresh(false);
            }
            else
            {
                unassigned.push(job);
            }
        }

        return unassigned;
    }

    private static find(job: Job, bots: Bot[]): Bot | null
    {
        if (bots.length == 0) return null;

        var target = Game.getObjectById(job.target);

        if (!target) return null;

        return target.pos.findClosestByPath(bots);
    }
}