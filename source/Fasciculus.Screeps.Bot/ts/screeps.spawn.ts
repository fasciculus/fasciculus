import { Objects } from "./types";

export class ScreepsSpawn
{
    static level(this: StructureSpawn): number
    {
        return this.room.level;
    }

    static setup()
    {
        Objects.setGetter(StructureSpawn.prototype, "level", ScreepsSpawn.level);
    }
}
