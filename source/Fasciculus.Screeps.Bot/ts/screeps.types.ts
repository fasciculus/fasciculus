
declare global
{
    type ContainerId = Id<StructureContainer>;
    type ControllerId = Id<StructureController>;
    type CreepId = Id<Creep>;
    type ExtensionId = Id<StructureExtension>;
    type SpawnId = Id<StructureSpawn>;
    type SiteId = Id<ConstructionSite>;
    type SourceId = Id<Source>;
    type WallId = Id<StructureWall>;

    type Assignable = Source;
    type AssignableId = Id<Source>

    type RepairableId = Id<StructureRoad | StructureWall>;
    type Repairable = StructureRoad | StructureWall;

    interface Game
    {
        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;
        all<T extends _HasId>(ids: Set<Id<T>>): Array<T>;

        existing<T extends _HasId>(ids: Set<Id<T>>): Set<Id<T>>;

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

        get costMatrix(): CostMatrix;
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
        get freeSlots(): number;

        get assignees(): Array<Creep>;
        get assignedWorkParts(): number;
        assign(creep: Creep): void;
        unassign(creep: Creep): void;
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

    interface ConstructionSite
    {
        get progressRemaining(): number;
    }

    interface ConstructionSiteConstructor
    {
        get my(): Array<ConstructionSite>;
        get myIds(): Set<SiteId>;
    }

    interface Creep
    {
        get type(): string;

        get workParts(): number;
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
