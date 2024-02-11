import { Objects } from "../es/object";
import { Cached } from "./cache";

export class Creeps
{
    private static _my: Cached<Map<CreepId, Creep>> = Cached.simple(Creeps.fetchMy);

    private static fetchMy(): Map<CreepId, Creep>
    {
        return Objects.values(Game.creeps).indexBy(c => c.id);
    }

    private static my(): Array<Creep>
    {
        return Creeps._my.value.data;
    }

    static setup()
    {
        Objects.setGetter(Creep, "my", Creeps.my);
    }
}