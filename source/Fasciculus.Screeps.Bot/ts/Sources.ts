import * as _ from "lodash";

import { Rooms } from "./Rooms";

export class Sources
{
    private static _all: Source[] = [];

    static get all(): Source[] { return Sources._all; }

    static allOfRoom(room: Room): Source[]
    {
        return room.find<FIND_SOURCES, Source>(FIND_SOURCES);
    }

    static initialize()
    {
        Sources._all = _.flatten( Rooms.all.map(Sources.allOfRoom));
    }
}