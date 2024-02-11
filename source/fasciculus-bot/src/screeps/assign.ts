
export class Assigns
{
    private static _assigns: Map<AssignableId, Set<CreepId>> = new Map();

    static assignees(id: AssignableId): Set<CreepId>
    {
        return Assigns._assigns.ensure(id, () => new Set());
    }

    static assign(id: AssignableId, creep: CreepId): void
    {
        Assigns.assignees(id).add(creep);
    }

    static unassign(id: AssignableId, creep: CreepId): void
    {
        Assigns.assignees(id).delete(creep);
    }

    static unassignAll(id: AssignableId): void
    {
        Assigns._assigns.delete(id);
    }

    static cleanup()
    {
        const assigns: Map<AssignableId, Set<CreepId>> = Assigns._assigns;

        assigns.keep(Game.existing(assigns.ids));
        assigns.forEach(creeps => creeps.keep(Game.existing(creeps)));
    }
}