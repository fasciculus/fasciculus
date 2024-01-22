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

export const SUPPLIER_PERFORMANCE_FACTOR = 1.2;

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

    private static energyAvailable(type: CreepType): number
    {
        let energy = Wellers.maxEnergyPerTick;

        switch (type)
        {
            case CreepType.Upgrader: return energy * 0.25;
            case CreepType.Builder: return energy * 0.6;
            case CreepType.Repairer: return energy * 0.15;
            default: return 0;
        }
    }

    private static get moreStarters(): boolean
    {
        let sourceCount = Sources.all.length;

        if (sourceCount == 0) return false;

        let starterCount = Starters.all.length;

        if (starterCount > 1) return false;

        if (Wellers.all.length > 0 && Suppliers.count > 0) return false;

        let slotCount = _.sum(Wells.all.map(w => w.freeSlots));

        return starterCount < slotCount;
    }

    private static get moreSuppliers(): boolean
    {
        if (Suppliers.idleCount > 0) return false;

        return Suppliers.performance * SUPPLIER_PERFORMANCE_FACTOR < Wellers.maxEnergyPerTick;
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
        let controllerCount = Controllers.my.length;
        let upgraderCount = Upgraders.all.length;

        if (upgraderCount < controllerCount) return true;

        return Upgraders.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Upgrader);
    }

    private static get moreBuilders(): boolean
    {
        let siteCount = Sites.all.length;

        if (siteCount == 0) return false;

        return Builders.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Builder);
    }

    private static get moreRepairers(): boolean
    {
        let repairCount = Repairs.all.length;

        if (repairCount == 0) return false;

        return Repairers.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Repairer);
    }
}