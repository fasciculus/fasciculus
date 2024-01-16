import { TargetId } from "./TargetId";

export class Targets
{
    static controller(id: TargetId | Id<StructureController>): StructureController | null
    {
        return Game.getObjectById<StructureController>(id as Id<StructureController>);
    }

    static spawn(id: TargetId | Id<StructureSpawn>): StructureSpawn | null
    {
        return Game.getObjectById<StructureSpawn>(id as Id<StructureSpawn>);
    }

    static source(id: TargetId | Id<Source>): Source | null
    {
        return Game.getObjectById<Source>(id as Id<Source>);
    }

    static site(id: TargetId | Id<ConstructionSite>): ConstructionSite | null
    {
        return Game.getObjectById<ConstructionSite>(id as Id<ConstructionSite>);
    }

    static extension(id: TargetId | Id<StructureExtension>): StructureExtension | null
    {
        return Game.getObjectById<StructureExtension>(id as Id<StructureExtension>);
    }

    static structure(id: TargetId): Structure | null
    {
        return Game.getObjectById<Structure>(id as Id<Structure>);
    }
}