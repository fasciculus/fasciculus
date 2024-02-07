import { Cached } from "./screeps.util";
import { Objects } from "./types.util";

export class ScreepsWall
{
    private static _my: Cached<Map<string, StructureWall>> = Cached.simple(ScreepsWall.fetchMy);

    private static fetchMy(): Map<string, StructureWall>
    {
        const wallss: Array<Array<StructureWall>> = Room.my.map(r => r.walls);

        return Array.flatten(wallss).indexBy(w => w.id);
    }

    private static my(): Array<StructureWall>
    {
        return ScreepsWall._my.value.vs();
    }

    static setup()
    {
        Objects.setGetter(StructureWall, "my", ScreepsWall.my);
    }
}