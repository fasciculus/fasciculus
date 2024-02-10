
declare global
{
    type ControllerId = Id<StructureController>;
    type SpawnId = Id<StructureSpawn>;

    interface Game
    {
        get username(): string;

        get<T extends _HasId>(id: Id<T> | undefined): T | undefined;
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