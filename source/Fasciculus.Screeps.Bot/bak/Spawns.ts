
import * as _ from "lodash";

import { Spawn } from "./Spawn";

export class Spawns
{
    static get all(): Spawn[] { return _.values<StructureSpawn>(Game.spawns).map(s => new Spawn(s)); }
    static get idle(): Spawn[] { return Spawns.all.filter(s => s.idle); }

    static spawn()
    {
        Spawns.idle.forEach(s => s.spawn());
    }
}