import { CreepState, CreepType } from "./Enums";
import { ContainerId, ControllerId, CustomerId, RepairableId, SupplyId, SiteId, SourceId } from "./Types";
import { Dictionaries, Dictionary } from "./Collections";

export interface NamesMemory
{
    next: Dictionary<number>;
}

export interface SourceMemory
{
    container?: ContainerId;
}

export interface WellMemory
{
    slots?: number;
    container?: ContainerId;
    assignees?: string[];
}

export interface StatisticsMemory
{
    supplied?: number;
    welled?: number;
}

export type ProfilerMemory = Dictionary<number>;

export interface ExtendedMemory
{
    names?: NamesMemory;
    sources?: Dictionary<SourceMemory>;
    wells?: Dictionary<WellMemory>;
    statistics?: StatisticsMemory;
    profiler?: ProfilerMemory;
}

export interface CreepBaseMemory extends CreepMemory
{
    type: CreepType;
    state: CreepState;
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

export interface SupplierMemory extends CreepBaseMemory
{
    customer?: CustomerId;
    supply?: SupplyId;

    worked?: number;
    supplied?: number;
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

    static get names(): NamesMemory
    {
        var memory = Memories.memory;

        return memory.names || (memory.names = { next: {} });
    }

    static source(source: Source): SourceMemory
    {
        var memory = Memories.memory;
        var root = memory.sources || (memory.sources = {});
        var id = source.id;

        return root[id] || (root[id] = {});
    }

    static well(id: Id<Source>): WellMemory
    {
        var memory = Memories.memory;
        var root = memory.wells || (memory.wells = {});

        return root[id] || (root[id] = {});
    }

    static get statistics(): StatisticsMemory
    {
        var memory = Memories.memory;

        return memory.statistics || (memory.statistics = {});
    }

    static get profiler(): ProfilerMemory
    {
        var memory = Memories.memory;

        return memory.profiler || (memory.profiler = {});
    }
}