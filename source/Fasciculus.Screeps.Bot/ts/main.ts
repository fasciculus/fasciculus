import { Initializer } from "./Initializer"
import { Scheduler } from "./Scheduler";

export const loop = function ()
{
    Initializer.run();
    Scheduler.run();
}