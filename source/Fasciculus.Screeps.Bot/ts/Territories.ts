import { Vector } from "./Collections";
import { Dictionary } from "./Common";
import { Point } from "./Geometry";

export type FieldType = 0 | TERRAIN_MASK_WALL | TERRAIN_MASK_SWAMP;

export class Territory
{
    readonly terrain: RoomTerrain;

    constructor(terrain: RoomTerrain)
    {
        this.terrain = terrain;
    }

    at(p: Point): FieldType { return this.terrain.get(p.x, p.y); }
    around(p: Point): Vector<FieldType> { return p.around().map(q => this.at(q)); }
}

export class Territories
{
    private static _cache: Dictionary<Territory> = {};

    static get(room: Room): Territory
    {
        let name = room.name;
        let result: Territory | undefined = Territories._cache[name];

        if (!result)
        {
            result = Territories._cache[name] = new Territory(room.getTerrain());
        }

        return result;
    }
}