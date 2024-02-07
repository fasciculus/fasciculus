import { Cached } from "./screeps.util";
import { Objects } from "./types.util";

export class ScreepsFlag
{
    private static _all: Cached<Map<string, Flag>> = Cached.simple(ScreepsFlag.fetchAll);

    private static fetchAll(): Map<string, Flag>
    {
        return Objects.values(Game.flags).indexBy(r => r.name);
    }

    static names(): Set<string>
    {
        return ScreepsFlag._all.value.ks();
    }

    static setup()
    {
        Objects.setGetter(Flag, "names", ScreepsFlag.names);
    }
}
