import { Vector, Vectors } from "./Common";
import { profile } from "./Profiling";
import { Chambers } from "./Rooming";

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
    private static _my: Vector<Wall> = new Vector();
    private static _newest: Vector<Wall> = new Vector();
    private static _weakest?: Wall = undefined;

    private static _avg: number = 0;

    static get my(): Vector<Wall> { return Walls._my.clone(); }
    static get avg(): number { return Walls._avg; }

    static get newest(): Vector<Wall> { return Walls._newest.clone(); }
    static get weakest(): Wall | undefined { return Walls._weakest; }

    @profile
    static initialize()
    {
        Walls._my = Vectors.flatten(Chambers.my.map(c => c.walls)).map(w => new Wall(w));
        Walls._newest = Walls._my.filter(w => w.hits == 1);
        Walls._weakest = Walls.findWeakest();
        Walls._avg = Walls._my.sum(w => w.hits) / Math.max(1, Walls._my.length);
    }

    private static findWeakest(): Wall | undefined
    {
        let weakest: Wall | undefined = Walls.my.sort((a, b) => a.hits - b.hits).at(0);

        if (!weakest) return undefined
        if (weakest.hits == WALL_HITS_MAX) return undefined;

        return weakest;
    }
}