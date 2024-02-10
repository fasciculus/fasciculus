import { Objects } from "../es/object";
import { Names } from "./name";

declare global
{
    interface StructureSpawn
    {
        spawn(type: string, body: Array<BodyPartConstant>): ScreepsReturnCode;
    }
}

export class SpawnExt
{
    private static spawn(this: StructureSpawn, type: string, body: Array<BodyPartConstant>): ScreepsReturnCode
    {
        const name = Names.nextCreepName(type);

        return this.spawnCreep(body, name);
    }

    static setup()
    {
        Objects.setFunction(Spawn.prototype, "spawn", SpawnExt.spawn);
    }
}