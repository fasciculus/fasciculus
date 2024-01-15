import * as _ from "lodash";

import { Construction } from "./Construction";

export class Constructions
{
    static allFrom(room: Room): Construction[]
    {
        return room.find<FIND_CONSTRUCTION_SITES, ConstructionSite>(FIND_CONSTRUCTION_SITES).map(s => new Construction(s));
    }

    static myFrom(room: Room): Construction[]
    {
        return room.find<FIND_MY_CONSTRUCTION_SITES, ConstructionSite>(FIND_MY_CONSTRUCTION_SITES).map(s => new Construction(s));
    }

    static get all(): Construction[] { return _.flatten(_.values<Room>(Game.rooms).map(Constructions.allFrom)); }
    static get my(): Construction[] { return _.flatten(_.values<Room>(Game.rooms).map(Constructions.myFrom)); }
}