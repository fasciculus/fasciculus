
import { Workers } from "./creep";

enum JobType
{
    Harvest = "H"
}

class Job
{
    type: JobType;
    target: RoomObject;

    constructor(type: JobType, target: RoomObject)
    {
        this.type = type;
        this.target = target;
    }
}

var _jobs: Job[] = [];

export class JobManager
{
    static addHarvest(source: Source)
    {
        _jobs.push(new Job(JobType.Harvest, source));
    }

    static run()
    {
        for (var job of _jobs)
        {

        }
    }
}