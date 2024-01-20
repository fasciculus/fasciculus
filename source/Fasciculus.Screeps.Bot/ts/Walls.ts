import * as _ from "lodash";
import { Rooms } from "./Rooms";

export class Wall
{
    readonly wall: StructureWall;

    get hits(): number { return this.wall.hits; }

    constructor(wall: StructureWall)
    {
        this.wall = wall;
    }
}

export class Walls
{
    private static _my: Wall[] = [];
    private static _newest: Wall[] = [];
    private static _weakest?: Wall = undefined;

    private static _avg: number = 0;

    static get my(): Wall[] { return Walls._my; }
    static get avg(): number { return Walls._avg; }

    static get newest(): Wall[] { return Walls._newest; }
    static get weakest(): Wall | undefined { return Walls._weakest; }

    static initialize()
    {
        var structures = _.flatten(Rooms.my.map(r => r.find<FIND_STRUCTURES, StructureWall>(FIND_STRUCTURES)));
        var walls = structures.filter(s => s.structureType == STRUCTURE_WALL);

        Walls._my = walls.map(w => new Wall(w));
        Walls._newest = Walls._my.filter(w => w.hits == 1);
        Walls._weakest = Walls.findWekest();

        Walls._avg = _.sum(walls.map(w => w.hits)) / Math.max(1, walls.length);
    }

    private static findWekest(): Wall | undefined
    {
        let walls = Walls._my;

        if (walls.length == 0) return undefined;

        let wall = walls.sort((a, b) => a.hits - b.hits)[0];

        if (wall.hits == WALL_HITS_MAX) return undefined;

        return wall;
    }
}