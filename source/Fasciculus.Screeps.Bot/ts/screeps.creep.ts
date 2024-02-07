import { Cached } from "./screeps.util";
import { Objects } from "./types.util";

export class ScreepsCreep
{
    private static type(this: Creep): string
    {
        return this.name.charAt(0);
    }

    private static _my: Cached<Map<string, Creep>> = Cached.simple(ScreepsCreep.fetchMy);

    private static fetchMy(): Map<string, Creep>
    {
        return Objects.values(Game.creeps).indexBy(c => c.name);
    }

    private static my(): Array<Creep>
    {
        return ScreepsCreep._my.value.vs();
    }

    private static myNames(): Set<string>
    {
        return ScreepsCreep._my.value.ks();
    }

    private static get(name: string): Creep | undefined
    {
        return ScreepsCreep._my.value.get(name);
    }

    static setup()
    {
        Objects.setGetter(Creep.prototype, "type", ScreepsCreep.type);

        Objects.setGetter(Creep, "my", ScreepsCreep.my);
        Objects.setGetter(Creep, "myNames", ScreepsCreep.myNames);
        Objects.setFunction(Creep, "get", ScreepsCreep.get);
    }
}