
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
        get level(): number;

        get my(): boolean;
        get safe(): boolean;

        get energy(): number;
        get energyCapacity(): number;

        get sourceIds(): Set<SourceId>;
        get walls(): Array<StructureWall>;
    }

    interface RoomConstructor
    {
        get all(): Array<Room>;
        get names(): Set<string>;

        get my(): Array<Room>;
        get myNames(): Set<string>;

        get safe(): Array<Room>;
        get safeNames(): Set<string>;

        get(name: string): Room | undefined;
    }

    interface Flag
    {
    }

    interface FlagConstructor
    {
        get names(): Set<string>;
    }

    interface Source
    {
        get slots(): number;
    }

    interface StructureSpawn
    {
        get level(): number;
    }

    interface StructureSpawnConstructor
    {
        get my(): Array<StructureSpawn>;
        get myIds(): Set<SpawnId>;
    }

    interface StructureController
    {
        get safe(): boolean;
    }

    interface StructureWallConstructor
    {
        get my(): Array<StructureWall>;
    }

    interface ConstructionSiteConstructor
    {
        get my(): Array<ConstructionSite>;
        get myIds(): Set<SiteId>;
    }

    interface Creep
    {
        get type(): string;
    }

    interface CreepConstructor
    {
        get my(): Array<Creep>;
        get myNames(): Set<string>;

        ofType(type: string): Array<Creep>;
        namesOfType(type: string): Set<string>;

        get(name: string): Creep | undefined;

        newName(type: string): string;

        cleanup(): void;
    }
}

export { };
