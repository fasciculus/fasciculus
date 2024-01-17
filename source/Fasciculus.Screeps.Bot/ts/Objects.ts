
export type IdSupply = Id<Creep | StructureContainer>;
export type Supply = Creep | StructureContainer;

export type IdCustomer = Id<Creep | StructureSpawn>;
export type Customer = Creep | StructureSpawn;

export class Objects
{
    static container(id: Id<StructureContainer> | undefined): StructureContainer | null
    {
        return id ? Game.getObjectById<StructureContainer>(id) : null;
    }

    static source(id: Id<Source> | undefined): Source | null
    {
        return id ? Game.getObjectById<Source>(id) : null;
    }

    static supply(id: IdSupply | undefined): Supply | null
    {
        return id ? Game.getObjectById<Supply>(id) : null;
    }

    static customer(id: IdCustomer | undefined): Customer | null
    {
        return id ? Game.getObjectById<Customer>(id) : null;
    }
}