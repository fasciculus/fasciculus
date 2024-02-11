
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

        assign(id: CreepId): void;
        unassign(id: CreepId): void;
        unassignAll(): void;
    }

    interface Memory
    {
        [index: string]: any;

        get<T>(key: string, initial: T): T;
    }

    interface Game
    {
        get username(): string;

        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;
        all<T extends _HasId>(ids: Set<Id<T>> | undefined): Array<T>;
    }

    interface Room
    {
        get safe(): boolean;

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
    }

    interface SourceConstructor
    {
        get known(): Array<Source>;
        get safe(): Array<Source>;
    }

    interface StructureSpawn
    {
        spawn(type: string, body: Array<BodyPartConstant>): ScreepsReturnCode;
    }

    interface StructureSpawnConstructor
    {
        get my(): Array<StructureSpawn>;
    }

    interface StructureController
    {
        get safe(): boolean;
    }
}

export { }