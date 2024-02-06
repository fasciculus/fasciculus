import { Cached } from "./screeps.util";
import { Objects } from "./types";

export class ScreepsFlag
{
    private static _all: Cached<Map<string, Flag>> = Cached.simple(ScreepsFlag.fetchAll);

    private static fetchAll(): Map<string, Flag>
    {
        const result: Map<string, Flag> = new Map<string, Flag>();

        Objects.keys(Game.flags).forEach(k => result.set(k, Game.flags[k]));

        return result;
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
