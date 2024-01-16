
import * as _ from "lodash";

export class Rooms
{
    private static _all: Room[] = [];
    private static _my: Room[] = [];

    static get all(): Room[] { return Rooms._all; }
    static get my(): Room[] { return Rooms._my; }

    static isMyRoom(room: Room): boolean
    {
        var controller = room.controller;

        return (controller !== undefined) && controller.my;
    }

    static initialize()
    {
        Rooms._all = _.values(Game.rooms);
        Rooms._my = Rooms._all.filter(Rooms.isMyRoom);
    }
}