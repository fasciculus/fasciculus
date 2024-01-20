import * as _ from "lodash";

import { Controllers } from "./Controllers";
import { CreepState, CreepType } from "./Enums";
import { Sources } from "./Sources";
import { Spawn, Spawns } from "./Spawns";
import { Suppliers } from "./Suppliers";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";
import { Wells } from "./Wells";
import { Sites } from "./Sites";
import { Builders } from "./Builders";
import { CreepBaseMemory } from "./Memories";
import { Bodies } from "./Bodies";
import { Names } from "./Names";
import { Repairs } from "./Repairs";
import { Repairers } from "./Repairers";
import { Starters } from "./Starters";
import { Chambers } from "./Chambers";
import { Statistics } from "./Statistics";

export class Spawning
{
    static run()
    {
        let idle = Spawns.idle;

        if (idle.length == 0) return;

        let spawn = idle.sort((a, b) => b.roomEnergyAvailable - a.roomEnergyAvailable)[0];
        let type = Spawning.nextType(spawn);

        if (!type) return;

        let body = Bodies.create(type, spawn.roomEnergyAvailable);

        if (!body) return;

        let name = Names.next(type);
        let memory: CreepBaseMemory = { type, state: CreepState.Idle };
        let opts: SpawnOptions = { memory };

        spawn.spawn.spawnCreep(body, name, opts);
    }

    private static nextType(spawn: Spawn): CreepType | undefined
    {
        let starterCount = Starters.all.length;
        let wellerCount = Wellers.all.length;
        let sourceCount = Sources.all.length;
        let supplierCount = Suppliers.all.length;

        if (wellerCount == 0 && supplierCount == 0 && starterCount == 0 && sourceCount > 0) return CreepType.Starter;
        if (wellerCount == 0 && sourceCount > 0) return CreepType.Weller;
        if (supplierCount == 0 && wellerCount > 0) return CreepType.Supplier;

        if (spawn.roomEnergyAvailable < spawn.roomEnergyCapacity) return undefined;

        if (Spawning.moreSuppliers) return CreepType.Supplier;

        let totalWellSlots = _.sum(Wells.all.map(w => w.slots));
        let unassignedWellWork = _.sum(Wells.all.map(w => w.unassignedWork));

        if (unassignedWellWork > 0 && wellerCount < totalWellSlots) return CreepType.Weller;

        let controllerCount = Controllers.my.length;
        let upgraderCount = Upgraders.all.length;

        if (upgraderCount < (controllerCount * 2)) return CreepType.Upgrader;

        let siteCount = Sites.all.length;

        if (siteCount > 0)
        {
            let buildersRequired = Math.min(3, 1 + siteCount);
            let builderCount = Builders.all.length;

            if (builderCount < buildersRequired) return CreepType.Builder;
        }

        let repairCount = Repairs.all.length;

        if (repairCount > 0)
        {
            let repairerCount = Repairers.all.length;

            if (repairerCount < 1) return CreepType.Repairer;
        }

        return undefined;
    }

    static get moreSuppliers(): boolean
    {
        return Wells.welled > (Statistics.supplied * 1.2);
    }
}