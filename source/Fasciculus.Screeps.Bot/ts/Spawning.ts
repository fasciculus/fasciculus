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
import { Statistics } from "./Statistics";

export class Spawning
{
    static run()
    {
        let spawn = Spawning.findSpawn();

        if (!spawn) return;

        let type = Spawning.nextType();

        if (!type) return;

        Spawning.spawnCreep(spawn, type);
    }

    private static findSpawn(): Spawn | undefined
    {
        let idle = Spawns.idle;

        if (idle.length == 0) return undefined;

        return idle.sort((a, b) => b.roomEnergyAvailable - a.roomEnergyAvailable)[0];
    }

    private static spawnCreep(spawn: Spawn, type: CreepType)
    {
        let body = Spawning.createBody(spawn, type);

        if (!body) return;

        let name = Names.next(type);
        let memory: CreepBaseMemory = { type, state: CreepState.Idle };
        let opts: SpawnOptions = { memory };

        spawn.spawn.spawnCreep(body, name, opts);
    }

    private static createBody(spawn: Spawn, type: CreepType): BodyPartConstant[] | undefined
    {
        let body1 = Bodies.create(type, spawn.roomEnergyAvailable);

        if (!body1) return undefined;

        let body2 = Bodies.create(type, spawn.roomEnergyCapacity);

        if (!body2) return undefined;
        if (body2.length > body1.length) return undefined;

        return body1;
    }

    private static nextType(): CreepType | undefined
    {
        if (Spawning.moreStarters) return CreepType.Starter;
        if (Spawning.moreSuppliers) return CreepType.Supplier;
        if (Spawning.moreWellers) return CreepType.Weller;
        if (Spawning.moreUpgraders) return CreepType.Upgrader;
        if (Spawning.moreBuilders) return CreepType.Builder;
        if (Spawning.moreRepairers) return CreepType.Repairer;

        return undefined;
    }

    private static get moreStarters(): boolean
    {
        let sourceCount = Sources.all.length;

        if (sourceCount == 0) return false;

        let wellerCount = Wellers.all.length;
        let supplierCount = Suppliers.all.length;

        if (wellerCount > 0 && supplierCount > 0) return false;

        let starterCount = Starters.all.length;
        let slotCount = _.sum(Wells.all.map(w => w.slots));

        return starterCount < 2 && starterCount < slotCount;
    }

    private static get moreSuppliers(): boolean
    {
        let idle = Suppliers.all.filter(s => s.state == CreepState.Idle).length;

        if (idle > 0) return false;

        return Statistics.welled > (Statistics.supplied * 1.2);
    }

    private static get moreWellers(): boolean
    {
        let wellerCount = Wellers.all.length;
        let slotCount = _.sum(Wells.all.map(w => w.slots));

        let unassignedWork = _.sum(Wells.all.map(w => w.unassignedWork));

        return unassignedWork > 0 && wellerCount < slotCount;
    }

    private static get moreUpgraders(): boolean
    {
        let existing = _.sum(Upgraders.all.map(u => u.capabilities.work));
        let required = Statistics.welled / 3.5;

        return required > existing;
    }

    private static get moreBuilders(): boolean
    {
        let siteCount = Sites.all.length;

        if (siteCount == 0) return false;

        return Builders.all.length < 2;
    }

    private static get moreRepairers(): boolean
    {
        let repairCount = Repairs.all.length;

        if (repairCount == 0) return false;

        return Repairers.all.length < 1;
    }
}