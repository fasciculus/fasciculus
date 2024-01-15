
import * as _ from "lodash";

import { Spring } from "./Spring";

export class Springs
{
    static allFrom(room: Room): Spring[] { return room.find<FIND_SOURCES, Source>(FIND_SOURCES).map(s => new Spring(s)); }
    static activeFrom(room: Room): Spring[] { return room.find<FIND_SOURCES_ACTIVE, Source>(FIND_SOURCES_ACTIVE).map(s => new Spring(s)); }

    static get all(): Spring[] { return _.flatten(_.values<Room>(Game.rooms).map(this.allFrom)); }
    static get active(): Spring[] { return _.flatten(_.values<Room>(Game.rooms).map(this.activeFrom)); }
}