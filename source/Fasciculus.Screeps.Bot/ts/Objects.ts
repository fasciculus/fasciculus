
export type IdSupply = Id<Creep | StructureContainer>;
export type Supply = Creep | StructureContainer;

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

    static supply(id: IdSupply | undefined): Creep | StructureContainer | null
    {
        return id ? Game.getObjectById<Creep | StructureContainer>(id) : null;
    }
}