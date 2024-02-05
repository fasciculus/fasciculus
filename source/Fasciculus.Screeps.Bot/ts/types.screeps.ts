
declare global
{
    type ContainerId = Id<StructureContainer>;
    type ControllerId = Id<StructureController>;
    type ExtensionId = Id<StructureExtension>;
    type SpawnId = Id<StructureSpawn>;
    type SiteId = Id<ConstructionSite>;
    type SourceId = Id<Source>;
    type WallId = Id<StructureWall>;

    type RepairableId = Id<StructureRoad | StructureWall>;
    type Repairable = StructureRoad | StructureWall;
}

declare global
{
    interface Game
    {
        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;

        get knownRooms(): Array<Room>;
        get knownRoomNames(): Set<string>;

        get myFlagNames(): Set<string>;

        get mySpawns(): Array<StructureSpawn>;
        get mySpawnIds(): Set<SpawnId>;

        get mySites(): Array<ConstructionSite>;
        get mySiteIds(): Set<SiteId>;

        myCreep(name: string | undefined): Creep | undefined;
        get myCreeps(): Array<Creep>;
        get myCreepNames(): Set<string>;

        get username(): string;
    }
}

declare global
{
    interface Memory
    {
        [index: string]: any;

        get<T>(key: string, initial: T): T;
        sub<T>(root: string, key: string, initial: T): T;
    }
}

declare global
{
    interface Room
    {
        get rcl(): number;
    }
}

declare global
{
    interface StructureSpawn
    {
        get rcl(): number;
    }
}

export { };
