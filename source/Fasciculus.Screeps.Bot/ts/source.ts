import { Rooms } from "./room";
import { JobManager } from "./job";

export class SourceManager
{
    static run()
    {
        for (let room of Rooms)
        {
            let sources = room.find<FIND_SOURCES_ACTIVE, Source>(FIND_SOURCES_ACTIVE);

            for (let source of sources)
            {
                JobManager.addHarvest(source);
            }
        }
    }
}