
export type ContainerId = Id<StructureContainer>;
export type ControllerId = Id<StructureController>;
export type SiteId = Id<ConstructionSite>;
export type SourceId = Id<Source>;

export type Supply = Creep | StructureContainer;
export type SupplyId = Id<Creep | StructureContainer>;

export type CustomerId = Id<Creep | StructureSpawn | StructureExtension>;
export type Customer = Creep | StructureSpawn | StructureExtension;

export type RepairableId = Id<StructureRoad | StructureWall>
export type Repairable = StructureRoad | StructureWall

export enum CreepType
{
    Starter = "Z",
    Weller = "W",
    Supplier = "S",
    Upgrader = "U",
    Builder = "B",
    Repairer = "R"
}

export enum CreepState
{
    Idle = "idle",
    Suicide = "suicide",
    ToContainer = "toContainer",
    ToController = "toController",
    ToCustomer = "toCustomer",
    ToRepairable = "toRepairable",
    ToSite = "toSite",
    ToSupply = "toSupply",
    ToWell = "toWell",
    Harvest = "harvest",
    Withdraw = "withdraw",
    Transfer = "transfer",
    Upgrade = "upgrade",
    Build = "build",
    Repair = "repair"
}

export enum MarkerType
{
    Info = "I",
    Unknown = "U"
}

interface ExtendedMemory extends Memory
{
    [index: string]: any;
}

export class Memories
{
    static get<T>(key: string, initial: T): T
    {
        const memory: ExtendedMemory = Memory as ExtendedMemory;
        let result: any | undefined = memory[key];

        if (!result)
        {
            memory[key] = result = initial;
        }

        return result as T;
    }
}