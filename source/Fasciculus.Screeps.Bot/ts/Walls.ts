import * as _ from "lodash";

import { Wall } from "./Wall";
import { Job } from "./Job";

export class Walls
{
    static _all: Wall[] = [];
    static _my: Wall[] = [];

    static get all(): Wall[] { return Walls._all; }
    static get my(): Wall[] { return Walls._my; }

    static initialize()
    {
        Walls._all = _.flatten(_.values<Room>(Game.rooms).map(Walls.allFrom));
        Walls._my = Walls._all.filter(w => w.my);
    }

    private static allFrom(room: Room): Wall[]
    {
        return room.find<FIND_STRUCTURES, StructureWall>(FIND_STRUCTURES)
            .filter(s => s.structureType == STRUCTURE_WALL)
            .map(w => new Wall(w));
    }
}