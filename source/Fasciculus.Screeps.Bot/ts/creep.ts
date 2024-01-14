
import * as _ from "lodash";

export function cleanupCreepsMemory()
{
    for (let name in Memory.creeps)
    {
        if (!Game.creeps[name])
        {
            delete Memory.creeps[name];
        }
    }
}

export interface ICreepMemory
{
    job?: string;
}

function getCreepMemory(creep: Creep): ICreepMemory
{
    return creep.memory as any as ICreepMemory;
}

export function getCreepJob(creep: Creep): string | undefined
{
    return getCreepMemory(creep).job;
}

export function setCreepJob(creep: Creep, id: string | undefined)
{
    getCreepMemory(creep).job = id;
}

export function getCreeps(): Creep[]
{
    var result: Creep[] = [];

    for (let name in Game.creeps)
    {
        result.push(Game.creeps[name]);
    }

    return result;
}

export function getIdleCreeps(): Creep[]
{
    return getCreeps().filter(c => !getCreepJob(c));
}

export function getIdleHarvesters(): Creep[]
{
    return getIdleCreeps().filter(canHarvest);
}

export function getIdleUpgraders(): Creep[]
{
    return getIdleCreeps().filter(canUpgrade);
}

function creepHasBodyPart(creep: Creep, type: BodyPartConstant): boolean
{
    return _.any(creep.body, p => p.hits > 0 && p.type == type);
}

function canHarvest(creep: Creep): boolean
{
    if (creep.store.getFreeCapacity(RESOURCE_ENERGY) == 0) return false;

    return creepHasBodyPart(creep, WORK);
}

function canUpgrade(creep: Creep): boolean
{
    if (creep.store.energy == 0) return false;

    return creepHasBodyPart(creep, WORK);
}