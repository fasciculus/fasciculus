import { RepairableId, Repairable } from "./Types";
import { Wall, Walls } from "./Walls";
import { Dictionary, Vector } from "./Collections";

export class Repairs
{
    private static _all: Vector<Repairable> = new Vector();
    private static _byId: Dictionary<Repairable> = {};

    static get(id: RepairableId | undefined): Repairable | undefined
    {
        return id ? Repairs._byId[id] : undefined;
    }

    static get all(): Vector<Repairable> { return Repairs._all.clone(); }
    static get count(): number { return Repairs._all.length; }

    static initialize()
    {
        let repairables: Vector<Repairable> = Repairs.findWalls();

        Repairs._all = repairables;
        Repairs._byId = Repairs._all.indexBy(r => r.id);
    }

    private static findWalls(): Vector<Repairable>
    {
        let walls: Vector<Wall> = Walls.newest;

        if (walls.length == 0)
        {
            walls = Walls.my;
        }

        return walls.map(w => w.wall);
    }
}