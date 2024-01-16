import { Job } from "./Job";

export interface IJobCreator
{
    createJobs(): Job[];
}