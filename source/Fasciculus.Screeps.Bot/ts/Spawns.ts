import * as _ from "lodash";

import { Rooms } from "./Rooms";

export class Spawns
{
    private static _my: StructureSpawn[] = [];

    static get my(): StructureSpawn[] { return Spawns._my; }

    private static myOf(room: Room): StructureSpawn[]
    {
        return room.find<FIND_MY_SPAWNS, StructureSpawn>(FIND_MY_SPAWNS)
    }

    static initialize()
    {
        Spawns._my = _.flatten(Rooms.all.map(Spawns.myOf));
    }

    static run()
    {

    }

}