
import { Dictionaries, Dictionary } from "./Collections";
import { profile } from "./Profiling";
import { ContainerId, ControllerId, CreepState, CreepType, CustomerId, RepairableId, SiteId, SourceId } from "./Types";

export interface ExtendedMemory extends Memory
{
    [index: string]: any;
}

export interface CreepBaseMemory extends CreepMemory
{
    type: CreepType;
    state: CreepState;
    path?: string;
}

export interface StarterMemory extends CreepBaseMemory
{
    well?: SourceId;
    customer?: CustomerId;
}

export interface WellerMemory extends CreepBaseMemory
{
    well?: SourceId;
}

export interface UpgraderMemory extends CreepBaseMemory
{
    controller?: ControllerId;
}

export interface BuilderMemory extends CreepBaseMemory
{
    site?: SiteId;
}

export interface RepairerMemory extends CreepBaseMemory
{
    repairable?: RepairableId;
}

export class Memories
{
    @profile
    static cleanup()
    {
        var existing: Set<string> = Dictionaries.keys(Game.creeps);

        for (let id in Memory.creeps)
        {
            if (!existing.has(id))
            {
                delete Memory.creeps[id];
            }
        }
    }

    static get memory(): ExtendedMemory
    {
        return Memory as ExtendedMemory;
    }

    static get<T>(key: string, initial: T): T
    {
        let result: any | undefined = Memories.memory[key];

        if (!result)
        {
            Memories.memory[key] = result = initial;
        }

        return result as T;
    }
}