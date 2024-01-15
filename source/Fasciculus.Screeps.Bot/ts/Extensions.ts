import * as _ from "lodash";

import { Extension } from "./Extension";

export class Extensions
{
    static allFrom(room: Room): Extension[]
    {
        return room.find<FIND_STRUCTURES, StructureExtension>(FIND_STRUCTURES)
            .filter(s => s.structureType == STRUCTURE_EXTENSION)
            .map(s => new Extension(s));
    }

    static myFrom(room: Room): Extension[]
    {
        return room.find<FIND_MY_STRUCTURES, StructureExtension>(FIND_MY_STRUCTURES)
            .filter(s => s.structureType == STRUCTURE_EXTENSION)
            .map(s => new Extension(s));
    }

    static get all(): Extension[] { return _.flatten(_.values<Room>(Game.rooms).map(Extensions.allFrom)); }
    static get my(): Extension[] { return _.flatten(_.values<Room>(Game.rooms).map(Extensions.myFrom)); }
}