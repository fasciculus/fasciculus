import { profile } from "./Profiling";

export class Repairs
{
    private static _all: Array<Repairable> = new Array();
    private static _byId: Map<RepairableId, Repairable> = new Map();

    static get(id: RepairableId | undefined): Repairable | undefined
    {
        return id ? Repairs._byId.get(id) : undefined;
    }

    static get all(): Array<Repairable> { return Array.from(Repairs._all); }
    static get count(): number { return Repairs._all.length; }

    @profile
    static initialize()
    {
        let repairables: Array<Repairable> = Repairs.findWalls();

        Repairs._all = repairables;
        Repairs._byId = Repairs._all.indexBy(r => r.id);
    }

    private static findWalls(): Array<Repairable>
    {
        return StructureWall.my
    }
}