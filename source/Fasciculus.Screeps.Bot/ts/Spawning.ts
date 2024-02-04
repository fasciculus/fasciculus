
import { CreepType, Vector } from "./Common";
import { UPGRADER_MAX_COUNT } from "./Config";
import { Bodies } from "./Creeps";
import { Guards } from "./Forces";
import { Controllers, Spawn, Spawns } from "./Infrastructure";
import { Tankers } from "./Logistics";
import { Markers } from "./Markers";
import { profile } from "./Profiling";
import { Repairs } from "./Repairs";
import { Wells } from "./Resources";
import { Sites } from "./Sites";
import { Builders, Repairers, Upgraders, Wellers } from "./Workers";

export class Spawning
{
    @profile
    static run()
    {
        let spawn: Spawn | undefined = Spawns.best;

        if (!spawn) return;
        if (spawn.roomEnergyAvailable < Bodies.minCost) return;

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
        const rcl = spawn.rcl;
        let body1: Vector<BodyPartConstant> | undefined = Bodies.createBody(type, rcl, spawn.roomEnergyAvailable);

        if (!body1) return undefined;

        let body2: Vector<BodyPartConstant> | undefined = Bodies.createBody(type, rcl, spawn.roomEnergyCapacity);

        if (!body2) return undefined;
        if (body2.length > body1.length) return undefined;

        return body1;
    }

    private static nextType(): CreepType | undefined
    {
        if (Spawning.moreTankers) return CreepType.Tanker;
        if (Spawning.moreWellers) return CreepType.Weller;
        if (Spawning.moreRepairers) return CreepType.Repairer;
        if (Spawning.moreBuilders) return CreepType.Builder;
        if (Spawning.moreUpgraders) return CreepType.Upgrader;
        if (Spawning.moreGuards) return CreepType.Guard;

        return undefined;
    }

    private static get upgraderEnergyFactor(): number
    {
        var result = 0.65;

        if (Sites.count > 0)
        {
            result -= 0.2;
        }

        if (Repairs.count > 0)
        {
            result -= 0.2;
        }

        return result;
    }

    private static energyAvailable(type: CreepType): number
    {
        let energy = Wellers.maxEnergyPerTick;

        switch (type)
        {
            case CreepType.Upgrader: return energy * Spawning.upgraderEnergyFactor;
            case CreepType.Builder: return energy * 0.6;
            case CreepType.Repairer: return energy * 0.15;
            default: return 0;
        }
    }

    private static get moreTankers(): boolean
    {
        return Tankers.count < (Wellers.count * 2);
    }

    private static get moreWellers(): boolean
    {
        return Wells.assignableCount > 0;
    }

    private static get moreUpgraders(): boolean
    {
        const upgraderCount = Upgraders.count;

        if (upgraderCount >= UPGRADER_MAX_COUNT) return false;
        if (upgraderCount < Controllers.myCount) return true;

        return Upgraders.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Upgrader);
    }

    private static get moreBuilders(): boolean
    {
        if (Sites.count == 0) return false;

        return Builders.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Builder);
    }

    private static get moreRepairers(): boolean
    {
        if (Repairs.count == 0) return false;

        return Repairers.maxEnergyPerTick < Spawning.energyAvailable(CreepType.Repairer);
    }

    private static get moreGuards(): boolean
    {
        return Guards.count < Markers.guardMarkerCount;
    }
}