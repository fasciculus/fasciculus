
import * as _ from "lodash";

import { GameWrap } from "./GameWrap";
import { Vector } from "./Collections";

export class Rooms
{
    private static _all: Vector<Room> = new Vector();
    private static _my: Vector<Room> = new Vector();

    static get all(): Room[] { return Rooms._all.values; }
    static get my(): Room[] { return Rooms._my.values; }

    static isMyRoom(room: Room): boolean
    {
        var controller = room.controller;

        return (controller !== undefined) && controller.my;
    }

    static initialize()
    {
        Rooms._all = GameWrap.rooms;
        Rooms._my = Rooms._all.filter(Rooms.isMyRoom);
    }
}