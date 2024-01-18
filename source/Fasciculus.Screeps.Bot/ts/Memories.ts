import * as _ from "lodash";
import { CreepState, CreepType } from "./Enums";
import { IdCustomer, IdSupply } from "./Types";

export interface NamesMemory
{
    next: { [prefix: string]: number };
}

export interface SourceMemory
{
    container?: Id<StructureContainer>;
}

export interface WellMemory
{
    slots?: DirectionConstant[];
    container?: Id<StructureContainer>;
    assignees?: string[];
    assignedWork?: number;
}

export interface ExtendedMemory
{
    names?: NamesMemory;
    sources?: { [id: Id<Source>]: SourceMemory };
    wells?: { [id: Id<Source>]: WellMemory };
}

export interface CreepBaseMemory extends CreepMemory
{
    type: CreepType;
    state: CreepState;
}

export interface WellerMemory extends CreepBaseMemory
{
    well?: Id<Source>;
}

export interface SupplierMemory extends CreepBaseMemory
{
    customer?: IdCustomer;
    supply?: IdSupply;
}

export interface UpgraderMemory extends CreepBaseMemory
{
    controller?: Id<StructureController>;
}

export interface BuilderMemory extends CreepBaseMemory
{
    site?: Id<ConstructionSite>;
}

export class Memories
{
    static cleanup()
    {
        var existing: Set<string> = new Set(_.keys(Game.creeps));

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
        var root = memory.sources;

        if (!root)
        {
            memory.sources = root = {};
        }

        var id = source.id;
        var result = root[id];

        if (!result)
        {
            root[id] = result = {};
        }

        return result;
    }

    static well(id: Id<Source>): WellMemory
    {
        var memory = Memories.memory;
        var root = memory.wells || (memory.wells = {});

        return root[id] || (root[id] = {});
    }
}