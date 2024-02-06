
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

    interface Game
    {
        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;

        get knownRooms(): Array<Room>;
        get knownRoomNames(): Set<string>;
        knownRoom(roomName: string): Room | undefined;

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

    interface Memory
    {
        [index: string]: any;

        get<T>(key: string, initial: T): T;
        sub<T>(root: string, key: string, initial: T): T;
    }

    interface Room
    {
        get rcl(): number;

        get sourceIds(): Set<SourceId>;
    }

    interface Source
    {
        get slots(): number;
    }

    interface StructureSpawn
    {
        get rcl(): number;
    }

    interface CreepMemory
    {
        type: string;
    }

    interface Creep
    {
        get type(): string;
    }
}

export { };
