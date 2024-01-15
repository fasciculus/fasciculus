import * as _ from "lodash";

import { Controller } from "./Controller";

export class Controllers
{
    static get all(): Controller[]
    {
        var result: Controller[] = [];

        for (let room of _.values<Room>(Game.rooms))
        {
            let structure = room.controller;

            if (!structure) continue;

            result.push(new Controller(structure));
        }

        return result;
    }

    static get my(): Controller[] { return Controllers.all.filter(c => c.my); }
}