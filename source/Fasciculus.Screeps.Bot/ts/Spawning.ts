import * as _ from "lodash";

import { Controllers } from "./Controllers";
import { CreepState, CreepType } from "./Enums";
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
import { Vector } from "./Collections";

export class Spawning
{
    static run()
    {
        let spawn: Spawn | undefined = Spawns.best;

        if (!spawn) return;

        let type: CreepType | undefined = Spawning.nextType();

        if (!type) return;

        Spawning.spawnCreep(spawn, type);
    }

    private static spawnCreep(spawn: Spawn, type: CreepType)
    {
        let body: Vector<BodyPartConstant> | undefined = Spawning.createBody(spawn, type);

        if (!body) return;

        spawn.spawnCreep(type, body);
    }

    private static createBody(spawn: Spawn, type: CreepType): Vector<BodyPartConstant> | undefined
    {
        let body1: Vector<BodyPartConstant> | undefined = Bodies.createBody(type, spawn.roomEnergyAvailable);

        if (!body1) return undefined;

        let body2: Vector<BodyPartConstant> | undefined = Bodies.createBody(type, spawn.roomEnergyCapacity);

        if (!body2) return undefined;
        if (body2.length > body1.length) return undefined;

        return body1;
    }

    private static nextType(): CreepType | undefined
    {
        if (Spawning.moreStarters) return CreepType.Starter;
        if (Spawning.moreSuppliers) return CreepType.Supplier;
        if (Spawning.moreWellers) return CreepType.Weller;
        if (Spawning.moreRepairers) return CreepType.Repairer;
        if (Spawning.moreBuilders) return CreepType.Builder;
        if (Spawning.moreUpgraders) return CreepType.Upgrader;

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
        if (Wellers.count > 0 && Suppliers.count > 0) return false;

        return Starters.count < Math.max(2, Wells.assignable.length);
    }

    private static get moreSuppliers(): boolean
    {
        if (Suppliers.idleCount > 0) return false;

        return Suppliers.performance < Wellers.maxEnergyPerTick;
    }

    private static get moreWellers(): boolean
    {
        return Wells.assignable.length > 0;
    }

    private static get moreUpgraders(): boolean
    {
        if (Suppliers.count == 0) return false;

        let controllerCount = Controllers.my.length;
        let upgraderCount = Upgraders.all.length;

        if (upgraderCount < controllerCount) return true;

        return Upgraders.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Upgrader);
    }

    private static get moreBuilders(): boolean
    {
        if (Suppliers.count == 0) return false;

        let siteCount = Sites.all.length;

        if (siteCount == 0) return false;

        return Builders.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Builder);
    }

    private static get moreRepairers(): boolean
    {
        if (Suppliers.count == 0) return false;

        let repairCount = Repairs.all.length;

        if (repairCount == 0) return false;

        return Repairers.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Repairer);
    }
}