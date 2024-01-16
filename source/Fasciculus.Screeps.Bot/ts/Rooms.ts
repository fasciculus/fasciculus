
import * as _ from "lodash";

export class Rooms
{
    private static _all: Room[] = [];

    static initialize()
    {
        Rooms._all = _.values(Game.rooms);
    }

    static get all(): Room[] { return Rooms._all; }
}