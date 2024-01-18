
export type IdSupply = Id<Creep | StructureContainer>;
export type Supply = Creep | StructureContainer;

export type IdCustomer = Id<Creep | StructureSpawn | StructureExtension>;
export type Customer = Creep | StructureSpawn | StructureExtension;

export class Objects
{
    static container(id: Id<StructureContainer> | undefined): StructureContainer | null
    {
        return id ? Game.getObjectById<StructureContainer>(id) : null;
    }

    static controller(id: Id<StructureController> | undefined): StructureController | null
    {
        return id ? Game.getObjectById<StructureController>(id) : null;
    }

    static customer(id: IdCustomer | undefined): Customer | null
    {
        return id ? Game.getObjectById<Customer>(id) : null;
    }

    static site(id: Id<ConstructionSite> | undefined): ConstructionSite | null
    {
        return id ? Game.getObjectById<ConstructionSite>(id) : null; 
    }

    static source(id: Id<Source> | undefined): Source | null
    {
        return id ? Game.getObjectById<Source>(id) : null;
    }

    static supply(id: IdSupply | undefined): Supply | null
    {
        return id ? Game.getObjectById<Supply>(id) : null;
    }
}