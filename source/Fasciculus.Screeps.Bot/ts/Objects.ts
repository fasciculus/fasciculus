
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
}