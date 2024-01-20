import * as _ from "lodash";

import { IdRepairable, Repairable } from "./Types";
import { Walls } from "./Walls";

export class Repairs
{
    private static _all: Repairable[] = [];
    private static _byId: _.Dictionary<Repairable> = {};

    static get(id: IdRepairable | undefined): Repairable | undefined
    {
        return id ? Repairs._byId[id] : undefined;
    }

    static get all(): Repairable[] { return Repairs._all; }

    static initialize()
    {
        let repairables: Repairable[] = Repairs.findWalls();

        Repairs._all = repairables;
        Repairs._byId = _.indexBy(Repairs._all, r => r.id);
    }

    private static findWalls(): StructureWall[]
    {
        let walls = Walls.newest;

        if (walls.length == 0)
        {
            walls = Walls.my;
        }

        return walls.map(w => w.wall);
    }
}