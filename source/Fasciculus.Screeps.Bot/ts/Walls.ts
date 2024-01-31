import { Dictionaries, Dictionary, GameWrap, Vector, Vectors, WallId } from "./Common";
import { profile } from "./Profiling";
import { Chambers } from "./Rooming";

export class Wall
{
    readonly id: WallId;

    get wall(): StructureWall { return GameWrap.get<StructureWall>(this.id)!; }

    get hits(): number { return this.wall.hits; }

    constructor(id: WallId)
    {
        this.id = id;
    }
}

export class Walls
{
    private static _myWalls: Dictionary<Wall> = {};

    static get my(): Vector<Wall> { return Dictionaries.values(Walls._myWalls); }
    static get avg(): number { return Walls.my.avg(w => w.hits); }

    static get newest(): Vector<Wall> { return Walls.my.filter(w => w.hits == 1); }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Walls._myWalls = {};
        }

        Dictionaries.update(Walls._myWalls, Chambers.myWalls, id => new Wall(id as WallId));
    }

    static get myWeakest(): Wall | undefined
    {
        var result: Wall | undefined = Walls.my.min(w => w.hits);

        if (result && result.hits == WALL_HITS_MAX)
        {
            result = undefined;
        }

        return result;
    }
}