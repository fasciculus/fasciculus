import { Objects } from "../es/object";
import { Cached } from "./cache";

export class Terrains
{
    private static _terrains: Cached<Map<string, RoomTerrain>> = Cached.simple(() => new Map());

    private static info(this: RoomTerrain, pos: RoomPosition): TerrainInfo
    {
        const x = pos.x;
        const y = pos.y;

        if (x < 0 || x > 49) return { pos, mask: TERRAIN_MASK_WALL };
        if (y < 0 || y > 49) return { pos, mask: TERRAIN_MASK_WALL };

        const mask: TerrainMask = this.get(x, y);

        return { pos, mask };
    }

    private static around(this: RoomTerrain, pos: RoomPosition, range?: number): Array<TerrainInfo>
    {
        return pos.around(range).map(p => this.info(p));
    }

    static ofRoom(room: Room): RoomTerrain
    {
        const terrains: Map<string, RoomTerrain> = Terrains._terrains.value;
        const name: string = room.name;
        var result: RoomTerrain | undefined = terrains.get(name);

        if (!result)
        {
            terrains.set(name, result = room.getTerrain());
        }

        return result;
    }

    static ofName(name: string): RoomTerrain
    {
        const terrains: Map<string, RoomTerrain> = Terrains._terrains.value;
        var result: RoomTerrain | undefined = terrains.get(name);

        if (!result)
        {
            terrains.set(name, result = new Room.Terrain(name))
        }

        return result;
    }

    static setup()
    {
        Objects.setFunction(Room.Terrain.prototype, "info", Terrains.info);
        Objects.setFunction(Room.Terrain.prototype, "around", Terrains.around);
    }
}