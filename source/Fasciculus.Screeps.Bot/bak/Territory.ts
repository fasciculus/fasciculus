import { Direction } from "./Direction";
import { TerrainType } from "./TerrainType";

export class Territory
{
    readonly terrain: RoomTerrain;

    constructor(room: Room)
    {
        this.terrain = room.getTerrain();
    }

    at(x: number, y: number): TerrainType
    {
        if (x < 0 || x > 49) return TERRAIN_MASK_WALL;
        if (y < 0 || y > 49) return TERRAIN_MASK_WALL;

        return this.terrain.get(x, y);
    }

    around(x: number, y: number): TerrainType[]
    {
        return Direction.neighbours.map(d => this.at(x + d.dx, y + d.dy));
    }
}