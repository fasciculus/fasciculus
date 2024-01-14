
import * as _ from "lodash";
import { getRooms } from "./room";

export function getRoomActiveSources(room: Room): Source[]
{
    return room.find<FIND_SOURCES_ACTIVE, Source>(FIND_SOURCES_ACTIVE);
}

export function getActiveSources(): Source[]
{
    return _.flatten(getRooms().map(getRoomActiveSources));
}