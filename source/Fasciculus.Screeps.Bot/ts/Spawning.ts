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
        let wellerCount = Wellers.all.length;
        let sourceCount = Sources.all.length;

        if (wellerCount == 0 && sourceCount > 0) return CreepType.Weller;

        let supplierCount = Suppliers.all.length;

        if (supplierCount == 0 && wellerCount > 0) return CreepType.Supplier;

        if (spawn.roomEnergyAvailable < spawn.roomEnergyCapacity) return undefined;

        if (supplierCount < wellerCount) return CreepType.Supplier;

        let totalWellSlots = _.sum(Wells.all.map(w => w.slots.length));
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

        return undefined;
    }
}