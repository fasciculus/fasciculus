
declare global
{
    type AssignableId = Id<Source>;
    type ControllerId = Id<StructureController>;
    type CreepId = Id<Creep>;
    type SourceId = Id<Source>;
    type SpawnId = Id<StructureSpawn>;

    interface _Assignable
    {
        get assignees(): Set<CreepId>;
        get assignedCreeps(): Array<Creep>;

        assign(creep: CreepId): void;
        unassign(creep: CreepId): void;
        unassignAll(): void;
    }

    interface Game
    {
        get username(): string;

        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;
        all<T extends _HasId>(ids: Set<Id<T>> | undefined): Array<T>;

        existing<T extends _HasId>(ids: Set<Id<T>>): Set<Id<T>>;
    }

    interface Memory
    {
        [index: string]: any;

        get<T>(key: string, initial: T): T;
    }

    type TerrainMask = 0 | 1 | 2;
    type TerrainInfo = { pos: RoomPosition, mask: TerrainMask };

    interface RoomTerrain
    {
        info(pos: RoomPosition): TerrainInfo;
        around(pos: RoomPosition, range?: number): Array<TerrainInfo>;
    }

    interface RoomPosition
    {
        around(range?: number): Array<RoomPosition>;
    }

    interface StructureController
    {
        get safe(): boolean;
    }

    interface Room
    {
        get safe(): boolean;

        get energy(): number;
        get energyCapacity(): number;

        get terrain(): RoomTerrain;
        get sources(): Array<Source>;
    }

    interface RoomConstructor
    {
        get(name: string): Room | undefined;

        get known(): Array<Room>;
        get safe(): Array<Room>;
    }

    interface Source extends _Assignable
    {
        get safe(): boolean;

        get slots(): Array<RoomPosition>;
        get freeSlots(): number;
        get freeWork(): number;
    }

    interface SourceConstructor
    {
        get known(): Array<Source>;
        get safe(): Array<Source>;

        get safeFreeSlots(): number;
        get safeFreeWork(): number;
    }

    interface StructureSpawn
    {
        spawn(type: string, body: Array<BodyPartConstant>): ScreepsReturnCode;
    }

    interface StructureSpawnConstructor
    {
        get my(): Array<StructureSpawn>;
        get idle(): Array<StructureSpawn>;
    }

    type BodyInfo = { work: number };

    interface Creep
    {
        get type(): string;

        get workParts(): number;
    }

    interface CreepConstructor
    {
        get my(): Array<Creep>;

        ofType(type: string): Array<Creep>;
    }
}

export { }