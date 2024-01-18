import * as _ from "lodash";

import { Rooms } from "./Rooms";

export class Extensions
{
    private static _my: StructureExtension[] = [];

    static get my(): StructureExtension[] { return Extensions._my; }

    static initialize()
    {
        Extensions._my = _.flatten(Rooms.my.map(Extensions.myOfRoom));
    }

    static myOfRoom(room: Room): StructureExtension[]
    {
        return room.find<FIND_MY_STRUCTURES, StructureExtension>(FIND_MY_STRUCTURES).filter(s => s.structureType == STRUCTURE_EXTENSION);
    }
}