import { Objects } from "./types";

export class ScreepsCreep
{
    static type(this: Creep): string
    {
        return this.name.charAt(0);
    }

    static setup()
    {
        Objects.setGetter(Creep.prototype, "type", ScreepsCreep.type);
    }
}