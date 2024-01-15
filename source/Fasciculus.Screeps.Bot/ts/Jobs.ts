import * as _ from "lodash";

import { JobFactory } from "./JobFactory";
import { JobAssigner } from "./JobAssigner";
import { JobRunner } from "./JobRunner";
import { Bots } from "./Bots";

export class Jobs
{
    static run()
    {
        var jobs = JobFactory.create();
        var unassigned = JobAssigner.assign(jobs);

        var n1 = unassigned.length;
        var n2 = 0;

        JobRunner.run();
        Bots.refresh(false);

        unassigned = JobAssigner.assign(unassigned);
        n2 = unassigned.length;
        JobRunner.run(unassigned);

        console.log(`unassigned ${n1} -> ${n2}`);
    }
}